using System;
using System.Collections.Generic;

namespace XrmTool_bravo
{
    /// <summary>
    /// This class can help you to store settings for your plugin
    /// </summary>
    /// <remarks>
    /// This class must be XML serializable
    /// </remarks>
    public class Settings
    {
        public string LastUsedOrganizationWebappUrl { get; set; }

        public List<MonitorDefinition> SavedMonitors { get; set; } = new List<MonitorDefinition>();
    }

    public class MonitorDefinition
    {
        public string Name { get; set; }

        public string EntityLogicalName { get; set; }

        public string PrimaryIdAttribute { get; set; }

        public string PrimaryNameAttribute { get; set; }

        public int IntervalSeconds { get; set; }

        public List<string> MonitoredColumns { get; set; } = new List<string>();

        public string FilterXml { get; set; }

        public string FetchXml { get; set; }

        public bool IsPaused { get; set; }

        public string EnvironmentUrl { get; set; }
    }

    public class MonitorExportPackage
    {
        public int SchemaVersion { get; set; } = 1;

        public DateTime ExportedAtUtc { get; set; }

        public List<MonitorDefinition> Monitors { get; set; } = new List<MonitorDefinition>();
    }
}
