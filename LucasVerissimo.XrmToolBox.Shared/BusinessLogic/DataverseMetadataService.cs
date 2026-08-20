using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace LucasVerissimo.XrmToolBox.Shared.BusinessLogic
{
    /// <summary>
    /// Centralizes Dataverse metadata requests used by the XrmToolBox plugins.
    /// UI-specific filtering and presentation remain in each plugin.
    /// </summary>
    public sealed class DataverseMetadataService
    {
        private readonly IOrganizationService organizationService;

        public DataverseMetadataService(IOrganizationService organizationService)
        {
            this.organizationService =
                organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        }

        public IReadOnlyCollection<EntityMetadata> GetEntities(
            CancellationToken cancellationToken,
            bool includeIntersectEntities = false,
            bool retrieveAsIfPublished = true
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = retrieveAsIfPublished,
            };

            var response = (RetrieveAllEntitiesResponse)organizationService.Execute(request);
            var entities = response.EntityMetadata.Where(entity =>
                !string.IsNullOrWhiteSpace(entity.LogicalName)
            );

            if (!includeIntersectEntities)
            {
                entities = entities.Where(entity => entity.IsIntersect != true);
            }

            return entities.ToList();
        }

        public EntityMetadata GetEntity(
            string logicalName,
            EntityFilters entityFilters,
            CancellationToken cancellationToken,
            bool retrieveAsIfPublished = true
        )
        {
            if (string.IsNullOrWhiteSpace(logicalName))
            {
                throw new ArgumentException(
                    "The table logical name is required.",
                    nameof(logicalName)
                );
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new RetrieveEntityRequest
            {
                LogicalName = logicalName,
                EntityFilters = entityFilters,
                RetrieveAsIfPublished = retrieveAsIfPublished,
            };

            var response = (RetrieveEntityResponse)organizationService.Execute(request);
            return response.EntityMetadata;
        }
    }
}
