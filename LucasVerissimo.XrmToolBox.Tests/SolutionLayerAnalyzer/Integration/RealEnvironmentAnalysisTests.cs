using System;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Services;
using LucasVerissimo.XrmToolBox.Tests.Integration;
using Microsoft.Xrm.Tooling.Connector;
using NUnit.Framework;

namespace LucasVerissimo.XrmToolBox.Tests.SolutionLayerAnalyzer.Integration
{
    [TestFixture]
    [Category("Integration")]
    [NonParallelizable]
    public sealed class RealEnvironmentAnalysisTests
    {
        private CrmServiceClient sourceClient;
        private CrmServiceClient targetClient;
        private ReadOnlyOrganizationService sourceService;
        private ReadOnlyOrganizationService targetService;
        private DataverseIntegrationSettings settings;

        [OneTimeSetUp]
        public void ConnectToConfiguredEnvironments()
        {
            if (!DataverseIntegrationSettings.TryLoad(out settings, out var reason))
            {
                Assert.Ignore(reason);
            }

            sourceClient = DataverseConnectionFactory.Create(
                settings.SourceConnectionString,
                "Source"
            );
            targetClient = DataverseConnectionFactory.Create(
                settings.TargetConnectionString,
                "Target"
            );
            sourceService = new ReadOnlyOrganizationService(sourceClient);
            targetService = new ReadOnlyOrganizationService(targetClient);
        }

        [OneTimeTearDown]
        public void DisconnectFromConfiguredEnvironments()
        {
            if (targetClient != null)
            {
                targetClient.Dispose();
            }

            if (sourceClient != null)
            {
                sourceClient.Dispose();
            }
        }

        [Test]
        public void ConfiguredSolution_CanBeReadFromSourceAndTarget()
        {
            var sourceSolution = FindConfiguredSolution(sourceService, "Source");
            var targetSolution = FindConfiguredSolution(targetService, "Target");

            Assert.Multiple(() =>
            {
                Assert.That(sourceSolution.UniqueName, Is.EqualTo(settings.SolutionUniqueName));
                Assert.That(targetSolution.UniqueName, Is.EqualTo(settings.SolutionUniqueName));
                Assert.That(sourceSolution.SolutionId, Is.Not.EqualTo(Guid.Empty));
                Assert.That(targetSolution.SolutionId, Is.Not.EqualTo(Guid.Empty));
            });
        }

        [Test]
        public void ConfiguredSolution_AnalysisCompletesWithoutMissingEnvironmentOrTimeoutErrors()
        {
            var sourceSolution = FindConfiguredSolution(sourceService, "Source");
            var targetSolution = FindConfiguredSolution(targetService, "Target");
            var service = new SolutionAnalysisService(
                sourceService,
                targetService,
                new AnalyzerOptions(),
                TestContext.Progress.WriteLine
            );

            var result = service.Analyze(
                sourceSolution,
                targetSolution,
                progress => TestContext.Progress.WriteLine(progress.Message),
                CancellationToken.None
            );

            var missingFromTargetEnvironment = result.Results.Where(item =>
                item.CorrelationStatus == ComponentCorrelationStatus.MissingFromTargetEnvironment
            );
            var timeoutErrors = result.Results.Where(item => IsTimeout(item.Error));

            Assert.Multiple(() =>
            {
                Assert.That(result.WasCancelled, Is.False);
                Assert.That(result.Results, Is.Not.Empty);
                Assert.That(
                    missingFromTargetEnvironment,
                    Is.Empty,
                    "Components from the configured solution should exist in both environments."
                );
                Assert.That(
                    timeoutErrors,
                    Is.Empty,
                    "Layer analysis should not reproduce the Dataverse timeout error."
                );
            });
        }

        private SolutionInfo FindConfiguredSolution(
            ReadOnlyOrganizationService service,
            string environmentName
        )
        {
            var repository = new SolutionRepository(service);
            var solution = repository.FindByUniqueName(
                settings.SolutionUniqueName,
                CancellationToken.None
            );

            Assert.That(
                solution,
                Is.Not.Null,
                settings.SolutionUniqueName
                    + " was not found in the configured "
                    + environmentName
                    + " environment."
            );
            return solution;
        }

        private static bool IsTimeout(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                return false;
            }

            return error.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0
                || error.IndexOf("timed out", StringComparison.OrdinalIgnoreCase) >= 0
                || error.IndexOf("tempo limite", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
