using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using LucasVerissimo.XrmToolBox.FieldChangeMonitor;
using NUnit.Framework;

namespace LucasVerissimo.XrmToolBox.Tests.FieldChangeMonitor
{
    [TestFixture]
    [Category("Unit")]
    internal sealed class SettingsSerializationTests
    {
        [Test]
        public void Settings_SavedMonitor_RoundTripsThroughXml()
        {
            var settings = new Settings
            {
                EnableWindowsPopups = false,
                MaximumRecentChanges = 25,
                SavedMonitors = new List<MonitorDefinition>
                {
                    new MonitorDefinition
                    {
                        Name = "Accounts",
                        EntityLogicalName = "account",
                        IntervalSeconds = 30,
                        MonitoredColumns = new List<string> { "name", "statecode" },
                    },
                },
            };

            var serializer = new XmlSerializer(typeof(Settings));
            string xml;
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, settings);
                xml = writer.ToString();
            }

            Settings restored;
            using (var reader = new StringReader(xml))
            {
                restored = (Settings)serializer.Deserialize(reader);
            }

            Assert.That(restored.EnableWindowsPopups, Is.False);
            Assert.That(restored.MaximumRecentChanges, Is.EqualTo(25));
            Assert.That(restored.SavedMonitors, Has.Count.EqualTo(1));
            Assert.That(restored.SavedMonitors[0].Name, Is.EqualTo("Accounts"));
            Assert.That(restored.SavedMonitors[0].EntityLogicalName, Is.EqualTo("account"));
            Assert.That(
                restored.SavedMonitors[0].MonitoredColumns,
                Is.EqualTo(new[] { "name", "statecode" })
            );
        }
    }
}
