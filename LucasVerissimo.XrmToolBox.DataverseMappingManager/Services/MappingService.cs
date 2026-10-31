using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Models;
using LucasVerissimo.XrmToolBox.Shared.BusinessLogic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager.Services
{
    internal sealed class MappingService
    {
        private readonly IOrganizationService service;
        private readonly DataverseQueryService queries;
        private readonly DataverseMetadataService metadata;

        public MappingService(IOrganizationService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            queries = new DataverseQueryService(service);
            metadata = new DataverseMetadataService(service);
        }

        public List<TableMap> GetMaps()
        {
            return Query("entitymap", "entitymapid", "sourceentityname", "targetentityname")
                .Select(entity => new TableMap
                {
                    Id = entity.Id,
                    Source = entity.GetAttributeValue<string>("sourceentityname"),
                    Target = entity.GetAttributeValue<string>("targetentityname"),
                })
                .ToList();
        }

        public List<PickerItem> GetTables()
        {
            return metadata
                .GetEntities(CancellationToken.None)
                .Select(table => new PickerItem
                {
                    Name = table.LogicalName,
                    Label = MetadataLabelResolver.GetDisplayName(table),
                    Value = table,
                })
                .OrderBy(item => item.Label)
                .ToList();
        }

        public MappingWorkspace Load(TableMap map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }
            var rows = GetRows(map.Id);
            return new MappingWorkspace
            {
                Map = map,
                Source = metadata.GetEntity(
                    map.Source,
                    EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships,
                    CancellationToken.None
                ),
                Target = metadata.GetEntity(
                    map.Target,
                    EntityFilters.Entity | EntityFilters.Attributes,
                    CancellationToken.None
                ),
                Rows = rows,
                Snapshot = rows.Select(row => new MappingRow
                    {
                        Id = row.Id,
                        Source = row.Source,
                        Target = row.Target,
                        Protected = row.Protected,
                        IsSystem = row.IsSystem,
                        IsManaged = row.IsManaged,
                    })
                    .ToList(),
            };
        }

        internal EntityMetadata GetTableColumns(string logicalName, bool includeRelationships)
        {
            if (string.IsNullOrWhiteSpace(logicalName))
            {
                throw new ArgumentException(
                    "Informe o nome lógico da tabela.",
                    nameof(logicalName)
                );
            }
            var filters = EntityFilters.Entity | EntityFilters.Attributes;
            if (includeRelationships)
            {
                filters |= EntityFilters.Relationships;
            }
            return metadata.GetEntity(logicalName, filters, CancellationToken.None);
        }

        internal List<MappingRow> GetRows(Guid mapId)
        {
            var query = new QueryExpression("attributemap")
            {
                ColumnSet = new ColumnSet(
                    "sourceattributename",
                    "targetattributename",
                    "ismanaged",
                    "issystem"
                ),
            };
            query.Criteria.AddCondition("entitymapid", ConditionOperator.Equal, mapId);
            return queries
                .RetrieveAll(query, CancellationToken.None)
                .Select(entity => new MappingRow
                {
                    Id = entity.Id,
                    Source = entity.GetAttributeValue<string>("sourceattributename"),
                    Target = entity.GetAttributeValue<string>("targetattributename"),
                    OriginalSource = entity.GetAttributeValue<string>("sourceattributename"),
                    OriginalTarget = entity.GetAttributeValue<string>("targetattributename"),
                    Protected =
                        entity.GetAttributeValue<bool>("ismanaged")
                        || entity.GetAttributeValue<bool>("issystem"),
                    IsSystem = entity.GetAttributeValue<bool>("issystem"),
                    IsManaged = entity.GetAttributeValue<bool>("ismanaged"),
                })
                .ToList();
        }

        public List<OperationResult> Save(
            MappingWorkspace workspace,
            IProgress<MappingProgress> progress = null
        )
        {
            if (workspace == null)
            {
                throw new ArgumentNullException(nameof(workspace));
            }
            if (workspace.RequiresReload)
            {
                throw new InvalidOperationException(
                    "O resultado de uma operação é incerto. Recarregue os mapeamentos antes de salvar novamente."
                );
            }
            if (!MappingRules.SameSnapshot(workspace.Snapshot, GetRows(workspace.Map.Id)))
            {
                throw new InvalidOperationException(
                    "Os mapeamentos mudaram no Dataverse desde o carregamento. Recarregue e revise os ajustes antes de salvar."
                );
            }
            // Re-read metadata so compatibility is not based on a stale column definition.
            workspace.Source = metadata.GetEntity(
                workspace.Map.Source,
                EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships,
                CancellationToken.None
            );
            workspace.Target = metadata.GetEntity(
                workspace.Map.Target,
                EntityFilters.Entity | EntityFilters.Attributes,
                CancellationToken.None
            );
            var error = MappingRules.Validate(workspace);
            if (error.Length > 0)
            {
                throw new InvalidOperationException(error);
            }
            var results = new List<OperationResult>();
            // Operations sharing old/new destinations are one atomic group. This also supports swaps.
            foreach (var group in GroupChanges(workspace.Rows.Where(row => row.Pending).ToList()))
            {
                if (workspace.RequiresReload)
                {
                    var skipped = group
                        .Select(row => new OperationResult
                        {
                            Mapping = row,
                            State = "Não executado",
                            Operation = row.Change + ": " + row.Summary,
                            Detail =
                                "Não executado: recarregue os mapeamentos para confirmar o estado no servidor.",
                        })
                        .ToList();
                    results.AddRange(skipped);
                    foreach (var result in skipped)
                    {
                        ReportResult(progress, result);
                    }
                    continue;
                }
                foreach (var row in group)
                {
                    progress?.Report(
                        new MappingProgress
                        {
                            Mapping = row,
                            State = "Em execução",
                            Detail =
                                "Salvando no Dataverse; vínculos dependentes são executados na mesma transação.",
                        }
                    );
                }
                var startIndex = results.Count;
                SaveGroup(workspace, group, results);
                foreach (var result in results.Skip(startIndex))
                {
                    ReportResult(progress, result);
                }
            }
            return results;
        }

        private static void ReportResult(
            IProgress<MappingProgress> progress,
            OperationResult result
        )
        {
            progress?.Report(
                new MappingProgress
                {
                    Mapping = result.Mapping,
                    State = result.State,
                    Detail = result.Detail,
                }
            );
        }

        public List<OperationResult> SaveAndPublish(
            MappingWorkspace workspace,
            IProgress<MappingProgress> progress
        )
        {
            progress?.Report(
                new MappingProgress
                {
                    State = "Validando",
                    Detail = "Validando ajustes e verificando alterações no servidor...",
                }
            );
            var results = Save(workspace, progress);
            if (!workspace.NeedsPublication || workspace.RequiresReload)
            {
                return results;
            }
            progress?.Report(
                new MappingProgress
                {
                    State = "Em execução",
                    Detail = "Publicando as tabelas envolvidas...",
                }
            );
            try
            {
                Publish(workspace);
                results.Add(
                    new OperationResult
                    {
                        State = "Publicado",
                        Success = true,
                        Operation = "Publicação",
                        Detail = "Tabelas publicadas.",
                    }
                );
            }
            catch (Exception exception)
            {
                results.Add(
                    new OperationResult
                    {
                        State = "Falha",
                        Operation = "Publicação",
                        Detail =
                            "Os ajustes foram salvos. Tente publicar novamente. "
                            + exception.Message,
                    }
                );
            }
            return results;
        }

        internal static List<List<MappingRow>> GroupChanges(List<MappingRow> changes)
        {
            var groups = new List<List<MappingRow>>();
            foreach (var row in changes)
            {
                var matching = groups
                    .Where(group => group.Any(other => SharesDestination(row, other)))
                    .ToList();
                var merged = new List<MappingRow> { row };
                foreach (var group in matching)
                {
                    merged.AddRange(group);
                    groups.Remove(group);
                }
                groups.Add(merged);
            }
            return groups;
        }

        private static bool SharesDestination(MappingRow first, MappingRow second)
        {
            var destinations = new[] { first.Target, first.OriginalTarget }.Where(value =>
                value != null
            );
            return destinations
                .Intersect(
                    new[] { second.Target, second.OriginalTarget },
                    StringComparer.OrdinalIgnoreCase
                )
                .Any();
        }

        private void SaveGroup(
            MappingWorkspace workspace,
            List<MappingRow> group,
            List<OperationResult> results
        )
        {
            var request = new ExecuteTransactionRequest
            {
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true,
            };
            foreach (var row in group.Where(row => row.Id != Guid.Empty))
            {
                request.Requests.Add(
                    new DeleteRequest { Target = new EntityReference("attributemap", row.Id) }
                );
            }
            var createIndices = new Dictionary<MappingRow, int>();
            foreach (var row in group.Where(row => !row.Removed))
            {
                var entity = new Entity("attributemap");
                entity["entitymapid"] = new EntityReference("entitymap", workspace.Map.Id);
                entity["sourceattributename"] = row.Source;
                entity["targetattributename"] = row.Target;
                createIndices[row] = request.Requests.Count;
                request.Requests.Add(new CreateRequest { Target = entity });
            }
            ExecuteTransactionResponse response;
            try
            {
                response = (ExecuteTransactionResponse)service.Execute(request);
            }
            catch (Exception exception)
            {
                var fault = exception as FaultException<OrganizationServiceFault>;
                workspace.RequiresReload = fault == null;
                var detail =
                    fault == null
                        ? "Não foi possível confirmar se o servidor concluiu a operação. Recarregue antes de tentar novamente. "
                            + exception.Message
                        : "Grupo revertido integralmente pelo Dataverse. "
                            + fault.Detail.Message
                            + " (código "
                            + fault.Detail.ErrorCode
                            + ")";
                foreach (var row in group)
                {
                    results.Add(
                        new OperationResult
                        {
                            Mapping = row,
                            State = workspace.RequiresReload ? "Resultado incerto" : "Falha",
                            Operation = row.Change + ": " + row.Summary,
                            Detail = detail,
                            Success = false,
                        }
                    );
                }
                return;
            }
            foreach (var row in group)
            {
                results.Add(
                    new OperationResult
                    {
                        Mapping = row,
                        State = "Salvo",
                        Operation = row.Change + ": " + row.Summary,
                        Detail = "Salvo; publicação pendente.",
                        Success = true,
                    }
                );
                if (row.Id != Guid.Empty)
                {
                    TrackExportExpectation(
                        workspace,
                        row.OriginalSource,
                        row.OriginalTarget,
                        false
                    );
                }
                if (!row.Removed)
                {
                    TrackExportExpectation(workspace, row.Source, row.Target, true);
                }
                workspace.Snapshot.RemoveAll(original => original.Id == row.Id);
                workspace.ChangedColumns.Add("source:" + row.Source);
                workspace.ChangedColumns.Add("target:" + row.Target);
                if (row.Removed)
                {
                    workspace.Rows.Remove(row);
                    continue;
                }
                var created = (CreateResponse)response.Responses[createIndices[row]];
                row.Accept(created.id);
                workspace.Snapshot.Add(
                    new MappingRow
                    {
                        Id = row.Id,
                        Source = row.Source,
                        Target = row.Target,
                        Protected = false,
                    }
                );
            }
            foreach (var expected in workspace.ExportExpectations)
            {
                expected.ShouldExist = workspace.Snapshot.Any(row =>
                    row.Source == expected.SourceColumn && row.Target == expected.TargetColumn
                );
            }
            workspace.HasSavedChanges = true;
            workspace.NeedsPublication = true;
        }

        private static void TrackExportExpectation(
            MappingWorkspace workspace,
            string source,
            string target,
            bool shouldExist
        )
        {
            workspace.ExportExpectations.RemoveAll(item =>
                item.SourceColumn == source && item.TargetColumn == target
            );
            workspace.ExportExpectations.Add(
                new MappingExportExpectation
                {
                    SourceTable = workspace.Map.Source,
                    TargetTable = workspace.Map.Target,
                    SourceColumn = source,
                    TargetColumn = target,
                    ShouldExist = shouldExist,
                }
            );
            workspace.ChangedColumns.Add("source:" + source);
            workspace.ChangedColumns.Add("target:" + target);
        }

        public void Publish(MappingWorkspace workspace)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException(nameof(workspace));
            }
            var xml = new System.Xml.Linq.XElement(
                "importexportxml",
                new System.Xml.Linq.XElement(
                    "entities",
                    new[] { workspace.Map.Source, workspace.Map.Target }
                        .Distinct()
                        .Select(name => new System.Xml.Linq.XElement("entity", name))
                )
            );
            var request = new OrganizationRequest("PublishXml");
            request["ParameterXml"] = xml.ToString();
            service.Execute(request);
            workspace.NeedsPublication = false;
        }

        public List<PickerItem> GetSolutions()
        {
            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet("friendlyname", "uniquename", "version"),
            };
            query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);
            query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("uniquename", ConditionOperator.NotIn, "Active", "Default");
            return queries
                .RetrieveAll(query, CancellationToken.None)
                .Select(entity => new PickerItem
                {
                    Name = entity.GetAttributeValue<string>("uniquename"),
                    Label =
                        entity.GetAttributeValue<string>("friendlyname")
                        + " — "
                        + entity.GetAttributeValue<string>("version"),
                    Value = entity.Id,
                })
                .OrderBy(item => item.Label)
                .ToList();
        }

        public List<SolutionMappingChoice> GetSolutionChoices(MappingWorkspace workspace)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException(nameof(workspace));
            }
            var choices = new List<SolutionMappingChoice>();
            foreach (var row in workspace.Rows)
            {
                var available =
                    !row.Pending
                    && workspace.Snapshot.Any(saved =>
                        saved.Id == row.Id
                        && saved.Source == row.Source
                        && saved.Target == row.Target
                    );
                var savedThisSession =
                    available
                    && workspace.ExportExpectations.Any(expected =>
                        expected.ShouldExist
                        && expected.SourceColumn == row.Source
                        && expected.TargetColumn == row.Target
                    );
                var status = "Existente";
                if (row.Pending)
                {
                    status = row.Change + " — salve primeiro";
                }
                else if (savedThisSession)
                {
                    status = "Salvo nesta sessão";
                }
                var detail = row.Pending
                    ? "Ajuste local: salve com sucesso antes de incluí-lo na solution."
                    : "A seleção inclui os componentes necessários e confere este vínculo no pacote exportado.";
                if (row.IsSystem || row.IsManaged)
                {
                    status += " — " + row.Protection.ToLowerInvariant();
                    detail =
                        "Vínculo de "
                        + row.Protection.ToLowerInvariant()
                        + ". Pode não constar nesta exportação; nesse caso será apresentado um aviso, sem confirmar transporte ou existência no destino.";
                }
                choices.Add(
                    new SolutionMappingChoice
                    {
                        Expectation = new MappingExportExpectation
                        {
                            SourceTable = workspace.Map.Source,
                            TargetTable = workspace.Map.Target,
                            SourceColumn = row.Source,
                            TargetColumn = row.Target,
                            ShouldExist = !row.Removed,
                            IsSystem = row.IsSystem,
                            IsManaged = row.IsManaged,
                        },
                        Available = available,
                        SelectedByDefault = savedThisSession,
                        Status = status,
                        Detail = detail,
                    }
                );
            }
            foreach (
                var removed in workspace.ExportExpectations.Where(expected => !expected.ShouldExist)
            )
            {
                choices.Add(
                    new SolutionMappingChoice
                    {
                        Expectation = removed,
                        Available = true,
                        SelectedByDefault = false,
                        Status = "Removido no ambiente",
                        Detail =
                            "Conferir ausência no pacote é opcional e não exclui o vínculo no destino. Verifique a origem e as camadas da solution antes de planejar o upgrade ou uma remoção explícita.",
                    }
                );
            }
            return choices
                .OrderBy(choice => choice.Source, StringComparer.OrdinalIgnoreCase)
                .ThenBy(choice => choice.Target, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public List<SolutionComponentItem> GetComponentsForMappings(
            MappingWorkspace workspace,
            IEnumerable<MappingExportExpectation> selectedMappings
        )
        {
            if (workspace == null)
            {
                throw new ArgumentNullException(nameof(workspace));
            }
            if (selectedMappings == null)
            {
                throw new ArgumentNullException(nameof(selectedMappings));
            }
            var expectations = selectedMappings.ToList();
            if (
                expectations.Any(expected =>
                    expected.SourceTable != workspace.Map.Source
                    || expected.TargetTable != workspace.Map.Target
                )
            )
            {
                throw new ArgumentException(
                    "Os mapeamentos devem pertencer ao par de tabelas carregado.",
                    nameof(selectedMappings)
                );
            }
            if (expectations.Count == 0)
            {
                return new List<SolutionComponentItem>();
            }
            var changedColumns = new HashSet<string>();
            foreach (var expected in expectations)
            {
                changedColumns.Add("source:" + expected.SourceColumn);
                changedColumns.Add("target:" + expected.TargetColumn);
            }
            var components = new List<SolutionComponentItem>();
            components.Add(
                new SolutionComponentItem(
                    1,
                    workspace.Source.MetadataId.Value,
                    "Tabela: " + workspace.Map.Source
                )
            );
            components.Add(
                new SolutionComponentItem(
                    1,
                    workspace.Target.MetadataId.Value,
                    "Tabela: " + workspace.Map.Target
                )
            );
            foreach (
                var relation in workspace.Source.OneToManyRelationships.Where(relation =>
                    relation.ReferencingEntity == workspace.Map.Target
                )
            )
            {
                components.Add(
                    new SolutionComponentItem(
                        10,
                        relation.MetadataId.Value,
                        "Relacionamento: " + relation.SchemaName
                    )
                );
            }
            foreach (var expected in expectations)
            {
                components.Add(
                    new SolutionComponentItem(
                        47,
                        Guid.Empty,
                        "Verificar no pacote: "
                            + expected.SourceColumn
                            + " → "
                            + expected.TargetColumn
                            + (expected.ShouldExist ? " (presença)" : " (remoção / upgrade)"),
                        expected
                    )
                );
            }
            foreach (
                var column in workspace.Source.Attributes.Where(column =>
                    changedColumns.Contains("source:" + column.LogicalName)
                )
            )
            {
                components.Add(
                    new SolutionComponentItem(
                        2,
                        column.MetadataId.Value,
                        "Coluna de origem: " + column.LogicalName
                    )
                );
            }
            foreach (
                var column in workspace.Target.Attributes.Where(column =>
                    changedColumns.Contains("target:" + column.LogicalName)
                )
            )
            {
                components.Add(
                    new SolutionComponentItem(
                        2,
                        column.MetadataId.Value,
                        "Coluna de destino: " + column.LogicalName
                    )
                );
            }
            return components
                .GroupBy(item =>
                    item.ExportExpectation == null
                        ? item.Type + ":" + item.Id
                        : "check:"
                            + item.ExportExpectation.SourceTable
                            + ":"
                            + item.ExportExpectation.TargetTable
                            + ":"
                            + item.ExportExpectation.SourceColumn
                            + ":"
                            + item.ExportExpectation.TargetColumn
                )
                .Select(group => group.First())
                .ToList();
        }

        public List<OperationResult> AddToSolution(
            string uniqueName,
            IEnumerable<SolutionComponentItem> components
        )
        {
            if (string.IsNullOrWhiteSpace(uniqueName))
            {
                throw new ArgumentException("Selecione uma solution.", nameof(uniqueName));
            }
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }
            var results = new List<OperationResult>();
            var selected = components.ToList();
            var checks = selected.Where(item => item.ExportExpectation != null).ToList();
            foreach (
                var component in selected
                    .Where(item => item.ExportExpectation == null)
                    .OrderBy(item => item.Type == 1 ? 0 : 1)
            )
            {
                try
                {
                    if (component.Type == 46 || component.Type == 47)
                    {
                        throw new InvalidOperationException(
                            "Mapas não são adicionados isoladamente. Inclua tabelas, relacionamento e colunas e valide a exportação."
                        );
                    }
                    var request = new OrganizationRequest("AddSolutionComponent");
                    request["ComponentType"] = component.Type;
                    request["ComponentId"] = component.Id;
                    request["SolutionUniqueName"] = uniqueName;
                    request["AddRequiredComponents"] = false;
                    if (component.Type == 1)
                    {
                        request["DoNotIncludeSubcomponents"] = true;
                    }
                    service.Execute(request);
                    results.Add(
                        new OperationResult
                        {
                            Success = true,
                            Operation = component.Label,
                            Detail = "Incluído em " + uniqueName,
                        }
                    );
                }
                catch (Exception exception)
                {
                    results.Add(
                        new OperationResult
                        {
                            Success = false,
                            Operation = component.Label,
                            Detail = exception.Message,
                        }
                    );
                }
            }
            if (checks.Count > 0)
            {
                try
                {
                    var request = new OrganizationRequest("ExportSolution");
                    request["SolutionName"] = uniqueName;
                    request["Managed"] = false;
                    var response = service.Execute(request);
                    var archive = response.Results.Contains("ExportSolutionFile")
                        ? response.Results["ExportSolutionFile"] as byte[]
                        : null;
                    results.AddRange(SolutionMappingExportVerifier.Verify(archive, checks));
                }
                catch (Exception exception)
                {
                    results.AddRange(
                        checks.Select(check => new OperationResult
                        {
                            Operation = check.Label,
                            State = "Não confirmado",
                            Detail =
                                "Os componentes já incluídos permanecem na solution, mas não foi possível validar os mapeamentos na exportação. "
                                + exception.Message,
                        })
                    );
                }
            }
            return results;
        }

        private IReadOnlyCollection<Entity> Query(string table, params string[] columns)
        {
            return queries.RetrieveAll(
                new QueryExpression(table) { ColumnSet = new ColumnSet(columns) },
                CancellationToken.None
            );
        }
    }

    internal sealed class SolutionComponentItem
    {
        public SolutionComponentItem(
            int type,
            Guid id,
            string label,
            MappingExportExpectation exportExpectation = null
        )
        {
            Type = type;
            Id = id;
            Label = label;
            ExportExpectation = exportExpectation;
        }

        public MappingExportExpectation ExportExpectation { get; }
        public int Type { get; }
        public Guid Id { get; }
        public string Label { get; }

        public override string ToString()
        {
            return Label;
        }
    }
}
