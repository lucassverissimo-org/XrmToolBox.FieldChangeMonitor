using System;
using LucasVerissimo.XrmToolBox.Tests.TestDoubles.Dataverse;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace LucasVerissimo.XrmToolBox.Tests.Integration
{
    [TestFixture]
    [Category("Unit")]
    public sealed class ReadOnlyOrganizationServiceTests
    {
        [Test]
        public void Execute_AlwaysRejectsRequestsBeforeTheyReachTheRealService()
        {
            var requestReachedInnerService = false;
            var innerService = new FakeOrganizationService
            {
                ExecuteHandler = request =>
                {
                    requestReachedInnerService = true;
                    return new OrganizationResponse();
                },
            };
            var service = new ReadOnlyOrganizationService(innerService);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                service.Execute(new OrganizationRequest("RemoveActiveCustomization"))
            );

            StringAssert.Contains("blocked", exception.Message);
            Assert.That(requestReachedInnerService, Is.False);
        }
    }
}
