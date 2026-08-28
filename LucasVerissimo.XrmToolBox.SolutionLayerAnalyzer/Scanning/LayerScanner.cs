using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Scanning
{
    internal sealed class LayerScanner
    {
        private readonly LayerQueryService layerQueryService;
        private readonly AnalyzerOptions options;
        private readonly Action<string> log;

        public LayerScanner(
            LayerQueryService layerQueryService,
            AnalyzerOptions options,
            Action<string> log
        )
        {
            this.layerQueryService =
                layerQueryService ?? throw new ArgumentNullException(nameof(layerQueryService));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.log = log ?? delegate { };
        }

        public void Scan(
            IReadOnlyCollection<LayerAnalysisResult> components,
            AnalysisMetrics metrics,
            Action<LayerAnalysisResult, AnalysisMetricsSnapshot> resultAvailable,
            CancellationToken cancellationToken
        )
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            if (resultAvailable == null)
            {
                throw new ArgumentNullException(nameof(resultAvailable));
            }

            var batches = CreateBatches(components.ToList(), options.BatchSize).ToList();
            Parallel.ForEach(
                batches,
                new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = options.MaxDegreeOfParallelism,
                },
                batch => ProcessBatch(batch, metrics, resultAvailable, cancellationToken)
            );
        }

        private void ProcessBatch(
            IReadOnlyCollection<LayerAnalysisResult> components,
            AnalysisMetrics metrics,
            Action<LayerAnalysisResult, AnalysisMetricsSnapshot> resultAvailable,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            var stopwatch = Stopwatch.StartNew();
            log("Layer batch started with " + components.Count + " component(s).");

            try
            {
                var queryResults = layerQueryService.QueryBatch(
                    components,
                    metrics,
                    cancellationToken
                );
                var timeoutComponents = components
                    .Where(component => IsTimeoutResult(component, queryResults))
                    .ToList();
                var completedComponents = components
                    .Where(component => !timeoutComponents.Contains(component))
                    .ToList();

                foreach (var component in completedComponents)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var componentStopwatch = Stopwatch.StartNew();
                    var identity = new ComponentIdentity(
                        component.ComponentType,
                        component.ComponentId
                    );
                    LayerQueryResult queryResult;
                    if (!queryResults.TryGetValue(identity, out queryResult))
                    {
                        queryResult = new LayerQueryResult
                        {
                            Layers = new List<LayerInfo>(),
                            Error = new InvalidOperationException(
                                "No layer result was returned for the component."
                            ),
                        };
                    }

                    ApplyLayerResult(component, queryResult);
                    component.DurationMs = componentStopwatch.ElapsedMilliseconds;
                    metrics.RecordProcessed(component);
                    resultAvailable(component, metrics.Snapshot());
                }

                if (timeoutComponents.Count > options.MinimumBatchSize)
                {
                    log(
                        "Layer batch returned timeout faults; reducing retry batch from "
                            + timeoutComponents.Count
                            + "."
                    );
                    var splitSize = Math.Max(
                        options.MinimumBatchSize,
                        (int)Math.Ceiling(timeoutComponents.Count / 2d)
                    );
                    foreach (var splitBatch in CreateBatches(timeoutComponents, splitSize))
                    {
                        ProcessBatch(splitBatch, metrics, resultAvailable, cancellationToken);
                    }
                }
                else
                {
                    foreach (var component in timeoutComponents)
                    {
                        var identity = new ComponentIdentity(
                            component.ComponentType,
                            component.ComponentId
                        );
                        ApplyLayerResult(component, queryResults[identity]);
                        metrics.RecordProcessed(component);
                        resultAvailable(component, metrics.Snapshot());
                    }
                }
            }
            catch (Exception error)
            {
                if (error is OperationCanceledException)
                {
                    throw;
                }

                if (
                    DataverseRetryPolicy.IsTimeout(error)
                    && components.Count > options.MinimumBatchSize
                )
                {
                    log(
                        "Layer batch timed out; reducing batch size from " + components.Count + "."
                    );
                    var splitSize = Math.Max(
                        options.MinimumBatchSize,
                        (int)Math.Ceiling(components.Count / 2d)
                    );
                    foreach (var splitBatch in CreateBatches(components.ToList(), splitSize))
                    {
                        ProcessBatch(splitBatch, metrics, resultAvailable, cancellationToken);
                    }

                    return;
                }

                foreach (var component in components)
                {
                    component.Status = "Layer analysis failed";
                    component.Error = error.Message;
                    metrics.RecordProcessed(component);
                    resultAvailable(component, metrics.Snapshot());
                }
            }
            finally
            {
                stopwatch.Stop();
                metrics.RecordBatch(stopwatch.ElapsedMilliseconds);
                log(
                    "Layer batch completed in "
                        + stopwatch.ElapsedMilliseconds
                        + "ms for "
                        + components.Count
                        + " component(s)."
                );
            }
        }

        private static void ApplyLayerResult(
            LayerAnalysisResult component,
            LayerQueryResult queryResult
        )
        {
            if (queryResult.Error != null)
            {
                component.Status = "Layer analysis failed";
                component.Error = queryResult.Error.Message;
                return;
            }

            var layers = queryResult.Layers ?? new List<LayerInfo>();
            component.LayerCount = layers.Count;
            component.HasActiveLayer = layers.Any(layer => layer.IsActiveLayer);
            var readableName = layers
                .Select(layer => layer.ComponentName)
                .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name));
            if (!string.IsNullOrWhiteSpace(readableName))
            {
                component.ComponentName = readableName;
            }

            component.Status =
                component.HasActiveLayer == true ? "Active layer found" : "No active layer";
        }

        private static bool IsTimeoutResult(
            LayerAnalysisResult component,
            IReadOnlyDictionary<ComponentIdentity, LayerQueryResult> queryResults
        )
        {
            var identity = new ComponentIdentity(component.ComponentType, component.ComponentId);
            LayerQueryResult queryResult;
            return queryResults.TryGetValue(identity, out queryResult)
                && DataverseRetryPolicy.IsTimeout(queryResult.Error);
        }

        private static IEnumerable<List<T>> CreateBatches<T>(IReadOnlyList<T> items, int size)
        {
            for (var index = 0; index < items.Count; index += size)
            {
                var count = Math.Min(size, items.Count - index);
                var batch = new List<T>(count);
                for (var offset = 0; offset < count; offset++)
                {
                    batch.Add(items[index + offset]);
                }

                yield return batch;
            }
        }
    }
}
