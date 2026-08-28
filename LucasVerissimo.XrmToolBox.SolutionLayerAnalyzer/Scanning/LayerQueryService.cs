using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Scanning
{
    internal sealed class LayerQueryResult
    {
        public IReadOnlyCollection<LayerInfo> Layers { get; set; }

        public Exception Error { get; set; }
    }

    internal sealed class LayerQueryService
    {
        private sealed class LayerRequestGroup
        {
            public IReadOnlyCollection<LayerAnalysisResult> Components { get; set; }
        }

        private sealed class LayerBatchRequest
        {
            public ExecuteMultipleRequest Request { get; set; }

            public IReadOnlyList<LayerRequestGroup> Groups { get; set; }
        }

        private readonly IOrganizationService organizationService;
        private readonly DataverseRetryPolicy retryPolicy;
        private readonly Action<string> log;
        private readonly object activeSolutionSync = new object();
        private Guid? activeSolutionId;

        public LayerQueryService(
            IOrganizationService organizationService,
            AnalyzerOptions options,
            Action<string> log
        )
        {
            this.organizationService =
                organizationService ?? throw new ArgumentNullException(nameof(organizationService));
            var validatedOptions = options ?? throw new ArgumentNullException(nameof(options));
            this.log = log ?? delegate { };
            retryPolicy = new DataverseRetryPolicy(validatedOptions, this.log);
        }

        public IReadOnlyDictionary<ComponentIdentity, LayerQueryResult> QueryBatch(
            IReadOnlyCollection<LayerAnalysisResult> components,
            AnalysisMetrics metrics,
            CancellationToken cancellationToken
        )
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            var results = new Dictionary<ComponentIdentity, LayerQueryResult>();
            if (components.Count == 0)
            {
                return results;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var activeSolution = GetActiveSolutionId(cancellationToken);
            var batchRequest = CreateExecuteMultipleRequest(components, activeSolution);
            var response = retryPolicy.Execute(
                () => (ExecuteMultipleResponse)organizationService.Execute(batchRequest.Request),
                "Active solution component batch",
                metrics,
                cancellationToken
            );

            var responsesByIndex = response.Responses.ToDictionary(responseItem =>
                responseItem.RequestIndex
            );
            for (var index = 0; index < batchRequest.Groups.Count; index++)
            {
                var requestGroup = batchRequest.Groups[index];
                ExecuteMultipleResponseItem responseItem;
                if (!responsesByIndex.TryGetValue(index, out responseItem))
                {
                    foreach (var component in requestGroup.Components)
                    {
                        results[CreateIdentity(component)] = new LayerQueryResult
                        {
                            Layers = new List<LayerInfo>(),
                            Error = new InvalidOperationException(
                                "Dataverse returned no response for the Active solution query."
                            ),
                        };
                    }

                    continue;
                }

                if (responseItem.Fault != null)
                {
                    if (IsTimeout(responseItem.Fault))
                    {
                        metrics.RecordTimeout();
                    }

                    if (IsThrottling(responseItem.Fault))
                    {
                        metrics.RecordThrottling();
                    }

                    foreach (var component in requestGroup.Components)
                    {
                        results[CreateIdentity(component)] = new LayerQueryResult
                        {
                            Layers = new List<LayerInfo>(),
                            Error = new InvalidOperationException(responseItem.Fault.Message),
                        };
                    }

                    continue;
                }

                var retrieveResponse = responseItem.Response as RetrieveMultipleResponse;
                var activeComponentIds = new HashSet<Guid>();
                if (retrieveResponse != null)
                {
                    foreach (var entity in retrieveResponse.EntityCollection.Entities)
                    {
                        var objectId = entity.GetAttributeValue<Guid?>("objectid");
                        if (objectId.HasValue)
                        {
                            activeComponentIds.Add(objectId.Value);
                        }
                    }
                }

                foreach (var component in requestGroup.Components)
                {
                    var layers = new List<LayerInfo>();
                    if (activeComponentIds.Contains(component.ComponentId))
                    {
                        layers.Add(CreateActiveLayer(component));
                    }

                    results[CreateIdentity(component)] = new LayerQueryResult { Layers = layers };
                }
            }

            return results;
        }

        public LayerQueryResult QuerySingle(
            LayerAnalysisResult component,
            CancellationToken cancellationToken
        )
        {
            var metrics = new AnalysisMetrics();
            return QueryBatch(new[] { component }, metrics, cancellationToken).Values.Single();
        }

        private static LayerBatchRequest CreateExecuteMultipleRequest(
            IReadOnlyCollection<LayerAnalysisResult> components,
            Guid activeSolutionId
        )
        {
            var request = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = true,
                },
                Requests = new OrganizationRequestCollection(),
            };

            var requestGroups = new List<LayerRequestGroup>();
            foreach (var componentTypeGroup in components.GroupBy(item => item.ComponentType))
            {
                var groupedComponents = componentTypeGroup.ToList();
                var query = CreateActiveLayerQuery(groupedComponents, activeSolutionId);
                var retrieveRequest = new RetrieveMultipleRequest { Query = query };
                request.Requests.Add(retrieveRequest);
                requestGroups.Add(new LayerRequestGroup { Components = groupedComponents });
            }

            return new LayerBatchRequest { Request = request, Groups = requestGroups };
        }

        internal static QueryExpression CreateActiveLayerQuery(
            IReadOnlyCollection<LayerAnalysisResult> components,
            Guid activeSolutionId
        )
        {
            var firstComponent = components.First();
            var query = new QueryExpression("solutioncomponent")
            {
                ColumnSet = new ColumnSet("objectid"),
            };
            query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, activeSolutionId);
            query.Criteria.AddCondition(
                "componenttype",
                ConditionOperator.Equal,
                firstComponent.ComponentType
            );
            query.Criteria.AddCondition(
                "objectid",
                ConditionOperator.In,
                components.Select(component => (object)component.ComponentId).ToArray()
            );
            return query;
        }

        private static ComponentIdentity CreateIdentity(LayerAnalysisResult component)
        {
            return new ComponentIdentity(component.ComponentType, component.ComponentId);
        }

        private Guid GetActiveSolutionId(CancellationToken cancellationToken)
        {
            if (activeSolutionId.HasValue)
            {
                return activeSolutionId.Value;
            }

            lock (activeSolutionSync)
            {
                if (activeSolutionId.HasValue)
                {
                    return activeSolutionId.Value;
                }

                cancellationToken.ThrowIfCancellationRequested();
                var query = new QueryExpression("solution")
                {
                    ColumnSet = new ColumnSet("solutionid"),
                    TopCount = 1,
                };
                query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, "Active");
                var response = retryPolicy.Execute(
                    () => organizationService.RetrieveMultiple(query),
                    "Find Active solution",
                    null,
                    cancellationToken
                );
                var solution = response.Entities.FirstOrDefault();
                if (solution == null)
                {
                    throw new InvalidOperationException(
                        "The internal Active solution was not found in the Target environment."
                    );
                }

                activeSolutionId = solution.Id;
                return activeSolutionId.Value;
            }
        }

        private static LayerInfo CreateActiveLayer(LayerAnalysisResult component)
        {
            return new LayerInfo
            {
                ComponentId = component.ComponentId.ToString("D"),
                ComponentName = component.ComponentName,
                Order = 0,
                SolutionComponentName = component.LayerComponentName,
                SolutionName = "Active",
            };
        }

        private static bool IsTransient(OrganizationServiceFault fault)
        {
            return IsThrottling(fault)
                || IsTimeout(fault)
                || Contains(fault?.Message, "temporar")
                || Contains(fault?.Message, "server busy");
        }

        private static bool IsThrottling(OrganizationServiceFault fault)
        {
            return fault != null
                && (
                    fault.ErrorCode == -2147015902
                    || fault.ErrorCode == -2147015903
                    || fault.ErrorCode == -2147015898
                    || Contains(fault.Message, "429")
                    || Contains(fault.Message, "throttl")
                    || Contains(fault.Message, "rate limit")
                );
        }

        private static bool IsTimeout(OrganizationServiceFault fault)
        {
            return fault != null
                && (
                    Contains(fault.Message, "timeout")
                    || Contains(fault.Message, "timed out")
                    || Contains(fault.Message, "tempo limite")
                    || Contains(fault.Message, "canal de solicitação")
                );
        }

        private static bool Contains(string value, string text)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
