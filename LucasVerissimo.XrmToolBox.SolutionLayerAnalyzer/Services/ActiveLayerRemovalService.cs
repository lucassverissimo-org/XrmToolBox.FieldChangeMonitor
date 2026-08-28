using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Scanning;
using Microsoft.Xrm.Sdk;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Services
{
    internal sealed class ActiveLayerRemovalService
    {
        private readonly IOrganizationService organizationService;
        private readonly LayerQueryService layerQueryService;
        private readonly DataverseRetryPolicy retryPolicy;
        private readonly Action<string> log;

        public ActiveLayerRemovalService(
            IOrganizationService organizationService,
            AnalyzerOptions options,
            Action<string> log
        )
        {
            this.organizationService =
                organizationService ?? throw new ArgumentNullException(nameof(organizationService));
            this.log = log ?? delegate { };
            var validatedOptions = options ?? throw new ArgumentNullException(nameof(options));
            layerQueryService = new LayerQueryService(
                organizationService,
                validatedOptions,
                this.log
            );
            retryPolicy = new DataverseRetryPolicy(validatedOptions, this.log);
        }

        public IReadOnlyCollection<RemovalResult> Remove(
            IReadOnlyCollection<LayerAnalysisResult> components,
            BackupStatus backupStatus,
            bool userConfirmedRisk,
            Action<RemovalProgress> progress,
            CancellationToken cancellationToken
        )
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            var results = new List<RemovalResult>();
            var current = 0;
            log(
                "Active layer removal started for "
                    + components.Count
                    + " component(s). BackupStatus="
                    + backupStatus
                    + ", UserConfirmedRisk="
                    + userConfirmedRisk
                    + "."
            );

            foreach (var component in components)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    log("Active layer removal cancelled by the user.");
                    break;
                }

                current++;
                var result = RemoveOne(
                    component,
                    backupStatus,
                    userConfirmedRisk,
                    current,
                    components.Count,
                    progress,
                    cancellationToken
                );
                results.Add(result);
                progress(
                    new RemovalProgress
                    {
                        Stage = RemovalStage.ComponentCompleted,
                        Current = current,
                        Total = components.Count,
                        Result = result,
                        Message = component.ComponentName + ": " + result.ValidationStatus,
                    }
                );
            }

            log(
                "Active layer removal completed. Selected="
                    + components.Count
                    + ", Removed="
                    + results.Count(result => result.RemovalStatus == RemovalStatus.Removed)
                    + ", Failed="
                    + results.Count(result => result.RemovalStatus == RemovalStatus.Failed)
                    + "."
            );
            return results;
        }

        private RemovalResult RemoveOne(
            LayerAnalysisResult component,
            BackupStatus backupStatus,
            bool userConfirmedRisk,
            int current,
            int total,
            Action<RemovalProgress> progress,
            CancellationToken cancellationToken
        )
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new RemovalResult
            {
                ComponentId = component.ComponentId,
                ComponentName = component.ComponentName,
                ComponentType = component.ComponentType,
                ComponentTypeName = component.ComponentTypeName,
                ExistsInTargetSolution = component.ExistsInTargetSolution,
                BackupStatus = backupStatus,
                RemovalStatus = RemovalStatus.NotStarted,
                ValidationStatus = ValidationStatus.NotStarted,
                TimestampUtc = DateTime.UtcNow,
                UserConfirmedRisk = userConfirmedRisk,
            };

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var logicalName = ResolveRemovalLogicalName(component);
                if (string.IsNullOrWhiteSpace(logicalName))
                {
                    throw new NotSupportedException(
                        "Active layer removal is not supported for component type "
                            + component.ComponentTypeName
                            + "."
                    );
                }

                log(
                    "Removing active layer for "
                        + component.ComponentTypeName
                        + " "
                        + component.ComponentId.ToString("D")
                        + ". TargetSolutionMembership="
                        + component.ExistsInTargetSolution
                        + "."
                );
                progress(
                    new RemovalProgress
                    {
                        Stage = RemovalStage.RemovingActiveLayer,
                        Current = current,
                        Total = total,
                        Message = "Removing " + component.ComponentName + " in Target...",
                    }
                );
                var request = new OrganizationRequest("RemoveActiveCustomization");
                request["LogicalName"] = logicalName;
                request["Id"] = component.ComponentId;
                retryPolicy.Execute(
                    () => organizationService.Execute(request),
                    "Remove active customization " + component.ComponentId.ToString("D"),
                    null,
                    cancellationToken
                );
                result.RemovalStatus = RemovalStatus.Removed;

                progress(
                    new RemovalProgress
                    {
                        Stage = RemovalStage.ValidatingRemoval,
                        Current = current,
                        Total = total,
                        Message = "Validating " + component.ComponentName + " in Target...",
                    }
                );
                var validation = layerQueryService.QuerySingle(component, cancellationToken);
                if (validation.Error != null)
                {
                    result.ValidationStatus = ValidationStatus.RemovalSucceededButValidationFailed;
                    result.Error = validation.Error.Message;
                }
                else if (validation.Layers.Any(layer => layer.IsActiveLayer))
                {
                    result.ValidationStatus = ValidationStatus.ActiveLayerStillPresent;
                    result.Error =
                        "Dataverse accepted the removal request, but an active layer is still present.";
                }
                else
                {
                    result.ValidationStatus = ValidationStatus.RemovedAndValidated;
                }

                log(
                    "Active layer removal validation for "
                        + component.ComponentId.ToString("D")
                        + ": "
                        + result.ValidationStatus
                        + "."
                );
            }
            catch (OperationCanceledException)
            {
                result.RemovalStatus = RemovalStatus.Cancelled;
                result.ValidationStatus = ValidationStatus.Cancelled;
                result.Error = "Operation cancelled by the user.";
            }
            catch (Exception error)
            {
                result.RemovalStatus = RemovalStatus.Failed;
                result.Error = error.Message;
                log(
                    "Active layer removal failed for "
                        + component.ComponentId.ToString("D")
                        + ": "
                        + error.Message
                );
            }
            finally
            {
                stopwatch.Stop();
                result.DurationMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        private static string ResolveRemovalLogicalName(LayerAnalysisResult component)
        {
            var definition = ComponentTypeRegistry.Get(component.ComponentType);
            if (!string.IsNullOrWhiteSpace(definition.RemovalLogicalName))
            {
                return definition.RemovalLogicalName;
            }

            return string.IsNullOrWhiteSpace(component.LayerComponentName)
                ? null
                : component.LayerComponentName.Replace(" ", string.Empty).ToLowerInvariant();
        }
    }
}
