using System;
using Microsoft.Xrm.Tooling.Connector;

namespace LucasVerissimo.XrmToolBox.Tests.Integration
{
    internal static class DataverseConnectionFactory
    {
        public static CrmServiceClient Create(string connectionString, string environmentName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(
                    "A connection string is required.",
                    nameof(connectionString)
                );
            }

            if (string.IsNullOrWhiteSpace(environmentName))
            {
                throw new ArgumentException(
                    "An environment name is required.",
                    nameof(environmentName)
                );
            }

            var client = new CrmServiceClient(connectionString);
            if (client.IsReady)
            {
                return client;
            }

            client.Dispose();
            throw new InvalidOperationException(
                "Unable to connect to the configured "
                    + environmentName
                    + " environment. Check local.settings.json and the Dataverse credentials."
            );
        }
    }
}
