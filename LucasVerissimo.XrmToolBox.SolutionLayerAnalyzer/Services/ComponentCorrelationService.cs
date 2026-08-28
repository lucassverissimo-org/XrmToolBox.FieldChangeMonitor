using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.Shared.BusinessLogic;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using SolutionComponentReference = LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models.SolutionComponentReference;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Services
{
    internal interface IComponentIdentityResolver
    {
        ComponentIdentity Resolve(SolutionComponentReference component);
    }

    internal sealed class DefaultComponentIdentityResolver : IComponentIdentityResolver
    {
        public ComponentIdentity Resolve(SolutionComponentReference component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            return component.ObjectId.HasValue
                ? new ComponentIdentity(component.ComponentType, component.ObjectId.Value)
                : null;
        }
    }

    internal sealed class ComponentCorrelationService
    {
        private sealed class MetadataNameRequest
        {
            public ComponentIdentity Identity { get; set; }

            public OrganizationRequest Request { get; set; }
        }

        private readonly IOrganizationService sourceService;
        private readonly IOrganizationService targetService;
        private readonly IComponentIdentityResolver identityResolver;
        private readonly Dictionary<ComponentIdentity, string> nameCache =
            new Dictionary<ComponentIdentity, string>();

        public ComponentCorrelationService(
            IOrganizationService sourceService,
            IOrganizationService targetService,
            IComponentIdentityResolver identityResolver
        )
        {
            this.sourceService =
                sourceService ?? throw new ArgumentNullException(nameof(sourceService));
            this.targetService =
                targetService ?? throw new ArgumentNullException(nameof(targetService));
            this.identityResolver =
                identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
        }

        public IReadOnlyCollection<LayerAnalysisResult> Correlate(
            IReadOnlyCollection<SolutionComponentReference> sourceComponents,
            IReadOnlyCollection<SolutionComponentReference> targetComponents,
            CancellationToken cancellationToken
        )
        {
            if (sourceComponents == null)
            {
                throw new ArgumentNullException(nameof(sourceComponents));
            }

            targetComponents = targetComponents ?? new List<SolutionComponentReference>();

            var sourceByIdentity = ToIdentityMap(sourceComponents);
            var targetByIdentity = ToIdentityMap(targetComponents);
            var sourceIdentities = new HashSet<ComponentIdentity>(sourceByIdentity.Keys);
            var targetIdentities = new HashSet<ComponentIdentity>(targetByIdentity.Keys);

            ResolveRecordNames(sourceService, sourceIdentities, cancellationToken);
            ResolveRecordNames(targetService, targetIdentities, cancellationToken);

            var sourceByCorrelationKey = ToCorrelationMap(sourceByIdentity);
            var targetByCorrelationKey = ToCorrelationMap(targetByIdentity);
            var allCorrelationKeys = new HashSet<string>(
                sourceByCorrelationKey.Keys,
                StringComparer.OrdinalIgnoreCase
            );
            allCorrelationKeys.UnionWith(targetByCorrelationKey.Keys);

            var unmatchedSourceIdentities = sourceByCorrelationKey
                .Where(pair => !targetByCorrelationKey.ContainsKey(pair.Key))
                .Select(pair => identityResolver.Resolve(pair.Value))
                .Where(identity => identity != null)
                .ToList();
            var targetEnvironmentIdentities = FindInTargetEnvironment(
                unmatchedSourceIdentities,
                cancellationToken
            );
            targetEnvironmentIdentities.UnionWith(targetIdentities);

            var results = new List<LayerAnalysisResult>();
            foreach (var correlationKey in allCorrelationKeys)
            {
                cancellationToken.ThrowIfCancellationRequested();

                SolutionComponentReference source;
                SolutionComponentReference target;
                sourceByCorrelationKey.TryGetValue(correlationKey, out source);
                targetByCorrelationKey.TryGetValue(correlationKey, out target);
                var sourceIdentity = source == null ? null : identityResolver.Resolve(source);
                var targetIdentity = target == null ? null : identityResolver.Resolve(target);
                var existsInSource = source != null;
                var existsInTargetSolution = target != null;
                var existsInTargetEnvironment =
                    targetIdentity != null
                    || (
                        sourceIdentity != null
                        && targetEnvironmentIdentities.Contains(sourceIdentity)
                    );
                var targetComponentId = targetIdentity?.ComponentId;
                if (!targetComponentId.HasValue && existsInTargetEnvironment)
                {
                    targetComponentId = sourceIdentity?.ComponentId;
                }

                var resultComponentId =
                    targetComponentId ?? sourceIdentity?.ComponentId ?? Guid.Empty;

                string resolvedName = null;
                if (
                    sourceIdentity == null
                    || !nameCache.TryGetValue(sourceIdentity, out resolvedName)
                )
                {
                    if (targetIdentity != null)
                    {
                        nameCache.TryGetValue(targetIdentity, out resolvedName);
                    }
                }

                results.Add(
                    new LayerAnalysisResult
                    {
                        ComponentType = source?.ComponentType ?? target?.ComponentType ?? -1,
                        ComponentTypeName = ComponentTypeRegistry
                            .Get(source?.ComponentType ?? target?.ComponentType ?? -1)
                            .DisplayName,
                        LayerComponentName = GetLayerComponentName(source, target),
                        ComponentId = resultComponentId,
                        SourceComponentId = source?.ObjectId,
                        TargetComponentId = targetComponentId,
                        ComponentName = string.IsNullOrWhiteSpace(resolvedName)
                            ? resultComponentId.ToString("D")
                            : resolvedName,
                        ExistsInSourceSolution = existsInSource,
                        ExistsInTargetSolution = existsInTargetSolution,
                        ExistsInTargetEnvironment = existsInTargetEnvironment,
                        CorrelationStatus = GetCorrelationStatus(
                            existsInSource,
                            existsInTargetSolution,
                            existsInTargetEnvironment
                        ),
                        Status = existsInTargetEnvironment
                            ? "Pending layer analysis"
                            : "Component not found in Target environment",
                    }
                );
            }

            return results
                .OrderBy(result => result.ComponentTypeName)
                .ThenBy(result => result.ComponentName)
                .ToList();
        }

        private Dictionary<ComponentIdentity, SolutionComponentReference> ToIdentityMap(
            IEnumerable<SolutionComponentReference> components
        )
        {
            var map = new Dictionary<ComponentIdentity, SolutionComponentReference>();
            foreach (var component in components)
            {
                var identity = identityResolver.Resolve(component);
                if (identity != null && !map.ContainsKey(identity))
                {
                    map.Add(identity, component);
                }
            }

            return map;
        }

        private Dictionary<string, SolutionComponentReference> ToCorrelationMap(
            IReadOnlyDictionary<ComponentIdentity, SolutionComponentReference> components
        )
        {
            var map = new Dictionary<string, SolutionComponentReference>(
                StringComparer.OrdinalIgnoreCase
            );
            foreach (var pair in components)
            {
                var key = CreateCorrelationKey(pair.Key);
                if (!map.ContainsKey(key))
                {
                    map.Add(key, pair.Value);
                }
            }

            return map;
        }

        private string CreateCorrelationKey(ComponentIdentity identity)
        {
            string resolvedName;
            if (!nameCache.TryGetValue(identity, out resolvedName))
            {
                return identity.ToString();
            }

            var stableName = GetStableComponentName(identity.ComponentType, resolvedName);
            return identity.ComponentType + ":name:" + stableName;
        }

        internal static string GetStableComponentName(int componentType, string resolvedName)
        {
            var stableName = resolvedName == null ? string.Empty : resolvedName.Trim();
            if (componentType == 1 || componentType == 2 || componentType == 9)
            {
                const string separator = " — ";
                var separatorIndex = stableName.LastIndexOf(separator, StringComparison.Ordinal);
                if (separatorIndex >= 0)
                {
                    stableName = stableName.Substring(separatorIndex + separator.Length).Trim();
                }
            }

            return stableName.ToUpperInvariant();
        }

        private HashSet<ComponentIdentity> FindInTargetEnvironment(
            IEnumerable<ComponentIdentity> identities,
            CancellationToken cancellationToken
        )
        {
            var found = new HashSet<ComponentIdentity>();
            var queryService = new DataverseQueryService(targetService);

            foreach (var typeGroup in identities.GroupBy(identity => identity.ComponentType))
            {
                foreach (var batch in Batch(typeGroup.ToList(), 500))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var query = new QueryExpression("solutioncomponent")
                    {
                        ColumnSet = new ColumnSet("objectid", "componenttype"),
                    };
                    query.Criteria.AddCondition(
                        "componenttype",
                        ConditionOperator.Equal,
                        typeGroup.Key
                    );
                    query.Criteria.AddCondition(
                        "objectid",
                        ConditionOperator.In,
                        batch.Select(identity => (object)identity.ComponentId).ToArray()
                    );

                    foreach (var entity in queryService.RetrieveAll(query, cancellationToken))
                    {
                        var componentType = entity.GetAttributeValue<OptionSetValue>(
                            "componenttype"
                        );
                        var objectId = entity.GetAttributeValue<Guid?>("objectid");
                        if (componentType != null && objectId.HasValue)
                        {
                            found.Add(new ComponentIdentity(componentType.Value, objectId.Value));
                        }
                    }
                }
            }

            return found;
        }

        private void ResolveRecordNames(
            IOrganizationService organizationService,
            IEnumerable<ComponentIdentity> identities,
            CancellationToken cancellationToken
        )
        {
            var queryService = new DataverseQueryService(organizationService);

            foreach (
                var group in identities
                    .Where(identity => !nameCache.ContainsKey(identity))
                    .GroupBy(identity => identity.ComponentType)
            )
            {
                if (ResolveMetadataNames(organizationService, group, cancellationToken))
                {
                    continue;
                }

                var definition = ComponentTypeRegistry.Get(group.Key);
                if (string.IsNullOrWhiteSpace(definition.BackingEntityName))
                {
                    continue;
                }

                foreach (var batch in Batch(group.ToList(), 500))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var query = new QueryExpression(definition.BackingEntityName)
                    {
                        ColumnSet = new ColumnSet(
                            definition.BackingEntityPrimaryId,
                            definition.BackingEntityPrimaryName
                        ),
                    };
                    query.Criteria.AddCondition(
                        definition.BackingEntityPrimaryId,
                        ConditionOperator.In,
                        batch.Select(identity => (object)identity.ComponentId).ToArray()
                    );

                    try
                    {
                        foreach (var entity in queryService.RetrieveAll(query, cancellationToken))
                        {
                            var identity = new ComponentIdentity(group.Key, entity.Id);
                            nameCache[identity] = entity.GetAttributeValue<string>(
                                definition.BackingEntityPrimaryName
                            );
                        }
                    }
                    catch
                    {
                        // Name resolution is best effort and never blocks layer analysis.
                    }
                }
            }
        }

        private bool ResolveMetadataNames(
            IOrganizationService organizationService,
            IEnumerable<ComponentIdentity> identities,
            CancellationToken cancellationToken
        )
        {
            var metadataRequests = identities
                .Select(identity => new MetadataNameRequest
                {
                    Identity = identity,
                    Request = CreateMetadataNameRequest(identity),
                })
                .Where(item => item.Request != null)
                .ToList();

            if (metadataRequests.Count == 0)
            {
                return false;
            }

            foreach (var batch in Batch(metadataRequests, 100))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var request = new ExecuteMultipleRequest
                {
                    Settings = new ExecuteMultipleSettings
                    {
                        ContinueOnError = true,
                        ReturnResponses = true,
                    },
                    Requests = new OrganizationRequestCollection(),
                };

                foreach (var item in batch)
                {
                    request.Requests.Add(item.Request);
                }

                try
                {
                    var response = (ExecuteMultipleResponse)organizationService.Execute(request);
                    foreach (var responseItem in response.Responses)
                    {
                        if (responseItem.Fault != null || responseItem.Response == null)
                        {
                            continue;
                        }

                        var resolvedName = GetMetadataName(responseItem.Response);
                        if (!string.IsNullOrWhiteSpace(resolvedName))
                        {
                            nameCache[batch[responseItem.RequestIndex].Identity] = resolvedName;
                        }
                    }
                }
                catch
                {
                    ResolveMetadataNamesIndividually(organizationService, batch, cancellationToken);
                }
            }

            return true;
        }

        private void ResolveMetadataNamesIndividually(
            IOrganizationService organizationService,
            IEnumerable<MetadataNameRequest> metadataRequests,
            CancellationToken cancellationToken
        )
        {
            foreach (var item in metadataRequests)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var response = organizationService.Execute(item.Request);
                    var resolvedName = GetMetadataName(response);
                    if (!string.IsNullOrWhiteSpace(resolvedName))
                    {
                        nameCache[item.Identity] = resolvedName;
                    }
                }
                catch
                {
                    // Metadata name resolution is best effort and never blocks analysis.
                }
            }
        }

        private static OrganizationRequest CreateMetadataNameRequest(ComponentIdentity identity)
        {
            switch (identity.ComponentType)
            {
                case 1:
                    return new RetrieveEntityRequest
                    {
                        MetadataId = identity.ComponentId,
                        EntityFilters = EntityFilters.Entity,
                        RetrieveAsIfPublished = true,
                    };
                case 2:
                    return new RetrieveAttributeRequest
                    {
                        MetadataId = identity.ComponentId,
                        RetrieveAsIfPublished = true,
                    };
                case 3:
                    return new RetrieveRelationshipRequest
                    {
                        MetadataId = identity.ComponentId,
                        RetrieveAsIfPublished = true,
                    };
                case 9:
                    return new RetrieveOptionSetRequest
                    {
                        MetadataId = identity.ComponentId,
                        RetrieveAsIfPublished = true,
                    };
                default:
                    return null;
            }
        }

        private static string GetMetadataName(OrganizationResponse response)
        {
            var entityResponse = response as RetrieveEntityResponse;
            if (entityResponse != null)
            {
                var entity = entityResponse.EntityMetadata;
                return FormatComponentName(
                    MetadataLabelResolver.GetDisplayName(entity),
                    entity?.LogicalName
                );
            }

            var attributeResponse = response as RetrieveAttributeResponse;
            if (attributeResponse != null)
            {
                var attribute = attributeResponse.AttributeMetadata;
                var qualifiedLogicalName = string.IsNullOrWhiteSpace(attribute?.EntityLogicalName)
                    ? attribute?.LogicalName
                    : attribute.EntityLogicalName + "." + attribute.LogicalName;
                return FormatComponentName(
                    MetadataLabelResolver.GetDisplayName(attribute),
                    qualifiedLogicalName
                );
            }

            var relationshipResponse = response as RetrieveRelationshipResponse;
            if (relationshipResponse != null)
            {
                return relationshipResponse.RelationshipMetadata?.SchemaName;
            }

            var optionSetResponse = response as RetrieveOptionSetResponse;
            if (optionSetResponse != null)
            {
                var optionSet = optionSetResponse.OptionSetMetadata;
                return FormatComponentName(
                    MetadataLabelResolver.GetText(optionSet?.DisplayName, optionSet?.Name),
                    optionSet?.Name
                );
            }

            return string.Empty;
        }

        private static string FormatComponentName(string displayName, string logicalName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return logicalName ?? string.Empty;
            }

            if (
                string.IsNullOrWhiteSpace(logicalName)
                || string.Equals(displayName, logicalName, StringComparison.OrdinalIgnoreCase)
            )
            {
                return displayName;
            }

            return displayName + " — " + logicalName;
        }

        private static ComponentCorrelationStatus GetCorrelationStatus(
            bool existsInSource,
            bool existsInTargetSolution,
            bool existsInTargetEnvironment
        )
        {
            if (existsInSource && existsInTargetSolution)
            {
                return ComponentCorrelationStatus.Matched;
            }

            if (!existsInSource && existsInTargetSolution)
            {
                return ComponentCorrelationStatus.MissingFromSourceSolution;
            }

            return existsInTargetEnvironment
                ? ComponentCorrelationStatus.MissingFromTargetSolution
                : ComponentCorrelationStatus.MissingFromTargetEnvironment;
        }

        private static string GetLayerComponentName(
            SolutionComponentReference source,
            SolutionComponentReference target
        )
        {
            var componentType = source?.ComponentType ?? target?.ComponentType ?? -1;
            var definition = ComponentTypeRegistry.Get(componentType);
            if (!string.Equals(definition.LayerComponentName, "Unknown"))
            {
                return definition.LayerComponentName;
            }

            var formattedName =
                source?.FormattedComponentTypeName ?? target?.FormattedComponentTypeName;
            if (!string.IsNullOrWhiteSpace(formattedName))
            {
                return formattedName.Replace(" ", string.Empty);
            }

            return definition.LayerComponentName;
        }

        private static IEnumerable<List<T>> Batch<T>(IReadOnlyList<T> items, int size)
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
