using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.Shared.BusinessLogic;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SolutionComponentReference = LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models.SolutionComponentReference;
using SolutionInfo = LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models.SolutionInfo;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Services
{
    internal sealed class SolutionRepository
    {
        private readonly DataverseQueryService queryService;

        public SolutionRepository(IOrganizationService organizationService)
        {
            if (organizationService == null)
            {
                throw new ArgumentNullException(nameof(organizationService));
            }

            queryService = new DataverseQueryService(organizationService);
        }

        public IReadOnlyCollection<SolutionInfo> GetSolutions(CancellationToken cancellationToken)
        {
            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet(
                    "solutionid",
                    "friendlyname",
                    "uniquename",
                    "version",
                    "ismanaged"
                ),
            };
            query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);
            query.Orders.Add(new OrderExpression("friendlyname", OrderType.Ascending));

            return queryService
                .RetrieveAll(query, cancellationToken)
                .Select(CreateSolutionInfo)
                .Where(solution => !string.IsNullOrWhiteSpace(solution.UniqueName))
                .ToList();
        }

        public SolutionInfo FindByUniqueName(string uniqueName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(uniqueName))
            {
                throw new ArgumentException(
                    "The solution unique name is required.",
                    nameof(uniqueName)
                );
            }

            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet(
                    "solutionid",
                    "friendlyname",
                    "uniquename",
                    "version",
                    "ismanaged"
                ),
            };
            query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, uniqueName);

            var entity = queryService.RetrieveAll(query, cancellationToken).FirstOrDefault();
            return entity == null ? null : CreateSolutionInfo(entity);
        }

        public IReadOnlyCollection<SolutionComponentReference> GetComponents(
            Guid solutionId,
            CancellationToken cancellationToken
        )
        {
            if (solutionId == Guid.Empty)
            {
                throw new ArgumentException(
                    "The solution identifier is required.",
                    nameof(solutionId)
                );
            }

            var query = new QueryExpression("solutioncomponent")
            {
                ColumnSet = new ColumnSet(
                    "solutioncomponentid",
                    "objectid",
                    "componenttype",
                    "rootsolutioncomponentid",
                    "rootcomponentbehavior"
                ),
            };
            query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, solutionId);

            return queryService
                .RetrieveAll(query, cancellationToken)
                .Select(CreateComponentReference)
                .ToList();
        }

        private static SolutionInfo CreateSolutionInfo(Entity entity)
        {
            return new SolutionInfo
            {
                SolutionId = entity.Id,
                FriendlyName =
                    entity.GetAttributeValue<string>("friendlyname")
                    ?? entity.GetAttributeValue<string>("uniquename"),
                UniqueName = entity.GetAttributeValue<string>("uniquename"),
                Version = entity.GetAttributeValue<string>("version") ?? string.Empty,
                IsManaged = entity.GetAttributeValue<bool>("ismanaged"),
            };
        }

        private static SolutionComponentReference CreateComponentReference(Entity entity)
        {
            var componentType = entity.GetAttributeValue<OptionSetValue>("componenttype");
            var rootBehavior = entity.GetAttributeValue<OptionSetValue>("rootcomponentbehavior");

            return new SolutionComponentReference
            {
                SolutionComponentId = entity.Id,
                ObjectId = entity.GetAttributeValue<Guid?>("objectid"),
                ComponentType = componentType == null ? -1 : componentType.Value,
                RootSolutionComponentId = entity.GetAttributeValue<Guid?>(
                    "rootsolutioncomponentid"
                ),
                RootComponentBehavior = rootBehavior == null ? (int?)null : rootBehavior.Value,
                FormattedComponentTypeName = entity.FormattedValues.Contains("componenttype")
                    ? entity.FormattedValues["componenttype"]
                    : null,
            };
        }
    }
}
