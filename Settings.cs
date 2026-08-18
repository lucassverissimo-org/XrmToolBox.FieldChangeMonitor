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
        public bool EnableWindowsPopups { get; set; } = true;

        public int MaximumRecentChanges { get; set; } = 100;

        public bool ConfirmBeforeOpeningRecord { get; set; } = true;

        public bool RestoreMonitorsOnStartup { get; set; } = true;

        public string LastUsedOrganizationWebappUrl { get; set; }

        public List<MonitorDefinition> SavedMonitors { get; set; } = new List<MonitorDefinition>();

        public List<PersistedFieldChange> RecentChanges { get; set; } = new List<PersistedFieldChange>();
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

        public List<PersistedRecordSnapshot> LastSnapshot { get; set; } = new List<PersistedRecordSnapshot>();
    }

    public class PersistedRecordSnapshot
    {
        public Guid RecordId { get; set; }
        public string RecordName { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public List<PersistedFieldValue> Values { get; set; } = new List<PersistedFieldValue>();
    }

    public class PersistedFieldValue
    {
        public string ColumnLogicalName { get; set; }
        public string NormalizedValue { get; set; }
        public string DisplayValue { get; set; }
    }

    public class PersistedFieldChange
    {
        public string EnvironmentUrl { get; set; }
        public Guid RecordId { get; set; }
        public string RecordName { get; set; }
        public string EntityLogicalName { get; set; }
        public string MonitorName { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ColumnLogicalName { get; set; }
        public int Kind { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    public class MonitorExportPackage
    {
        public int SchemaVersion { get; set; } = 1;

        public DateTime ExportedAtUtc { get; set; }

        public List<MonitorDefinition> Monitors { get; set; } = new List<MonitorDefinition>();
    }
}
