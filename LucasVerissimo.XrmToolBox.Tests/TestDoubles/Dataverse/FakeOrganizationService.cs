using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.Tests.TestDoubles.Dataverse
{
    internal sealed class FakeOrganizationService : IOrganizationService
    {
        public Func<QueryBase, EntityCollection> RetrieveMultipleHandler { get; set; }

        public Func<OrganizationRequest, OrganizationResponse> ExecuteHandler { get; set; }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            if (RetrieveMultipleHandler == null)
            {
                throw new InvalidOperationException(
                    "No RetrieveMultiple handler was configured for this test."
                );
            }

            return RetrieveMultipleHandler(query);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            if (
                string.Equals(
                    request?.RequestName,
                    "RemoveActiveCustomization",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new InvalidOperationException(
                    "Destructive Dataverse requests are forbidden in automated tests."
                );
            }

            if (ExecuteHandler == null)
            {
                throw new InvalidOperationException(
                    "No Execute handler was configured for this test."
                );
            }

            return ExecuteHandler(request);
        }

        public Guid Create(Entity entity)
        {
            throw new NotSupportedException();
        }

        public void Update(Entity entity)
        {
            throw new NotSupportedException();
        }

        public void Delete(string entityName, Guid id)
        {
            throw new NotSupportedException();
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            throw new NotSupportedException();
        }

        public void Associate(
            string entityName,
            Guid entityId,
            Relationship relationship,
            EntityReferenceCollection relatedEntities
        )
        {
            throw new NotSupportedException();
        }

        public void Disassociate(
            string entityName,
            Guid entityId,
            Relationship relationship,
            EntityReferenceCollection relatedEntities
        )
        {
            throw new NotSupportedException();
        }
    }
}
