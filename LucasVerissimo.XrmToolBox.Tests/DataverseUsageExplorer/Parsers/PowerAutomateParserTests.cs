using System.Linq;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Parsers;
using NUnit.Framework;

namespace LucasVerissimo.XrmToolBox.Tests.DataverseUsageExplorer.Parsers
{
    [TestFixture]
    [Category("Unit")]
    internal sealed class PowerAutomateParserTests
    {
        [Test]
        public void Find_DataverseTriggerTable_ReturnsTableReference()
        {
            const string json = "{\"subscriptionRequest\":{\"entityName\":\"account\"}}";

            var references = PowerAutomateParser.Find(json, "account", null, false);

            Assert.That(references, Has.Count.EqualTo(1));
            Assert.That(references.Single().ReferenceType, Is.EqualTo("Dataverse Trigger Table"));
        }

        [Test]
        public void Find_FilterColumn_ReturnsFilterRowsReference()
        {
            const string json = "{\"inputs\":{\"filter\":\"statecode eq 0\"}}";

            var references = PowerAutomateParser.Find(json, "account", "statecode", true);

            Assert.That(references, Has.Count.EqualTo(1));
            Assert.That(references.Single().ReferenceType, Is.EqualTo("Filter Rows"));
        }
    }
}
