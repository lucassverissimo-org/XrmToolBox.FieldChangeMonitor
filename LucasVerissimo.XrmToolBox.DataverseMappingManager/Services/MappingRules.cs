using System;
using System.Collections.Generic;
using System.Linq;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Models;
using Microsoft.Xrm.Sdk.Metadata;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager.Services
{
    internal static class MappingRules
    {
        // Empty means structurally compatible. Dataverse remains the authority for server rules.
        public static string Incompatibility(AttributeMetadata source, AttributeMetadata target)
        {
            if (source == null || target == null)
            {
                return "Selecione origem e destino";
            }
            if (source.AttributeOf != null || target.AttributeOf != null)
            {
                return "Coluna derivada de outra coluna";
            }
            if (
                source.AttributeType == AttributeTypeCode.PartyList
                || source.LogicalName?.EndsWith("addressid", StringComparison.OrdinalIgnoreCase)
                    == true
                || target.LogicalName?.EndsWith("addressid", StringComparison.OrdinalIgnoreCase)
                    == true
            )
            {
                return "Tipo não permitido para mapeamento";
            }
            if (
                source.AttributeType != target.AttributeType
                || source.AttributeTypeName?.Value != target.AttributeTypeName?.Value
            )
            {
                return "Tipos diferentes";
            }
            if (target.IsValidForCreate != true || target.SourceType.GetValueOrDefault() != 0)
            {
                return "Destino não permite entrada na criação";
            }
            var sourceText = source as StringAttributeMetadata;
            var targetText = target as StringAttributeMetadata;
            if (sourceText != null && targetText != null)
            {
                if (
                    sourceText.Format != targetText.Format
                    || sourceText.FormatName?.Value != targetText.FormatName?.Value
                )
                {
                    return "Formatos diferentes";
                }
                if (sourceText.MaxLength > targetText.MaxLength)
                {
                    return "Tamanho insuficiente no destino";
                }
            }
            var sourceMemo = source as MemoAttributeMetadata;
            var targetMemo = target as MemoAttributeMetadata;
            if (sourceMemo != null && targetMemo != null)
            {
                if (
                    sourceMemo.Format != targetMemo.Format
                    || sourceMemo.MaxLength > targetMemo.MaxLength
                )
                {
                    return "Formato ou tamanho incompatível";
                }
            }
            var sourceDate = source as DateTimeAttributeMetadata;
            var targetDate = target as DateTimeAttributeMetadata;
            if (
                sourceDate != null
                && targetDate != null
                && (
                    sourceDate.Format != targetDate.Format
                    || sourceDate.DateTimeBehavior?.Value != targetDate.DateTimeBehavior?.Value
                )
            )
            {
                return "Formato ou comportamento de data diferente";
            }
            var sourceInteger = source as IntegerAttributeMetadata;
            var targetInteger = target as IntegerAttributeMetadata;
            if (
                sourceInteger != null
                && targetInteger != null
                && sourceInteger.Format != targetInteger.Format
            )
            {
                return "Formatos diferentes";
            }
            var sourceLookup = source as LookupAttributeMetadata;
            var targetLookup = target as LookupAttributeMetadata;
            if (
                sourceLookup != null
                && targetLookup != null
                && (sourceLookup.Targets ?? Array.Empty<string>())
                    .Except(targetLookup.Targets ?? Array.Empty<string>())
                    .Any()
            )
            {
                return "Destino não aceita todas as tabelas do lookup de origem";
            }
            var sourceChoice = source as EnumAttributeMetadata;
            var targetChoice = target as EnumAttributeMetadata;
            if (
                sourceChoice != null
                && targetChoice != null
                && (sourceChoice.OptionSet?.Options ?? new OptionMetadataCollection()).Any(option =>
                    !(targetChoice.OptionSet?.Options ?? new OptionMetadataCollection()).Any(
                        candidate => candidate.Value == option.Value
                    )
                )
            )
            {
                return "Opções com valores numéricos incompatíveis";
            }
            return string.Empty;
        }

        public static string Validate(MappingWorkspace workspace)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException(nameof(workspace));
            }
            var active = workspace.Rows.Where(row => !row.Removed).ToList();
            if (
                active
                    .GroupBy(row => row.Target, StringComparer.OrdinalIgnoreCase)
                    .Any(group => group.Count() > 1)
            )
            {
                return "Um destino não pode receber mais de um mapeamento.";
            }
            foreach (var row in workspace.Rows.Where(row => row.Pending))
            {
                if (row.Protected)
                {
                    return "Mapeamentos de sistema ou gerenciados não podem ser alterados.";
                }
                if (row.Removed)
                {
                    continue;
                }
                var reason = Incompatibility(
                    workspace.Source.Attributes.FirstOrDefault(column =>
                        column.LogicalName == row.Source
                    ),
                    workspace.Target.Attributes.FirstOrDefault(column =>
                        column.LogicalName == row.Target
                    )
                );
                if (reason.Length > 0)
                {
                    return row.Description + ": " + reason;
                }
            }
            return string.Empty;
        }

        public static bool SameSnapshot(
            IEnumerable<MappingRow> expected,
            IEnumerable<MappingRow> actual
        )
        {
            return expected
                .Select(Fingerprint)
                .OrderBy(value => value)
                .SequenceEqual(actual.Select(Fingerprint).OrderBy(value => value));
        }

        private static string Fingerprint(MappingRow row)
        {
            return row.Id + "|" + row.Source + "|" + row.Target + "|" + row.Protected;
        }
    }
}
