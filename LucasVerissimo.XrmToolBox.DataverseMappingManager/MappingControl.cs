using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Models;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Services;
using LucasVerissimo.XrmToolBox.Shared.Controls;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager
{
    public partial class MappingControl : PluginControlBase
    {
        private MappingService mappingService;
        private List<TableMap> maps = new List<TableMap>();
        private List<PickerItem> tables = new List<PickerItem>();
        private MappingWorkspace workspace;
        private bool busy;
        private bool updating;
        private bool sourceIsAnchor = true;
        private bool refreshQueued;
        private string checkedSource;
        private string checkedTarget;
        private MappingRow checkedMapping;
        private bool mappingCheckedExplicitly;
        private int connectionVersion;
        private int workflowStep = 1;

        public MappingControl()
        {
            InitializeComponent();
            sourcePicker.Configure(CreatePickerConfiguration("tabelas de origem"));
            targetPicker.Configure(CreatePickerConfiguration("tabelas de destino"));
            SetWorkflowStep(1);
        }

        internal static GridPickerConfiguration CreatePickerConfiguration(string name)
        {
            return new GridPickerConfiguration
            {
                ItemName = name,
                IdentitySelector = item => ((PickerItem)item).Name,
                DisplayTextSelector = item => item.ToString(),
                SearchPredicate = (item, search) => Matches(item.ToString(), search),
                Columns = new[]
                {
                    new GridPickerColumnDefinition("Nome", item => ((PickerItem)item).Label),
                    new GridPickerColumnDefinition(
                        "Nome lógico / único",
                        item => ((PickerItem)item).Name
                    ),
                },
            };
        }

        public override void UpdateConnection(
            IOrganizationService newService,
            ConnectionDetail detail,
            string actionName,
            object parameter
        )
        {
            if (workspace?.HasPending == true || workspace?.NeedsPublication == true)
            {
                MessageBox.Show(
                    this,
                    "A conexão foi alterada. Os ajustes locais e a publicação pendente pertencem ao ambiente anterior. Revise esse ambiente antes de continuar.",
                    "Conexão alterada"
                );
            }
            connectionVersion++;
            base.UpdateConnection(newService, detail, actionName, parameter);
            mappingService = newService == null ? null : new MappingService(newService);
            workspace = null;
            maps.Clear();
            tables.Clear();
            sourcePicker.ClearItems();
            targetPicker.ClearItems();
            ClearGrids();
            status.Text = "Conecte ao Dataverse e carregue as tabelas.";
            SetWorkflowStep(1);
        }

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            if (busy)
            {
                info.Cancel = true;
                MessageBox.Show(this, "Aguarde a operação em andamento.");
                return;
            }
            if (!ConfirmDiscard())
            {
                info.Cancel = true;
                return;
            }
            base.ClosingPlugin(info);
        }

        private async Task Run<T>(string message, Func<T> operation, Action<T> completed)
        {
            if (busy || mappingService == null)
            {
                return;
            }
            var version = connectionVersion;
            busy = true;
            layout.Enabled = false;
            status.Text = message;
            try
            {
                var result = await Task.Run(operation);
                if (!IsDisposed && version == connectionVersion)
                {
                    completed(result);
                }
            }
            catch (Exception exception)
            {
                if (!IsDisposed && version == connectionVersion)
                {
                    status.Text = "Falha: " + exception.Message;
                    MessageBox.Show(
                        this,
                        exception.Message,
                        "Dataverse Mapping Manager",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            finally
            {
                busy = false;
                if (!IsDisposed)
                {
                    layout.Enabled = true;
                    UpdateButtons();
                }
            }
        }

        private async void LoadTablesClick(object sender, EventArgs eventArguments)
        {
            if (!ConfirmDiscard())
            {
                return;
            }
            var service = mappingService;
            await Run(
                "Carregando tabelas e mapas disponíveis...",
                () => new { Maps = service.GetMaps(), Tables = service.GetTables() },
                result =>
                {
                    workspace = null;
                    ClearGrids();
                    SetWorkflowStep(1);
                    maps = result.Maps;
                    tables = result.Tables;
                    sourcePicker.ClearItems();
                    targetPicker.ClearItems();
                    sourcePicker.SetItems(
                        tables.Where(table => maps.Any(map => map.Source == table.Name))
                    );
                    status.Text =
                        "Selecione a origem e o destino. Apenas pares com suporte a mapeamento são apresentados.";
                }
            );
        }

        private void SourcePickerChanged(object sender, EventArgs eventArguments)
        {
            var selected = sourcePicker.GetSelectedItem<PickerItem>();
            targetPicker.ClearItems();
            if (selected != null)
            {
                targetPicker.SetItems(
                    tables.Where(table =>
                        maps.Any(map => map.Source == selected.Name && map.Target == table.Name)
                    )
                );
            }
            UpdateButtons();
        }

        private void TargetPickerChanged(object sender, EventArgs eventArguments)
        {
            UpdateButtons();
        }

        private async void ViewMappingsClick(object sender, EventArgs eventArguments)
        {
            var source = sourcePicker.GetSelectedItem<PickerItem>();
            var target = targetPicker.GetSelectedItem<PickerItem>();
            if (source == null || target == null)
            {
                MessageBox.Show(this, "Selecione uma tabela de origem e uma de destino.");
                return;
            }
            if (!ConfirmDiscard())
            {
                return;
            }
            var map = maps.SingleOrDefault(candidate =>
                candidate.Source == source.Name && candidate.Target == target.Name
            );
            if (map == null)
            {
                MessageBox.Show(this, "Mapa indisponível. Recarregue as tabelas.");
                return;
            }
            var service = mappingService;
            await Run(
                "Carregando colunas e mapeamentos...",
                () => service.Load(map),
                result =>
                {
                    ClearGrids();
                    workspace = result;
                    sourceLabel.Text = "Origem · " + map.Source;
                    targetLabel.Text = "Destino · " + map.Target;
                    RefreshLists();
                    ShowMappingStep();
                }
            );
        }

        private void TablesStepClick(object sender, EventArgs eventArguments)
        {
            if (!busy)
            {
                SetWorkflowStep(1);
            }
        }

        private void MappingStepClick(object sender, EventArgs eventArguments)
        {
            if (!busy)
            {
                ShowMappingStep();
            }
        }

        private void ReviewStepClick(object sender, EventArgs eventArguments)
        {
            if (workspace?.HasPending != true || workspace.RequiresReload || busy)
            {
                return;
            }
            ShowMappingStep();
            SaveChangesClick(sender, eventArguments);
        }

        private void ShowMappingStep()
        {
            if (workspace != null)
            {
                SetWorkflowStep(2);
            }
        }

        private void SetWorkflowStep(int step)
        {
            if (step != 1 && workspace == null)
            {
                return;
            }
            workflowStep = step;
            tablesPage.Visible = step == 1;
            mappingPage.Visible = step != 1;
            tablesStep.Font = step == 1 ? activeStepFont : Font;
            mappingStep.Font = step == 2 ? activeStepFont : Font;
            reviewStep.Font = step == 3 ? activeStepFont : Font;
            stepPosition.Text = "Etapa " + step + " de 3";
            UpdateButtons();
        }

        private void MoreActionsClick(object sender, EventArgs eventArguments)
        {
            moreMenu.Show(moreActions, new Point(0, moreActions.Height));
        }

        private void HelpClick(object sender, EventArgs eventArguments)
        {
            MessageBox.Show(
                this,
                "Marque os campos de origem e destino para criar um vínculo. Para alterar ou remover, marque um mapeamento na terceira lista.\r\n\r\nOs ajustes ficam locais até Revisar e salvar. Os mapeamentos preenchem valores iniciais na criação de registros relacionados; não sincronizam registros existentes.",
                "Ajuda — Dataverse Mapping Manager",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private bool ConfirmDiscard()
        {
            if (workspace == null || (!workspace.HasPending && !workspace.NeedsPublication))
            {
                return true;
            }
            return MessageBox.Show(
                    this,
                    "Existem ajustes locais ou publicação pendente. Deseja sair desta edição? Os ajustes locais serão descartados; alterações já salvas permanecem no Dataverse.",
                    "Ajustes pendentes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                ) == DialogResult.Yes;
        }

        private static bool Matches(string value, string search)
        {
            return string.IsNullOrWhiteSpace(search)
                || value?.IndexOf(search.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private ColumnItem SelectedColumn(DataGridView grid)
        {
            if (workspace == null)
            {
                return null;
            }
            var name = grid == sourceGrid ? checkedSource : checkedTarget;
            var columns =
                grid == sourceGrid ? workspace.Source.Attributes : workspace.Target.Attributes;
            var column = columns.FirstOrDefault(item => item.LogicalName == name);
            return column == null ? null : new ColumnItem { Metadata = column };
        }

        private MappingRow SelectedMapping =>
            workspace?.Rows.Contains(checkedMapping) == true ? checkedMapping : null;

        private void ClearGrids()
        {
            checkedSource = null;
            checkedTarget = null;
            checkedMapping = null;
            mappingCheckedExplicitly = false;
            updating = true;
            sourceGrid.Rows.Clear();
            targetGrid.Rows.Clear();
            mappingGrid.Rows.Clear();
            updating = false;
        }

        private void RefreshLists()
        {
            if (workspace == null || updating)
            {
                return;
            }
            var source = SelectedColumn(sourceGrid)?.Name;
            var target = SelectedColumn(targetGrid)?.Name;
            var selectedMap = SelectedMapping;
            updating = true;
            try
            {
                FillColumns(
                    sourceGrid,
                    workspace.Source.Attributes,
                    sourceSearch.Text,
                    true,
                    source,
                    target
                );
                FillColumns(
                    targetGrid,
                    workspace.Target.Attributes,
                    targetSearch.Text,
                    false,
                    target,
                    source
                );
                mappingGrid.Rows.Clear();
                foreach (
                    var row in workspace
                        .Rows.Where(row =>
                            Matches(row.Description + " " + row.Change, mappingSearch.Text)
                        )
                        .OrderBy(row => row.Source, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(row => row.Target, StringComparer.OrdinalIgnoreCase)
                )
                {
                    var index = mappingGrid.Rows.Add(
                        ReferenceEquals(row, selectedMap),
                        row.Source,
                        row.Target,
                        row.Change,
                        row.Protection
                    );
                    var visual = mappingGrid.Rows[index];
                    visual.Tag = row;
                    if (
                        (sourceIsAnchor && row.Source == source)
                        || (!sourceIsAnchor && row.Target == target)
                    )
                    {
                        visual.DefaultCellStyle.BackColor = SystemColors.ControlLight;
                    }
                    if (row.Pending)
                    {
                        visual.Cells["Change"].Style.ForeColor = SystemColors.HotTrack;
                    }
                    MatchFocusColors(visual);
                }
                mappingGrid.ClearSelection();
                mappingGrid.CurrentCell = null;
            }
            finally
            {
                updating = false;
            }
            UpdateButtons();
        }

        private void FillColumns(
            DataGridView grid,
            IEnumerable<AttributeMetadata> columns,
            string search,
            bool isSource,
            string selection,
            string otherSelection
        )
        {
            var otherColumns = isSource ? workspace.Target.Attributes : workspace.Source.Attributes;
            var other = otherColumns.FirstOrDefault(column => column.LogicalName == otherSelection);
            grid.Rows.Clear();
            foreach (
                var column in columns
                    .OrderByDescending(column => column.LogicalName == selection)
                    .ThenBy(column => column.LogicalName, StringComparer.OrdinalIgnoreCase)
            )
            {
                var item = new ColumnItem { Metadata = column };
                if (!Matches(item.Name + " " + item.Label + " " + item.Type, search))
                {
                    continue;
                }
                var linked =
                    other != null
                    && workspace.Rows.Any(row =>
                        !row.Removed
                        && (isSource ? row.Source == item.Name : row.Target == item.Name)
                        && (
                            other == null
                            || (isSource ? row.Target : row.Source) == other.LogicalName
                        )
                    );
                var incompatibility = string.Empty;
                if (other != null)
                {
                    incompatibility = isSource
                        ? MappingRules.Incompatibility(column, other)
                        : MappingRules.Incompatibility(other, column);
                }
                var destination = isSource ? otherSelection : item.Name;
                var source = isSource ? item.Name : otherSelection;
                if (
                    other != null
                    && incompatibility.Length == 0
                    && workspace.Rows.Any(row =>
                        !row.Removed && row.Target == destination && row.Source != source
                    )
                )
                {
                    incompatibility = "Destino já mapeado para outra origem";
                }
                if (
                    compatibleOnly.Checked
                    && other != null
                    && incompatibility.Length > 0
                    && !linked
                    && ((isSource && !sourceIsAnchor) || (!isSource && sourceIsAnchor))
                )
                {
                    continue;
                }
                item.Reason = linked ? "Já mapeado" : incompatibility;
                if (!linked && other != null && incompatibility.Length == 0)
                {
                    item.Reason = "Compatível; verificar formulário";
                }
                var index = grid.Rows.Add(
                    item.Name == selection,
                    item.Name,
                    item.Label,
                    item.Type,
                    item.Reason
                );
                var visual = grid.Rows[index];
                visual.Tag = item;
                visual.Cells["Reason"].ToolTipText = item.Reason;
                if (linked)
                {
                    visual.DefaultCellStyle.BackColor = SystemColors.ControlLight;
                }
                else if (other != null)
                {
                    if (incompatibility.Length > 0)
                    {
                        visual.DefaultCellStyle.ForeColor = SystemColors.GrayText;
                    }
                }
                MatchFocusColors(visual);
            }
            grid.ClearSelection();
            grid.CurrentCell = null;
        }

        private static void MatchFocusColors(DataGridViewRow row)
        {
            // Focusing a row must not obscure the meaning of the checked state or status colors.
            var background = row.DefaultCellStyle.BackColor;
            row.DefaultCellStyle.SelectionBackColor = background.IsEmpty
                ? SystemColors.Window
                : background;
            row.DefaultCellStyle.SelectionForeColor = row.DefaultCellStyle.ForeColor.IsEmpty
                ? SystemColors.WindowText
                : row.DefaultCellStyle.ForeColor;
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (!cell.Style.BackColor.IsEmpty)
                {
                    cell.Style.SelectionBackColor = cell.Style.BackColor;
                    cell.Style.SelectionForeColor = SystemColors.WindowText;
                }
            }
        }

        private void FilterChanged(object sender, EventArgs eventArguments)
        {
            RefreshLists();
        }

        private void GridCheckboxClick(object sender, DataGridViewCellEventArgs eventArguments)
        {
            if (updating || busy || eventArguments.RowIndex < 0 || eventArguments.ColumnIndex != 0)
            {
                return;
            }
            ToggleCheckedRow((DataGridView)sender, eventArguments.RowIndex);
        }

        private void GridCheckboxKeyDown(object sender, KeyEventArgs eventArguments)
        {
            var grid = (DataGridView)sender;
            if (
                eventArguments.KeyCode != Keys.Space
                || grid.CurrentCell?.ColumnIndex != 0
                || updating
                || busy
            )
            {
                return;
            }
            eventArguments.Handled = true;
            eventArguments.SuppressKeyPress = true;
            ToggleCheckedRow(grid, grid.CurrentCell.RowIndex);
        }

        private void ToggleCheckedRow(DataGridView grid, int index)
        {
            if (grid == mappingGrid)
            {
                var mapping = (MappingRow)grid.Rows[index].Tag;
                checkedMapping = ReferenceEquals(checkedMapping, mapping) ? null : mapping;
                mappingCheckedExplicitly = checkedMapping != null;
                if (checkedMapping != null)
                {
                    checkedSource = mapping.Source;
                    checkedTarget = mapping.Target;
                    updating = true;
                    sourceSearch.Clear();
                    targetSearch.Clear();
                    compatibleOnly.Checked = false;
                    updating = false;
                }
            }
            else
            {
                var name = ((ColumnItem)grid.Rows[index].Tag).Name;
                sourceIsAnchor = grid == sourceGrid;
                if (sourceIsAnchor)
                {
                    checkedSource = checkedSource == name ? null : name;
                }
                else
                {
                    checkedTarget = checkedTarget == name ? null : name;
                }
                SynchronizeMappingCheck();
            }
            QueueRefresh();
        }

        private void SynchronizeMappingCheck()
        {
            var matching = workspace.Rows.FirstOrDefault(row =>
                !row.Removed && row.Source == checkedSource && row.Target == checkedTarget
            );
            if (matching == null)
            {
                // Keep a mapping explicitly chosen for editing while its replacement pair is selected.
                if (!mappingCheckedExplicitly)
                {
                    checkedMapping = null;
                }
                return;
            }
            if (!ReferenceEquals(checkedMapping, matching))
            {
                mappingCheckedExplicitly = false;
            }
            checkedMapping = matching;
            if (!Matches(matching.Description + " " + matching.Change, mappingSearch.Text))
            {
                updating = true;
                mappingSearch.Clear();
                updating = false;
            }
        }

        private void ClearChecksClick(object sender, EventArgs eventArguments)
        {
            checkedSource = null;
            checkedTarget = null;
            checkedMapping = null;
            mappingCheckedExplicitly = false;
            RefreshLists();
        }

        private void QueueRefresh()
        {
            // Rebuild only after the grid finishes processing the checkbox click.
            if (refreshQueued || !IsHandleCreated || IsDisposed)
            {
                return;
            }
            refreshQueued = true;
            BeginInvoke(new Action(ApplyQueuedSelection));
        }

        private void ApplyQueuedSelection()
        {
            refreshQueued = false;
            if (!IsDisposed && workspace != null && !busy)
            {
                RefreshLists();
            }
        }

        private bool ValidatePair(MappingRow edited, out string source, out string target)
        {
            var origin = SelectedColumn(sourceGrid);
            var destination = SelectedColumn(targetGrid);
            source = origin?.Name;
            target = destination?.Name;
            var reason = MappingRules.Incompatibility(origin?.Metadata, destination?.Metadata);
            if (reason.Length == 0)
            {
                var targetName = target;
                if (
                    workspace.Rows.Any(row =>
                        row != edited && !row.Removed && row.Target == targetName
                    )
                )
                {
                    reason =
                        "O destino já está mapeado. Remova ou edite o vínculo existente antes.";
                }
            }
            if (reason.Length > 0)
            {
                MessageBox.Show(this, reason, "Mapeamento inválido");
                return false;
            }
            return true;
        }

        private void AddMappingClick(object sender, EventArgs eventArguments)
        {
            if (!ValidatePair(null, out var source, out var target))
            {
                return;
            }
            workspace.Rows.Add(new MappingRow { Source = source, Target = target });
            RefreshLists();
        }

        private void EditMappingClick(object sender, EventArgs eventArguments)
        {
            var row = SelectedMapping;
            if (
                busy
                || workspace?.RequiresReload == true
                || row == null
                || row.Protected
                || row.Removed
            )
            {
                return;
            }
            using (var dialog = new MappingEditForm(workspace, row))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }
            checkedSource = row.Source;
            checkedTarget = row.Target;
            RefreshLists();
        }

        private async void RefreshSourceClick(object sender, EventArgs eventArguments)
        {
            await RefreshColumns(true);
        }

        private async void RefreshTargetClick(object sender, EventArgs eventArguments)
        {
            await RefreshColumns(false);
        }

        private async Task RefreshColumns(bool source)
        {
            if (workspace == null || busy)
            {
                return;
            }
            var current = workspace;
            var service = mappingService;
            var name = source ? current.Map.Source : current.Map.Target;
            await Run(
                "Atualizando colunas de " + name + "...",
                () => service.GetTableColumns(name, source),
                metadata =>
                {
                    if (source)
                    {
                        current.Source = metadata;
                    }
                    else
                    {
                        current.Target = metadata;
                    }
                    RefreshLists();
                }
            );
        }

        private async void RefreshMappingsClick(object sender, EventArgs eventArguments)
        {
            if (workspace == null || busy)
            {
                return;
            }
            var current = workspace;
            var service = mappingService;
            await Run(
                "Atualizando mapeamentos...",
                () => service.GetRows(current.Map.Id),
                rows =>
                {
                    current.RefreshMappings(rows);
                    if (!current.Rows.Contains(checkedMapping))
                    {
                        checkedMapping = null;
                        mappingCheckedExplicitly = false;
                    }
                    RefreshLists();
                }
            );
        }

        private void RemoveMappingClick(object sender, EventArgs eventArguments)
        {
            var row = SelectedMapping;
            if (row == null || row.Protected)
            {
                return;
            }
            if (row.Id == Guid.Empty)
            {
                workspace.Rows.Remove(row);
            }
            else
            {
                row.Removed = true;
            }
            RefreshLists();
        }

        private void UndoMappingClick(object sender, EventArgs eventArguments)
        {
            var row = SelectedMapping;
            if (row == null)
            {
                return;
            }
            if (row.Id == Guid.Empty)
            {
                workspace.Rows.Remove(row);
            }
            else
            {
                row.Undo();
            }
            RefreshLists();
        }

        private void UndoAllClick(object sender, EventArgs eventArguments)
        {
            if (
                workspace == null
                || MessageBox.Show(
                    this,
                    "Descartar todos os ajustes locais?",
                    "Descartar",
                    MessageBoxButtons.YesNo
                ) != DialogResult.Yes
            )
            {
                return;
            }
            workspace.Rows.RemoveAll(row => row.Id == Guid.Empty);
            foreach (var row in workspace.Rows)
            {
                row.Undo();
            }
            RefreshLists();
        }

        private void SaveChangesClick(object sender, EventArgs eventArguments)
        {
            if (workspace?.HasPending != true || workspace.RequiresReload || busy)
            {
                return;
            }
            var error = MappingRules.Validate(workspace);
            if (error.Length > 0)
            {
                MessageBox.Show(this, error);
                return;
            }
            var service = mappingService;
            var current = workspace;
            var version = connectionVersion;
            SetWorkflowStep(3);
            busy = true;
            UpdateButtons();
            try
            {
                using (
                    var review = new ReviewForm(
                        current.Rows.Where(row => row.Pending),
                        progress => Task.Run(() => service.SaveAndPublish(current, progress)),
                        owner => ChooseSolutionDialog(owner, service, current, version)
                    )
                )
                {
                    review.ShowDialog(this);
                }
            }
            finally
            {
                busy = false;
                SetWorkflowStep(workspace == null ? 1 : 2);
                RefreshLists();
                UpdateButtons();
            }
        }

        private async void PublishClick(object sender, EventArgs eventArguments)
        {
            if (
                MessageBox.Show(
                    this,
                    "Publicar as tabelas de origem e destino? Outras customizações pendentes nessas tabelas também serão publicadas.",
                    "Publicar tabelas",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                ) != DialogResult.Yes
            )
            {
                return;
            }
            var service = mappingService;
            var current = workspace;
            await Run(
                "Publicando tabelas...",
                () =>
                {
                    service.Publish(current);
                    return true;
                },
                result => status.Text = "Publicação concluída."
            );
        }

        private async void SolutionClick(object sender, EventArgs eventArguments)
        {
            await ChooseSolution();
        }

        private async Task ChooseSolution()
        {
            if (busy || mappingService == null || workspace == null)
            {
                return;
            }
            busy = true;
            layout.Enabled = false;
            try
            {
                await ChooseSolutionDialog(this, mappingService, workspace, connectionVersion);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    this,
                    exception.Message,
                    "Adicionar à solution",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                busy = false;
                layout.Enabled = true;
                UpdateButtons();
            }
        }

        private async Task ChooseSolutionDialog(
            IWin32Window owner,
            MappingService service,
            MappingWorkspace current,
            int version
        )
        {
            ValidateSolutionContext(current, version);
            var solutions = await Task.Run(() => service.GetSolutions());
            ValidateSolutionContext(current, version);
            if (solutions.Count == 0)
            {
                MessageBox.Show(owner, "Nenhuma solution não gerenciada disponível.");
                return;
            }
            string solutionName;
            List<SolutionComponentItem> components;
            using (
                var form = new SolutionForm(
                    solutions,
                    service.GetSolutionChoices(current),
                    selected => service.GetComponentsForMappings(current, selected)
                )
            )
            {
                if (form.ShowDialog(owner) != DialogResult.OK)
                {
                    return;
                }
                solutionName = form.SolutionName;
                components = form.SelectedComponents;
            }
            ValidateSolutionContext(current, version);
            var results = await Task.Run(() => service.AddToSolution(solutionName, components));
            using (var resultForm = new ReviewForm("Resultado da solution", results))
            {
                resultForm.ShowDialog(owner);
            }
        }

        private void ValidateSolutionContext(MappingWorkspace current, int version)
        {
            if (
                version != connectionVersion
                || !ReferenceEquals(current, workspace)
                || current.RequiresReload
            )
            {
                throw new InvalidOperationException(
                    "A conexão ou o contexto dos mapeamentos mudou. Recarregue antes de adicionar à solution."
                );
            }
        }

        private void UpdateButtons()
        {
            var hasWorkspace = workspace != null;
            var hasChanges = workspace?.HasPending == true;
            var canReview = hasChanges && !workspace.RequiresReload && !busy;
            loadTables.Enabled = mappingService != null && !busy;
            sourcePicker.Enabled = tables.Count > 0 && !busy;
            targetPicker.Enabled =
                sourcePicker.GetSelectedItem<PickerItem>() != null && tables.Count > 0 && !busy;
            viewMappings.Enabled =
                mappingService != null
                && !busy
                && sourcePicker.GetSelectedItem<PickerItem>() != null
                && targetPicker.GetSelectedItem<PickerItem>() != null;
            tablesStep.Enabled = !busy;
            mappingStep.Enabled = hasWorkspace && !busy;
            reviewStep.Enabled = canReview;
            saveChanges.Enabled = canReview && workflowStep == 2;
            saveChanges.Text = hasChanges
                ? "Revisar e salvar (" + workspace.Rows.Count(row => row.Pending) + ")"
                : "Revisar e salvar";
            undoAll.Enabled = hasChanges && !busy;
            undoAll.Visible = hasChanges;
            publish.Enabled = hasWorkspace && !workspace.RequiresReload && !busy;
            publish.Visible = workspace?.NeedsPublication == true;
            solution.Enabled = hasWorkspace && !workspace.RequiresReload && !busy;
            solution.Visible = hasWorkspace;
            moreActions.Enabled = !busy;
            refreshSource.Enabled = hasWorkspace && mappingService != null && !busy;
            refreshTarget.Enabled = refreshSource.Enabled;
            refreshMappings.Enabled = refreshSource.Enabled;
            clearChecks.Visible = workflowStep == 2;
            addMapping.Visible = workflowStep == 2;
            clearChecks.Enabled =
                !busy && (checkedSource != null || checkedTarget != null || checkedMapping != null);
            addMapping.Enabled = !busy && CanUseSelectedPair(null);
            editMapping.Enabled =
                SelectedMapping != null
                && !SelectedMapping.Protected
                && !SelectedMapping.Removed
                && !busy
                && !workspace.RequiresReload;
            removeMapping.Enabled =
                SelectedMapping != null
                && !SelectedMapping.Protected
                && !SelectedMapping.Removed
                && !busy;
            undoMapping.Enabled = SelectedMapping?.Pending == true && !busy;
            selectionSummary.Text =
                "Origem: " + (checkedSource ?? "—") + "  →  Destino: " + (checkedTarget ?? "—");
            if (mappingCheckedExplicitly && SelectedMapping != null)
            {
                selectionSummary.Text += "   |   Mapeamento: " + SelectedMapping.Description;
            }
            tips.SetToolTip(selectionSummary, selectionSummary.Text);
            if (hasWorkspace)
            {
                loadedPair.Text =
                    LucasVerissimo.XrmToolBox.Shared.BusinessLogic.MetadataLabelResolver.GetDisplayName(
                        workspace.Source
                    )
                    + " ("
                    + workspace.Map.Source
                    + ") → "
                    + LucasVerissimo.XrmToolBox.Shared.BusinessLogic.MetadataLabelResolver.GetDisplayName(
                        workspace.Target
                    )
                    + " ("
                    + workspace.Map.Target
                    + ")";
                tips.SetToolTip(loadedPair, loadedPair.Text);
            }
            if (hasWorkspace && !busy)
            {
                status.Text =
                    workspace.Map.Source
                    + " → "
                    + workspace.Map.Target
                    + " | "
                    + workspace.Rows.Count(row => row.Pending)
                    + " ajuste(s) pendente(s).";
                if (workspace.RequiresReload)
                {
                    status.Text += " Recarregue: resultado da última operação não confirmado.";
                }
                else if (workspace.NeedsPublication)
                {
                    status.Text += " Publicação pendente em Mais ações.";
                }
                else if (workspace.HasSavedChanges && !workspace.HasPending)
                {
                    status.Text =
                        "Salvo e publicado · "
                        + workspace.Map.Source
                        + " → "
                        + workspace.Map.Target;
                }
                tips.SetToolTip(status, status.Text);
            }
        }

        private bool CanUseSelectedPair(MappingRow edited)
        {
            if (workspace == null || checkedSource == null || checkedTarget == null)
            {
                return false;
            }
            if (
                MappingRules
                    .Incompatibility(
                        SelectedColumn(sourceGrid)?.Metadata,
                        SelectedColumn(targetGrid)?.Metadata
                    )
                    .Length > 0
            )
            {
                return false;
            }
            return !workspace.Rows.Any(row =>
                row != edited && !row.Removed && row.Target == checkedTarget
            );
        }
    }
}
