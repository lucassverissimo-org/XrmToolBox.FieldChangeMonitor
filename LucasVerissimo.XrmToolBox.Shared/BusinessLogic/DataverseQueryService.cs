using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.Shared.BusinessLogic
{
    /// <summary>
    /// Executes paged Dataverse queries and returns all matching records.
    /// </summary>
    public sealed class DataverseQueryService
    {
        private const int DefaultPageSize = 5000;
        private readonly IOrganizationService organizationService;

        public DataverseQueryService(IOrganizationService organizationService)
        {
            this.organizationService =
                organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        }

        public IReadOnlyCollection<Entity> RetrieveAll(
            QueryExpression query,
            CancellationToken cancellationToken,
            int pageSize = DefaultPageSize
        )
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            var originalPageInfo = query.PageInfo;

            try
            {
                var records = new List<Entity>();
                query.PageInfo = new PagingInfo { Count = pageSize, PageNumber = 1 };

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var page = organizationService.RetrieveMultiple(query);
                    records.AddRange(page.Entities);

                    if (!page.MoreRecords)
                    {
                        return records;
                    }

                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = page.PagingCookie;
                }
            }
            finally
            {
                // A caller may reuse the QueryExpression after this operation.
                query.PageInfo = originalPageInfo;
            }
        }
    }
}
