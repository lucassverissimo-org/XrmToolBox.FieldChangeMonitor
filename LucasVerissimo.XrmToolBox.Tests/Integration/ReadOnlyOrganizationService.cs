using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.Tests.Integration
{
    internal sealed class ReadOnlyOrganizationService : IOrganizationService
    {
        private readonly IOrganizationService innerService;

        public ReadOnlyOrganizationService(IOrganizationService innerService)
        {
            this.innerService =
                innerService ?? throw new ArgumentNullException(nameof(innerService));
        }

        public Guid Create(Entity entity)
        {
            throw CreateWriteBlockedException("Create");
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return innerService.Retrieve(entityName, id, columnSet);
        }

        public void Update(Entity entity)
        {
            throw CreateWriteBlockedException("Update");
        }

        public void Delete(string entityName, Guid id)
        {
            throw CreateWriteBlockedException("Delete");
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            var requestName = request == null ? "null" : request.RequestName;
            throw CreateWriteBlockedException("Execute(" + requestName + ")");
        }

        public void Associate(
            string entityName,
            Guid entityId,
            Relationship relationship,
            EntityReferenceCollection relatedEntities
        )
        {
            throw CreateWriteBlockedException("Associate");
        }

        public void Disassociate(
            string entityName,
            Guid entityId,
            Relationship relationship,
            EntityReferenceCollection relatedEntities
        )
        {
            throw CreateWriteBlockedException("Disassociate");
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return innerService.RetrieveMultiple(query);
        }

        private static InvalidOperationException CreateWriteBlockedException(string operation)
        {
            return new InvalidOperationException(
                operation + " is blocked in real-environment integration tests."
            );
        }
    }
}
