using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Services
{
    internal sealed class DefaultSolutionNavigationContext
    {
        public DefaultSolutionNavigationContext(string environmentId, Guid solutionId)
        {
            EnvironmentId = environmentId;
            SolutionId = solutionId;
        }

        public string EnvironmentId { get; }

        public Guid SolutionId { get; }
    }

    internal sealed class DefaultSolutionNavigationService
    {
        private readonly string environmentId;
        private readonly IOrganizationService organizationService;
        private DefaultSolutionNavigationContext cachedContext;

        public DefaultSolutionNavigationService(
            IOrganizationService organizationService,
            string environmentId
        )
        {
            this.organizationService =
                organizationService ?? throw new ArgumentNullException(nameof(organizationService));
            this.environmentId = environmentId;
        }

        public DefaultSolutionNavigationContext GetContext()
        {
            if (cachedContext != null)
            {
                return cachedContext;
            }

            if (string.IsNullOrWhiteSpace(environmentId))
            {
                return null;
            }

            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet("solutionid"),
                TopCount = 1,
            };
            query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, "Default");

            var solutions = organizationService.RetrieveMultiple(query).Entities;
            if (solutions.Count == 0)
            {
                return null;
            }

            cachedContext = new DefaultSolutionNavigationContext(environmentId, solutions[0].Id);
            return cachedContext;
        }
    }
}
