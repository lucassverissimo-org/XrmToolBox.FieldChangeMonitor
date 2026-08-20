using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models;
using LucasVerissimo.XrmToolBox.Shared.BusinessLogic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Services
{
    /// <summary>
    /// Provides metadata formatted for the Usage Explorer selectors.
    /// Dataverse communication is delegated to the shared business layer.
    /// </summary>
    internal sealed class MetadataService
    {
        private readonly DataverseMetadataService dataverseMetadataService;
        private readonly Dictionary<string, IReadOnlyCollection<MetadataListItem>> cachedColumns;
        private IReadOnlyCollection<MetadataListItem> cachedTables;

        public MetadataService(IOrganizationService organizationService)
        {
            dataverseMetadataService = new DataverseMetadataService(organizationService);
            cachedColumns = new Dictionary<string, IReadOnlyCollection<MetadataListItem>>(
                StringComparer.OrdinalIgnoreCase
            );
        }

        public IReadOnlyCollection<MetadataListItem> GetTables(CancellationToken cancellationToken)
        {
            if (cachedTables != null)
            {
                return cachedTables;
            }

            cachedTables = dataverseMetadataService
                .GetEntities(cancellationToken)
                .Select(CreateTableListItem)
                .OrderBy(item => item.DisplayName)
                .ToList();

            return cachedTables;
        }

        public IReadOnlyCollection<MetadataListItem> GetColumns(
            string tableLogicalName,
            CancellationToken cancellationToken
        )
        {
            IReadOnlyCollection<MetadataListItem> columns;
            if (cachedColumns.TryGetValue(tableLogicalName, out columns))
            {
                return columns;
            }

            var entity = dataverseMetadataService.GetEntity(
                tableLogicalName,
                EntityFilters.Attributes,
                cancellationToken
            );

            columns = entity
                .Attributes.Where(attribute => !string.IsNullOrWhiteSpace(attribute.LogicalName))
                .Select(CreateColumnListItem)
                .OrderBy(item => item.DisplayName)
                .ToList();

            cachedColumns[tableLogicalName] = columns;
            return columns;
        }

        private static MetadataListItem CreateTableListItem(EntityMetadata entity)
        {
            return new MetadataListItem
            {
                LogicalName = entity.LogicalName,
                DisplayName = MetadataLabelResolver.GetDisplayName(entity),
                ObjectTypeCode = entity.ObjectTypeCode,
            };
        }

        private static MetadataListItem CreateColumnListItem(AttributeMetadata attribute)
        {
            return new MetadataListItem
            {
                LogicalName = attribute.LogicalName,
                DisplayName = MetadataLabelResolver.GetDisplayName(attribute),
            };
        }
    }

    internal sealed class WorkflowRepository
    {
        private readonly DataverseQueryService dataverseQueryService;
        private IReadOnlyCollection<Entity> cachedWorkflows;

        public WorkflowRepository(IOrganizationService organizationService)
        {
            dataverseQueryService = new DataverseQueryService(organizationService);
        }

        public IReadOnlyCollection<Entity> GetAll(CancellationToken cancellationToken)
        {
            if (cachedWorkflows != null)
            {
                return cachedWorkflows;
            }

            var query = new QueryExpression("workflow")
            {
                ColumnSet = new ColumnSet(
                    "workflowid",
                    "name",
                    "uniquename",
                    "category",
                    "primaryentity",
                    "statecode",
                    "statuscode",
                    "modifiedon",
                    "triggeronupdateattributelist",
                    "clientdata",
                    "xaml",
                    "description",
                    "ismanaged"
                ),
            };

            query.Criteria.AddCondition("category", ConditionOperator.In, 0, 2, 4, 5);
            cachedWorkflows = dataverseQueryService.RetrieveAll(query, cancellationToken);

            return cachedWorkflows;
        }
    }
}
