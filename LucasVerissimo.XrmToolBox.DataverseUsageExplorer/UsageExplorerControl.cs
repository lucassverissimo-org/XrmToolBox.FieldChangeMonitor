using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Scanners;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Services;
using LucasVerissimo.XrmToolBox.Shared.Controls;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer
{
    public partial class UsageExplorerControl : PluginControlBase
    {
        private const int MaximumConcurrentScanners = 4;

        private MetadataService metadata;
        private DefaultSolutionNavigationService defaultSolutionNavigation;
        private CancellationTokenSource cancellation;
        private bool isScanRunning;
        private List<UsageReference> results = new List<UsageReference>();
        private string environmentUrl;
        private readonly ScanProgressBuffer scanProgressBuffer = new ScanProgressBuffer();

        public UsageExplorerControl()
        {
            InitializeComponent();
            ConfigureMetadataPickers();
        }

        private void ConfigureMetadataPickers()
        {
            tables.Configure(CreateMetadataPickerConfiguration("tables"));
            columns.Configure(CreateMetadataPickerConfiguration("columns"));
        }

        private static GridPickerConfiguration CreateMetadataPickerConfiguration(string itemName)
        {
            return new GridPickerConfiguration
            {
                ItemName = itemName,
                SearchEnabled = true,
                SortingEnabled = true,
                DisplayTextSelector = item =>
                {
                    var metadataItem = (MetadataListItem)item;
                    return metadataItem.DisplayName + "  —  " + metadataItem.LogicalName;
                },
                IdentitySelector = item => ((MetadataListItem)item).LogicalName,
                SearchPredicate = (item, search) =>
                {
                    var metadataItem = (MetadataListItem)item;
                    return ContainsIgnoreCase(metadataItem.DisplayName, search)
                        || ContainsIgnoreCase(metadataItem.LogicalName, search);
                },
                Columns = new[]
                {
                    new GridPickerColumnDefinition(
                        "Display Name",
                        item => ((MetadataListItem)item).DisplayName
                    )
                    {
                        FillWeight = 55F,
                    },
                    new GridPickerColumnDefinition(
                        "Logical Name",
                        item => ((MetadataListItem)item).LogicalName
                    )
                    {
                        FillWeight = 45F,
                    },
                },
            };
        }

        private static bool ContainsIgnoreCase(string value, string search)
        {
            return value?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override void UpdateConnection(
            IOrganizationService newService,
            ConnectionDetail detail,
            string actionName,
            object parameter
        )
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            metadata = newService == null ? null : new MetadataService(newService);
            defaultSolutionNavigation =
                newService == null
                    ? null
                    : new DefaultSolutionNavigationService(newService, detail?.EnvironmentId);
            environmentUrl =
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
            tables.ClearItems();
            columns.ClearItems();
            status.Text =
                newService == null
                    ? "Connect to Dataverse to begin."
                    : "Connected. Load tables to begin.";
            loadTables.Enabled = newService != null;
            scan.Enabled = newService != null;
            openComponent.Enabled = false;
        }

        private void ByTableCheckedChanged(object sender, EventArgs e)
        {
            UpdateColumnStep();
        }

        private void LoadTablesClick(object sender, EventArgs e)
        {
            ExecuteMethod(LoadTableMetadata);
        }

        private void TablesSelectedItemChanged(object sender, EventArgs e)
        {
            UpdateTableSelection();
        }

        private void ByColumnCheckedChanged(object sender, EventArgs e)
        {
            UpdateColumnStep();

            if (byColumn.Checked && tables.SelectedItem != null)
            {
                ExecuteMethod(LoadColumnMetadata);
            }
        }

        private void ColumnsSelectedItemChanged(object sender, EventArgs e)
        {
            UpdateColumnSelection();
        }

        private async void ScanClick(object sender, EventArgs e)
        {
            await StartScan();
        }

        private void CancelClick(object sender, EventArgs e)
        {
            cancel.Enabled = false;
            cancel.Text = "Cancelling...";
            cancellation?.Cancel();
        }

        private void GridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var selectedItem = (UsageReference)grid.Rows[e.RowIndex].DataBoundItem;
            using (
                var detailsForm = new UsageDetailsForm(
                    selectedItem,
                    environmentUrl,
                    GetDefaultSolutionNavigationContext()
                )
            )
            {
                detailsForm.ShowDialog(this);
            }
        }

        private void GridSelectionChanged(object sender, EventArgs e)
        {
            openComponent.Enabled =
                SelectedReference != null && !string.IsNullOrWhiteSpace(environmentUrl);
        }

        private void OpenComponentClick(object sender, EventArgs e)
        {
            OpenSelectedComponent();
        }

        private void SearchTextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void ComponentFilterSelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void UpdateTableSelection()
        {
            var item = tables.SelectedItem as MetadataListItem;
            selectedTableInfo.Text =
                item == null ? "No table selected" : item.DisplayName + "  •  " + item.LogicalName;
            selectedTableInfo.ForeColor =
                item == null ? Color.DimGray : Color.FromArgb(35, 94, 139);
            columns.ClearItems();
            selectedColumnInfo.Text = "No column selected";
            UpdateColumnStep();
            if (item != null && byColumn.Checked)
                ExecuteMethod(LoadColumnMetadata);
        }

        private void UpdateColumnSelection()
        {
            var item = columns.SelectedItem as MetadataListItem;
            selectedColumnInfo.Text =
                item == null ? "No column selected" : item.DisplayName + "  •  " + item.LogicalName;
            selectedColumnInfo.ForeColor =
                item == null ? Color.DimGray : Color.FromArgb(35, 94, 139);
        }

        private void UpdateColumnStep()
        {
            var canSelectColumn =
                !isScanRunning && byColumn.Checked && tables.SelectedItem is MetadataListItem;

            columnStep.Enabled = canSelectColumn;
            columns.Enabled = canSelectColumn;
        }

        private string SelectedTable
        {
            get
            {
                return (tables.SelectedItem as MetadataListItem) == null
                    ? null
                    : ((MetadataListItem)tables.SelectedItem).LogicalName;
            }
        }
        private int? SelectedTableObjectTypeCode
        {
            get
            {
                return (tables.SelectedItem as MetadataListItem) == null
                    ? null
                    : ((MetadataListItem)tables.SelectedItem).ObjectTypeCode;
            }
        }
        private string SelectedTableEntitySetName
        {
            get
            {
                return (tables.SelectedItem as MetadataListItem) == null
                    ? null
                    : ((MetadataListItem)tables.SelectedItem).EntitySetName;
            }
        }
        private string SelectedColumn
        {
            get
            {
                return (columns.SelectedItem as MetadataListItem) == null
                    ? null
                    : ((MetadataListItem)columns.SelectedItem).LogicalName;
            }
        }

        private void LoadTableMetadata()
        {
            if (Service == null)
                return;
            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = "Loading Dataverse tables...",
                    Work = (worker, a) => a.Result = metadata.GetTables(CancellationToken.None),
                    PostWorkCallBack = a =>
                    {
                        if (a.Error != null)
                            ShowError(a.Error, "Unable to load tables");
                        else
                        {
                            var loaded = (IReadOnlyCollection<MetadataListItem>)a.Result;
                            tables.SetItems(loaded);
                            status.Text =
                                loaded.Count
                                + " tables loaded. Open the list or type to filter by display or logical name.";
                        }
                    },
                }
            );
        }

        private void LoadColumnMetadata()
        {
            var table = SelectedTable;
            if (Service == null || string.IsNullOrWhiteSpace(table))
                return;
            columns.ClearItems();
            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = "Loading columns for " + table + "...",
                    Work = (worker, a) =>
                        a.Result = metadata.GetColumns(table, CancellationToken.None),
                    PostWorkCallBack = a =>
                    {
                        if (a.Error != null)
                            ShowError(a.Error, "Unable to load columns");
                        else
                        {
                            var loaded = (IReadOnlyCollection<MetadataListItem>)a.Result;
                            columns.SetItems(loaded);
                            status.Text =
                                loaded.Count
                                + " columns loaded. Open the list or type to filter by display or logical name.";
                        }
                    },
                }
            );
        }

        private async Task StartScan()
        {
            if (
                Service == null
                || string.IsNullOrWhiteSpace(SelectedTable)
                || (byColumn.Checked && string.IsNullOrWhiteSpace(SelectedColumn))
            )
            {
                MessageBox.Show(
                    this,
                    byColumn.Checked ? "Select a table and a column." : "Select a table.",
                    "Dataverse Usage Explorer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }
            var context = new UsageSearchContext
            {
                Service = Service,
                SearchType = byColumn.Checked ? UsageSearchType.Column : UsageSearchType.Table,
                TableLogicalName = SelectedTable,
                TableEntitySetName = SelectedTableEntitySetName,
                TableObjectTypeCode = SelectedTableObjectTypeCode,
                ColumnLogicalName = SelectedColumn,
            };
            var scanners = CreateScanners()
                .Where(scanner => scannerList.CheckedItems.Contains(DisplayName(scanner.Name)))
                .ToList();
            if (scanners.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Select at least one component type.",
                    "Dataverse Usage Explorer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var scanCancellation = new CancellationTokenSource();
            cancellation = scanCancellation;
            status.Text = "Scanning...";
            ResetResultsForScan();
            scannerList.BeginScan(scanners.Select(scanner => DisplayName(scanner.Name)));
            SetScanRunningState(true);

            scanProgressBuffer.Clear();
            scanProgressTimer.Start();
            try
            {
                var scannerResults = await Task.Run(() =>
                    ExecuteScannersInParallel(
                        scanners,
                        context,
                        scanCancellation.Token,
                        scanProgressBuffer.Publish
                    )
                );
                FlushScanProgress();
                ShowResults(scannerResults);
            }
            catch (OperationCanceledException)
            {
                FlushScanProgress();
                scannerList.FinishCancellation();
                status.Text =
                    "Scan cancelled. "
                    + results.Count
                    + " reference(s) found before cancellation were preserved.";
            }
            catch (Exception error)
            {
                ShowError(error, "Scan failed");
            }
            finally
            {
                scanProgressTimer.Stop();
                FlushScanProgress();
                if (ReferenceEquals(cancellation, scanCancellation))
                {
                    cancellation = null;
                }

                scanCancellation.Dispose();
                scannerList.EndScan();
                SetScanRunningState(false);
            }
        }

        private static List<ScannerResult> ExecuteScannersInParallel(
            IReadOnlyCollection<IUsageScanner> scanners,
            UsageSearchContext context,
            CancellationToken token,
            Action<ScanProgressState> reportProgress
        )
        {
            using (var concurrencyGate = new SemaphoreSlim(MaximumConcurrentScanners))
            {
                var scannerTasks = scanners
                    .Select(scanner =>
                        Task.Run(
                            () =>
                            {
                                concurrencyGate.Wait(token);
                                try
                                {
                                    reportProgress(ScanProgressState.Started(scanner.Name));
                                    return ExecuteScanner(scanner, context, token, reportProgress);
                                }
                                finally
                                {
                                    concurrencyGate.Release();
                                }
                            },
                            token
                        )
                    )
                    .ToArray();

                return Task.WhenAll(scannerTasks).GetAwaiter().GetResult().ToList();
            }
        }

        private static ScannerResult ExecuteScanner(
            IUsageScanner scanner,
            UsageSearchContext context,
            CancellationToken token,
            Action<ScanProgressState> reportProgress
        )
        {
            ScannerResult scannerResult;
            try
            {
                token.ThrowIfCancellationRequested();
                scannerResult = new ScannerResult
                {
                    ScannerName = scanner.Name,
                    References = scanner.Scan(
                        context,
                        token,
                        (name, current, total) =>
                        {
                            reportProgress(
                                ScanProgressState.Progress(
                                    scanner.Name,
                                    name + ": " + current + "/" + total,
                                    current,
                                    total
                                )
                            );
                        }
                    ),
                };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception error)
            {
                scannerResult = new ScannerResult
                {
                    ScannerName = scanner.Name,
                    References = new List<UsageReference>(),
                    Error = error,
                };
            }

            reportProgress(ScanProgressState.Completed(scannerResult));
            return scannerResult;
        }

        private List<IUsageScanner> CreateScanners()
        {
            var workflows = new WorkflowRepository(Service);
            return new List<IUsageScanner>
            {
                new WorkflowUsageScanner(workflows, 2, "Business Rule"),
                new FormUsageScanner(),
                new PowerAutomateUsageScanner(workflows),
                new ViewUsageScanner(),
                new WorkflowUsageScanner(workflows, 0, "Classic Workflow"),
                new PluginStepUsageScanner(),
                new WorkflowUsageScanner(workflows, 4, "Business Process Flow"),
                new WebResourceUsageScanner(),
            };
        }

        private static string DisplayName(string name)
        {
            if (name == "Business Rule")
                return "Business Rules";
            if (name == "Classic Workflow")
                return "Classic Workflows";
            if (name == "Business Process Flow")
                return "Business Process Flows";
            if (name == "Form")
                return "Forms";
            if (name == "View")
                return "Views";
            if (name == "Plugin Step")
                return "Plugin Steps";
            if (name == "Web Resource")
                return "Web Resources";
            return name;
        }

        private void ShowResults(List<ScannerResult> scannerResults)
        {
            foreach (var scannerResult in scannerResults)
            {
                MergeScannerResult(scannerResult);
            }

            ApplyFilter();
            var failures = scannerResults.Where(x => x.Error != null).ToList();
            status.Text =
                failures.Count == 0
                    ? "Scan completed successfully. " + results.Count + " reference(s) found."
                    : "Scan completed; "
                        + failures.Count
                        + " scanner(s) failed: "
                        + string.Join(
                            "; ",
                            failures.Select(x => x.ScannerName + " - " + x.Error.Message)
                        );
        }

        private void ResetResultsForScan()
        {
            results.Clear();
            componentFilter.Items.Clear();
            componentFilter.Items.Add("All component types");
            componentFilter.SelectedIndex = 0;
            ApplyFilter();
        }

        private void SetScanRunningState(bool isRunning)
        {
            isScanRunning = isRunning;
            byTable.Enabled = !isRunning;
            byColumn.Enabled = !isRunning;
            tables.Enabled = !isRunning;
            loadTables.Enabled = !isRunning && Service != null;
            componentFilter.Enabled = !isRunning;
            search.Enabled = !isRunning;
            openComponent.Enabled =
                SelectedReference != null && !string.IsNullOrWhiteSpace(environmentUrl);
            grid.Enabled = true;
            cancel.Enabled = isRunning;
            UpdateColumnStep();

            if (isRunning)
            {
                scan.StartLoading();
            }
            else
            {
                scan.StopLoading();
                scan.Enabled = Service != null;
                cancel.Text = "Cancel";
            }
        }

        private void HandleScanProgress(ScanProgressState progressState)
        {
            if (progressState == null)
            {
                return;
            }

            if (progressState.CompletedScanner != null)
            {
                MergeScannerResult(progressState.CompletedScanner);
            }

            status.Text = progressState.StatusMessage;
            switch (progressState.State)
            {
                case ScannerProgressKind.Started:
                    scannerList.MarkStarted(DisplayName(progressState.ScannerName));
                    break;
                case ScannerProgressKind.Progress:
                    scannerList.MarkProgress(
                        DisplayName(progressState.ScannerName),
                        progressState.Current,
                        progressState.Total
                    );
                    break;
                case ScannerProgressKind.Completed:
                    if (progressState.CompletedScanner.Error == null)
                    {
                        scannerList.MarkCompleted(
                            DisplayName(progressState.ScannerName),
                            progressState.CompletedScanner.References.Count
                        );
                    }
                    else
                    {
                        scannerList.MarkFailed(
                            DisplayName(progressState.ScannerName),
                            progressState.CompletedScanner.Error.Message
                        );
                    }

                    break;
            }
        }

        private void ScanProgressTimerTick(object sender, EventArgs eventArguments)
        {
            FlushScanProgress();
        }

        private void FlushScanProgress()
        {
            foreach (var progressState in scanProgressBuffer.Drain())
            {
                HandleScanProgress(progressState);
            }
        }

        private void MergeScannerResult(ScannerResult scannerResult)
        {
            if (scannerResult == null || scannerResult.References == null)
            {
                return;
            }

            var knownIds = new HashSet<Guid>(
                results.Where(x => x.ComponentId.HasValue).Select(x => x.ComponentId.Value)
            );
            var newReferences = scannerResult.References.Where(reference =>
                !reference.ComponentId.HasValue || knownIds.Add(reference.ComponentId.Value)
            );

            results.AddRange(newReferences);
            results = results.OrderBy(x => x.ComponentType).ThenBy(x => x.Name).ToList();

            RefreshComponentFilter();
            ApplyFilter();
        }

        private void RefreshComponentFilter()
        {
            var selectedType = componentFilter.SelectedItem as string;
            var componentTypes = results
                .Select(x => x.ComponentType)
                .Distinct()
                .OrderBy(x => x)
                .Cast<object>()
                .ToArray();

            componentFilter.Items.Clear();
            componentFilter.Items.Add("All component types");
            componentFilter.Items.AddRange(componentTypes);
            componentFilter.SelectedItem =
                selectedType != null && componentFilter.Items.Contains(selectedType)
                    ? selectedType
                    : "All component types";
        }

        private static string CreateScannerCompletionMessage(ScannerResult scannerResult)
        {
            if (scannerResult.Error != null)
            {
                return scannerResult.ScannerName + " failed: " + scannerResult.Error.Message;
            }

            return scannerResult.ScannerName
                + " completed. "
                + scannerResult.References.Count
                + " reference(s) found.";
        }

        private void ApplyFilter()
        {
            var type = componentFilter.SelectedItem as string;
            var text = search.Text.Trim();
            var filtered = results
                .Where(x =>
                    (type == null || type == "All component types" || x.ComponentType == type)
                    && (
                        text.Length == 0
                        || new[]
                        {
                            x.ComponentType,
                            x.Name,
                            x.TableLogicalName,
                            x.Status,
                            x.ReferenceType,
                            x.FoundIn,
                        }.Any(v =>
                            !string.IsNullOrWhiteSpace(v)
                            && v.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0
                        )
                    )
                )
                .ToList();
            grid.DataSource = new BindingList<UsageReference>(filtered);
            summary.Text =
                filtered.Count
                + " references found"
                + (results.Count == filtered.Count ? "" : " (" + results.Count + " total)");
        }

        private void ShowError(Exception error, string title)
        {
            status.Text = title + ": " + error.Message;
            MessageBox.Show(this, error.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            LogError(error.ToString());
        }

        private UsageReference SelectedReference
        {
            get
            {
                return grid.SelectedRows.Count == 0
                    ? null
                    : grid.SelectedRows[0].DataBoundItem as UsageReference;
            }
        }

        private void OpenSelectedComponent()
        {
            var item = SelectedReference;
            var navigationTarget = ComponentNavigationService.Resolve(
                item,
                environmentUrl,
                GetDefaultSolutionNavigationContext()
            );
            if (!navigationTarget.CanOpen)
            {
                MessageBox.Show(
                    this,
                    navigationTarget.UnavailableReason,
                    "Component editor",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            try
            {
                ComponentNavigationService.Open(navigationTarget);
            }
            catch (Exception ex)
            {
                ShowError(ex, "Unable to open component");
            }
        }

        private DefaultSolutionNavigationContext GetDefaultSolutionNavigationContext()
        {
            try
            {
                return defaultSolutionNavigation?.GetContext();
            }
            catch (Exception exception)
            {
                LogError("Unable to resolve the Default solution: " + exception);
                return null;
            }
        }

        private sealed class ScanProgressState
        {
            public string ScannerName { get; private set; }

            public string StatusMessage { get; set; }

            public ScannerProgressKind State { get; private set; }

            public int Current { get; private set; }

            public int Total { get; private set; }

            public ScannerResult CompletedScanner { get; set; }

            public static ScanProgressState Started(string scannerName)
            {
                return new ScanProgressState
                {
                    ScannerName = scannerName,
                    StatusMessage = "Starting " + DisplayName(scannerName) + "...",
                    State = ScannerProgressKind.Started,
                };
            }

            public static ScanProgressState Progress(
                string scannerName,
                string statusMessage,
                int current,
                int total
            )
            {
                return new ScanProgressState
                {
                    ScannerName = scannerName,
                    StatusMessage = statusMessage,
                    State = ScannerProgressKind.Progress,
                    Current = current,
                    Total = total,
                };
            }

            public static ScanProgressState Completed(ScannerResult scannerResult)
            {
                return new ScanProgressState
                {
                    ScannerName = scannerResult.ScannerName,
                    StatusMessage = CreateScannerCompletionMessage(scannerResult),
                    State = ScannerProgressKind.Completed,
                    CompletedScanner = scannerResult,
                };
            }
        }

        private sealed class ScanProgressBuffer
        {
            private readonly object syncRoot = new object();
            private readonly Dictionary<string, ScanProgressState> latestStates = new Dictionary<
                string,
                ScanProgressState
            >(StringComparer.OrdinalIgnoreCase);

            public void Publish(ScanProgressState progressState)
            {
                if (progressState == null)
                {
                    return;
                }

                lock (syncRoot)
                {
                    latestStates[progressState.ScannerName] = progressState;
                }
            }

            public IReadOnlyCollection<ScanProgressState> Drain()
            {
                lock (syncRoot)
                {
                    var states = latestStates.Values.ToList();
                    latestStates.Clear();
                    return states;
                }
            }

            public void Clear()
            {
                lock (syncRoot)
                {
                    latestStates.Clear();
                }
            }
        }

        private enum ScannerProgressKind
        {
            Started,
            Progress,
            Completed,
        }
    }
}
