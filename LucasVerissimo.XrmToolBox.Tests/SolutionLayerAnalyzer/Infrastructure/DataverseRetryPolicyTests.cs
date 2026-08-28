using System;
using System.Threading;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure;
using NUnit.Framework;

namespace LucasVerissimo.XrmToolBox.Tests.SolutionLayerAnalyzer.Infrastructure
{
    [TestFixture]
    [Category("Unit")]
    internal sealed class DataverseRetryPolicyTests
    {
        [Test]
        public void IsTimeout_PortugueseRequestChannelMessage_ReturnsTrue()
        {
            var error = new InvalidOperationException(
                "O canal de solicitação atingiu o tempo limite ao aguardar uma resposta."
            );

            Assert.That(DataverseRetryPolicy.IsTimeout(error), Is.True);
        }

        [Test]
        public void Execute_TimeoutRetriesDisabled_DoesNotRepeatOperation()
        {
            var attempts = 0;
            var metrics = new AnalysisMetrics();
            var options = new AnalyzerOptions { InitialRetryDelay = TimeSpan.Zero, MaxRetries = 3 };
            var policy = new DataverseRetryPolicy(options, delegate { });

            Assert.Throws<TimeoutException>(() =>
                policy.Execute<object>(
                    () =>
                    {
                        attempts++;
                        throw new TimeoutException("Dataverse timeout");
                    },
                    "Unit test",
                    metrics,
                    CancellationToken.None,
                    false
                )
            );

            var snapshot = metrics.Snapshot();
            Assert.That(attempts, Is.EqualTo(1));
            Assert.That(snapshot.Requests, Is.EqualTo(1));
            Assert.That(snapshot.Retries, Is.Zero);
            Assert.That(snapshot.Timeouts, Is.EqualTo(1));
        }
    }
}
