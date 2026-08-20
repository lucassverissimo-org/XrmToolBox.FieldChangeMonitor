using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using LucasVerissimo.XrmToolBox.Shared.BusinessLogic;
using LucasVerissimo.XrmToolBox.Shared.WinForms;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using XrmToolBox.Extensibility;

namespace LucasVerissimo.XrmToolBox.FieldChangeMonitor
{
    public partial class MyPluginControl : PluginControlBase
    {
        private Settings mySettings;
        private EntityMetadata currentEntityMetadata;
        private string currentEntityLogicalName;
        private readonly List<AttributeListItem> allColumnItems = new List<AttributeListItem>();
        private readonly List<AttributeListItem> allConditionAttributeItems =
            new List<AttributeListItem>();
        private readonly List<EntityListItem> allEntityItems = new List<EntityListItem>();
        private readonly HashSet<string> checkedMonitoredColumns = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase
        );
        private readonly List<ActiveMonitor> activeMonitors = new List<ActiveMonitor>();
        private readonly List<FilterCondition> filterConditions = new List<FilterCondition>();
        private readonly object monitorsLock = new object();
        private readonly object serviceLock = new object();
        private bool isRefreshingColumnList;
        private string currentEnvironmentName = "Ambiente Dataverse";
        private string currentEnvironmentUrl;
        private bool savedMonitorsRestored;
        private bool recentChangesRestored;
        private const int DefaultMaximumRecentChanges = 100;
        private ActiveMonitor editingMonitor;
        private bool editingMonitorWasPaused;

        public MyPluginControl()
        {
            InitializeComponent();
            ApplyVisualTheme();
            PopulateConditionOperators();
            SetMonitoringControls(false);
            UpdateConfigurationSummary();
            ResizeConfigurationSummary();
            InitializeModernInterface();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();
                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

            chkWindowsPopups.Checked = mySettings.EnableWindowsPopups;

            if (mySettings.RestoreMonitorsOnStartup)
            {
                RestoreSavedMonitorsIfPossible();
            }

            ApplyModernSettings();
        }

