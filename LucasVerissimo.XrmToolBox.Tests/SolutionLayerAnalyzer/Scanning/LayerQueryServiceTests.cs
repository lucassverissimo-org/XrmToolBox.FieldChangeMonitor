using System;
using System.Linq;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Scanning;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace LucasVerissimo.XrmToolBox.Tests.SolutionLayerAnalyzer.Scanning
{
    [TestFixture]
    [Category("Unit")]
    internal sealed class LayerQueryServiceTests
    {
        [Test]
        public void CreateActiveLayerQuery_ComponentInTarget_UsesStandardSolutionComponentTable()
        {
            var activeSolutionId = Guid.NewGuid();
            var componentId = Guid.NewGuid();
            var component = new LayerAnalysisResult
            {
                ComponentId = componentId,
                ComponentType = 91,
            };

            var query = LayerQueryService.CreateActiveLayerQuery(
                new[] { component },
                activeSolutionId
            );

            Assert.That(query.EntityName, Is.EqualTo("solutioncomponent"));
            AssertCondition(query, "solutionid", ConditionOperator.Equal, activeSolutionId);
            AssertCondition(query, "componenttype", ConditionOperator.Equal, 91);
            AssertCondition(query, "objectid", ConditionOperator.In, componentId);
        }

        private static void AssertCondition(
            QueryExpression query,
            string attributeName,
            ConditionOperator conditionOperator,
            object expectedValue
        )
        {
            var condition = query.Criteria.Conditions.Single(item =>
                string.Equals(item.AttributeName, attributeName, StringComparison.OrdinalIgnoreCase)
            );
            Assert.That(condition.Operator, Is.EqualTo(conditionOperator));
            Assert.That(condition.Values.Cast<object>(), Does.Contain(expectedValue));
        }
    }
}
