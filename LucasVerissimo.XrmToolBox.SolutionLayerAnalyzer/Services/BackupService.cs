using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Services
{
    internal sealed class BackupService
    {
        private readonly IOrganizationService organizationService;
        private readonly DataverseRetryPolicy retryPolicy;
        private readonly Action<string> log;

        public BackupService(
            IOrganizationService organizationService,
            AnalyzerOptions options,
            Action<string> log
        )
        {
            this.organizationService =
                organizationService ?? throw new ArgumentNullException(nameof(organizationService));
            this.log = log ?? delegate { };
            retryPolicy = new DataverseRetryPolicy(
                options ?? throw new ArgumentNullException(nameof(options)),
                this.log
            );
        }

        public BackupResult CreateBackup(
            IReadOnlyCollection<LayerAnalysisResult> components,
            string filePath,
            CancellationToken cancellationToken
        )
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            if (components.Count == 0)
            {
                throw new ArgumentException("Select at least one component.", nameof(components));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("The backup file path is required.", nameof(filePath));
            }

            var uniqueName =
                "LayerAnalyzerBackup_"
                + DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                + "_"
                + Guid.NewGuid().ToString("N").Substring(0, 8);
            var result = new BackupResult
            {
                Requested = true,
                Status = BackupStatus.Failed,
                FilePath = filePath,
                TemporarySolutionUniqueName = uniqueName,
                SelectedComponents = components.Count,
            };
            Guid temporarySolutionId = Guid.Empty;
            var componentErrors = new List<string>();

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                log("Backup started for " + components.Count + " selected component(s).");
                temporarySolutionId = CreateTemporarySolution(uniqueName, cancellationToken);

                foreach (var component in components)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        AddComponent(uniqueName, component, cancellationToken);
                        result.AddedComponents++;
                    }
                    catch (Exception error)
                    {
                        componentErrors.Add(component.ComponentName + ": " + error.Message);
                        log(
                            "Backup component failed for "
                                + component.ComponentName
                                + ": "
                                + error.Message
                        );
                    }
                }

                if (result.AddedComponents == 0)
                {
                    throw new InvalidOperationException(
                        "None of the selected components could be added to the temporary backup solution."
                    );
                }

                var exportResponse = retryPolicy.Execute(
                    () =>
                        (ExportSolutionResponse)
                            organizationService.Execute(
                                new ExportSolutionRequest
                                {
                                    Managed = false,
                                    SolutionName = uniqueName,
                                }
                            ),
                    "Export temporary backup solution",
                    null,
                    cancellationToken
                );
                File.WriteAllBytes(filePath, exportResponse.ExportSolutionFile);

                result.Status =
                    componentErrors.Count == 0
                        ? BackupStatus.Succeeded
                        : BackupStatus.PartiallySucceeded;
                result.Error =
                    componentErrors.Count == 0
                        ? null
                        : string.Join(Environment.NewLine, componentErrors);
                log(
                    "Backup completed. Added="
                        + result.AddedComponents
                        + "/"
                        + result.SelectedComponents
                        + ", Status="
                        + result.Status
                        + "."
                );
                return result;
            }
            catch (OperationCanceledException)
            {
                result.Status = BackupStatus.Cancelled;
                result.Error = "Backup cancelled by the user.";
                log("Backup cancelled by the user.");
                return result;
            }
            catch (Exception error)
            {
                result.Status = BackupStatus.Failed;
                result.Error = error.Message;
                log("Backup failed: " + error.Message);
                return result;
            }
            finally
            {
                if (temporarySolutionId != Guid.Empty)
                {
                    try
                    {
                        organizationService.Delete("solution", temporarySolutionId);
                        log("Temporary backup solution removed.");
                    }
                    catch (Exception cleanupError)
                    {
                        log("Temporary backup solution cleanup failed: " + cleanupError.Message);
                    }
                }
            }
        }

        private Guid CreateTemporarySolution(string uniqueName, CancellationToken cancellationToken)
        {
            var publisherId = GetDefaultPublisherId(cancellationToken);
            var solution = new Entity("solution");
            solution["friendlyname"] =
                "Solution Layer Analyzer Backup " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            solution["uniquename"] = uniqueName;
            solution["version"] = "1.0.0.0";
            solution["publisherid"] = new EntityReference("publisher", publisherId);

            return retryPolicy.Execute(
                () => organizationService.Create(solution),
                "Create temporary backup solution",
                null,
                cancellationToken
            );
        }

        private Guid GetDefaultPublisherId(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet("publisherid"),
                TopCount = 1,
            };
            query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, "Default");
            var defaultSolution = retryPolicy.Execute(
                () => organizationService.RetrieveMultiple(query).Entities.FirstOrDefault(),
                "Retrieve Target default publisher",
                null,
                cancellationToken
            );
            var publisher = defaultSolution?.GetAttributeValue<EntityReference>("publisherid");
            if (publisher == null)
            {
                throw new InvalidOperationException(
                    "The Target default publisher could not be resolved."
                );
            }

            return publisher.Id;
        }

        private void AddComponent(
            string solutionUniqueName,
            LayerAnalysisResult component,
            CancellationToken cancellationToken
        )
        {
            var request = new AddSolutionComponentRequest
            {
                AddRequiredComponents = false,
                ComponentId = component.ComponentId,
                ComponentType = component.ComponentType,
                DoNotIncludeSubcomponents = true,
                SolutionUniqueName = solutionUniqueName,
            };

            retryPolicy.Execute(
                () => organizationService.Execute(request),
                "Add backup component " + component.ComponentId.ToString("D"),
                null,
                cancellationToken
            );
        }
    }
}