        private void chkWindowsPopups_CheckedChanged(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;

            if (mySettings == null)
            {
                return;
            }

            mySettings.EnableWindowsPopups = chkWindowsPopups.Checked;
            SettingsManager.Instance.Save(GetType(), mySettings);
            SetStatus(
                chkWindowsPopups.Checked
                    ? "Popups do Windows ativados."
                    : "Popups do Windows desativados."
            );
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void btnLoadColumns_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadColumns);
        }

        private void btnSearchEntities_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadEntities);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ExecuteMethod(editingMonitor == null ? (Action)StartMonitoring : SaveMonitorEdits);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMonitoring(true);
        }

        private void btnSelectAllColumns_Click(object sender, EventArgs e)
        {
            SetAllColumnsChecked(true);
        }

        private void btnClearColumnSelection_Click(object sender, EventArgs e)
        {
            SetAllColumnsChecked(false);
        }

        private void txtColumnSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyColumnFilter();
        }

        private void ConfigurationValueChanged(object sender, EventArgs e)
        {
            UpdateConfigurationSummary();
        }

        private void ListView_SizeChanged(object sender, EventArgs e)
        {
            if (sender == lvConditions && lvConditions.ClientSize.Width > 120)
            {
                var width = lvConditions.ClientSize.Width;
                colConditionField.Width = (int)(width * 0.36);
                colConditionOperator.Width = (int)(width * 0.27);
                colConditionValue.Width = Math.Max(
                    80,
                    width - colConditionField.Width - colConditionOperator.Width - 5
                );
            }
            else if (sender == lvActiveMonitors && lvActiveMonitors.ClientSize.Width > 180)
            {
                var width = lvActiveMonitors.ClientSize.Width;
                colActiveName.Width = (int)(width * 0.20);
                colActiveEntity.Width = (int)(width * 0.16);
                colActiveColumns.Width = (int)(width * 0.20);
                colActiveInterval.Width = (int)(width * 0.09);
                colActiveStatus.Width = (int)(width * 0.14);
                colActiveFilter.Width = Math.Max(
                    90,
                    width
                        - colActiveName.Width
                        - colActiveEntity.Width
                        - colActiveColumns.Width
                        - colActiveInterval.Width
                        - colActiveStatus.Width
                        - 5
                );
            }
            else if (sender == lvRecentChanges && lvRecentChanges.ClientSize.Width > 300)
            {
                var width = lvRecentChanges.ClientSize.Width;
                colChangeModifiedOn.Width = 135;
                colChangeEventType.Width = 125;
                colChangeRecordId.Width = 245;
                colChangeModifiedBy.Width = 150;
                colChangeRecordName.Width = 150;
                colChangeField.Width = 130;
                colChangeMonitor.Width = 150;
                colChangeValues.Width = Math.Max(220, width - 1095);
            }
        }

        private void btnToggleAdvanced_Click(object sender, EventArgs e)
        {
            var showAdvanced = !txtFilterXml.Visible;
            txtFilterXml.Visible = showAdvanced;
            lblFilterHint.Visible = showAdvanced;
            btnSaveFilterXml.Visible = showAdvanced;
            filterLayout.RowStyles[4].SizeType = SizeType.Absolute;
            filterLayout.RowStyles[4].Height = showAdvanced ? 92F : 0F;
            btnToggleAdvanced.Text = showAdvanced
                ? "v  Opcoes avancadas - Ocultar FetchXML"
                : ">  Opcoes avancadas - Editar FetchXML";
        }

        private void clbColumns_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (isRefreshingColumnList || e.Index < 0 || e.Index >= clbColumns.Items.Count)
            {
                return;
            }

            var item = clbColumns.Items[e.Index] as AttributeListItem;
            if (item == null)
            {
                return;
            }

            if (e.NewValue == CheckState.Checked)
            {
                checkedMonitoredColumns.Add(item.LogicalName);
            }
            else
            {
                checkedMonitoredColumns.Remove(item.LogicalName);
            }

            BeginInvoke(new Action(UpdateConfigurationSummary));
        }

        private void txtConditionFieldSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyConditionFieldFilter();
        }

        private void cboConditionOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateConditionValueState();
        }

        private void cboConditionAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateConditionValueHint();
        }

        private void cboFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SyncFilterXmlFromConditions();
        }

        private void btnAddCondition_Click(object sender, EventArgs e)
        {
            AddConditionFromBuilder();
        }

        private void btnRemoveCondition_Click(object sender, EventArgs e)
        {
            RemoveSelectedConditions();
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            filterConditions.Clear();
            RefreshConditionList();
            txtConditionValue.Clear();
            txtFilterXml.Clear();
            SetStatus("Filtro limpo.");
            UpdateConfigurationSummary();
        }

        private void btnPickConditionValue_Click(object sender, EventArgs e)
        {
            PickConditionValue();
        }

        private void btnStopSelectedMonitor_Click(object sender, EventArgs e)
        {
            StopSelectedMonitor();
        }

        private void btnSaveFilterXml_Click(object sender, EventArgs e)
        {
            SaveFilterXmlToBuilder();
        }

        private void btnRemoveSelectedMonitors_Click(object sender, EventArgs e)
        {
            RemoveSelectedMonitors();
        }

        private void btnPauseSelectedMonitors_Click(object sender, EventArgs e)
        {
            TogglePauseSelectedMonitors();
        }

        private void btnSelectAllMonitors_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvActiveMonitors.Items)
            {
                item.Selected = true;
            }

            lvActiveMonitors.Focus();
        }

        private void btnExportMonitors_Click(object sender, EventArgs e)
        {
            ExportSelectedMonitors();
        }

        private void btnImportMonitors_Click(object sender, EventArgs e)
        {
            ImportMonitors();
        }

        private void btnEditMonitor_Click(object sender, EventArgs e)
        {
            BeginEditingSelectedMonitor();
        }

        private void lvActiveMonitors_DoubleClick(object sender, EventArgs e)
        {
            BeginEditingSelectedMonitor();
        }

        private void btnCancelEdit_Click(object sender, EventArgs e)
        {
            CancelMonitorEditing(true);
        }

        private void lvRecentChanges_MouseClick(object sender, MouseEventArgs e)
        {
            var hit = lvRecentChanges.HitTest(e.Location);
            if (
                hit.Item == null
                || hit.SubItem == null
                || hit.Item.SubItems.IndexOf(hit.SubItem) != 2
            )
            {
                return;
            }

            var change = hit.Item.Tag as FieldChange;
            if (change == null)
            {
                return;
            }

            OpenRecordInBrowser(change);
        }

        private void LoadColumns()
        {
            var entityLogicalName = GetSelectedEntityLogicalName();

            if (string.IsNullOrWhiteSpace(entityLogicalName))
            {
                MessageBox.Show(
                    "Informe o nome logico da entidade.",
                    "Entidade obrigatoria",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = $"Carregando colunas de {entityLogicalName}",
                    Work = (worker, args) =>
                    {
                        var metadataService = new DataverseMetadataService(Service);
                        args.Result = metadataService.GetEntity(
                            entityLogicalName,
                            EntityFilters.Attributes,
                            CancellationToken.None
                        );
                    },
                    PostWorkCallBack = args =>
                    {
                        if (args.Error != null)
                        {
                            modernColumnsLoadedCallback = null;
                            MessageBox.Show(
                                args.Error.Message,
                                "Erro ao carregar colunas",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            SetStatus("Falha ao carregar colunas.");
                            return;
                        }

                        var entityMetadata = args.Result as EntityMetadata;
                        if (entityMetadata == null)
                        {
                            modernColumnsLoadedCallback = null;
                            MessageBox.Show(
                                "Nao foi possivel ler os metadados da entidade.",
                                "Metadados indisponiveis",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }

                        var entityChanged = !string.Equals(
                            currentEntityLogicalName,
                            entityLogicalName,
                            StringComparison.OrdinalIgnoreCase
                        );
                        currentEntityMetadata = entityMetadata;
                        currentEntityLogicalName = entityLogicalName;
                        PopulateColumns(currentEntityMetadata);
                        PopulateConditionAttributes(currentEntityMetadata);
                        if (entityChanged)
                        {
                            filterConditions.Clear();
                            RefreshConditionList();
                            txtConditionValue.Clear();
                            txtFilterXml.Clear();
                        }

                        AddLog($"Colunas carregadas para {entityLogicalName}.");
                        SetStatus(
                            $"{clbColumns.Items.Count} colunas disponiveis para {entityLogicalName}."
                        );
                        UpdateConfigurationSummary();
                        var callback = modernColumnsLoadedCallback;
                        modernColumnsLoadedCallback = null;
                        callback?.Invoke();
                    },
                }
            );
        }

        private void StartMonitoring()
        {
            var configuration = BuildMonitoringConfiguration();
            if (configuration == null)
            {
                return;
            }

            var monitor = new ActiveMonitor
            {
                Id = Guid.NewGuid(),
                Configuration = configuration,
                CancellationTokenSource = new CancellationTokenSource(),
                PreviousSnapshot = new Dictionary<Guid, RecordSnapshot>(),
                CreatedOn = DateTime.Now,
                Status = "Iniciando",
            };

            lock (monitorsLock)
            {
                activeMonitors.Add(monitor);
            }

            AddActiveMonitorListItem(monitor);
            SetMonitoringControls(false);
            PersistMonitorConfigurations();
            AddLog(
                $"Monitor adicionado para {configuration.EntityLogicalName} a cada {configuration.IntervalSeconds} segundo(s)."
            );
            SetStatus("Monitorando...");
            notifyIcon.Visible = true;

            StartMonitorTask(monitor);
        }

        private void BeginEditingSelectedMonitor()
        {
            if (lvActiveMonitors.SelectedItems.Count != 1)
            {
                MessageBox.Show(
                    "Selecione exatamente um monitoramento para editar.",
                    "Selecao obrigatoria",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var monitor = lvActiveMonitors.SelectedItems[0].Tag as ActiveMonitor;
            if (monitor == null)
            {
                return;
            }

            BeginEditingMonitor(monitor);
        }

        private void BeginEditingMonitor(ActiveMonitor monitor)
        {
            if (monitor == null)
            {
                return;
            }

            if (editingMonitor != null)
            {
                MessageBox.Show(
                    "Conclua ou cancele a edicao atual.",
                    "Edicao em andamento",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            editingMonitor = monitor;
            editingMonitorWasPaused = monitor.IsPaused;
            monitor.IsPaused = true;
            UpdateActiveMonitorStatus(monitor, "Editando");
            SetEditingUiState(true);
            SetMonitoringControls(false);

            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = $"Carregando configuracao de {monitor.DisplayName}",
                    Work = (worker, args) =>
                    {
                        var metadataService = new DataverseMetadataService(Service);
                        args.Result = metadataService.GetEntity(
                            monitor.Configuration.EntityLogicalName,
                            EntityFilters.Attributes,
                            CancellationToken.None
                        );
                    },
                    PostWorkCallBack = args =>
                    {
                        if (editingMonitor != monitor)
                        {
                            return;
                        }

                        if (args.Error != null)
                        {
                            MessageBox.Show(
                                args.Error.Message,
                                "Nao foi possivel editar",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            CancelMonitorEditing(false);
                            return;
                        }

                        var metadata = (EntityMetadata)args.Result;
                        currentEntityMetadata = metadata;
                        currentEntityLogicalName = monitor.Configuration.EntityLogicalName;
                        txtMonitorName.Text = monitor.DisplayName;
                        txtEntityLogicalName.Text = monitor.Configuration.EntityLogicalName;
                        nudIntervalSeconds.Value = Math.Max(
                            nudIntervalSeconds.Minimum,
                            Math.Min(
                                nudIntervalSeconds.Maximum,
                                monitor.Configuration.IntervalSeconds
                            )
                        );
                        PopulateColumns(metadata);
                        foreach (var column in monitor.Configuration.MonitoredColumns)
                        {
                            checkedMonitoredColumns.Add(column);
                        }
                        ApplyColumnFilter();
                        PopulateConditionAttributes(metadata);
                        LoadFilterForEditing(monitor.Configuration.FilterXml);
                        UpdateConfigurationSummary();
                        SetStatus($"Editando {monitor.DisplayName}.");
                    },
                }
            );
        }

        private void LoadEntities()
        {
            if (Service == null)
            {
                MessageBox.Show(
                    "Conecte-se a um ambiente antes de buscar entidades.",
                    "Conexao obrigatoria",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var currentLogicalName =
                (txtEntityLogicalName.SelectedItem as EntityListItem)?.LogicalName
                ?? txtEntityLogicalName.Text.Trim();

            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = "Carregando entidades do Dataverse",
                    Work = (worker, args) =>
                    {
                        var metadataService = new DataverseMetadataService(Service);
                        args.Result = metadataService.GetEntities(CancellationToken.None);
                    },
                    PostWorkCallBack = args =>
                    {
                        if (args.Error != null)
                        {
                            MessageBox.Show(
                                args.Error.Message,
                                "Erro ao buscar entidades",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            SetStatus("Falha ao buscar entidades.");
                            return;
                        }

                        var entityMetadata = args.Result as IReadOnlyCollection<EntityMetadata>;
                        allEntityItems.Clear();
                        allEntityItems.AddRange(
                            (entityMetadata ?? new EntityMetadata[0])
                                .Where(metadata => !string.IsNullOrWhiteSpace(metadata.LogicalName))
                                .Select(metadata => new EntityListItem(metadata))
                                .OrderBy(item => item.DisplayName)
                                .ThenBy(item => item.LogicalName)
                        );

                        txtEntityLogicalName.BeginUpdate();
                        txtEntityLogicalName.Items.Clear();
                        txtEntityLogicalName.Items.AddRange(
                            allEntityItems.Cast<object>().ToArray()
                        );
                        txtEntityLogicalName.EndUpdate();

                        var selectedIndex = allEntityItems.FindIndex(item =>
                            string.Equals(
                                item.LogicalName,
                                currentLogicalName,
                                StringComparison.OrdinalIgnoreCase
                            )
                        );
                        if (selectedIndex >= 0)
                        {
                            txtEntityLogicalName.SelectedIndex = selectedIndex;
                        }
                        else
                        {
                            txtEntityLogicalName.Text = currentLogicalName;
                        }

                        txtEntityLogicalName.DroppedDown = true;
                        SetStatus($"{allEntityItems.Count} entidades disponiveis para pesquisa.");
                    },
                }
            );
        }

        private string GetSelectedEntityLogicalName()
        {
            var selectedEntity = txtEntityLogicalName.SelectedItem as EntityListItem;
            return selectedEntity == null
                ? txtEntityLogicalName.Text.Trim()
                : selectedEntity.LogicalName;
        }

        private void LoadFilterForEditing(string filterXml)
        {
            filterConditions.Clear();
            txtFilterXml.Text = filterXml ?? string.Empty;
            if (string.IsNullOrWhiteSpace(filterXml))
            {
                RefreshConditionList();
                return;
            }

            try
            {
                var filter = XElement.Parse(filterXml);
                if (filter.Descendants("filter").Any())
                {
                    throw new InvalidOperationException("O filtro possui grupos aninhados.");
                }

                var filterType = ((string)filter.Attribute("type") ?? "and").ToLowerInvariant();
                foreach (var element in filter.Elements("condition"))
                {
                    var attributeName = (string)element.Attribute("attribute");
                    var operatorName = (string)element.Attribute("operator");
                    var attribute = allConditionAttributeItems.FirstOrDefault(item =>
                        string.Equals(
                            item.LogicalName,
                            attributeName,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );
                    var conditionOperator = cboConditionOperator
                        .Items.Cast<object>()
                        .OfType<ConditionOperatorItem>()
                        .FirstOrDefault(item =>
                            string.Equals(
                                item.Operator,
                                operatorName,
                                StringComparison.OrdinalIgnoreCase
                            )
                        );
                    if (string.IsNullOrWhiteSpace(attributeName) || conditionOperator == null)
                    {
                        throw new InvalidOperationException(
                            "Uma condicao nao pode ser representada pelo construtor visual."
                        );
                    }

                    var values = element.Elements("value").Select(value => value.Value).ToList();
                    var singleValue = (string)element.Attribute("value");
                    if (singleValue != null)
                    {
                        values.Insert(0, singleValue);
                    }

                    filterConditions.Add(
                        new FilterCondition
                        {
                            AttributeLogicalName = attributeName,
                            AttributeDisplayName =
                                attribute == null ? attributeName : attribute.DisplayName,
                            Operator = conditionOperator.Operator,
                            OperatorDisplayName = conditionOperator.DisplayName,
                            Values = values,
                        }
                    );
                }

                cboFilterType.SelectedItem = filterType == "or" ? "or" : "and";
                txtFilterXml.Text = filter.ToString();
                RefreshConditionList();
            }
            catch (Exception ex)
            {
                filterConditions.Clear();
                RefreshConditionList();
                txtFilterXml.Text = filterXml;
                AddLog(
                    $"Filtro de {editingMonitor?.DisplayName}: edite pelo FetchXML avancado. {ex.Message}"
                );
            }
        }

        private void SaveMonitorEdits()
        {
            if (editingMonitor == null)
            {
                return;
            }

            var configuration = BuildMonitoringConfiguration();
            if (configuration == null)
            {
                return;
            }

            var monitor = editingMonitor;
            monitor.CancellationTokenSource.Cancel();
            monitor.Configuration = configuration;
            monitor.CancellationTokenSource = new CancellationTokenSource();
            monitor.Task = null;
            monitor.PreviousSnapshot = new Dictionary<Guid, RecordSnapshot>();
            monitor.MonitoredRecordCount = 0;
            monitor.LastQueryOn = null;
            monitor.IsPaused = editingMonitorWasPaused;
            monitor.NeedsBaselineReset = true;
            RefreshActiveMonitorListItem(monitor);
            UpdateActiveMonitorStatus(monitor, monitor.IsPaused ? "Pausado" : "Iniciando");

            editingMonitor = null;
            SetEditingUiState(false);
            if (!monitor.IsPaused)
            {
                StartMonitorTask(monitor);
            }

            PersistMonitorConfigurations();
            SetMonitoringControls(false);
            AddLog($"Monitoramento atualizado: {monitor.DisplayName}.");
            SetStatus("Alteracoes do monitoramento salvas.");
        }

        private void CancelMonitorEditing(bool showStatus)
        {
            if (editingMonitor == null)
            {
                return;
            }

            var monitor = editingMonitor;
            monitor.IsPaused = editingMonitorWasPaused;
            monitor.NeedsBaselineReset = !editingMonitorWasPaused;
            UpdateActiveMonitorStatus(monitor, editingMonitorWasPaused ? "Pausado" : "Retomando");
            editingMonitor = null;
            SetEditingUiState(false);
            if (!monitor.IsPaused)
            {
                StartMonitorTask(monitor);
            }

            SetMonitoringControls(false);
            if (showStatus)
            {
                SetStatus("Edicao cancelada; nenhuma alteracao foi aplicada.");
            }
        }

        private void SetEditingUiState(bool editing)
        {
            btnStart.Text = editing ? "Salvar alteracoes" : "Iniciar monitoramento";
            btnCancelEdit.Visible = editing;
            btnStart.BringToFront();
            btnCancelEdit.BringToFront();
            ResizeConfigurationSummary();
            lblConfigurationReady.Text =
                editing && editingMonitor != null
                    ? $"Editando: {editingMonitor.DisplayName}"
                    : "Configure o monitoramento";
            lvActiveMonitors.Enabled = !editing;
            btnEditMonitor.Enabled = !editing && lvActiveMonitors.Items.Count > 0;
            btnRemoveSelectedMonitors.Enabled = !editing && HasActiveMonitors();
            btnPauseSelectedMonitors.Enabled = !editing && HasActiveMonitors();
            btnExportMonitors.Enabled = !editing && HasActiveMonitors();
            btnImportMonitors.Enabled = !editing && Service != null;
        }

        private void summaryPanel_SizeChanged(object sender, EventArgs e)
        {
            ResizeConfigurationSummary();
        }

        private void ResizeConfigurationSummary()
        {
            if (lblConfigurationSummary == null || btnStart == null || btnCancelEdit == null)
            {
                return;
            }

            var rightBoundary = btnCancelEdit.Visible ? btnCancelEdit.Left : btnStart.Left;
            lblConfigurationSummary.Width = Math.Max(
                80,
                rightBoundary - lblConfigurationSummary.Left - 12
            );
            lblConfigurationReady.Width = Math.Max(
                80,
                rightBoundary - lblConfigurationReady.Left - 12
            );
        }

        private void RefreshActiveMonitorListItem(ActiveMonitor monitor)
        {
            if (monitor.ListViewItem == null)
            {
                return;
            }

            var item = monitor.ListViewItem;
            item.SubItems[0].Text = monitor.DisplayName;
            item.SubItems[1].Text = monitor.Configuration.EntityLogicalName;
            item.SubItems[2].Text = string.Join(", ", monitor.Configuration.MonitoredColumns);
            item.SubItems[3].Text = monitor.Configuration.IntervalSeconds.ToString(
                CultureInfo.InvariantCulture
            );
            item.SubItems[4].Text = monitor.Status;
            item.SubItems[5].Text = string.IsNullOrWhiteSpace(monitor.Configuration.FilterXml)
                ? "(sem filtro)"
                : monitor.Configuration.FilterXml;
        }

        private void StartMonitorTask(ActiveMonitor monitor)
        {
            if (monitor.Task != null && !monitor.Task.IsCompleted)
            {
                return;
            }

            if (
                monitor.CancellationTokenSource == null
                || monitor.CancellationTokenSource.IsCancellationRequested
            )
            {
                monitor.CancellationTokenSource = new CancellationTokenSource();
            }

            var token = monitor.CancellationTokenSource.Token;
            monitor.Task = Task.Run(() => MonitorAsync(monitor, token), token);
        }

        private MonitoringConfiguration BuildMonitoringConfiguration()
        {
            var monitorName = txtMonitorName.Text.Trim();
            if (string.IsNullOrWhiteSpace(monitorName))
            {
                MessageBox.Show(
                    "Informe um nome para identificar o monitoramento.",
                    "Nome obrigatorio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtMonitorName.Focus();
                return null;
            }

            lock (monitorsLock)
            {
                if (
                    activeMonitors.Any(monitor =>
                        monitor != editingMonitor
                        && string.Equals(
                            monitor.DisplayName,
                            monitorName,
                            StringComparison.CurrentCultureIgnoreCase
                        )
                    )
                )
                {
                    MessageBox.Show(
                        "Ja existe um monitoramento com esse nome.",
                        "Nome duplicado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    txtMonitorName.Focus();
                    return null;
                }
            }

            var entityLogicalName = GetSelectedEntityLogicalName();
            if (string.IsNullOrWhiteSpace(entityLogicalName))
            {
                MessageBox.Show(
                    "Informe o nome logico da entidade.",
                    "Entidade obrigatoria",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return null;
            }

            if (
                currentEntityMetadata == null
                || !string.Equals(
                    currentEntityLogicalName,
                    entityLogicalName,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                MessageBox.Show(
                    "Carregue as colunas da entidade antes de iniciar o monitoramento.",
                    "Colunas nao carregadas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return null;
            }

            var monitoredColumns = checkedMonitoredColumns
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(column => column)
                .ToList();

            if (monitoredColumns.Count == 0)
            {
                MessageBox.Show(
                    "Selecione ao menos uma coluna para monitorar.",
                    "Colunas obrigatorias",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return null;
            }

            if (string.IsNullOrWhiteSpace(currentEntityMetadata.PrimaryIdAttribute))
            {
                MessageBox.Show(
                    "A entidade nao possui uma coluna primaria identificavel.",
                    "Entidade invalida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return null;
            }

            string normalizedFilter;
            string filterError;
            if (!TryNormalizeFilterXml(txtFilterXml.Text, out normalizedFilter, out filterError))
            {
                MessageBox.Show(
                    filterError,
                    "Filtro invalido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return null;
            }

            return new MonitoringConfiguration
            {
                MonitorName = monitorName,
                Service = Service,
                EntityLogicalName = entityLogicalName,
                PrimaryIdAttribute = currentEntityMetadata.PrimaryIdAttribute,
                PrimaryNameAttribute = currentEntityMetadata.PrimaryNameAttribute,
                MonitoredColumns = monitoredColumns,
                IntervalSeconds = Convert.ToInt32(nudIntervalSeconds.Value),
                FilterXml = normalizedFilter,
                FetchXml = BuildFetchXml(
                    entityLogicalName,
                    currentEntityMetadata.PrimaryIdAttribute,
                    currentEntityMetadata.PrimaryNameAttribute,
                    monitoredColumns,
                    normalizedFilter
                ),
            };
        }

        private async Task MonitorAsync(ActiveMonitor monitor, CancellationToken cancellationToken)
        {
            var isFirstRun = true;
            var configuration = monitor.Configuration;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (monitor.IsPaused)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }

                    continue;
                }

                try
                {
                    var currentSnapshot = RetrieveSnapshot(configuration);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    monitor.MonitoredRecordCount = currentSnapshot.Count;
                    monitor.LastQueryOn = DateTime.Now;

                    if (isFirstRun && monitor.PreviousSnapshot.Count == 0)
                    {
                        monitor.PreviousSnapshot = currentSnapshot;
                        monitor.NeedsBaselineReset = false;
                        isFirstRun = false;
                        RunOnUiThread(() =>
                        {
                            UpdateActiveMonitorStatus(monitor, $"Ativo ({currentSnapshot.Count})");
                            AddLog(
                                $"[{monitor.DisplayName}] Snapshot inicial registrado com {currentSnapshot.Count} registro(s)."
                            );
                            SetStatus($"Monitorando {currentSnapshot.Count} registro(s).");
                            RefreshModernMonitorGrid();
                        });
                    }
                    else
                    {
                        var changes = DetectChanges(
                            monitor.PreviousSnapshot,
                            currentSnapshot,
                            configuration
                        );
                        monitor.PreviousSnapshot = currentSnapshot;
                        monitor.NeedsBaselineReset = false;
                        isFirstRun = false;

                        RunOnUiThread(() =>
                        {
                            if (changes.Count > 0)
                            {
                                UpdateActiveMonitorStatus(
                                    monitor,
                                    $"{changes.Count} alteracao(oes)"
                                );
                                ReportChanges(monitor, changes);
                                SetStatus($"{changes.Count} alteracao(oes) detectada(s).");
                            }
                            else
                            {
                                UpdateActiveMonitorStatus(
                                    monitor,
                                    $"Ativo ({currentSnapshot.Count})"
                                );
                                SetStatus(
                                    $"Sem alteracoes. Ultima consulta: {DateTime.Now:HH:mm:ss}"
                                );
                            }

                            RefreshModernMonitorGrid();
                        });
                    }
                }
                catch (Exception ex)
                {
                    RunOnUiThread(() =>
                    {
                        UpdateActiveMonitorStatus(monitor, "Erro");
                        AddLog($"[{monitor.DisplayName}] Erro na consulta: {ex.Message}");
                        SetStatus("Erro na ultima consulta; o monitor continua ativo.");
                    });
                }

                try
                {
                    await Task.Delay(
                            TimeSpan.FromSeconds(configuration.IntervalSeconds),
                            cancellationToken
                        )
                        .ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            if (
                monitor.CancellationTokenSource != null
                && monitor.CancellationTokenSource.Token == cancellationToken
            )
            {
                RunOnUiThread(() => UpdateActiveMonitorStatus(monitor, "Parado"));
            }
        }

        private Dictionary<Guid, RecordSnapshot> RetrieveSnapshot(
            MonitoringConfiguration configuration
        )
        {
            var snapshot = new Dictionary<Guid, RecordSnapshot>();
            var pageNumber = 1;
            string pagingCookie = null;
            bool moreRecords;

            do
            {
                var fetchXml = ApplyPaging(configuration.FetchXml, pageNumber, pagingCookie);
                EntityCollection result;

                lock (serviceLock)
                {
                    result = configuration.Service.RetrieveMultiple(new FetchExpression(fetchXml));
                }

                foreach (var entity in result.Entities)
                {
                    var recordId = entity.Id;
                    if (recordId == Guid.Empty && entity.Contains(configuration.PrimaryIdAttribute))
                    {
                        recordId = (Guid)entity[configuration.PrimaryIdAttribute];
                    }

                    if (recordId == Guid.Empty)
                    {
                        continue;
                    }

                    snapshot[recordId] = CreateRecordSnapshot(entity, configuration);
                }

                moreRecords = result.MoreRecords;
                pagingCookie = result.PagingCookie;
                pageNumber++;
            } while (moreRecords);

            return snapshot;
        }

        private List<FieldChange> DetectChanges(
            Dictionary<Guid, RecordSnapshot> oldSnapshot,
            Dictionary<Guid, RecordSnapshot> currentSnapshot,
            MonitoringConfiguration configuration
        )
        {
            var changes = new List<FieldChange>();

            foreach (var currentRecord in currentSnapshot.Values)
            {
                RecordSnapshot oldRecord;
                if (!oldSnapshot.TryGetValue(currentRecord.RecordId, out oldRecord))
                {
                    foreach (var column in configuration.MonitoredColumns)
                    {
                        FieldValue currentValue;
                        currentRecord.Values.TryGetValue(column, out currentValue);
                        changes.Add(
                            CreateFieldChange(
                                currentRecord,
                                configuration.EntityLogicalName,
                                column,
                                ChangeKind.EnteredFilter,
                                null,
                                currentValue
                            )
                        );
                    }

                    continue;
                }

                foreach (var column in configuration.MonitoredColumns)
                {
                    FieldValue oldValue;
                    FieldValue currentValue;

                    oldRecord.Values.TryGetValue(column, out oldValue);
                    currentRecord.Values.TryGetValue(column, out currentValue);

                    var oldNormalizedValue =
                        oldValue == null ? string.Empty : oldValue.NormalizedValue;
                    var currentNormalizedValue =
                        currentValue == null ? string.Empty : currentValue.NormalizedValue;

                    if (
                        !string.Equals(
                            oldNormalizedValue,
                            currentNormalizedValue,
                            StringComparison.Ordinal
                        )
                    )
                    {
                        changes.Add(
                            CreateFieldChange(
                                currentRecord,
                                configuration.EntityLogicalName,
                                column,
                                ChangeKind.ValueChanged,
                                oldValue,
                                currentValue
                            )
                        );
                    }
                }
            }

            foreach (
                var oldRecord in oldSnapshot.Values.Where(record =>
                    !currentSnapshot.ContainsKey(record.RecordId)
                )
            )
            {
                var recordOutsideFilter = RetrieveRecordWithoutMonitorFilter(
                    configuration,
                    oldRecord.RecordId
                );
                if (recordOutsideFilter == null)
                {
                    foreach (var column in configuration.MonitoredColumns)
                    {
                        FieldValue oldValue;
                        oldRecord.Values.TryGetValue(column, out oldValue);
                        changes.Add(
                            CreateFieldChange(
                                oldRecord,
                                configuration.EntityLogicalName,
                                column,
                                ChangeKind.RecordUnavailable,
                                oldValue,
                                null
                            )
                        );
                    }

                    continue;
                }

                foreach (var column in configuration.MonitoredColumns)
                {
                    FieldValue oldValue;
                    FieldValue currentValue;
                    oldRecord.Values.TryGetValue(column, out oldValue);
                    recordOutsideFilter.Values.TryGetValue(column, out currentValue);
                    changes.Add(
                        CreateFieldChange(
                            recordOutsideFilter,
                            configuration.EntityLogicalName,
                            column,
                            ChangeKind.ExitedFilter,
                            oldValue,
                            currentValue
                        )
                    );
                }
            }

            return changes;
        }

        private RecordSnapshot RetrieveRecordWithoutMonitorFilter(
            MonitoringConfiguration configuration,
            Guid recordId
        )
        {
            var columns = configuration
                .MonitoredColumns.Concat(
                    new[]
                    {
                        configuration.PrimaryIdAttribute,
                        configuration.PrimaryNameAttribute,
                        "modifiedon",
                        "modifiedby",
                    }
                )
                .Where(column => !string.IsNullOrWhiteSpace(column))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var query = new QueryExpression(configuration.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(columns),
                TopCount = 1,
            };
            query.Criteria.AddCondition(
                configuration.PrimaryIdAttribute,
                ConditionOperator.Equal,
                recordId
            );

            EntityCollection result;
            lock (serviceLock)
            {
                result = configuration.Service.RetrieveMultiple(query);
            }

            var entity = result.Entities.FirstOrDefault();
            return entity == null ? null : CreateRecordSnapshot(entity, configuration);
        }

        private RecordSnapshot CreateRecordSnapshot(
            Entity entity,
            MonitoringConfiguration configuration
        )
        {
            var values = new Dictionary<string, FieldValue>(StringComparer.OrdinalIgnoreCase);
            foreach (var column in configuration.MonitoredColumns)
            {
                var rawValue = entity.Contains(column) ? entity[column] : null;
                var formattedValue = entity.FormattedValues.Contains(column)
                    ? entity.FormattedValues[column]
                    : null;
                values[column] = new FieldValue
                {
                    NormalizedValue = NormalizeValue(rawValue),
                    DisplayValue = FormatValue(rawValue, formattedValue),
                };
            }

            return new RecordSnapshot
            {
                RecordId = entity.Id,
                RecordName = GetRecordName(entity, configuration.PrimaryNameAttribute),
                ModifiedOn =
                    entity.Contains("modifiedon") && entity["modifiedon"] is DateTime
                        ? ((DateTime)entity["modifiedon"]).ToLocalTime()
                        : DateTime.Now,
                ModifiedBy = ResolveModifiedBy(entity, configuration),
                Values = values,
            };
        }

        private string ResolveModifiedBy(Entity entity, MonitoringConfiguration configuration)
        {
            var formattedValue = entity.FormattedValues.Contains("modifiedby")
                ? entity.FormattedValues["modifiedby"]
                : null;
            var reference = entity.Contains("modifiedby")
                ? entity["modifiedby"] as EntityReference
                : null;

            if (!string.IsNullOrWhiteSpace(formattedValue))
            {
                if (reference != null)
                {
                    lock (configuration.ModifiedByNamesLock)
                    {
                        configuration.ModifiedByNames[reference.Id] = formattedValue;
                    }
                }

                return formattedValue;
            }

            if (reference == null)
            {
                return FormatValue(
                    entity.Contains("modifiedby") ? entity["modifiedby"] : null,
                    null
                );
            }

            if (!string.IsNullOrWhiteSpace(reference.Name))
            {
                lock (configuration.ModifiedByNamesLock)
                {
                    configuration.ModifiedByNames[reference.Id] = reference.Name;
                }

                return reference.Name;
            }

            lock (configuration.ModifiedByNamesLock)
            {
                string cachedName;
                if (configuration.ModifiedByNames.TryGetValue(reference.Id, out cachedName))
                {
                    return cachedName;
                }
            }

            try
            {
                Entity user;
                lock (serviceLock)
                {
                    user = configuration.Service.Retrieve(
                        "systemuser",
                        reference.Id,
                        new ColumnSet("fullname")
                    );
                }

                var fullName = user.GetAttributeValue<string>("fullname");
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    lock (configuration.ModifiedByNamesLock)
                    {
                        configuration.ModifiedByNames[reference.Id] = fullName;
                    }

                    return fullName;
                }
            }
            catch
            {
                // Keep monitoring even if the user cannot be read (for example, due to permissions).
            }

            return reference.Id.ToString("D");
        }

        private static FieldChange CreateFieldChange(
            RecordSnapshot record,
            string entityLogicalName,
            string column,
            ChangeKind kind,
            FieldValue oldValue,
            FieldValue newValue
        )
        {
            return new FieldChange
            {
                RecordId = record.RecordId,
                RecordName = record.RecordName,
                EntityLogicalName = entityLogicalName,
                ModifiedOn = record.ModifiedOn,
                ModifiedBy = record.ModifiedBy,
                ColumnLogicalName = column,
                Kind = kind,
                OldValue = oldValue == null ? "(vazio)" : oldValue.DisplayValue,
                NewValue = newValue == null ? "(vazio)" : newValue.DisplayValue,
            };
        }

        private void ReportChanges(ActiveMonitor monitor, List<FieldChange> changes)
        {
            foreach (var change in changes.Take(20))
            {
                AddLog(
                    $"[{monitor.DisplayName}] {change.EventDescription}: {change.RecordName} [{change.RecordId}] - {change.ColumnLogicalName}: {change.ChangeDescription}"
                );
            }

            foreach (var change in changes)
            {
                AddRecentChange(monitor, change);
            }

            if (changes.Count > 20)
            {
                AddLog($"Mais {changes.Count - 20} alteracao(oes) omitida(s) do log.");
            }

            PersistRecentChanges();
            ShowWindowsAlert(monitor, changes);
        }

        private void ShowWindowsAlert(ActiveMonitor monitor, List<FieldChange> changes)
        {
            if (!chkWindowsPopups.Checked)
            {
                return;
            }

            notifyIcon.Visible = true;
            notifyIcon.BalloonTipTitle = $"{currentEnvironmentName} - {monitor.DisplayName}";
            notifyIcon.BalloonTipText = BuildAlertMessage(changes);
            notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
            notifyIcon.ShowBalloonTip(10000);
            System.Media.SystemSounds.Exclamation.Play();
        }

        private void AddRecentChange(ActiveMonitor monitor, FieldChange change)
        {
            change.MonitorName = monitor.DisplayName;
            var item = new ListViewItem(change.ModifiedOn.ToString("dd/MM/yyyy HH:mm:ss"));
            item.SubItems.Add(change.EventDescription);
            item.SubItems.Add(change.RecordId.ToString("D"));
            item.SubItems.Add(change.ModifiedBy);
            item.SubItems.Add(change.RecordName);
            item.SubItems.Add(change.ColumnLogicalName);
            item.SubItems.Add(change.ChangeDescription);
            item.SubItems.Add(monitor.DisplayName);
            item.SubItems[2].ForeColor = Color.FromArgb(0, 102, 204);
            item.UseItemStyleForSubItems = false;
            item.Tag = change;
            lvRecentChanges.Items.Insert(0, item);

            while (lvRecentChanges.Items.Count > GetMaximumRecentChanges())
            {
                lvRecentChanges.Items.RemoveAt(lvRecentChanges.Items.Count - 1);
            }

            RefreshModernRecentGrid();
        }

        private void RestoreRecentChangesIfPossible()
        {
            if (
                recentChangesRestored
                || mySettings == null
                || string.IsNullOrWhiteSpace(currentEnvironmentUrl)
            )
            {
                return;
            }

            recentChangesRestored = true;
            var changes = (mySettings.RecentChanges ?? new List<PersistedFieldChange>())
                .Where(change =>
                    string.Equals(
                        NormalizeEnvironmentUrl(change.EnvironmentUrl),
                        NormalizeEnvironmentUrl(currentEnvironmentUrl),
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                .OrderByDescending(change => change.ModifiedOn)
                .Take(GetMaximumRecentChanges())
                .ToList();

            lvRecentChanges.BeginUpdate();
            try
            {
                foreach (var persisted in changes.OrderBy(change => change.ModifiedOn))
                {
                    AddRecentChangeToList(
                        new FieldChange
                        {
                            RecordId = persisted.RecordId,
                            RecordName = persisted.RecordName,
                            EntityLogicalName = persisted.EntityLogicalName,
                            MonitorName = persisted.MonitorName,
                            ModifiedOn = persisted.ModifiedOn,
                            ModifiedBy = persisted.ModifiedBy,
                            ColumnLogicalName = persisted.ColumnLogicalName,
                            Kind = (ChangeKind)persisted.Kind,
                            OldValue = persisted.OldValue,
                            NewValue = persisted.NewValue,
                        }
                    );
                }
            }
            finally
            {
                lvRecentChanges.EndUpdate();
            }

            PersistRecentChanges();
        }

        private void AddRecentChangeToList(FieldChange change)
        {
            var item = new ListViewItem(change.ModifiedOn.ToString("dd/MM/yyyy HH:mm:ss"));
            item.SubItems.Add(change.EventDescription);
            item.SubItems.Add(change.RecordId.ToString("D"));
            item.SubItems.Add(change.ModifiedBy);
            item.SubItems.Add(change.RecordName);
            item.SubItems.Add(change.ColumnLogicalName);
            item.SubItems.Add(change.ChangeDescription);
            item.SubItems.Add(change.MonitorName);
            item.SubItems[2].ForeColor = Color.FromArgb(0, 102, 204);
            item.UseItemStyleForSubItems = false;
            item.Tag = change;
            lvRecentChanges.Items.Insert(0, item);
        }

        private void PersistRecentChanges()
        {
            if (mySettings == null || string.IsNullOrWhiteSpace(currentEnvironmentUrl))
            {
                return;
            }

            var otherEnvironments = (mySettings.RecentChanges ?? new List<PersistedFieldChange>())
                .Where(change =>
                    !string.Equals(
                        NormalizeEnvironmentUrl(change.EnvironmentUrl),
                        NormalizeEnvironmentUrl(currentEnvironmentUrl),
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                .ToList();
            otherEnvironments.AddRange(
                lvRecentChanges
                    .Items.Cast<ListViewItem>()
                    .Take(GetMaximumRecentChanges())
                    .Select(item => item.Tag as FieldChange)
                    .Where(change => change != null)
                    .Select(change => new PersistedFieldChange
                    {
                        EnvironmentUrl = currentEnvironmentUrl,
                        RecordId = change.RecordId,
                        RecordName = change.RecordName,
                        EntityLogicalName = change.EntityLogicalName,
                        MonitorName = change.MonitorName,
                        ModifiedOn = change.ModifiedOn,
                        ModifiedBy = change.ModifiedBy,
                        ColumnLogicalName = change.ColumnLogicalName,
                        Kind = (int)change.Kind,
                        OldValue = change.OldValue,
                        NewValue = change.NewValue,
                    })
            );
            mySettings.RecentChanges = otherEnvironments;
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        private void OpenRecordInBrowser(FieldChange change)
        {
            if (string.IsNullOrWhiteSpace(currentEnvironmentUrl))
            {
                MessageBox.Show(
                    "A URL do ambiente nao esta disponivel.",
                    "Nao foi possivel abrir o registro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (mySettings == null || mySettings.ConfirmBeforeOpeningRecord)
            {
                var confirmation = MessageBox.Show(
                    $"Deseja abrir o registro {change.RecordId:D} no navegador padrao?",
                    "Abrir registro do Dataverse",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (confirmation != DialogResult.Yes)
                {
                    return;
                }
            }

            var baseUrl = currentEnvironmentUrl.TrimEnd('/');
            var recordId = Uri.EscapeDataString(change.RecordId.ToString("D"));
            var url =
                $"{baseUrl}/main.aspx?pagetype=entityrecord&etn={Uri.EscapeDataString(change.EntityLogicalName)}&id={recordId}";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private static string BuildAlertMessage(List<FieldChange> changes)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{changes.Count} evento(s) encontrado(s).");

            foreach (var change in changes.Take(4))
            {
                builder.AppendLine(
                    $"{change.EventDescription} - {change.ColumnLogicalName}: {TrimForAlert(change.ChangeDescription)}"
                );
            }

            if (changes.Count > 4)
            {
                builder.AppendLine("Abra o plugin para ver os demais itens.");
            }

            return builder.ToString();
        }

        private static string TrimForAlert(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "(vazio)";
            }

            return value.Length <= 40 ? value : value.Substring(0, 37) + "...";
        }

        private MonitorDefinition ToDefinition(
            ActiveMonitor monitor,
            bool includeEnvironment,
            bool includeGeneratedFetch,
            bool includeSnapshot
        )
        {
            return new MonitorDefinition
            {
                Name = monitor.DisplayName,
                EntityLogicalName = monitor.Configuration.EntityLogicalName,
                PrimaryIdAttribute = monitor.Configuration.PrimaryIdAttribute,
                PrimaryNameAttribute = monitor.Configuration.PrimaryNameAttribute,
                IntervalSeconds = monitor.Configuration.IntervalSeconds,
                MonitoredColumns = monitor.Configuration.MonitoredColumns.ToList(),
                FilterXml = monitor.Configuration.FilterXml,
                FetchXml = includeGeneratedFetch ? monitor.Configuration.FetchXml : null,
                IsPaused = monitor.IsPaused,
                EnvironmentUrl = includeEnvironment ? currentEnvironmentUrl : null,
                LastSnapshot = includeSnapshot
                    ? ToPersistedSnapshot(monitor.PreviousSnapshot)
                    : null,
            };
        }

        private static List<PersistedRecordSnapshot> ToPersistedSnapshot(
            Dictionary<Guid, RecordSnapshot> snapshot
        )
        {
            return (snapshot ?? new Dictionary<Guid, RecordSnapshot>())
                .Values.Select(record => new PersistedRecordSnapshot
                {
                    RecordId = record.RecordId,
                    RecordName = record.RecordName,
                    ModifiedOn = record.ModifiedOn,
                    ModifiedBy = record.ModifiedBy,
                    Values = (record.Values ?? new Dictionary<string, FieldValue>())
                        .Select(value => new PersistedFieldValue
                        {
                            ColumnLogicalName = value.Key,
                            NormalizedValue =
                                value.Value == null ? null : value.Value.NormalizedValue,
                            DisplayValue = value.Value == null ? null : value.Value.DisplayValue,
                        })
                        .ToList(),
                })
                .ToList();
        }

        private static Dictionary<Guid, RecordSnapshot> FromPersistedSnapshot(
            IEnumerable<PersistedRecordSnapshot> snapshot
        )
        {
            return (snapshot ?? Enumerable.Empty<PersistedRecordSnapshot>()).ToDictionary(
                record => record.RecordId,
                record => new RecordSnapshot
                {
                    RecordId = record.RecordId,
                    RecordName = record.RecordName,
                    ModifiedOn = record.ModifiedOn,
                    ModifiedBy = record.ModifiedBy,
                    Values = (record.Values ?? new List<PersistedFieldValue>())
                        .Where(value => !string.IsNullOrWhiteSpace(value.ColumnLogicalName))
                        .ToDictionary(
                            value => value.ColumnLogicalName,
                            value => new FieldValue
                            {
                                NormalizedValue = value.NormalizedValue,
                                DisplayValue = value.DisplayValue,
                            },
                            StringComparer.OrdinalIgnoreCase
                        ),
                }
            );
        }

        private void PersistMonitorConfigurations()
        {
            if (mySettings == null)
            {
                return;
            }

            lock (monitorsLock)
            {
                var otherEnvironmentMonitors = (
                    mySettings.SavedMonitors ?? new List<MonitorDefinition>()
                )
                    .Where(definition =>
                        !string.Equals(
                            NormalizeEnvironmentUrl(definition.EnvironmentUrl),
                            NormalizeEnvironmentUrl(currentEnvironmentUrl),
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();
                otherEnvironmentMonitors.AddRange(
                    activeMonitors
                        .Select(monitor => ToDefinition(monitor, true, true, true))
                        .ToList()
                );
                mySettings.SavedMonitors = otherEnvironmentMonitors;
            }

            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        private void RestoreSavedMonitorsIfPossible()
        {
            if (
                savedMonitorsRestored
                || mySettings == null
                || Service == null
                || string.IsNullOrWhiteSpace(currentEnvironmentUrl)
            )
            {
                return;
            }

            savedMonitorsRestored = true;
            RestoreRecentChangesIfPossible();
            var definitions = mySettings.SavedMonitors ?? new List<MonitorDefinition>();
            var matchingDefinitions = definitions
                .Where(definition =>
                    string.Equals(
                        NormalizeEnvironmentUrl(definition.EnvironmentUrl),
                        NormalizeEnvironmentUrl(currentEnvironmentUrl),
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                .ToList();

            foreach (var definition in matchingDefinitions)
            {
                if (
                    string.IsNullOrWhiteSpace(definition.FetchXml)
                    || definition.MonitoredColumns == null
                )
                {
                    continue;
                }

                AddConfiguredMonitor(
                    new MonitoringConfiguration
                    {
                        MonitorName = EnsureUniqueMonitorName(definition.Name),
                        Service = Service,
                        EntityLogicalName = definition.EntityLogicalName,
                        PrimaryIdAttribute = definition.PrimaryIdAttribute,
                        PrimaryNameAttribute = definition.PrimaryNameAttribute,
                        MonitoredColumns = definition.MonitoredColumns.ToList(),
                        IntervalSeconds = Math.Max(1, definition.IntervalSeconds),
                        FilterXml = definition.FilterXml,
                        FetchXml = definition.FetchXml,
                    },
                    "Restaurado",
                    true,
                    FromPersistedSnapshot(definition.LastSnapshot)
                );
            }

            if (matchingDefinitions.Count > 0)
            {
                AddLog(
                    $"{matchingDefinitions.Count} monitoramento(s) restaurado(s) como pausado(s)."
                );
                SetMonitoringControls(false);
            }
        }

        private static string NormalizeEnvironmentUrl(string url)
        {
            return string.IsNullOrWhiteSpace(url) ? string.Empty : url.Trim().TrimEnd('/');
        }

        private void AddConfiguredMonitor(
            MonitoringConfiguration configuration,
            string source,
            bool isPaused,
            Dictionary<Guid, RecordSnapshot> previousSnapshot = null
        )
        {
            var monitor = new ActiveMonitor
            {
                Id = Guid.NewGuid(),
                Configuration = configuration,
                CancellationTokenSource = new CancellationTokenSource(),
                PreviousSnapshot = previousSnapshot ?? new Dictionary<Guid, RecordSnapshot>(),
                MonitoredRecordCount = previousSnapshot == null ? 0 : previousSnapshot.Count,
                CreatedOn = DateTime.Now,
                Status = isPaused ? "Pausado" : "Iniciando",
                IsPaused = isPaused,
                NeedsBaselineReset = false,
            };

            lock (monitorsLock)
            {
                activeMonitors.Add(monitor);
            }

            AddActiveMonitorListItem(monitor);
            AddLog(
                $"{source}: {monitor.DisplayName} adicionado como {(isPaused ? "pausado" : "ativo")}."
            );
            if (!isPaused)
            {
                StartMonitorTask(monitor);
            }
        }

        private string EnsureUniqueMonitorName(string requestedName)
        {
            var baseName = string.IsNullOrWhiteSpace(requestedName)
                ? "Monitor importado"
                : requestedName.Trim();
            var candidate = baseName;
            var suffix = 2;

            lock (monitorsLock)
            {
                while (
                    activeMonitors.Any(monitor =>
                        string.Equals(
                            monitor.DisplayName,
                            candidate,
                            StringComparison.CurrentCultureIgnoreCase
                        )
                    )
                )
                {
                    candidate = $"{baseName} ({suffix++})";
                }
            }

            return candidate;
        }

        private void ExportSelectedMonitors()
        {
            if (lvActiveMonitors.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Selecione ao menos um monitoramento para exportar.",
                    "Nenhum monitoramento selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var selectedMonitors = lvActiveMonitors
                .SelectedItems.Cast<ListViewItem>()
                .Select(item => item.Tag as ActiveMonitor)
                .Where(monitor => monitor != null)
                .ToList();
            ExportMonitors(selectedMonitors);
        }

        private void ExportMonitors(IEnumerable<ActiveMonitor> monitors)
        {
            var selectedMonitors = (monitors ?? Enumerable.Empty<ActiveMonitor>())
                .Where(monitor => monitor != null)
                .Distinct()
                .ToList();
            if (selectedMonitors.Count == 0)
            {
                MessageBox.Show(
                    "Selecione ao menos um monitoramento para exportar.",
                    "Nenhum monitoramento selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var package = new MonitorExportPackage
            {
                SchemaVersion = 1,
                ExportedAtUtc = DateTime.UtcNow,
                Monitors = selectedMonitors
                    .Select(monitor => ToDefinition(monitor, false, false, false))
                    .ToList(),
            };

            using (
                var dialog = new SaveFileDialog
                {
                    Filter = "Field Change Monitor (*.fcm.json)|*.fcm.json|JSON (*.json)|*.json",
                    FileName =
                        selectedMonitors.Count == 1
                            ? SanitizeFileName(selectedMonitors[0].DisplayName) + ".fcm.json"
                            : "monitoramentos.fcm.json",
                    AddExtension = true,
                    DefaultExt = "fcm.json",
                }
            )
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                File.WriteAllText(
                    dialog.FileName,
                    JsonConvert.SerializeObject(
                        package,
                        Formatting.Indented,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
                    ),
                    Encoding.UTF8
                );
                SetStatus($"{selectedMonitors.Count} monitoramento(s) exportado(s).");
                MessageBox.Show(
                    "Exportacao concluida.",
                    "Monitoramentos exportados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private static string SanitizeFileName(string value)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return new string(
                (value ?? "monitoramento")
                    .Select(character => invalidChars.Contains(character) ? '_' : character)
                    .ToArray()
            );
        }

        private void ImportMonitors()
        {
            if (Service == null)
            {
                MessageBox.Show(
                    "Conecte-se a um ambiente antes de importar.",
                    "Conexao obrigatoria",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            MonitorExportPackage package;
            using (
                var dialog = new OpenFileDialog
                {
                    Filter = "Field Change Monitor (*.fcm.json)|*.fcm.json|JSON (*.json)|*.json",
                    Multiselect = false,
                }
            )
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    package = JsonConvert.DeserializeObject<MonitorExportPackage>(
                        File.ReadAllText(dialog.FileName, Encoding.UTF8)
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Nao foi possivel ler o arquivo. " + ex.Message,
                        "Importacao invalida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
            }

            if (
                package == null
                || package.SchemaVersion != 1
                || package.Monitors == null
                || package.Monitors.Count == 0
            )
            {
                MessageBox.Show(
                    "O arquivo nao possui um pacote compativel com a versao 1.",
                    "Importacao invalida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = "Validando monitoramentos importados",
                    Work = (worker, args) =>
                    {
                        var results = new List<ImportValidationResult>();
                        var metadataService = new DataverseMetadataService(Service);

                        foreach (var definition in package.Monitors)
                        {
                            var result = new ImportValidationResult { Definition = definition };
                            try
                            {
                                string normalizedFilter;
                                string filterError;
                                if (
                                    !TryNormalizeFilterXml(
                                        definition.FilterXml,
                                        out normalizedFilter,
                                        out filterError
                                    )
                                )
                                {
                                    throw new InvalidOperationException(filterError);
                                }

                                var metadata = metadataService.GetEntity(
                                    definition.EntityLogicalName,
                                    EntityFilters.Attributes,
                                    CancellationToken.None
                                );
                                var readableColumns = new HashSet<string>(
                                    GetReadableAttributes(metadata)
                                        .Select(attribute => attribute.LogicalName),
                                    StringComparer.OrdinalIgnoreCase
                                );
                                var missingColumns = (
                                    definition.MonitoredColumns ?? new List<string>()
                                )
                                    .Where(column => !readableColumns.Contains(column))
                                    .ToList();
                                if (missingColumns.Count > 0)
                                {
                                    throw new InvalidOperationException(
                                        "Campos inexistentes ou sem leitura: "
                                            + string.Join(", ", missingColumns)
                                    );
                                }
                                if (
                                    definition.MonitoredColumns == null
                                    || definition.MonitoredColumns.Count == 0
                                )
                                {
                                    throw new InvalidOperationException(
                                        "Nenhum campo monitorado foi informado."
                                    );
                                }

                                result.Metadata = metadata;
                                result.NormalizedFilter = normalizedFilter;
                            }
                            catch (Exception ex)
                            {
                                result.Error = ex.Message;
                            }

                            results.Add(result);
                        }

                        args.Result = results;
                    },
                    PostWorkCallBack = args =>
                    {
                        if (args.Error != null)
                        {
                            MessageBox.Show(
                                args.Error.Message,
                                "Erro na importacao",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }

                        var results = (List<ImportValidationResult>)args.Result;
                        var importedCount = 0;
                        foreach (
                            var result in results.Where(result =>
                                string.IsNullOrWhiteSpace(result.Error)
                            )
                        )
                        {
                            var definition = result.Definition;
                            AddConfiguredMonitor(
                                new MonitoringConfiguration
                                {
                                    MonitorName = EnsureUniqueMonitorName(definition.Name),
                                    Service = Service,
                                    EntityLogicalName = definition.EntityLogicalName,
                                    PrimaryIdAttribute = result.Metadata.PrimaryIdAttribute,
                                    PrimaryNameAttribute = result.Metadata.PrimaryNameAttribute,
                                    MonitoredColumns = definition
                                        .MonitoredColumns.Distinct(StringComparer.OrdinalIgnoreCase)
                                        .ToList(),
                                    IntervalSeconds = Math.Max(1, definition.IntervalSeconds),
                                    FilterXml = result.NormalizedFilter,
                                    FetchXml = BuildFetchXml(
                                        definition.EntityLogicalName,
                                        result.Metadata.PrimaryIdAttribute,
                                        result.Metadata.PrimaryNameAttribute,
                                        definition.MonitoredColumns,
                                        result.NormalizedFilter
                                    ),
                                },
                                "Importado",
                                definition.IsPaused
                            );
                            importedCount++;
                        }

                        PersistMonitorConfigurations();
                        SetMonitoringControls(false);
                        var errors = results
                            .Where(result => !string.IsNullOrWhiteSpace(result.Error))
                            .Select(result =>
                                $"{result.Definition?.Name ?? "(sem nome)"}: {result.Error}"
                            )
                            .ToList();
                        var importedActiveCount = results.Count(result =>
                            string.IsNullOrWhiteSpace(result.Error)
                            && result.Definition != null
                            && !result.Definition.IsPaused
                        );
                        var importedPausedCount = importedCount - importedActiveCount;
                        var message =
                            $"{importedCount} monitoramento(s) importado(s): "
                            + $"{importedActiveCount} ativo(s) e {importedPausedCount} pausado(s).";
                        if (errors.Count > 0)
                        {
                            message +=
                                Environment.NewLine
                                + Environment.NewLine
                                + "Nao importados:"
                                + Environment.NewLine
                                + string.Join(Environment.NewLine, errors);
                        }
                        MessageBox.Show(
                            message,
                            "Importacao concluida",
                            MessageBoxButtons.OK,
                            errors.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning
                        );
                    },
                }
            );
        }

        private void StopMonitoring(bool addLog)
        {
            List<ActiveMonitor> monitorsToStop;

            lock (monitorsLock)
            {
                monitorsToStop = activeMonitors.ToList();
                activeMonitors.Clear();
            }

            foreach (var monitor in monitorsToStop)
            {
                monitor.CancellationTokenSource.Cancel();
            }

            lvActiveMonitors.Items.Clear();
            notifyIcon.Visible = false;
            SetMonitoringControls(false);
            SetStatus("Monitoramentos parados.");

            if (addLog && monitorsToStop.Count > 0)
            {
                AddLog($"{monitorsToStop.Count} monitoramento(s) parado(s).");
            }
        }

        private void StopSelectedMonitor()
        {
            RemoveSelectedMonitors();
        }

        private void RemoveSelectedMonitors()
        {
            if (lvActiveMonitors.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Selecione ao menos um monitoramento.",
                    "Nenhum monitoramento selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var monitorsToStop = lvActiveMonitors
                .SelectedItems.Cast<ListViewItem>()
                .Select(item => item.Tag as ActiveMonitor)
                .Where(monitor => monitor != null)
                .ToList();

            RemoveMonitors(monitorsToStop);
        }

        private void RemoveMonitors(IEnumerable<ActiveMonitor> monitors)
        {
            var monitorsToStop = (monitors ?? Enumerable.Empty<ActiveMonitor>())
                .Where(monitor => monitor != null)
                .Distinct()
                .ToList();
            if (monitorsToStop.Count == 0)
            {
                return;
            }

            var message =
                monitorsToStop.Count == 1
                    ? $"Deseja remover o monitoramento '{monitorsToStop[0].DisplayName}'?"
                    : $"Deseja remover os {monitorsToStop.Count} monitoramentos selecionados?";
            if (
                MessageBox.Show(
                    message,
                    "Remover monitoramento",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                ) != DialogResult.Yes
            )
            {
                return;
            }

            foreach (var monitor in monitorsToStop)
            {
                monitor.CancellationTokenSource.Cancel();

                lock (monitorsLock)
                {
                    activeMonitors.Remove(monitor);
                }

                if (monitor.ListViewItem != null)
                {
                    lvActiveMonitors.Items.Remove(monitor.ListViewItem);
                }

                AddLog($"Monitoramento parado: {monitor.DisplayName}.");
            }

            SetMonitoringControls(false);
            PersistMonitorConfigurations();
            if (!HasActiveMonitors())
            {
                notifyIcon.Visible = false;
                SetStatus("Monitoramentos parados.");
            }
        }

        private bool HasActiveMonitors()
        {
            lock (monitorsLock)
            {
                return activeMonitors.Count > 0;
            }
        }

        private void AddActiveMonitorListItem(ActiveMonitor monitor)
        {
            var item = new ListViewItem(monitor.DisplayName);
            item.SubItems.Add(monitor.Configuration.EntityLogicalName);
            item.SubItems.Add(string.Join(", ", monitor.Configuration.MonitoredColumns));
            item.SubItems.Add(
                monitor.Configuration.IntervalSeconds.ToString(CultureInfo.InvariantCulture)
            );
            item.SubItems.Add(monitor.Status);
            item.SubItems.Add(
                string.IsNullOrWhiteSpace(monitor.Configuration.FilterXml)
                    ? "(sem filtro)"
                    : monitor.Configuration.FilterXml
            );
            item.Tag = monitor;
            monitor.ListViewItem = item;
            lvActiveMonitors.Items.Add(item);
        }

        private void UpdateActiveMonitorStatus(ActiveMonitor monitor, string status)
        {
            monitor.Status = status;

            if (monitor.ListViewItem != null && monitor.ListViewItem.ListView != null)
            {
                monitor.ListViewItem.SubItems[4].Text = status;
            }
        }

        private void PopulateColumns(EntityMetadata entityMetadata)
        {
            allColumnItems.Clear();
            checkedMonitoredColumns.Clear();
            allColumnItems.AddRange(
                GetReadableAttributes(entityMetadata)
                    .Select(attribute => new AttributeListItem(attribute))
            );
            txtColumnSearch.Clear();
            ApplyColumnFilter();
        }

        private void PopulateConditionAttributes(EntityMetadata entityMetadata)
        {
            allConditionAttributeItems.Clear();
            allConditionAttributeItems.AddRange(
                GetReadableAttributes(entityMetadata)
                    .Select(attribute => new AttributeListItem(attribute))
            );
            txtConditionFieldSearch.Clear();
            ApplyConditionFieldFilter();
            UpdateConditionValueHint();
        }

        private static List<AttributeMetadata> GetReadableAttributes(EntityMetadata entityMetadata)
        {
            return entityMetadata
                .Attributes.Where(attribute =>
                    attribute.IsValidForRead == true
                    && !string.IsNullOrWhiteSpace(attribute.LogicalName)
                )
                .OrderBy(GetAttributeDisplayName)
                .ThenBy(attribute => attribute.LogicalName)
                .ToList();
        }

        private void SetAllColumnsChecked(bool isChecked)
        {
            if (!isChecked)
            {
                checkedMonitoredColumns.Clear();
            }

            isRefreshingColumnList = true;

            for (var index = 0; index < clbColumns.Items.Count; index++)
            {
                var item = clbColumns.Items[index] as AttributeListItem;
                if (item != null && isChecked)
                {
                    checkedMonitoredColumns.Add(item.LogicalName);
                }

                clbColumns.SetItemChecked(index, isChecked);
            }

            isRefreshingColumnList = false;
            BeginInvoke(new Action(UpdateConfigurationSummary));
        }

        private void ApplyColumnFilter()
        {
            var searchText = txtColumnSearch == null ? string.Empty : txtColumnSearch.Text.Trim();
            var filteredItems = allColumnItems
                .Where(item => MatchesSearch(item, searchText))
                .ToList();

            isRefreshingColumnList = true;
            clbColumns.BeginUpdate();
            clbColumns.Items.Clear();

            foreach (var item in filteredItems)
            {
                clbColumns.Items.Add(item, checkedMonitoredColumns.Contains(item.LogicalName));
            }

            clbColumns.EndUpdate();
            isRefreshingColumnList = false;
        }

        private void ApplyConditionFieldFilter()
        {
            var selectedLogicalName = (
                cboConditionAttribute.SelectedItem as AttributeListItem
            )?.LogicalName;
            var searchText =
                txtConditionFieldSearch == null
                    ? string.Empty
                    : txtConditionFieldSearch.Text.Trim();
            var filteredItems = allConditionAttributeItems
                .Where(item => MatchesSearch(item, searchText))
                .ToList();

            cboConditionAttribute.BeginUpdate();
            cboConditionAttribute.Items.Clear();

            foreach (var item in filteredItems)
            {
                cboConditionAttribute.Items.Add(item);
            }

            cboConditionAttribute.EndUpdate();

            if (cboConditionAttribute.Items.Count == 0)
            {
                UpdateConditionValueHint();
                return;
            }

            var selectedIndex = 0;
            if (!string.IsNullOrWhiteSpace(selectedLogicalName))
            {
                for (var index = 0; index < cboConditionAttribute.Items.Count; index++)
                {
                    var item = cboConditionAttribute.Items[index] as AttributeListItem;
                    if (
                        item != null
                        && string.Equals(
                            item.LogicalName,
                            selectedLogicalName,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        selectedIndex = index;
                        break;
                    }
                }
            }

            cboConditionAttribute.SelectedIndex = selectedIndex;
        }

        private static bool MatchesSearch(AttributeListItem item, string searchText)
        {
            return item != null
                && (
                    string.IsNullOrWhiteSpace(searchText)
                    || item.LogicalName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || item.DisplayName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                );
        }

        private void PopulateConditionOperators()
        {
            cboFilterType.Items.Clear();
            cboFilterType.Items.Add("and");
            cboFilterType.Items.Add("or");
            cboFilterType.SelectedIndex = 0;

            cboConditionOperator.Items.Clear();
            cboConditionOperator.Items.Add(new ConditionOperatorItem("eq", "Igual a", true, false));
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("ne", "Diferente de", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("like", "Contem texto (like)", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("not-like", "Nao contem texto", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("null", "Sem valor", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("not-null", "Com valor", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("gt", "Maior que", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("ge", "Maior ou igual", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("lt", "Menor que", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("le", "Menor ou igual", true, false)
            );
            cboConditionOperator.Items.Add(new ConditionOperatorItem("on", "Na data", true, false));
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("on-or-after", "Na data ou depois", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("on-or-before", "Na data ou antes", true, false)
            );
            cboConditionOperator.Items.Add(new ConditionOperatorItem("in", "Esta em", true, true));
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("not-in", "Nao esta em", true, true)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("between", "Entre", true, true)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("not-between", "Fora do intervalo", true, true)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("last-x-days", "Ultimos X dias", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("next-x-days", "Proximos X dias", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("olderthan-x-days", "Mais antigo que X dias", true, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("today", "Hoje", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("yesterday", "Ontem", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("tomorrow", "Amanha", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("this-week", "Esta semana", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("last-week", "Semana passada", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("next-week", "Proxima semana", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("this-month", "Este mes", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("last-month", "Mes passado", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("next-month", "Proximo mes", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("this-year", "Este ano", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("last-year", "Ano passado", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("next-year", "Proximo ano", false, false)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("contain-values", "Contem valores", true, true)
            );
            cboConditionOperator.Items.Add(
                new ConditionOperatorItem("not-contain-values", "Nao contem valores", true, true)
            );
            cboConditionOperator.SelectedIndex = 0;

            UpdateConditionValueState();
        }

        private void AddConditionFromBuilder()
        {
            var attribute = cboConditionAttribute.SelectedItem as AttributeListItem;
            var conditionOperator = cboConditionOperator.SelectedItem as ConditionOperatorItem;

            if (attribute == null)
            {
                MessageBox.Show(
                    "Carregue a entidade e selecione um campo para a condicao.",
                    "Campo obrigatorio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (conditionOperator == null)
            {
                MessageBox.Show(
                    "Selecione um operador para a condicao.",
                    "Operador obrigatorio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var values = new List<string>();
            if (conditionOperator.RequiresValue)
            {
                values = conditionOperator.AllowsMultipleValues
                    ? SplitConditionValues(txtConditionValue.Text)
                    : new List<string> { txtConditionValue.Text.Trim() };

                values = values
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Select(value => NormalizeConditionBuilderValue(attribute.Metadata, value))
                    .ToList();

                if (values.Count == 0)
                {
                    MessageBox.Show(
                        "Informe o valor da condicao.",
                        "Valor obrigatorio",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (
                    (
                        conditionOperator.Operator == "between"
                        || conditionOperator.Operator == "not-between"
                    )
                    && values.Count != 2
                )
                {
                    MessageBox.Show(
                        "O operador selecionado precisa de exatamente dois valores.",
                        "Valores invalidos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }

            filterConditions.Add(
                new FilterCondition
                {
                    AttributeLogicalName = attribute.LogicalName,
                    AttributeDisplayName = attribute.DisplayName,
                    Operator = conditionOperator.Operator,
                    OperatorDisplayName = conditionOperator.DisplayName,
                    Values = values,
                }
            );

            RefreshConditionList();
            SyncFilterXmlFromConditions();
            txtConditionValue.Clear();
            SetStatus("Condicao adicionada ao filtro.");
            UpdateConfigurationSummary();
        }

        private static List<string> SplitConditionValues(string valueText)
        {
            return valueText
                .Split(new[] { "\r\n", "\n", ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .Where(value => value.Length > 0)
                .ToList();
        }

        private static string NormalizeConditionBuilderValue(
            AttributeMetadata attribute,
            string value
        )
        {
            if (attribute == null || value == null)
            {
                return value;
            }

            var trimmedValue = value.Trim();

            if (attribute.AttributeType == AttributeTypeCode.Boolean)
            {
                if (IsTruthy(trimmedValue))
                {
                    return "1";
                }

                if (IsFalsy(trimmedValue))
                {
                    return "0";
                }
            }

            var optionValue = TryResolveOptionValue(attribute, trimmedValue);
            return optionValue ?? trimmedValue;
        }

        private static bool IsTruthy(string value)
        {
            return string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "sim", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "verdadeiro", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsFalsy(string value)
        {
            return string.Equals(value, "0", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "nao", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "n\u00e3o", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "no", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "falso", StringComparison.OrdinalIgnoreCase);
        }

        private static string TryResolveOptionValue(AttributeMetadata attribute, string value)
        {
            var booleanAttribute = attribute as BooleanAttributeMetadata;
            if (booleanAttribute != null && booleanAttribute.OptionSet != null)
            {
                var trueLabel = GetOptionLabel(booleanAttribute.OptionSet.TrueOption);
                if (
                    !string.IsNullOrWhiteSpace(trueLabel)
                    && string.Equals(trueLabel, value, StringComparison.CurrentCultureIgnoreCase)
                )
                {
                    return "1";
                }

                var falseLabel = GetOptionLabel(booleanAttribute.OptionSet.FalseOption);
                if (
                    !string.IsNullOrWhiteSpace(falseLabel)
                    && string.Equals(falseLabel, value, StringComparison.CurrentCultureIgnoreCase)
                )
                {
                    return "0";
                }
            }

            var enumAttribute = attribute as EnumAttributeMetadata;
            if (enumAttribute == null || enumAttribute.OptionSet == null)
            {
                return null;
            }

            foreach (var option in enumAttribute.OptionSet.Options)
            {
                var optionLabel = GetOptionLabel(option);
                if (
                    !string.IsNullOrWhiteSpace(optionLabel)
                    && string.Equals(optionLabel, value, StringComparison.CurrentCultureIgnoreCase)
                    && option.Value.HasValue
                )
                {
                    return option.Value.Value.ToString(CultureInfo.InvariantCulture);
                }
            }

            return null;
        }

        private static string GetOptionLabel(OptionMetadata option)
        {
            if (option == null || option.Label == null)
            {
                return null;
            }

            if (option.Label.UserLocalizedLabel != null)
            {
                return option.Label.UserLocalizedLabel.Label;
            }

            return option.Label.LocalizedLabels.Count > 0
                ? option.Label.LocalizedLabels[0].Label
                : null;
        }

        private void RemoveSelectedConditions()
        {
            if (lvConditions.SelectedItems.Count == 0)
            {
                return;
            }

            var conditionsToRemove = lvConditions
                .SelectedItems.Cast<ListViewItem>()
                .Select(item => item.Tag as FilterCondition)
                .Where(condition => condition != null)
                .ToList();

            foreach (var condition in conditionsToRemove)
            {
                filterConditions.Remove(condition);
            }

            RefreshConditionList();
            SyncFilterXmlFromConditions();
            SetStatus("Condicao removida do filtro.");
            UpdateConfigurationSummary();
        }

        private void RefreshConditionList()
        {
            lvConditions.BeginUpdate();
            lvConditions.Items.Clear();

            foreach (var condition in filterConditions)
            {
                var item = new ListViewItem(condition.AttributeDisplayName);
                item.SubItems.Add(condition.OperatorDisplayName);
                item.SubItems.Add(string.Join(", ", condition.Values));
                item.Tag = condition;
                lvConditions.Items.Add(item);
            }

            lvConditions.EndUpdate();
        }

        private void SyncFilterXmlFromConditions()
        {
            if (txtFilterXml == null)
            {
                return;
            }

            if (filterConditions.Count == 0)
            {
                txtFilterXml.Clear();
                return;
            }

            txtFilterXml.Text = BuildFilterElementFromConditions().ToString();
        }

        private XElement BuildFilterElementFromConditions()
        {
            var filterType =
                cboFilterType.SelectedItem == null ? "and" : cboFilterType.SelectedItem.ToString();
            var filter = new XElement("filter", new XAttribute("type", filterType));

            foreach (var condition in filterConditions)
            {
                filter.Add(BuildConditionElement(condition));
            }

            return filter;
        }

        private static XElement BuildConditionElement(FilterCondition condition)
        {
            var element = new XElement(
                "condition",
                new XAttribute("attribute", condition.AttributeLogicalName),
                new XAttribute("operator", condition.Operator)
            );

            if (condition.Values == null || condition.Values.Count == 0)
            {
                return element;
            }

            if (
                condition.Values.Count == 1
                && condition.Operator != "in"
                && condition.Operator != "not-in"
                && condition.Operator != "contain-values"
                && condition.Operator != "not-contain-values"
            )
            {
                element.SetAttributeValue("value", condition.Values[0]);
                return element;
            }

            foreach (var value in condition.Values)
            {
                element.Add(new XElement("value", value));
            }

            return element;
        }

        private void UpdateConditionValueState()
        {
            var conditionOperator = cboConditionOperator.SelectedItem as ConditionOperatorItem;
            var requiresValue = conditionOperator == null || conditionOperator.RequiresValue;

            txtConditionValue.Enabled = requiresValue;
            if (!requiresValue)
            {
                txtConditionValue.Clear();
            }

            UpdateConditionValueHint();
        }

        private void PickConditionValue()
        {
            var attribute = cboConditionAttribute.SelectedItem as AttributeListItem;
            var conditionOperator = cboConditionOperator.SelectedItem as ConditionOperatorItem;

            if (attribute == null || conditionOperator == null || !conditionOperator.RequiresValue)
            {
                return;
            }

            var lookupAttribute = attribute.Metadata as LookupAttributeMetadata;
            if (lookupAttribute != null)
            {
                using (var picker = new LookupValuePickerForm(Service, lookupAttribute))
                {
                    if (
                        picker.ShowDialog(this) == DialogResult.OK
                        && !string.IsNullOrWhiteSpace(picker.SelectedValue)
                    )
                    {
                        txtConditionValue.Text = picker.SelectedValue;
                    }
                }

                return;
            }

            if (IsOptionSetAttribute(attribute.Metadata))
            {
                using (
                    var picker = new OptionSetValuePickerForm(
                        attribute.Metadata,
                        conditionOperator.AllowsMultipleValues
                    )
                )
                {
                    if (
                        picker.ShowDialog(this) == DialogResult.OK
                        && picker.SelectedValues.Count > 0
                    )
                    {
                        txtConditionValue.Text = string.Join(", ", picker.SelectedValues);
                    }
                }
            }
        }

        private void UpdateConditionValueHint()
        {
            if (lblConditionValueHint == null)
            {
                return;
            }

            var attribute = cboConditionAttribute.SelectedItem as AttributeListItem;
            var conditionOperator = cboConditionOperator.SelectedItem as ConditionOperatorItem;
            var canPickValue = false;

            if (conditionOperator != null && !conditionOperator.RequiresValue)
            {
                lblConditionValueHint.Text = "Este operador nao precisa de valor.";
                btnPickConditionValue.Enabled = false;
                return;
            }

            var typeHint =
                attribute == null
                    ? "Selecione um campo carregado da entidade."
                    : GetValueHint(attribute.Metadata);
            if (attribute != null)
            {
                canPickValue =
                    attribute.Metadata is LookupAttributeMetadata
                    || IsOptionSetAttribute(attribute.Metadata);
            }

            if (conditionOperator != null && conditionOperator.AllowsMultipleValues)
            {
                typeHint += " Para varios valores, separe por virgula, ponto e virgula ou linha.";
            }

            lblConditionValueHint.Text = typeHint;
            btnPickConditionValue.Enabled = canPickValue;
        }

        private static bool IsOptionSetAttribute(AttributeMetadata attribute)
        {
            return attribute is EnumAttributeMetadata || attribute is BooleanAttributeMetadata;
        }

        private static string GetValueHint(AttributeMetadata attribute)
        {
            if (attribute == null || !attribute.AttributeType.HasValue)
            {
                return "Informe o valor no formato aceito pelo FetchXML.";
            }

            switch (attribute.AttributeType.Value)
            {
                case AttributeTypeCode.Boolean:
                    return "Use true/false, sim/nao, 1/0 ou o rotulo da opcao.";
                case AttributeTypeCode.DateTime:
                    return "Use datas como yyyy-MM-dd ou yyyy-MM-ddTHH:mm:ss.";
                case AttributeTypeCode.Decimal:
                case AttributeTypeCode.Double:
                case AttributeTypeCode.Integer:
                case AttributeTypeCode.BigInt:
                case AttributeTypeCode.Money:
                    return "Use valor numerico sem mascara.";
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Owner:
                    return "Use o GUID do registro relacionado.";
                case AttributeTypeCode.Picklist:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                    return "Use o valor numerico da opcao ou o rotulo.";
                case AttributeTypeCode.Memo:
                case AttributeTypeCode.String:
                    return "Use texto. Para like, use % como curinga.";
                default:
                    return "Informe o valor no formato aceito pelo FetchXML.";
            }
        }

        private static bool TryNormalizeFilterXml(
            string filterText,
            out string normalizedFilter,
            out string error
        )
        {
            normalizedFilter = string.Empty;
            error = null;

            if (string.IsNullOrWhiteSpace(filterText))
            {
                return true;
            }

            var trimmedFilter = filterText.Trim();

            try
            {
                var element = XElement.Parse(trimmedFilter);
                if (
                    string.Equals(
                        element.Name.LocalName,
                        "filter",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    normalizedFilter = element.ToString(SaveOptions.DisableFormatting);
                    return true;
                }

                if (
                    string.Equals(
                        element.Name.LocalName,
                        "condition",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    normalizedFilter = new XElement(
                        "filter",
                        new XAttribute("type", "and"),
                        element
                    ).ToString(SaveOptions.DisableFormatting);
                    return true;
                }
            }
            catch
            {
                // Try again below by wrapping multiple condition nodes in a filter.
            }

            try
            {
                var wrapper = XElement.Parse("<filter type=\"and\">" + trimmedFilter + "</filter>");
                normalizedFilter = wrapper.ToString(SaveOptions.DisableFormatting);
                return true;
            }
            catch (Exception ex)
            {
                error =
                    "Informe um filtro FetchXML valido. Use <filter>...</filter> ou uma ou mais tags <condition ... />. Detalhe: "
                    + ex.Message;
                return false;
            }
        }

        private static string BuildFetchXml(
            string entityLogicalName,
            string primaryIdAttribute,
            string primaryNameAttribute,
            List<string> monitoredColumns,
            string filterXml
        )
        {
            var attributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            attributes.Add(primaryIdAttribute);
            attributes.Add("modifiedon");
            attributes.Add("modifiedby");

            if (!string.IsNullOrWhiteSpace(primaryNameAttribute))
            {
                attributes.Add(primaryNameAttribute);
            }

            foreach (var column in monitoredColumns)
            {
                attributes.Add(column);
            }

            var builder = new StringBuilder();
            builder.Append(
                "<fetch version=\"1.0\" mapping=\"logical\" no-lock=\"true\" count=\"5000\">"
            );
            builder.AppendFormat(
                CultureInfo.InvariantCulture,
                "<entity name=\"{0}\">",
                EscapeXml(entityLogicalName)
            );

            foreach (var attribute in attributes)
            {
                builder.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "<attribute name=\"{0}\" />",
                    EscapeXml(attribute)
                );
            }

            if (!string.IsNullOrWhiteSpace(filterXml))
            {
                builder.Append(filterXml);
            }

            builder.Append("</entity>");
            builder.Append("</fetch>");

            return builder.ToString();
        }

        private static string ApplyPaging(string fetchXml, int pageNumber, string pagingCookie)
        {
            var document = XElement.Parse(fetchXml);
            document.SetAttributeValue("page", pageNumber.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(pagingCookie))
            {
                document.SetAttributeValue("paging-cookie", pagingCookie);
            }
            else
            {
                document.Attribute("paging-cookie")?.Remove();
            }

            return document.ToString(SaveOptions.DisableFormatting);
        }

        private static string EscapeXml(string value)
        {
            return SecurityElement.Escape(value);
        }

        private static string GetAttributeDisplayName(AttributeMetadata attribute)
        {
            return MetadataLabelResolver.GetDisplayName(attribute);
        }

        private static string GetEntityDisplayName(EntityMetadata entity)
        {
            return MetadataLabelResolver.GetDisplayName(entity);
        }

        private static string GetRecordName(Entity entity, string primaryNameAttribute)
        {
            if (
                !string.IsNullOrWhiteSpace(primaryNameAttribute)
                && entity.Contains(primaryNameAttribute)
            )
            {
                var value = FormatValue(
                    entity[primaryNameAttribute],
                    entity.FormattedValues.Contains(primaryNameAttribute)
                        ? entity.FormattedValues[primaryNameAttribute]
                        : null
                );
                if (!string.IsNullOrWhiteSpace(value) && value != "(vazio)")
                {
                    return value;
                }
            }

            return entity.Id.ToString();
        }

        private static string NormalizeValue(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var aliasedValue = value as AliasedValue;
            if (aliasedValue != null)
            {
                return NormalizeValue(aliasedValue.Value);
            }

            var optionSetValue = value as OptionSetValue;
            if (optionSetValue != null)
            {
                return optionSetValue.Value.ToString(CultureInfo.InvariantCulture);
            }

            var optionSetValueCollection = value as OptionSetValueCollection;
            if (optionSetValueCollection != null)
            {
                return string.Join(
                    ",",
                    optionSetValueCollection
                        .Select(item => item.Value.ToString(CultureInfo.InvariantCulture))
                        .OrderBy(item => item)
                );
            }

            var money = value as Money;
            if (money != null)
            {
                return money.Value.ToString(CultureInfo.InvariantCulture);
            }

            var entityReference = value as EntityReference;
            if (entityReference != null)
            {
                return entityReference.LogicalName + ":" + entityReference.Id.ToString("D");
            }

            if (value is DateTime)
            {
                return ((DateTime)value)
                    .ToUniversalTime()
                    .ToString("O", CultureInfo.InvariantCulture);
            }

            if (value is bool)
            {
                return ((bool)value).ToString(CultureInfo.InvariantCulture);
            }

            if (value is IFormattable)
            {
                return ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture);
            }

            return value.ToString();
        }

        private static string FormatValue(object value, string formattedValue)
        {
            if (!string.IsNullOrWhiteSpace(formattedValue))
            {
                return formattedValue;
            }

            if (value == null)
            {
                return "(vazio)";
            }

            var aliasedValue = value as AliasedValue;
            if (aliasedValue != null)
            {
                return FormatValue(aliasedValue.Value, formattedValue);
            }

            var entityReference = value as EntityReference;
            if (entityReference != null)
            {
                return string.IsNullOrWhiteSpace(entityReference.Name)
                    ? entityReference.Id.ToString("D")
                    : entityReference.Name;
            }

            var money = value as Money;
            if (money != null)
            {
                return money.Value.ToString("G", CultureInfo.CurrentCulture);
            }

            var optionSetValue = value as OptionSetValue;
            if (optionSetValue != null)
            {
                return optionSetValue.Value.ToString(CultureInfo.CurrentCulture);
            }

            var optionSetValueCollection = value as OptionSetValueCollection;
            if (optionSetValueCollection != null)
            {
                return string.Join(
                    ", ",
                    optionSetValueCollection.Select(item =>
                        item.Value.ToString(CultureInfo.CurrentCulture)
                    )
                );
            }

            if (value is DateTime)
            {
                return ((DateTime)value).ToLocalTime().ToString("G", CultureInfo.CurrentCulture);
            }

            if (value is bool)
            {
                return (bool)value ? "Sim" : "Nao";
            }

            if (value is IFormattable)
            {
                return ((IFormattable)value).ToString(null, CultureInfo.CurrentCulture);
            }

            return value.ToString();
        }

        private void RunOnUiThread(Action action)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(action);
                return;
            }

            action();
        }

        private void AddLog(string message)
        {
            if (InvokeRequired)
            {
                RunOnUiThread(() => AddLog(message));
                return;
            }

            lstEvents.Items.Insert(0, $"{DateTime.Now:HH:mm:ss} - {message}");

            while (lstEvents.Items.Count > 300)
            {
                lstEvents.Items.RemoveAt(lstEvents.Items.Count - 1);
            }
        }

        private void SetStatus(string status)
        {
            tsslStatus.Text = $"Status: {status}";
        }

        private void SaveFilterXmlToBuilder()
        {
            string normalizedFilter;
            string error;
            var xmlToValidate = txtFilterXml.Text;
            try
            {
                if (
                    !string.IsNullOrWhiteSpace(xmlToValidate)
                    && xmlToValidate
                        .TrimStart()
                        .StartsWith("<fetch", StringComparison.OrdinalIgnoreCase)
                )
                {
                    var document = XElement.Parse(xmlToValidate);
                    if (
                        string.Equals(
                            document.Name.LocalName,
                            "fetch",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        var entity = document
                            .Elements()
                            .FirstOrDefault(element =>
                                string.Equals(
                                    element.Name.LocalName,
                                    "entity",
                                    StringComparison.OrdinalIgnoreCase
                                )
                            );
                        if (entity == null)
                        {
                            throw new InvalidOperationException(
                                "O FetchXML nao possui uma tag entity."
                            );
                        }

                        var filters = entity
                            .Elements()
                            .Where(element =>
                                string.Equals(
                                    element.Name.LocalName,
                                    "filter",
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            .ToList();
                        if (filters.Count > 1)
                        {
                            throw new InvalidOperationException(
                                "O construtor visual aceita apenas um filtro principal."
                            );
                        }

                        xmlToValidate = filters.Count == 0 ? string.Empty : filters[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "O FetchXML nao e valido. " + ex.Message,
                    "FetchXML invalido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (!TryNormalizeFilterXml(xmlToValidate, out normalizedFilter, out error))
            {
                MessageBox.Show(
                    error,
                    "FetchXML invalido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(normalizedFilter))
            {
                filterConditions.Clear();
                RefreshConditionList();
                UpdateConfigurationSummary();
                SetStatus("Filtro vazio salvo.");
                return;
            }

            try
            {
                var filter = XElement.Parse(normalizedFilter);
                if (filter.Elements("filter").Any() || filter.Descendants("filter").Any())
                {
                    throw new InvalidOperationException(
                        "Filtros aninhados nao podem ser editados pelo construtor visual."
                    );
                }

                var parsedConditions = new List<FilterCondition>();
                foreach (var conditionElement in filter.Elements("condition"))
                {
                    var unsupportedAttributes = conditionElement
                        .Attributes()
                        .Where(attribute =>
                            attribute.Name.LocalName != "attribute"
                            && attribute.Name.LocalName != "operator"
                            && attribute.Name.LocalName != "value"
                        )
                        .Select(attribute => attribute.Name.LocalName)
                        .ToList();
                    if (unsupportedAttributes.Count > 0)
                    {
                        throw new InvalidOperationException(
                            "A condicao possui propriedades nao suportadas pelo construtor visual: "
                                + string.Join(", ", unsupportedAttributes)
                        );
                    }

                    var attributeLogicalName = (string)conditionElement.Attribute("attribute");
                    var operatorName = (string)conditionElement.Attribute("operator");
                    if (
                        string.IsNullOrWhiteSpace(attributeLogicalName)
                        || string.IsNullOrWhiteSpace(operatorName)
                    )
                    {
                        throw new InvalidOperationException(
                            "Cada condition deve possuir attribute e operator."
                        );
                    }

                    var attributeItem = allConditionAttributeItems.FirstOrDefault(item =>
                        string.Equals(
                            item.LogicalName,
                            attributeLogicalName,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );
                    var operatorItem = cboConditionOperator
                        .Items.Cast<object>()
                        .OfType<ConditionOperatorItem>()
                        .FirstOrDefault(item =>
                            string.Equals(
                                item.Operator,
                                operatorName,
                                StringComparison.OrdinalIgnoreCase
                            )
                        );
                    if (operatorItem == null)
                    {
                        throw new InvalidOperationException(
                            $"O operador '{operatorName}' nao e suportado pelo construtor visual."
                        );
                    }

                    var values = conditionElement
                        .Elements("value")
                        .Select(value => value.Value)
                        .ToList();
                    var valueAttribute = (string)conditionElement.Attribute("value");
                    if (!string.IsNullOrWhiteSpace(valueAttribute))
                    {
                        values.Insert(0, valueAttribute);
                    }

                    parsedConditions.Add(
                        new FilterCondition
                        {
                            AttributeLogicalName = attributeLogicalName,
                            AttributeDisplayName =
                                attributeItem == null
                                    ? attributeLogicalName
                                    : attributeItem.DisplayName,
                            Operator = operatorItem.Operator,
                            OperatorDisplayName = operatorItem.DisplayName,
                            Values = values,
                        }
                    );
                }

                filterConditions.Clear();
                filterConditions.AddRange(parsedConditions);
                var filterType = ((string)filter.Attribute("type") ?? "and").ToLowerInvariant();
                cboFilterType.SelectedItem = filterType == "or" ? "or" : "and";
                txtFilterXml.Text = filter.ToString();
                RefreshConditionList();
                UpdateConfigurationSummary();
                SetStatus("FetchXML validado e carregado no construtor visual.");
                MessageBox.Show(
                    "FetchXML valido. As condicoes foram atualizadas no grid.",
                    "Filtro salvo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "O XML e valido, mas nao pode ser representado pelo construtor visual sem perder informacoes. "
                        + ex.Message,
                    "Filtro nao suportado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void UpdateConfigurationSummary()
        {
            if (lblConfigurationSummary == null || lblSelectedCount == null)
            {
                return;
            }

            var selectedEntityLogicalName = GetSelectedEntityLogicalName();
            var entityName = string.IsNullOrWhiteSpace(selectedEntityLogicalName)
                ? "Selecione uma tabela"
                : selectedEntityLogicalName;
            var monitorName = string.IsNullOrWhiteSpace(txtMonitorName.Text)
                ? "Monitor sem nome"
                : txtMonitorName.Text.Trim();
            var selectedCount = checkedMonitoredColumns.Count;
            var conditionCount = filterConditions.Count;

            lblSelectedCount.Text = $"{selectedCount} selecionado(s)";
            lblConfigurationSummary.Text =
                $"{monitorName}  •  {entityName}  •  {selectedCount} campo(s)  •  a cada {nudIntervalSeconds.Value:0} segundo(s)  •  {conditionCount} condição(ões)";
            lblConfigurationReady.Text =
                editingMonitor != null
                    ? $"Editando: {editingMonitor.DisplayName}"
                    : (
                        selectedCount > 0
                        && !string.IsNullOrWhiteSpace(selectedEntityLogicalName)
                        && !string.IsNullOrWhiteSpace(txtMonitorName.Text)
                            ? "Pronto para monitorar"
                            : "Configure o monitoramento"
                    );
        }

        private void ApplyVisualTheme()
        {
            BackColor = Color.FromArgb(250, 250, 250);
            Font = new Font("Segoe UI", 9F);
            ApplyThemeToChildren(this);

            btnStart.BackColor = Color.FromArgb(8, 127, 140);
            btnStart.ForeColor = Color.White;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Font = new Font("Segoe UI Semibold", 10F);
            lblConfigurationReady.ForeColor = Color.FromArgb(30, 41, 45);
            lblConfigurationSummary.ForeColor = Color.FromArgb(90, 100, 105);
            lblSelectedCount.ForeColor = Color.FromArgb(8, 127, 140);
            btnToggleAdvanced.ForeColor = Color.FromArgb(8, 127, 140);
        }

        private static void ApplyThemeToChildren(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is GroupBox)
                {
                    control.Font = new Font("Segoe UI Semibold", 9.5F);
                    control.BackColor = Color.White;
                }
                else if (
                    control is TextBox
                    || control is ComboBox
                    || control is NumericUpDown
                    || control is CheckedListBox
                    || control is ListView
                    || control is ListBox
                )
                {
                    control.Font = new Font("Segoe UI", 9F);
                    control.BackColor = Color.White;
                }

                ApplyThemeToChildren(control);
            }
        }

        private void TogglePauseSelectedMonitors()
        {
            if (lvActiveMonitors.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Selecione ao menos um monitoramento.",
                    "Nenhum monitoramento selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var selectedMonitors = lvActiveMonitors
                .SelectedItems.Cast<ListViewItem>()
                .Select(item => item.Tag as ActiveMonitor)
                .Where(monitor => monitor != null)
                .ToList();
            var shouldPause = selectedMonitors.Any(monitor => !monitor.IsPaused);

            foreach (var monitor in selectedMonitors)
            {
                monitor.IsPaused = shouldPause;
                if (!shouldPause)
                {
                    monitor.NeedsBaselineReset = false;
                    StartMonitorTask(monitor);
                }
                UpdateActiveMonitorStatus(monitor, shouldPause ? "Pausado" : "Retomando");
                AddLog($"{monitor.DisplayName}: {(shouldPause ? "pausado" : "retomado")}.");
            }

            SetStatus(
                shouldPause ? "Monitoramento(s) pausado(s)." : "Monitoramento(s) retomado(s)."
            );
            SetMonitoringControls(false);
            PersistMonitorConfigurations();
        }

        private void SetMonitoringControls(bool monitoring)
        {
            var hasActiveMonitors = HasActiveMonitors();
            int activeCount;
            int pausedCount;
            lock (monitorsLock)
            {
                activeCount = activeMonitors.Count(monitor => !monitor.IsPaused);
                pausedCount = activeMonitors.Count(monitor => monitor.IsPaused);
            }
            tslActiveMonitors.Text = $"Ativos: {activeCount}  |  Pausados: {pausedCount}";
            txtEntityLogicalName.Enabled = true;
            txtMonitorName.Enabled = true;
            btnSearchEntities.Enabled = Service != null;
            btnLoadColumns.Enabled = true;
            txtColumnSearch.Enabled = true;
            clbColumns.Enabled = true;
            btnSelectAllColumns.Enabled = true;
            btnClearColumnSelection.Enabled = true;
            cboFilterType.Enabled = true;
            txtConditionFieldSearch.Enabled = true;
            cboConditionAttribute.Enabled = true;
            cboConditionOperator.Enabled = true;
            txtConditionValue.Enabled =
                (cboConditionOperator.SelectedItem as ConditionOperatorItem)?.RequiresValue
                != false;
            btnPickConditionValue.Enabled =
                btnPickConditionValue.Enabled && txtConditionValue.Enabled;
            btnAddCondition.Enabled = true;
            btnRemoveCondition.Enabled = true;
            btnClearFilter.Enabled = true;
            lvConditions.Enabled = true;
            txtFilterXml.Enabled = true;
            nudIntervalSeconds.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = hasActiveMonitors;
            btnStopSelectedMonitor.Enabled = hasActiveMonitors;
            btnRemoveSelectedMonitors.Enabled = hasActiveMonitors && editingMonitor == null;
            btnPauseSelectedMonitors.Enabled = hasActiveMonitors && editingMonitor == null;
            btnSelectAllMonitors.Enabled = hasActiveMonitors && editingMonitor == null;
            btnExportMonitors.Enabled = hasActiveMonitors && editingMonitor == null;
            btnImportMonitors.Enabled = Service != null && editingMonitor == null;
            btnEditMonitor.Enabled = hasActiveMonitors && editingMonitor == null;
            lvActiveMonitors.Enabled = editingMonitor == null;
            RefreshModernMonitorGrid();
            RefreshModernRecentGrid();
        }

        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            CancelMonitorEditing(false);
            PersistMonitorConfigurations();
            PersistRecentChanges();
            StopMonitoring(false);
            notifyIcon.Visible = false;

            if (mySettings != null)
            {
                SettingsManager.Instance.Save(GetType(), mySettings);
            }
        }

        public override void UpdateConnection(
            IOrganizationService newService,
            ConnectionDetail detail,
            string actionName,
            object parameter
        )
        {
            CancelMonitorEditing(false);
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (HasActiveMonitors())
            {
                PersistMonitorConfigurations();
                StopMonitoring(true);
                AddLog("Conexao alterada; monitoramento interrompido.");
            }

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }

            currentEnvironmentUrl =
                detail == null
                    ? null
                    : (
                        !string.IsNullOrWhiteSpace(detail.WebApplicationUrl)
                            ? detail.WebApplicationUrl
                            : (
                                !string.IsNullOrWhiteSpace(detail.OriginalUrl)
                                    ? detail.OriginalUrl
                                    : detail.OrganizationDataServiceUrl
                            )
                    );
            currentEnvironmentName =
                detail == null
                    ? "Ambiente Dataverse"
                    : (
                        !string.IsNullOrWhiteSpace(detail.OrganizationFriendlyName)
                            ? detail.OrganizationFriendlyName
                            : (
                                !string.IsNullOrWhiteSpace(detail.ConnectionName)
                                    ? detail.ConnectionName
                                    : "Ambiente Dataverse"
                            )
                    );
            tslConnection.Text = detail == null ? "Aguardando conexao" : "Conectado";
            savedMonitorsRestored = false;
            recentChangesRestored = false;
            lvRecentChanges.Items.Clear();
            if (mySettings == null || mySettings.RestoreMonitorsOnStartup)
            {
                RestoreSavedMonitorsIfPossible();
            }
            SetMonitoringControls(false);
        }

        private sealed class AttributeListItem
        {
            public AttributeListItem(AttributeMetadata metadata)
            {
                Metadata = metadata;
                LogicalName = metadata.LogicalName;
                DisplayName = GetAttributeDisplayName(metadata);
                AttributeType = metadata.AttributeType.HasValue
                    ? metadata.AttributeType.Value.ToString()
                    : "Unknown";
            }

            public AttributeMetadata Metadata { get; private set; }

            public string LogicalName { get; private set; }

            public string DisplayName { get; private set; }

            private string AttributeType { get; set; }

            public override string ToString()
            {
                return string.Equals(DisplayName, LogicalName, StringComparison.OrdinalIgnoreCase)
                    ? $"{LogicalName} ({AttributeType})"
                    : $"{DisplayName} ({LogicalName}, {AttributeType})";
            }
        }

        private sealed class ConditionOperatorItem
        {
            public ConditionOperatorItem(
                string fetchXmlOperator,
                string displayName,
                bool requiresValue,
                bool allowsMultipleValues
            )
            {
                Operator = fetchXmlOperator;
                DisplayName = displayName;
                RequiresValue = requiresValue;
                AllowsMultipleValues = allowsMultipleValues;
            }

            public string Operator { get; private set; }

            public string DisplayName { get; private set; }

            public bool RequiresValue { get; private set; }

            public bool AllowsMultipleValues { get; private set; }

            public override string ToString()
            {
                return $"{DisplayName} ({Operator})";
            }
        }

        private sealed class FilterCondition
        {
            public string AttributeLogicalName { get; set; }

            public string AttributeDisplayName { get; set; }

            public string Operator { get; set; }

            public string OperatorDisplayName { get; set; }

            public List<string> Values { get; set; }
        }

        private sealed class ActiveMonitor
        {
            public Guid Id { get; set; }

            public MonitoringConfiguration Configuration { get; set; }

            public CancellationTokenSource CancellationTokenSource { get; set; }

            public Task Task { get; set; }

            public Dictionary<Guid, RecordSnapshot> PreviousSnapshot { get; set; }

            public int MonitoredRecordCount { get; set; }

            public DateTime? LastQueryOn { get; set; }

            public DateTime CreatedOn { get; set; }

            public string Status { get; set; }

            public volatile bool IsPaused;

            public volatile bool NeedsBaselineReset;

            public ListViewItem ListViewItem { get; set; }

            public string DisplayName
            {
                get { return Configuration.MonitorName; }
            }
        }

        private sealed class MonitoringConfiguration
        {
            public MonitoringConfiguration()
            {
                ModifiedByNames = new Dictionary<Guid, string>();
                ModifiedByNamesLock = new object();
            }

            public string MonitorName { get; set; }

            public IOrganizationService Service { get; set; }

            public string EntityLogicalName { get; set; }

            public string PrimaryIdAttribute { get; set; }

            public string PrimaryNameAttribute { get; set; }

            public List<string> MonitoredColumns { get; set; }

            public int IntervalSeconds { get; set; }

            public string FilterXml { get; set; }

            public string FetchXml { get; set; }

            public Dictionary<Guid, string> ModifiedByNames { get; private set; }

            public object ModifiedByNamesLock { get; private set; }
        }

        private sealed class EntityListItem
        {
            public EntityListItem(EntityMetadata metadata)
            {
                LogicalName = metadata.LogicalName;
                DisplayName = GetEntityDisplayName(metadata);
            }

            public string LogicalName { get; private set; }

            public string DisplayName { get; private set; }

            public override string ToString()
            {
                return string.Equals(DisplayName, LogicalName, StringComparison.OrdinalIgnoreCase)
                    ? LogicalName
                    : $"{DisplayName} ({LogicalName})";
            }
        }

        private sealed class RecordSnapshot
        {
            public Guid RecordId { get; set; }

            public string RecordName { get; set; }

            public DateTime ModifiedOn { get; set; }

            public string ModifiedBy { get; set; }

            public Dictionary<string, FieldValue> Values { get; set; }
        }

        private sealed class FieldValue
        {
            public string NormalizedValue { get; set; }

            public string DisplayValue { get; set; }
        }

        private sealed class FieldChange
        {
            public Guid RecordId { get; set; }

            public string RecordName { get; set; }

            public string EntityLogicalName { get; set; }

            public string MonitorName { get; set; }

            public DateTime ModifiedOn { get; set; }

            public string ModifiedBy { get; set; }

            public string ColumnLogicalName { get; set; }

            public ChangeKind Kind { get; set; }

            public string OldValue { get; set; }

            public string NewValue { get; set; }

            public string EventDescription
            {
                get
                {
                    switch (Kind)
                    {
                        case ChangeKind.EnteredFilter:
                            return "Entrou no filtro";
                        case ChangeKind.ExitedFilter:
                            return "Saiu do filtro";
                        case ChangeKind.RecordUnavailable:
                            return "Registro indisponível";
                        default:
                            return "Campo alterado";
                    }
                }
            }

            public string ChangeDescription
            {
                get
                {
                    if (Kind == ChangeKind.EnteredFilter)
                    {
                        return $"Valor atual: {NewValue}";
                    }

                    if (Kind == ChangeKind.RecordUnavailable)
                    {
                        return $"{OldValue} -> (registro excluído ou inacessível)";
                    }

                    return $"{OldValue} -> {NewValue}";
                }
            }
        }

        private enum ChangeKind
        {
            ValueChanged,
            EnteredFilter,
            ExitedFilter,
            RecordUnavailable,
        }

        private sealed class ImportValidationResult
        {
            public MonitorDefinition Definition { get; set; }

            public EntityMetadata Metadata { get; set; }

            public string NormalizedFilter { get; set; }

            public string Error { get; set; }
        }
    }
}
