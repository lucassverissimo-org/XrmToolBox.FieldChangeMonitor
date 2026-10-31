using System;
using System.Collections.Generic;
using System.Linq;
using LucasVerissimo.XrmToolBox.Shared.BusinessLogic;
using Microsoft.Xrm.Sdk.Metadata;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager.Models
{
    internal sealed class TableMap
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
    }

    internal sealed class MappingRow
    {
        public Guid Id { get; set; }
        public string OriginalSource { get; set; }
        public string OriginalTarget { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public bool Protected { get; set; }
        public bool IsSystem { get; set; }
        public bool IsManaged { get; set; }
        public string Protection
        {
            get
            {
                if (IsSystem && IsManaged)
                {
                    return "Sistema e gerenciado";
                }
                if (IsSystem)
                {
                    return "Sistema";
                }
                if (IsManaged)
                {
                    return "Gerenciado";
                }
                return Protected ? "Protegido" : "Editável";
            }
        }
        public bool Removed { get; set; }
        public string Change
        {
            get
            {
                if (Removed)
                {
                    return "Removido";
                }
                if (Id == Guid.Empty)
                {
                    return "Adicionado";
                }
                if (Source != OriginalSource || Target != OriginalTarget)
                {
                    return "Alterado";
                }
                return "Existente";
            }
        }
        public bool Pending => Change != "Existente";
        public string Description => Source + " → " + Target;
        public string Summary =>
            Change == "Alterado"
                ? OriginalSource + " → " + OriginalTarget + "  ⇒  " + Description
                : Description;

        public void Accept(Guid id)
        {
            Id = id;
            OriginalSource = Source;
            OriginalTarget = Target;
        }

        public void Undo()
        {
            Source = OriginalSource;
            Target = OriginalTarget;
            Removed = false;
        }

        internal MappingRow Copy()
        {
            return (MappingRow)MemberwiseClone();
        }
    }

    internal sealed class MappingWorkspace
    {
        public TableMap Map { get; set; }
        public EntityMetadata Source { get; set; }
        public EntityMetadata Target { get; set; }
        public List<MappingRow> Rows { get; set; }
        public List<MappingRow> Snapshot { get; set; }
        public bool NeedsPublication { get; set; }
        public bool RequiresReload { get; set; }
        public bool HasSavedChanges { get; set; }
        public List<MappingExportExpectation> ExportExpectations { get; } =
            new List<MappingExportExpectation>();
        public HashSet<string> ChangedColumns { get; } = new HashSet<string>();
        public bool HasPending => Rows.Any(row => row.Pending);

        // Refreshing server state must never replace staged changes or their conflict baseline.
        internal void RefreshMappings(List<MappingRow> serverRows)
        {
            if (serverRows == null)
            {
                throw new ArgumentNullException(nameof(serverRows));
            }
            if (HasPending)
            {
                if (RequiresReload || !Services.MappingRules.SameSnapshot(Snapshot, serverRows))
                {
                    throw new InvalidOperationException(
                        "Há ajustes locais e o estado do ambiente mudou ou não foi confirmado. Salve uma cópia dos ajustes, descarte-os e atualize os mapeamentos antes de continuar."
                    );
                }
                return;
            }
            Rows = serverRows;
            Snapshot = serverRows.Select(row => row.Copy()).ToList();
            RequiresReload = false;
        }
    }

    internal sealed class MappingExportExpectation
    {
        public string SourceTable { get; set; }
        public string TargetTable { get; set; }
        public string SourceColumn { get; set; }
        public string TargetColumn { get; set; }
        public bool ShouldExist { get; set; }
        public bool IsSystem { get; set; }
        public bool IsManaged { get; set; }
    }

    internal sealed class SolutionMappingChoice
    {
        public MappingExportExpectation Expectation { get; set; }
        public bool Available { get; set; }
        public bool SelectedByDefault { get; set; }
        public string Status { get; set; }
        public string Detail { get; set; }
        public string Source => Expectation.SourceColumn;
        public string Target => Expectation.TargetColumn;
        public bool IsRemoval => !Expectation.ShouldExist;
    }

    internal sealed class OperationResult
    {
        public MappingRow Mapping { get; set; }
        public string State { get; set; }
        public string Operation { get; set; }
        public string Detail { get; set; }
        public bool Success { get; set; }
        public bool IsWarning => !Success && IsWarningState(State);

        public static bool IsWarningState(string state)
        {
            return state == "Atenção"
                || state == "Não confirmado"
                || state == "Nativo não exportado"
                || state == "Gerenciado não exportado";
        }

        public override string ToString()
        {
            var prefix = "FALHA: ";
            if (Success)
            {
                prefix = "SUCESSO: ";
            }
            else if (IsWarning)
            {
                prefix = "AVISO: ";
            }
            return prefix + Operation + Environment.NewLine + Detail;
        }
    }

    internal sealed class MappingProgress
    {
        public MappingRow Mapping { get; set; }
        public string State { get; set; }
        public string Detail { get; set; }
    }

    internal sealed class PickerItem
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Label + " (" + Name + ")";
        }
    }

    internal sealed class ColumnItem
    {
        public AttributeMetadata Metadata { get; set; }
        public string Name => Metadata.LogicalName;
        public string Label => MetadataLabelResolver.GetDisplayName(Metadata);
        public string Type =>
            Metadata.AttributeTypeName?.Value ?? Metadata.AttributeType.ToString();
        public string Reason { get; set; }
    }
}
