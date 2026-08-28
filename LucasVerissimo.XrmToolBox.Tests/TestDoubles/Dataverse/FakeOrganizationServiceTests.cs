using System;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace LucasVerissimo.XrmToolBox.Tests.TestDoubles.Dataverse
{
    [TestFixture]
    [Category("Unit")]
    internal sealed class FakeOrganizationServiceTests
    {
        [Test]
        public void Execute_RemoveActiveCustomization_AlwaysRejectsDestructiveRequest()
        {
            var service = new FakeOrganizationService
            {
                ExecuteHandler = organizationRequest => new OrganizationResponse(),
            };
            var request = new OrganizationRequest("RemoveActiveCustomization");

            var error = Assert.Throws<InvalidOperationException>(() => service.Execute(request));

            Assert.That(error.Message, Does.Contain("forbidden"));
        }
    }
}
