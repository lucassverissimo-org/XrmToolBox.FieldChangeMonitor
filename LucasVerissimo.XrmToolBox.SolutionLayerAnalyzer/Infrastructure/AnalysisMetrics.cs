using System;
using System.Diagnostics;
using System.Threading;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure
{
    internal sealed class AnalysisMetrics
    {
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();
        private long totalBatchDurationMs;
        private int totalSourceComponents;
        private int totalTargetSolutionComponents;
        private int processedComponents;
        private int foundInTargetEnvironment;
        private int missingFromTargetEnvironment;
        private int matched;
        private int missingFromTargetSolution;
        private int missingFromSourceSolution;
        private int activeLayers;
        private int withoutActiveLayer;
        private int errors;
        private int batches;
        private int requests;
        private int retries;
        private int throttlings;
        private int timeouts;

        public void SetTotals(int sourceComponents, int targetComponents)
        {
            Interlocked.Exchange(ref totalSourceComponents, sourceComponents);
            Interlocked.Exchange(ref totalTargetSolutionComponents, targetComponents);
        }

        public void RecordCorrelation(LayerAnalysisResult result)
        {
            if (result.ExistsInTargetEnvironment)
            {
                Interlocked.Increment(ref foundInTargetEnvironment);
            }
            else
            {
                Interlocked.Increment(ref missingFromTargetEnvironment);
            }

            switch (result.CorrelationStatus)
            {
                case ComponentCorrelationStatus.Matched:
                    Interlocked.Increment(ref matched);
                    break;
                case ComponentCorrelationStatus.MissingFromTargetSolution:
                    Interlocked.Increment(ref missingFromTargetSolution);
                    break;
                case ComponentCorrelationStatus.MissingFromSourceSolution:
                    Interlocked.Increment(ref missingFromSourceSolution);
                    break;
            }
        }

        public void RecordProcessed(LayerAnalysisResult result)
        {
            Interlocked.Increment(ref processedComponents);

            if (!string.IsNullOrWhiteSpace(result.Error))
            {
                Interlocked.Increment(ref errors);
            }
            else if (result.HasActiveLayer == true)
            {
                Interlocked.Increment(ref activeLayers);
            }
            else if (result.HasActiveLayer == false)
            {
                Interlocked.Increment(ref withoutActiveLayer);
            }
        }

        public void RecordBatch(long durationMs)
        {
            Interlocked.Increment(ref batches);
            Interlocked.Add(ref totalBatchDurationMs, durationMs);
        }

        public void RecordRequest()
        {
            Interlocked.Increment(ref requests);
        }

        public void RecordRetry()
        {
            Interlocked.Increment(ref retries);
        }

        public void RecordThrottling()
        {
            Interlocked.Increment(ref throttlings);
        }

        public void RecordTimeout()
        {
            Interlocked.Increment(ref timeouts);
        }

        public AnalysisMetricsSnapshot Snapshot()
        {
            var batchCount = Volatile.Read(ref batches);
            var batchDuration = Interlocked.Read(ref totalBatchDurationMs);

            return new AnalysisMetricsSnapshot
            {
                TotalSourceComponents = Volatile.Read(ref totalSourceComponents),
                TotalTargetSolutionComponents = Volatile.Read(ref totalTargetSolutionComponents),
                ProcessedComponents = Volatile.Read(ref processedComponents),
                FoundInTargetEnvironment = Volatile.Read(ref foundInTargetEnvironment),
                MissingFromTargetEnvironment = Volatile.Read(ref missingFromTargetEnvironment),
                Matched = Volatile.Read(ref matched),
                MissingFromTargetSolution = Volatile.Read(ref missingFromTargetSolution),
                MissingFromSourceSolution = Volatile.Read(ref missingFromSourceSolution),
                ActiveLayers = Volatile.Read(ref activeLayers),
                WithoutActiveLayer = Volatile.Read(ref withoutActiveLayer),
                Errors = Volatile.Read(ref errors),
                Batches = batchCount,
                Requests = Volatile.Read(ref requests),
                Retries = Volatile.Read(ref retries),
                Throttlings = Volatile.Read(ref throttlings),
                Timeouts = Volatile.Read(ref timeouts),
                Elapsed = stopwatch.Elapsed,
                AverageBatchDurationMs = batchCount == 0 ? 0 : (double)batchDuration / batchCount,
            };
        }
    }
}
