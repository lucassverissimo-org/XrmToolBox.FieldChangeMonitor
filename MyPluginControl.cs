using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using XrmToolBox.Extensibility;

namespace XrmTool_bravo
{
    public partial class MyPluginControl : PluginControlBase
    {
        private Settings mySettings;
        private EntityMetadata currentEntityMetadata;
        private string currentEntityLogicalName;
        private readonly List<AttributeListItem> allColumnItems = new List<AttributeListItem>();
        private readonly List<AttributeListItem> allConditionAttributeItems = new List<AttributeListItem>();
        private readonly HashSet<string> checkedMonitoredColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<ActiveMonitor> activeMonitors = new List<ActiveMonitor>();
        private readonly List<FilterCondition> filterConditions = new List<FilterCondition>();
        private readonly object monitorsLock = new object();
        private readonly object serviceLock = new object();
        private bool isRefreshingColumnList;

        public MyPluginControl()
        {
            InitializeComponent();
            PopulateConditionOperators();
            SetMonitoringControls(false);
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
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void btnLoadColumns_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadColumns);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ExecuteMethod(StartMonitoring);
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
        }

        private void btnPickConditionValue_Click(object sender, EventArgs e)
        {
            PickConditionValue();
        }

        private void btnStopSelectedMonitor_Click(object sender, EventArgs e)
        {
            StopSelectedMonitor();
        }

