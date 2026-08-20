using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models
{
    public enum UsageSearchType
    {
        Table,
        Column,
    }

    public enum ReferenceConfidence
    {
        Confirmed,
        TextMatch,
    }

    public sealed class UsageSearchContext
    {
        public UsageSearchType SearchType { get; set; }
        public string TableLogicalName { get; set; }
        public int? TableObjectTypeCode { get; set; }
        public string ColumnLogicalName { get; set; }
        public IOrganizationService Service { get; set; }
    }

    public sealed class UsageReference
    {
        public string ComponentType { get; set; }
        public string Name { get; set; }
        public Guid? ComponentId { get; set; }
        public string ComponentEntityName { get; set; }
        public string TableLogicalName { get; set; }
        public string ColumnLogicalName { get; set; }
        public string Status { get; set; }
        public string ReferenceType { get; set; }
        public string FoundIn { get; set; }
        public string RawReference { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool? IsManaged { get; set; }
        public ReferenceConfidence Confidence { get; set; }
        public string Details { get; set; }
    }

    public sealed class ScannerResult
    {
        public string ScannerName { get; set; }
        public IReadOnlyCollection<UsageReference> References { get; set; }
        public Exception Error { get; set; }
    }

    internal sealed class MetadataListItem
    {
        public string LogicalName { get; set; }
        public string DisplayName { get; set; }
        public int? ObjectTypeCode { get; set; }

        public override string ToString()
        {
            return string.Equals(LogicalName, DisplayName, StringComparison.OrdinalIgnoreCase)
                ? LogicalName
                : DisplayName + " (" + LogicalName + ")";
        }
    }
}
