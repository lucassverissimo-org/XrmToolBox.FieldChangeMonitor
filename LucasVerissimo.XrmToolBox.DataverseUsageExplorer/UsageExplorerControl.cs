using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Scanners;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Services;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer
{
    public partial class UsageExplorerControl : PluginControlBase
    {
        private MetadataService metadata;
        private CancellationTokenSource cancellation;
        private List<UsageReference> results = new List<UsageReference>();
        private string environmentUrl;

        public UsageExplorerControl()
        {
            InitializeComponent();
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

        private void TablesSelectedIndexChanged(object sender, EventArgs e)
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

        private void ColumnsSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateColumnSelection();
        }

        private void ScanClick(object sender, EventArgs e)
        {
            ExecuteMethod(StartScan);
        }

        private void CancelClick(object sender, EventArgs e)
        {
            cancellation?.Cancel();
        }

        private void GridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var selectedItem = (UsageReference)grid.Rows[e.RowIndex].DataBoundItem;
            using (var detailsForm = new UsageDetailsForm(selectedItem))
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
            columnStep.Enabled = byColumn.Checked && tables.SelectedItem is MetadataListItem;
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

        private void StartScan()
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
                TableObjectTypeCode = SelectedTableObjectTypeCode,
                ColumnLogicalName = SelectedColumn,
            };
            var scanners = CreateScanners()
                .Where(x => scannerList.CheckedItems.Cast<string>().Contains(DisplayName(x.Name)))
                .ToList();
            cancellation = new CancellationTokenSource();
            scan.Enabled = false;
            cancel.Enabled = true;
            status.Text = "Scanning...";
            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = "Scanning Dataverse components...",
                    Work = (worker, a) =>
                    {
                        var output = new List<ScannerResult>();
                        foreach (var scanner in scanners)
                        {
                            try
                            {
                                cancellation.Token.ThrowIfCancellationRequested();
                                output.Add(
                                    new ScannerResult
                                    {
                                        ScannerName = scanner.Name,
                                        References = scanner.Scan(
                                            context,
                                            cancellation.Token,
                                            (name, current, total) =>
                                                worker.ReportProgress(
                                                    total == 0 ? 0 : current * 100 / total,
                                                    name + ": " + current + "/" + total
                                                )
                                        ),
                                    }
                                );
                            }
                            catch (OperationCanceledException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                output.Add(
                                    new ScannerResult
                                    {
                                        ScannerName = scanner.Name,
                                        References = new List<UsageReference>(),
                                        Error = ex,
                                    }
                                );
                            }
                        }
                        a.Result = output;
                    },
                    ProgressChanged = a => status.Text = Convert.ToString(a.UserState),
                    PostWorkCallBack = a =>
                    {
                        scan.Enabled = Service != null;
                        cancel.Enabled = false;
                        if (a.Error is OperationCanceledException)
                        {
                            status.Text = "Scan cancelled.";
                            return;
                        }
                        if (a.Error != null)
                        {
                            ShowError(a.Error, "Scan failed");
                            return;
                        }
                        ShowResults((List<ScannerResult>)a.Result);
                    },
                }
            );
        }

        private List<IUsageScanner> CreateScanners()
        {
            var workflows = new WorkflowRepository(Service);
            return new List<IUsageScanner>
            {
                new WorkflowUsageScanner(workflows, 2, "Business Rule"),
                new PowerAutomateUsageScanner(workflows),
                new WorkflowUsageScanner(workflows, 0, "Classic Workflow"),
                new WorkflowUsageScanner(workflows, 4, "Business Process Flow"),
                new FormUsageScanner(),
                new ViewUsageScanner(),
                new PluginStepUsageScanner(),
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
            return name;
        }

        private void ShowResults(List<ScannerResult> scannerResults)
        {
            results = DeduplicateByComponentId(scannerResults.SelectMany(x => x.References))
                .OrderBy(x => x.ComponentType)
                .ThenBy(x => x.Name)
                .ToList();
            componentFilter.Items.Clear();
            componentFilter.Items.Add("All component types");
            componentFilter.Items.AddRange(
                results
                    .Select(x => x.ComponentType)
                    .Distinct()
                    .OrderBy(x => x)
                    .Cast<object>()
                    .ToArray()
            );
            componentFilter.SelectedIndex = 0;
            ApplyFilter();
            var failures = scannerResults.Where(x => x.Error != null).ToList();
            status.Text =
                failures.Count == 0
                    ? "Scan completed successfully."
                    : "Scan completed; "
                        + failures.Count
                        + " scanner(s) failed: "
                        + string.Join(
                            "; ",
                            failures.Select(x => x.ScannerName + " - " + x.Error.Message)
                        );
        }

        private static IEnumerable<UsageReference> DeduplicateByComponentId(
            IEnumerable<UsageReference> references
        )
        {
            var ids = new HashSet<Guid>();
            foreach (var reference in references)
            {
                if (!reference.ComponentId.HasValue || ids.Add(reference.ComponentId.Value))
                    yield return reference;
            }
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
            if (
                item == null
                || !item.ComponentId.HasValue
                || string.IsNullOrWhiteSpace(environmentUrl)
            )
                return;
            var entityName = item.ComponentEntityName;
            if (string.IsNullOrWhiteSpace(entityName))
                entityName =
                    item.ComponentType == "Form" ? "systemform"
                    : item.ComponentType == "View" ? "savedquery"
                    : item.ComponentType == "Plugin Step" ? "sdkmessageprocessingstep"
                    : "workflow";
            var url =
                environmentUrl.TrimEnd('/')
                + "/main.aspx?pagetype=entityrecord&etn="
                + Uri.EscapeDataString(entityName)
                + "&id="
                + Uri.EscapeDataString(item.ComponentId.Value.ToString("D"));
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                ShowError(ex, "Unable to open component");
            }
        }
    }
}