        private void LoadColumns()
        {
            var entityLogicalName = txtEntityLogicalName.Text.Trim();

            if (string.IsNullOrWhiteSpace(entityLogicalName))
            {
                MessageBox.Show("Informe o nome logico da entidade.", "Entidade obrigatoria", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            WorkAsync(new WorkAsyncInfo
            {
                Message = $"Carregando colunas de {entityLogicalName}",
                Work = (worker, args) =>
                {
                    var request = new RetrieveEntityRequest
                    {
                        LogicalName = entityLogicalName,
                        EntityFilters = EntityFilters.Attributes
                    };

                    args.Result = (RetrieveEntityResponse)Service.Execute(request);
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message, "Erro ao carregar colunas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        SetStatus("Falha ao carregar colunas.");
                        return;
                    }

                    var response = args.Result as RetrieveEntityResponse;
                    if (response == null || response.EntityMetadata == null)
                    {
                        MessageBox.Show("Nao foi possivel ler os metadados da entidade.", "Metadados indisponiveis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var entityChanged = !string.Equals(currentEntityLogicalName, entityLogicalName, StringComparison.OrdinalIgnoreCase);
                    currentEntityMetadata = response.EntityMetadata;
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
                    SetStatus($"{clbColumns.Items.Count} colunas disponiveis para {entityLogicalName}.");
                }
            });
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
                Status = "Iniciando"
            };

            lock (monitorsLock)
            {
                activeMonitors.Add(monitor);
            }

            AddActiveMonitorListItem(monitor);
            SetMonitoringControls(false);
            AddLog($"Monitor adicionado para {configuration.EntityLogicalName} a cada {configuration.IntervalSeconds} segundo(s).");
            SetStatus("Monitorando...");
            notifyIcon.Visible = true;

            var token = monitor.CancellationTokenSource.Token;
            monitor.Task = Task.Run(() => MonitorAsync(monitor, token), token);
        }

        private MonitoringConfiguration BuildMonitoringConfiguration()
        {
            var entityLogicalName = txtEntityLogicalName.Text.Trim();
            if (string.IsNullOrWhiteSpace(entityLogicalName))
            {
                MessageBox.Show("Informe o nome logico da entidade.", "Entidade obrigatoria", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (currentEntityMetadata == null || !string.Equals(currentEntityLogicalName, entityLogicalName, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Carregue as colunas da entidade antes de iniciar o monitoramento.", "Colunas nao carregadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            var monitoredColumns = checkedMonitoredColumns
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(column => column)
                .ToList();

            if (monitoredColumns.Count == 0)
            {
                MessageBox.Show("Selecione ao menos uma coluna para monitorar.", "Colunas obrigatorias", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (string.IsNullOrWhiteSpace(currentEntityMetadata.PrimaryIdAttribute))
            {
                MessageBox.Show("A entidade nao possui uma coluna primaria identificavel.", "Entidade invalida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            string normalizedFilter;
            string filterError;
            if (!TryNormalizeFilterXml(txtFilterXml.Text, out normalizedFilter, out filterError))
            {
                MessageBox.Show(filterError, "Filtro invalido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return new MonitoringConfiguration
            {
                Service = Service,
                EntityLogicalName = entityLogicalName,
                PrimaryIdAttribute = currentEntityMetadata.PrimaryIdAttribute,
                PrimaryNameAttribute = currentEntityMetadata.PrimaryNameAttribute,
                MonitoredColumns = monitoredColumns,
                IntervalSeconds = Convert.ToInt32(nudIntervalSeconds.Value),
                FilterXml = normalizedFilter,
                FetchXml = BuildFetchXml(entityLogicalName, currentEntityMetadata.PrimaryIdAttribute, currentEntityMetadata.PrimaryNameAttribute, monitoredColumns, normalizedFilter)
            };
        }

        private async Task MonitorAsync(ActiveMonitor monitor, CancellationToken cancellationToken)
        {
            var isFirstRun = true;
            var configuration = monitor.Configuration;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var currentSnapshot = RetrieveSnapshot(configuration);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (isFirstRun)
                    {
                        monitor.PreviousSnapshot = currentSnapshot;
                        isFirstRun = false;
                        RunOnUiThread(() =>
                        {
                            UpdateActiveMonitorStatus(monitor, $"Ativo ({currentSnapshot.Count})");
                            AddLog($"[{monitor.DisplayName}] Snapshot inicial registrado com {currentSnapshot.Count} registro(s).");
                            SetStatus($"Monitorando {currentSnapshot.Count} registro(s).");
                        });
                    }
                    else
                    {
                        var changes = DetectChanges(monitor.PreviousSnapshot, currentSnapshot, configuration.MonitoredColumns);
                        monitor.PreviousSnapshot = currentSnapshot;

                        RunOnUiThread(() =>
                        {
                            if (changes.Count > 0)
                            {
                                UpdateActiveMonitorStatus(monitor, $"{changes.Count} alteracao(oes)");
                                ReportChanges(monitor, changes);
                                SetStatus($"{changes.Count} alteracao(oes) detectada(s).");
                            }
                            else
                            {
                                UpdateActiveMonitorStatus(monitor, $"Ativo ({currentSnapshot.Count})");
                                SetStatus($"Sem alteracoes. Ultima consulta: {DateTime.Now:HH:mm:ss}");
                            }
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
                    await Task.Delay(TimeSpan.FromSeconds(configuration.IntervalSeconds), cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            RunOnUiThread(() => UpdateActiveMonitorStatus(monitor, "Parado"));
        }

        private Dictionary<Guid, RecordSnapshot> RetrieveSnapshot(MonitoringConfiguration configuration)
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

                    var values = new Dictionary<string, FieldValue>(StringComparer.OrdinalIgnoreCase);
                    foreach (var column in configuration.MonitoredColumns)
                    {
                        var rawValue = entity.Contains(column) ? entity[column] : null;
                        var formattedValue = entity.FormattedValues.Contains(column) ? entity.FormattedValues[column] : null;
                        values[column] = new FieldValue
                        {
                            NormalizedValue = NormalizeValue(rawValue),
                            DisplayValue = FormatValue(rawValue, formattedValue)
                        };
                    }

                    snapshot[recordId] = new RecordSnapshot
                    {
                        RecordId = recordId,
                        RecordName = GetRecordName(entity, configuration.PrimaryNameAttribute),
                        Values = values
                    };
                }

                moreRecords = result.MoreRecords;
                pagingCookie = result.PagingCookie;
                pageNumber++;
            }
            while (moreRecords);

            return snapshot;
        }

        private List<FieldChange> DetectChanges(Dictionary<Guid, RecordSnapshot> oldSnapshot, Dictionary<Guid, RecordSnapshot> currentSnapshot, List<string> monitoredColumns)
        {
            var changes = new List<FieldChange>();

            foreach (var currentRecord in currentSnapshot.Values)
            {
                RecordSnapshot oldRecord;
                if (!oldSnapshot.TryGetValue(currentRecord.RecordId, out oldRecord))
                {
                    continue;
                }

                foreach (var column in monitoredColumns)
                {
                    FieldValue oldValue;
                    FieldValue currentValue;

                    oldRecord.Values.TryGetValue(column, out oldValue);
                    currentRecord.Values.TryGetValue(column, out currentValue);

                    var oldNormalizedValue = oldValue == null ? string.Empty : oldValue.NormalizedValue;
                    var currentNormalizedValue = currentValue == null ? string.Empty : currentValue.NormalizedValue;

                    if (!string.Equals(oldNormalizedValue, currentNormalizedValue, StringComparison.Ordinal))
                    {
                        changes.Add(new FieldChange
                        {
                            RecordId = currentRecord.RecordId,
                            RecordName = currentRecord.RecordName,
                            ColumnLogicalName = column,
                            OldValue = oldValue == null ? "(vazio)" : oldValue.DisplayValue,
                            NewValue = currentValue == null ? "(vazio)" : currentValue.DisplayValue
                        });
                    }
                }
            }

            return changes;
        }

        private void ReportChanges(ActiveMonitor monitor, List<FieldChange> changes)
        {
            foreach (var change in changes.Take(20))
            {
                AddLog($"[{monitor.DisplayName}] {change.RecordName} [{change.RecordId}] - {change.ColumnLogicalName}: {change.OldValue} -> {change.NewValue}");
            }

            if (changes.Count > 20)
            {
                AddLog($"Mais {changes.Count - 20} alteracao(oes) omitida(s) do log.");
            }

            ShowWindowsAlert(monitor, changes);
        }

        private void ShowWindowsAlert(ActiveMonitor monitor, List<FieldChange> changes)
        {
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipTitle = $"Mudanca detectada - {monitor.DisplayName}";
            notifyIcon.BalloonTipText = BuildAlertMessage(changes);
            notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
            notifyIcon.ShowBalloonTip(10000);
            System.Media.SystemSounds.Exclamation.Play();
        }

        private static string BuildAlertMessage(List<FieldChange> changes)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{changes.Count} alteracao(oes) encontrada(s).");

            foreach (var change in changes.Take(4))
            {
                builder.AppendLine($"{change.ColumnLogicalName}: {TrimForAlert(change.OldValue)} -> {TrimForAlert(change.NewValue)}");
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
            if (lvActiveMonitors.SelectedItems.Count == 0)
            {
                return;
            }

            var monitorsToStop = lvActiveMonitors.SelectedItems
                .Cast<ListViewItem>()
                .Select(item => item.Tag as ActiveMonitor)
                .Where(monitor => monitor != null)
                .ToList();

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
            var item = new ListViewItem(monitor.Configuration.EntityLogicalName);
            item.SubItems.Add(string.Join(", ", monitor.Configuration.MonitoredColumns));
            item.SubItems.Add(monitor.Configuration.IntervalSeconds.ToString(CultureInfo.InvariantCulture));
            item.SubItems.Add(monitor.Status);
            item.SubItems.Add(string.IsNullOrWhiteSpace(monitor.Configuration.FilterXml) ? "(sem filtro)" : monitor.Configuration.FilterXml);
            item.Tag = monitor;
            monitor.ListViewItem = item;
            lvActiveMonitors.Items.Add(item);
        }

        private void UpdateActiveMonitorStatus(ActiveMonitor monitor, string status)
        {
            monitor.Status = status;

            if (monitor.ListViewItem != null && monitor.ListViewItem.ListView != null)
            {
                monitor.ListViewItem.SubItems[3].Text = status;
            }
        }

        private void PopulateColumns(EntityMetadata entityMetadata)
        {
            allColumnItems.Clear();
            checkedMonitoredColumns.Clear();
            allColumnItems.AddRange(GetReadableAttributes(entityMetadata).Select(attribute => new AttributeListItem(attribute)));
            txtColumnSearch.Clear();
            ApplyColumnFilter();
        }

        private void PopulateConditionAttributes(EntityMetadata entityMetadata)
        {
            allConditionAttributeItems.Clear();
            allConditionAttributeItems.AddRange(GetReadableAttributes(entityMetadata).Select(attribute => new AttributeListItem(attribute)));
            txtConditionFieldSearch.Clear();
            ApplyConditionFieldFilter();
            UpdateConditionValueHint();
        }

        private static List<AttributeMetadata> GetReadableAttributes(EntityMetadata entityMetadata)
        {
            return entityMetadata.Attributes
                .Where(attribute => attribute.IsValidForRead == true && !string.IsNullOrWhiteSpace(attribute.LogicalName))
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
            var selectedLogicalName = (cboConditionAttribute.SelectedItem as AttributeListItem)?.LogicalName;
            var searchText = txtConditionFieldSearch == null ? string.Empty : txtConditionFieldSearch.Text.Trim();
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
                    if (item != null && string.Equals(item.LogicalName, selectedLogicalName, StringComparison.OrdinalIgnoreCase))
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
                && (string.IsNullOrWhiteSpace(searchText)
                    || item.LogicalName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || item.DisplayName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void PopulateConditionOperators()
        {
            cboFilterType.Items.Clear();
            cboFilterType.Items.Add("and");
            cboFilterType.Items.Add("or");
            cboFilterType.SelectedIndex = 0;

            cboConditionOperator.Items.Clear();
            cboConditionOperator.Items.Add(new ConditionOperatorItem("eq", "Igual a", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("ne", "Diferente de", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("like", "Contem texto (like)", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("not-like", "Nao contem texto", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("null", "Sem valor", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("not-null", "Com valor", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("gt", "Maior que", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("ge", "Maior ou igual", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("lt", "Menor que", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("le", "Menor ou igual", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("on", "Na data", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("on-or-after", "Na data ou depois", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("on-or-before", "Na data ou antes", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("in", "Esta em", true, true));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("not-in", "Nao esta em", true, true));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("between", "Entre", true, true));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("not-between", "Fora do intervalo", true, true));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("last-x-days", "Ultimos X dias", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("next-x-days", "Proximos X dias", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("olderthan-x-days", "Mais antigo que X dias", true, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("today", "Hoje", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("yesterday", "Ontem", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("tomorrow", "Amanha", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("this-week", "Esta semana", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("last-week", "Semana passada", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("next-week", "Proxima semana", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("this-month", "Este mes", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("last-month", "Mes passado", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("next-month", "Proximo mes", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("this-year", "Este ano", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("last-year", "Ano passado", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("next-year", "Proximo ano", false, false));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("contain-values", "Contem valores", true, true));
            cboConditionOperator.Items.Add(new ConditionOperatorItem("not-contain-values", "Nao contem valores", true, true));
            cboConditionOperator.SelectedIndex = 0;

            UpdateConditionValueState();
        }

        private void AddConditionFromBuilder()
        {
            var attribute = cboConditionAttribute.SelectedItem as AttributeListItem;
            var conditionOperator = cboConditionOperator.SelectedItem as ConditionOperatorItem;

            if (attribute == null)
            {
                MessageBox.Show("Carregue a entidade e selecione um campo para a condicao.", "Campo obrigatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (conditionOperator == null)
            {
                MessageBox.Show("Selecione um operador para a condicao.", "Operador obrigatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show("Informe o valor da condicao.", "Valor obrigatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if ((conditionOperator.Operator == "between" || conditionOperator.Operator == "not-between") && values.Count != 2)
                {
                    MessageBox.Show("O operador selecionado precisa de exatamente dois valores.", "Valores invalidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            filterConditions.Add(new FilterCondition
            {
                AttributeLogicalName = attribute.LogicalName,
                AttributeDisplayName = attribute.DisplayName,
                Operator = conditionOperator.Operator,
                OperatorDisplayName = conditionOperator.DisplayName,
                Values = values
            });

            RefreshConditionList();
            SyncFilterXmlFromConditions();
            txtConditionValue.Clear();
            SetStatus("Condicao adicionada ao filtro.");
        }

        private static List<string> SplitConditionValues(string valueText)
        {
            return valueText
                .Split(new[] { "\r\n", "\n", ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .Where(value => value.Length > 0)
                .ToList();
        }

        private static string NormalizeConditionBuilderValue(AttributeMetadata attribute, string value)
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
                if (!string.IsNullOrWhiteSpace(trueLabel) && string.Equals(trueLabel, value, StringComparison.CurrentCultureIgnoreCase))
                {
                    return "1";
                }

                var falseLabel = GetOptionLabel(booleanAttribute.OptionSet.FalseOption);
                if (!string.IsNullOrWhiteSpace(falseLabel) && string.Equals(falseLabel, value, StringComparison.CurrentCultureIgnoreCase))
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
                if (!string.IsNullOrWhiteSpace(optionLabel) && string.Equals(optionLabel, value, StringComparison.CurrentCultureIgnoreCase) && option.Value.HasValue)
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

            return option.Label.LocalizedLabels.Count > 0 ? option.Label.LocalizedLabels[0].Label : null;
        }

        private void RemoveSelectedConditions()
        {
            if (lvConditions.SelectedItems.Count == 0)
            {
                return;
            }

            var conditionsToRemove = lvConditions.SelectedItems
                .Cast<ListViewItem>()
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
            if (txtFilterXml == null || filterConditions.Count == 0)
            {
                return;
            }

            txtFilterXml.Text = BuildFilterElementFromConditions().ToString();
        }

        private XElement BuildFilterElementFromConditions()
        {
            var filterType = cboFilterType.SelectedItem == null ? "and" : cboFilterType.SelectedItem.ToString();
            var filter = new XElement("filter", new XAttribute("type", filterType));

            foreach (var condition in filterConditions)
            {
                filter.Add(BuildConditionElement(condition));
            }

            return filter;
        }

        private static XElement BuildConditionElement(FilterCondition condition)
        {
            var element = new XElement("condition",
                new XAttribute("attribute", condition.AttributeLogicalName),
                new XAttribute("operator", condition.Operator));

            if (condition.Values == null || condition.Values.Count == 0)
            {
                return element;
            }

            if (condition.Values.Count == 1 && condition.Operator != "in" && condition.Operator != "not-in" && condition.Operator != "contain-values" && condition.Operator != "not-contain-values")
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
                    if (picker.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(picker.SelectedValue))
                    {
                        txtConditionValue.Text = picker.SelectedValue;
                    }
                }

                return;
            }

            if (IsOptionSetAttribute(attribute.Metadata))
            {
                using (var picker = new OptionSetValuePickerForm(attribute.Metadata, conditionOperator.AllowsMultipleValues))
                {
                    if (picker.ShowDialog(this) == DialogResult.OK && picker.SelectedValues.Count > 0)
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

            var typeHint = attribute == null ? "Selecione um campo carregado da entidade." : GetValueHint(attribute.Metadata);
            if (attribute != null)
            {
                canPickValue = attribute.Metadata is LookupAttributeMetadata || IsOptionSetAttribute(attribute.Metadata);
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

        private static bool TryNormalizeFilterXml(string filterText, out string normalizedFilter, out string error)
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
                if (string.Equals(element.Name.LocalName, "filter", StringComparison.OrdinalIgnoreCase))
                {
                    normalizedFilter = element.ToString(SaveOptions.DisableFormatting);
                    return true;
                }

                if (string.Equals(element.Name.LocalName, "condition", StringComparison.OrdinalIgnoreCase))
                {
                    normalizedFilter = new XElement("filter", new XAttribute("type", "and"), element).ToString(SaveOptions.DisableFormatting);
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
                error = "Informe um filtro FetchXML valido. Use <filter>...</filter> ou uma ou mais tags <condition ... />. Detalhe: " + ex.Message;
                return false;
            }
        }

        private static string BuildFetchXml(string entityLogicalName, string primaryIdAttribute, string primaryNameAttribute, List<string> monitoredColumns, string filterXml)
        {
            var attributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            attributes.Add(primaryIdAttribute);

            if (!string.IsNullOrWhiteSpace(primaryNameAttribute))
            {
                attributes.Add(primaryNameAttribute);
            }

            foreach (var column in monitoredColumns)
            {
                attributes.Add(column);
            }

            var builder = new StringBuilder();
            builder.Append("<fetch version=\"1.0\" mapping=\"logical\" no-lock=\"true\" count=\"5000\">");
            builder.AppendFormat(CultureInfo.InvariantCulture, "<entity name=\"{0}\">", EscapeXml(entityLogicalName));

            foreach (var attribute in attributes)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "<attribute name=\"{0}\" />", EscapeXml(attribute));
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
            var label = attribute.DisplayName != null && attribute.DisplayName.UserLocalizedLabel != null
                ? attribute.DisplayName.UserLocalizedLabel.Label
                : null;

            if (string.IsNullOrWhiteSpace(label) && attribute.DisplayName != null && attribute.DisplayName.LocalizedLabels.Count > 0)
            {
                label = attribute.DisplayName.LocalizedLabels[0].Label;
            }

            return string.IsNullOrWhiteSpace(label) ? attribute.LogicalName : label;
        }

        private static string GetRecordName(Entity entity, string primaryNameAttribute)
        {
            if (!string.IsNullOrWhiteSpace(primaryNameAttribute) && entity.Contains(primaryNameAttribute))
            {
                var value = FormatValue(entity[primaryNameAttribute], entity.FormattedValues.Contains(primaryNameAttribute) ? entity.FormattedValues[primaryNameAttribute] : null);
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
                return string.Join(",", optionSetValueCollection.Select(item => item.Value.ToString(CultureInfo.InvariantCulture)).OrderBy(item => item));
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
                return ((DateTime)value).ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
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
                return string.IsNullOrWhiteSpace(entityReference.Name) ? entityReference.Id.ToString("D") : entityReference.Name;
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
                return string.Join(", ", optionSetValueCollection.Select(item => item.Value.ToString(CultureInfo.CurrentCulture)));
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
            tsslStatus.Text = status;
        }

        private void SetMonitoringControls(bool monitoring)
        {
            var hasActiveMonitors = HasActiveMonitors();
            txtEntityLogicalName.Enabled = true;
            btnLoadColumns.Enabled = true;
            txtColumnSearch.Enabled = true;
            clbColumns.Enabled = true;
            btnSelectAllColumns.Enabled = true;
            btnClearColumnSelection.Enabled = true;
            cboFilterType.Enabled = true;
            txtConditionFieldSearch.Enabled = true;
            cboConditionAttribute.Enabled = true;
            cboConditionOperator.Enabled = true;
            txtConditionValue.Enabled = (cboConditionOperator.SelectedItem as ConditionOperatorItem)?.RequiresValue != false;
            btnPickConditionValue.Enabled = btnPickConditionValue.Enabled && txtConditionValue.Enabled;
            btnAddCondition.Enabled = true;
            btnRemoveCondition.Enabled = true;
            btnClearFilter.Enabled = true;
            lvConditions.Enabled = true;
            txtFilterXml.Enabled = true;
            nudIntervalSeconds.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = hasActiveMonitors;
            btnStopSelectedMonitor.Enabled = hasActiveMonitors;
        }

        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            StopMonitoring(false);
            notifyIcon.Visible = false;

            if (mySettings != null)
            {
                SettingsManager.Instance.Save(GetType(), mySettings);
            }
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (HasActiveMonitors())
            {
                StopMonitoring(true);
                AddLog("Conexao alterada; monitoramento interrompido.");
            }

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        private sealed class AttributeListItem
        {
            public AttributeListItem(AttributeMetadata metadata)
            {
                Metadata = metadata;
                LogicalName = metadata.LogicalName;
                DisplayName = GetAttributeDisplayName(metadata);
                AttributeType = metadata.AttributeType.HasValue ? metadata.AttributeType.Value.ToString() : "Unknown";
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
            public ConditionOperatorItem(string fetchXmlOperator, string displayName, bool requiresValue, bool allowsMultipleValues)
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

            public DateTime CreatedOn { get; set; }

            public string Status { get; set; }

            public ListViewItem ListViewItem { get; set; }

            public string DisplayName
            {
                get
                {
                    return $"{Configuration.EntityLogicalName} #{CreatedOn:HHmmss}";
                }
            }
        }

        private sealed class MonitoringConfiguration
        {
            public IOrganizationService Service { get; set; }

            public string EntityLogicalName { get; set; }

            public string PrimaryIdAttribute { get; set; }

            public string PrimaryNameAttribute { get; set; }

            public List<string> MonitoredColumns { get; set; }

            public int IntervalSeconds { get; set; }

            public string FilterXml { get; set; }

            public string FetchXml { get; set; }
        }

        private sealed class RecordSnapshot
        {
            public Guid RecordId { get; set; }

            public string RecordName { get; set; }

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

            public string ColumnLogicalName { get; set; }

            public string OldValue { get; set; }

            public string NewValue { get; set; }
        }
    }
}
