using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Scanning;
using Microsoft.Xrm.Sdk;
using SolutionComponentReference = LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models.SolutionComponentReference;
using SolutionInfo = LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models.SolutionInfo;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Services
{
    internal sealed class SolutionAnalysisService
    {
        private readonly IOrganizationService sourceService;
        private readonly IOrganizationService targetService;
        private readonly AnalyzerOptions options;
        private readonly Action<string> log;

        public SolutionAnalysisService(
            IOrganizationService sourceService,
            IOrganizationService targetService,
            AnalyzerOptions options,
            Action<string> log
        )
        {
            this.sourceService =
                sourceService ?? throw new ArgumentNullException(nameof(sourceService));
            this.targetService =
                targetService ?? throw new ArgumentNullException(nameof(targetService));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.log = log ?? delegate { };
            options.Validate();
        }

        public AnalysisExecutionResult Analyze(
            SolutionInfo sourceSolution,
            SolutionInfo targetSolution,
            Action<AnalysisProgress> progress,
            CancellationToken cancellationToken
        )
        {
            if (sourceSolution == null)
            {
                throw new ArgumentNullException(nameof(sourceSolution));
            }

            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            var metrics = new AnalysisMetrics();
            var sourceRepository = new SolutionRepository(sourceService);
            var targetRepository = new SolutionRepository(targetService);
            var allResults = new List<LayerAnalysisResult>();

            try
            {
                log("Analysis started for solution " + sourceSolution.UniqueName + ".");
                progress(
                    new AnalysisProgress
                    {
                        Stage = AnalysisStage.LoadingSourceComponents,
                        Message = "Loading Source solution components...",
                        Metrics = metrics.Snapshot(),
                    }
                );
                var sourceComponents = sourceRepository.GetComponents(
                    sourceSolution.SolutionId,
                    cancellationToken
                );
                log("Source solution components: " + sourceComponents.Count + ".");

                IReadOnlyCollection<SolutionComponentReference> targetComponents =
                    new List<SolutionComponentReference>();
                if (targetSolution != null)
                {
                    progress(
                        new AnalysisProgress
                        {
                            Stage = AnalysisStage.LoadingTargetComponents,
                            Message = "Loading Target solution components...",
                            Metrics = metrics.Snapshot(),
                        }
                    );
                    targetComponents = targetRepository.GetComponents(
                        targetSolution.SolutionId,
                        cancellationToken
                    );
                    log("Target solution components: " + targetComponents.Count + ".");
                }
                else
                {
                    log("The solution was not found in Target; Source remains the reference.");
                }

                metrics.SetTotals(sourceComponents.Count, targetComponents.Count);
                progress(
                    new AnalysisProgress
                    {
                        Stage = AnalysisStage.CorrelatingComponents,
                        Message = "Correlating solution composition and Target existence...",
                        Metrics = metrics.Snapshot(),
                    }
                );

                var correlationService = new ComponentCorrelationService(
                    sourceService,
                    targetService,
                    new DefaultComponentIdentityResolver()
                );
                allResults = correlationService
                    .Correlate(sourceComponents, targetComponents, cancellationToken)
                    .ToList();

                foreach (var result in allResults)
                {
                    metrics.RecordCorrelation(result);
                    if (result.CorrelationStatus != ComponentCorrelationStatus.Matched)
                    {
                        log(
                            "ALM divergence: "
                                + result.CorrelationStatus
                                + ", ComponentType="
                                + result.ComponentTypeName
                                + ", ComponentId="
                                + result.ComponentId.ToString("D")
                                + ", TargetEnvironment="
                                + result.ExistsInTargetEnvironment
                                + "."
                        );
                    }
                }

                var missingComponents = allResults
                    .Where(result => !result.ExistsInTargetEnvironment)
                    .ToList();
                foreach (var result in missingComponents)
                {
                    metrics.RecordProcessed(result);
                    progress(
                        new AnalysisProgress
                        {
                            Stage = AnalysisStage.CorrelatingComponents,
                            Message = result.Status,
                            Result = result,
                            Metrics = metrics.Snapshot(),
                        }
                    );
                }

                var layerCandidates = allResults
                    .Where(result => result.ExistsInTargetEnvironment)
                    .ToList();
                var layerQueryService = new LayerQueryService(targetService, options, log);
                var scanner = new LayerScanner(layerQueryService, options, log);
                progress(
                    new AnalysisProgress
                    {
                        Stage = AnalysisStage.QueryingTargetLayers,
                        Message =
                            "Checking the Target Active solution for "
                            + layerCandidates.Count
                            + " component(s) in Target...",
                        Metrics = metrics.Snapshot(),
                    }
                );
                scanner.Scan(
                    layerCandidates,
                    metrics,
                    (result, snapshot) =>
                        progress(
                            new AnalysisProgress
                            {
                                Stage = AnalysisStage.QueryingTargetLayers,
                                Message = result.Status,
                                Result = result,
                                Metrics = snapshot,
                            }
                        ),
                    cancellationToken
                );

                progress(
                    new AnalysisProgress
                    {
                        Stage = AnalysisStage.FinalizingResults,
                        Message = "Finalizing metrics and analysis results...",
                        Metrics = metrics.Snapshot(),
                    }
                );
                var finalMetrics = metrics.Snapshot();
                log(
                    "Analysis completed. Processed="
                        + finalMetrics.ProcessedComponents
                        + ", ActiveLayers="
                        + finalMetrics.ActiveLayers
                        + ", Errors="
                        + finalMetrics.Errors
                        + ", Batches="
                        + finalMetrics.Batches
                        + ", Retries="
                        + finalMetrics.Retries
                        + "."
                );
                return new AnalysisExecutionResult { Results = allResults, Metrics = finalMetrics };
            }
            catch (OperationCanceledException)
            {
                log("Analysis cancelled by the user.");
                return new AnalysisExecutionResult
                {
                    Results = allResults,
                    Metrics = metrics.Snapshot(),
                    WasCancelled = true,
                };
            }
        }
    }
}
