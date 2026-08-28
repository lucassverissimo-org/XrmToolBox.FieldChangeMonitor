using System;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Services;
using LucasVerissimo.XrmToolBox.Tests.TestDoubles.Dataverse;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;
using AnalyzerSolutionComponentReference = LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models.SolutionComponentReference;

namespace LucasVerissimo.XrmToolBox.Tests.SolutionLayerAnalyzer.Correlation
{
    [TestFixture]
    [Category("Unit")]
    internal sealed class ComponentCorrelationServiceTests
    {
        [Test]
        public void Correlate_DifferentIdsAndSamePluginAssemblyName_MatchesTargetComponent()
        {
            var sourceId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var sourceService = CreatePluginAssemblyService(sourceId, "EDPSmart.B2B.Plugins");
            var targetService = CreatePluginAssemblyService(targetId, "EDPSmart.B2B.Plugins");
            var service = new ComponentCorrelationService(
                sourceService,
                targetService,
                new DefaultComponentIdentityResolver()
            );

            var results = service.Correlate(
                new[] { CreatePluginAssemblyReference(sourceId) },
                new[] { CreatePluginAssemblyReference(targetId) },
                CancellationToken.None
            );

            var result = results.Single();
            Assert.That(result.CorrelationStatus, Is.EqualTo(ComponentCorrelationStatus.Matched));
            Assert.That(result.ExistsInSourceSolution, Is.True);
            Assert.That(result.ExistsInTargetSolution, Is.True);
            Assert.That(result.ExistsInTargetEnvironment, Is.True);
            Assert.That(result.SourceComponentId, Is.EqualTo(sourceId));
            Assert.That(result.TargetComponentId, Is.EqualTo(targetId));
            Assert.That(result.ComponentId, Is.EqualTo(targetId));
            Assert.That(result.ComponentName, Is.EqualTo("EDPSmart.B2B.Plugins"));
        }

        [Test]
        public void GetStableComponentName_EntityDisplayAndLogicalName_UsesLogicalName()
        {
            var stableName = ComponentCorrelationService.GetStableComponentName(
                1,
                "Histórico de Consumo — edpb2c_historicodeconsumo"
            );

            Assert.That(stableName, Is.EqualTo("EDPB2C_HISTORICODECONSUMO"));
        }

        private static FakeOrganizationService CreatePluginAssemblyService(
            Guid pluginAssemblyId,
            string name
        )
        {
            return new FakeOrganizationService
            {
                RetrieveMultipleHandler = query =>
                {
                    var queryExpression = (QueryExpression)query;
                    if (
                        !string.Equals(
                            queryExpression.EntityName,
                            "pluginassembly",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        return new EntityCollection();
                    }

                    var entity = new Entity("pluginassembly", pluginAssemblyId)
                    {
                        ["pluginassemblyid"] = pluginAssemblyId,
                        ["name"] = name,
                    };
                    return new EntityCollection(new[] { entity });
                },
            };
        }

        private static AnalyzerSolutionComponentReference CreatePluginAssemblyReference(
            Guid objectId
        )
        {
            return new AnalyzerSolutionComponentReference
            {
                ComponentType = 91,
                ObjectId = objectId,
            };
        }
    }
}
