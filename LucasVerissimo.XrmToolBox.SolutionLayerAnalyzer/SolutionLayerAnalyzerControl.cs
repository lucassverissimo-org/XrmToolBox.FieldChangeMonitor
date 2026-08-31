using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.Shared.Controls;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Services;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility;
using SolutionInfo = LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models.SolutionInfo;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer
{
    public partial class SolutionLayerAnalyzerControl : MultipleConnectionsPluginControlBase
    {
        private const int OperationStepsRowIndex = 2;
        private const float OperationStepsRowHeight = 150F;
        private static readonly string[] AnalysisSteps =
        {
            "Load components from the Source solution",
            "Load components from the matching Target solution",
            "Correlate composition, names, and Target existence",
            "Check Active solution components in the Target environment",
            "Finalize metrics and analysis results",
        };
        private static readonly string[] RemovalSteps =
        {
            "Review selected components and confirmations",
            "Choose the backup strategy",
            "Create and export the backup",
            "Remove selected Active Layers in Target",
            "Validate removals and refresh the results",
        };
        private readonly AnalyzerOptions options = new AnalyzerOptions();
        private readonly List<LayerAnalysisResult> analysisResults =
            new List<LayerAnalysisResult>();
        private readonly HashSet<ComponentIdentity> displayedResults =
            new HashSet<ComponentIdentity>();
        private ConnectionDetail targetConnectionDetail;
        private SolutionInfo targetSolution;
        private CancellationTokenSource cancellation;
        private AnalysisMetricsSnapshot latestMetrics = new AnalysisMetricsSnapshot();
        private bool operationRunning;

        public SolutionLayerAnalyzerControl()
        {
            InitializeComponent();
            ConfigureSolutionPicker();
            componentTypeFilter.SelectedIndex = 0;
            statusFilter.SelectedIndex = 0;
            resultsGrid.AutoGenerateColumns = false;
            UpdateConnectionLabels();
            UpdateActionState();
        }

        private void ConfigureSolutionPicker()
        {
            sourceSolutions.Configure(
                new GridPickerConfiguration
                {
                    ItemName = "solutions",
                    SearchEnabled = true,
                    SortingEnabled = true,
                    DisplayTextSelector = item =>
                    {
                        var solution = (SolutionInfo)item;
                        return solution.FriendlyName + "  —  " + solution.UniqueName;
                    },
                    IdentitySelector = item => ((SolutionInfo)item).SolutionId,
                    SearchPredicate = (item, search) =>
                    {
                        var solution = (SolutionInfo)item;
                        return Contains(solution.FriendlyName, search)
                            || Contains(solution.UniqueName, search)
                            || Contains(solution.Version, search);
                    },
                    Columns = new[]
                    {
                        new GridPickerColumnDefinition(
                            "Display Name",
                            item => ((SolutionInfo)item).FriendlyName
                        )
                        {
                            FillWeight = 38F,
                        },
                        new GridPickerColumnDefinition(
                            "Unique Name",
                            item => ((SolutionInfo)item).UniqueName
                        )
                        {
                            FillWeight = 32F,
                        },
                        new GridPickerColumnDefinition(
                            "Version",
                            item => ((SolutionInfo)item).Version
                        )
                        {
                            FillWeight = 16F,
                        },
                        new GridPickerColumnDefinition(
                            "Type",
                            item => ((SolutionInfo)item).IsManaged ? "Managed" : "Unmanaged"
                        )
                        {
                            FillWeight = 14F,
                        },
                    },
                }
            );
        }

        public override void UpdateConnection(
            IOrganizationService newService,
            ConnectionDetail detail,
            string actionName,
            object parameter
        )
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (!string.Equals(actionName, "AdditionalOrganization", StringComparison.Ordinal))
            {
                sourceSolutions.ClearItems();
                targetSolution = null;
                ClearAnalysis();
                UpdateConnectionLabels();
                statusLabel.Text =
                    newService == null
                        ? "Connect the Source environment to begin."
                        : "Source connected. Connect Target and load solutions.";
                UpdateActionState();
            }
        }

        protected override void ConnectionDetailsUpdated(
            NotifyCollectionChangedEventArgs eventArguments
        )
        {
            if (
                eventArguments.Action == NotifyCollectionChangedAction.Add
                && eventArguments.NewItems != null
            )
            {
                targetConnectionDetail = eventArguments
                    .NewItems.Cast<ConnectionDetail>()
                    .LastOrDefault();
            }
            else if (
                targetConnectionDetail != null
                && !AdditionalConnectionDetails.Contains(targetConnectionDetail)
            )
            {
                targetConnectionDetail = AdditionalConnectionDetails.FirstOrDefault();
            }

            targetSolution = null;
            ClearAnalysis();
            UpdateConnectionLabels();
            UpdateActionState();
            statusLabel.Text =
                targetConnectionDetail == null
                    ? "Connect the Target environment."
                    : "Target connected. Load Source solutions.";
        }

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            cancellation?.Cancel();
            cancellation?.Dispose();
            base.ClosingPlugin(info);
        }

        private void ConnectTargetClick(object sender, EventArgs e)
        {
            AddAdditionalOrganization();
        }

        private void DisconnectTargetClick(object sender, EventArgs e)
        {
            if (targetConnectionDetail != null)
            {
                RemoveAdditionalOrganization(targetConnectionDetail);
            }
        }

        private void LoadSolutionsClick(object sender, EventArgs e)
        {
            ExecuteMethod(LoadSourceSolutions);
        }

        private void SourceSolutionChanged(object sender, EventArgs e)
        {
            if (SelectedSourceSolution != null && TargetService != null)
            {
                ResolveTargetSolution();
            }
            else
            {
                targetSolution = null;
                UpdateSolutionDetails();
                UpdateActionState();
            }
        }

        private void AnalyzeClick(object sender, EventArgs e)
        {
            ExecuteMethod(StartAnalysis);
        }

        private void CancelClick(object sender, EventArgs e)
        {
            cancellation?.Cancel();
            statusLabel.Text = "Cancellation requested. Finishing the current Dataverse call...";
            LogInfo("Cancellation requested by the user.");
        }

        private void ExportClick(object sender, EventArgs e)
        {
            ExportCsv();
        }

        private void PrepareRemovalClick(object sender, EventArgs e)
        {
            PrepareRemoval();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ResultsGridCurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (resultsGrid.IsCurrentCellDirty)
            {
                resultsGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void ResultsGridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var result = resultsGrid.Rows[e.RowIndex].DataBoundItem as LayerAnalysisResult;
            if (result == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(result.Error))
            {
                e.CellStyle.ForeColor = Color.Firebrick;
            }
            else if (result.HasActiveLayer == true)
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 248, 225);
            }
            else if (result.CorrelationStatus != ComponentCorrelationStatus.Matched)
            {
                e.CellStyle.BackColor = Color.FromArgb(239, 246, 255);
            }
        }

        private void ResultsGridCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != selectColumn.Index)
            {
                return;
            }

            var result = resultsGrid.Rows[e.RowIndex].DataBoundItem as LayerAnalysisResult;
            if (result == null || result.HasActiveLayer != true)
            {
                e.Cancel = true;
            }
        }

        private void LoadSourceSolutions()
        {
            if (Service == null || TargetService == null)
            {
                MessageBox.Show(
                    this,
                    "Connect both Source and Target environments before loading solutions.",
                    "Solution Layer Analyzer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            SetOperationState(true, "Loading Source solutions...");
            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = "Loading Source solutions...",
                    Work = (worker, arguments) =>
                    {
                        var repository = new SolutionRepository(Service);
                        arguments.Result = repository.GetSolutions(CancellationToken.None);
                    },
                    PostWorkCallBack = arguments =>
                    {
                        SetOperationState(false, null);
                        if (arguments.Error != null)
                        {
                            ShowError(arguments.Error, "Unable to load Source solutions");
                            return;
                        }

                        var solutions = (
                            (IReadOnlyCollection<SolutionInfo>)arguments.Result
                        ).ToList();
                        sourceSolutions.SetItems(solutions);
                        statusLabel.Text =
                            solutions.Count
                            + " Source solutions loaded. Select a solution to compare.";
                    },
                }
            );
        }

        private void ResolveTargetSolution()
        {
            var sourceSolution = SelectedSourceSolution;
            var targetService = TargetService;
            if (sourceSolution == null || targetService == null)
            {
                return;
            }

            targetSolution = null;
            UpdateSolutionDetails();
            SetOperationState(true, "Looking for the solution in Target...");
            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = "Looking for " + sourceSolution.UniqueName + " in Target...",
                    Work = (worker, arguments) =>
                    {
                        var repository = new SolutionRepository(targetService);
                        arguments.Result = repository.FindByUniqueName(
                            sourceSolution.UniqueName,
                            CancellationToken.None
                        );
                    },
                    PostWorkCallBack = arguments =>
                    {
                        SetOperationState(false, null);
                        if (arguments.Error != null)
                        {
                            ShowError(arguments.Error, "Unable to query the Target solution");
                            return;
                        }

                        targetSolution = arguments.Result as SolutionInfo;
                        UpdateSolutionDetails();
                        if (targetSolution == null)
                        {
                            statusLabel.Text =
                                "The solution was not found in Target. Analysis can continue from the Source components.";
                            LogWarning(
                                "Solution {0} was not found in Target.",
                                sourceSolution.UniqueName
                            );
                        }
                        else
                        {
                            statusLabel.Text = "Matching Target solution found. Ready to analyze.";
                        }

                        UpdateActionState();
                    },
                }
            );
        }

        private void StartAnalysis()
        {
            var sourceSolution = SelectedSourceSolution;
            var targetService = TargetService;
            if (Service == null || targetService == null || sourceSolution == null)
            {
                MessageBox.Show(
                    this,
                    "Connect Source and Target, then select a Source solution.",
                    "Solution Layer Analyzer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (targetSolution == null)
            {
                var choice = MessageBox.Show(
                    this,
                    "The solution "
                        + sourceSolution.UniqueName
                        + " was not found in Target.\r\n\r\n"
                        + "Analysis can continue using the Source components as reference. "
                        + "Target solution composition comparison will not be available, but Target existence and Active Layers will still be checked.\r\n\r\n"
                        + "Continue?",
                    "Target solution not found",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );
                if (choice != DialogResult.Yes)
                {
                    return;
                }
            }

            ClearAnalysis();
            cancellation?.Dispose();
            cancellation = new CancellationTokenSource();
            BeginOperationSteps("Solution analysis progress", AnalysisSteps);
            operationSteps.SetCurrentStep(0, "Preparing the Source component query...");
            SetOperationState(true, "Starting analysis...");
            LogInfo(
                "Analysis started. Source={0}, Target={1}, Solution={2}.",
                SourceEnvironmentName,
                TargetEnvironmentName,
                sourceSolution.UniqueName
            );

            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = null,
                    Work = (worker, arguments) =>
                    {
                        var service = new SolutionAnalysisService(
                            Service,
                            targetService,
                            options,
                            message => LogInfo(message)
                        );
                        arguments.Result = service.Analyze(
                            sourceSolution,
                            targetSolution,
                            progress =>
                                worker.ReportProgress(
                                    CalculateProgressPercentage(progress.Metrics),
                                    progress
                                ),
                            cancellation.Token
                        );
                    },
                    ProgressChanged = arguments =>
                    {
                        var progress = arguments.UserState as AnalysisProgress;
                        if (progress != null)
                        {
                            ApplyAnalysisProgress(progress);
                        }
                    },
                    PostWorkCallBack = arguments =>
                    {
                        SetOperationState(false, null);
                        if (arguments.Error != null)
                        {
                            operationSteps.FailOperation(arguments.Error.Message);
                            ShowError(arguments.Error, "Analysis failed");
                            return;
                        }

                        var execution = (AnalysisExecutionResult)arguments.Result;
                        latestMetrics = execution.Metrics;
                        UpdateMetrics(execution.Metrics);
                        statusLabel.Text = execution.WasCancelled
                            ? "Analysis cancelled. Completed results were preserved."
                            : "Analysis completed.";
                        if (execution.WasCancelled)
                        {
                            operationSteps.CancelOperation(statusLabel.Text);
                        }
                        else
                        {
                            operationSteps.CompleteOperation(
                                "Analysis completed. Results are ready for review."
                            );
                        }

                        progressBar.Value = execution.WasCancelled ? progressBar.Value : 100;
                        PopulateFilterValues();
                        ApplyFilters();
                        UpdateActionState();
                    },
                }
            );
        }

        private void ApplyAnalysisProgress(AnalysisProgress progress)
        {
            operationSteps.SetCurrentStep(GetAnalysisStepIndex(progress.Stage), progress.Message);

            if (progress.Result != null)
            {
                var identity = new ComponentIdentity(
                    progress.Result.ComponentType,
                    progress.Result.ComponentId
                );
                if (displayedResults.Add(identity))
                {
                    analysisResults.Add(progress.Result);
                }
            }

            if (progress.Metrics != null)
            {
                latestMetrics = progress.Metrics;
                UpdateMetrics(progress.Metrics);
                progressBar.Value = CalculateProgressPercentage(progress.Metrics);
            }

            statusLabel.Text = progress.Message ?? "Analyzing...";
            ApplyFilters();
        }

        private void ExportCsv()
        {
            if (analysisResults.Count == 0)
            {
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                dialog.DefaultExt = "csv";
                dialog.FileName =
                    "solution-layer-analysis-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    new CsvExportService().Export(
                        dialog.FileName,
                        new CsvExportContext
                        {
                            SourceEnvironment = SourceEnvironmentName,
                            TargetEnvironment = TargetEnvironmentName,
                            SourceSolution = SelectedSourceSolution,
                            TargetSolution = targetSolution,
                        },
                        analysisResults
                    );
                    statusLabel.Text = "CSV exported to " + dialog.FileName;
                    LogInfo("Analysis CSV exported to {0}.", dialog.FileName);
                }
                catch (Exception error)
                {
                    ShowError(error, "Unable to export CSV");
                }
            }
        }

        private void PrepareRemoval()
        {
            var selected = analysisResults
                .Where(result =>
                    result.Selected
                    && result.HasActiveLayer == true
                    && result.ExistsInTargetEnvironment
                )
                .ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Select at least one component that has an Active Layer.",
                    "Prepare removal",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var summary = BuildRemovalSummary(selected);
            BeginOperationSteps("Active Layer removal progress", RemovalSteps);
            operationSteps.SetCurrentStep(
                0,
                "Reviewing " + selected.Count + " selected component(s)..."
            );
            if (
                MessageBox.Show(
                    this,
                    summary + "\r\n\r\nPrepare this removal?",
                    "Review selected components",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                ) != DialogResult.Yes
            )
            {
                operationSteps.CancelOperation("Removal preparation cancelled during review.");
                return;
            }

            operationSteps.SetCurrentStep(1, "Waiting for the backup choice...");
            var backupChoice = MessageBox.Show(
                this,
                "Create a recommended unmanaged solution backup for the selected components before removal?\r\n\r\n"
                    + "Yes: create and export a temporary backup solution.\r\n"
                    + "No: continue without a confirmed backup.\r\n"
                    + "Cancel: stop.",
                "Backup before removal",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );
            if (backupChoice == DialogResult.Cancel)
            {
                operationSteps.CancelOperation("Removal preparation cancelled before backup.");
                return;
            }

            if (backupChoice == DialogResult.No)
            {
                LogWarning("Backup skipped by the user.");
                operationSteps.SetCurrentStep(2, "Backup skipped by the user.");
                ContinueAfterBackup(
                    selected,
                    new BackupResult
                    {
                        Status = BackupStatus.Skipped,
                        SelectedComponents = selected.Count,
                    }
                );
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Dataverse solution (*.zip)|*.zip";
                dialog.DefaultExt = "zip";
                dialog.FileName =
                    "active-layer-backup-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".zip";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    operationSteps.CancelOperation("Removal preparation cancelled before backup.");
                    return;
                }

                StartBackup(selected, dialog.FileName);
            }
        }

        private void StartBackup(
            IReadOnlyCollection<LayerAnalysisResult> components,
            string filePath
        )
        {
            cancellation?.Dispose();
            cancellation = new CancellationTokenSource();
            operationSteps.SetCurrentStep(2, "Creating the selected-component backup...");
            SetOperationState(true, "Creating backup...");
            var targetService = TargetService;
            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = null,
                    Work = (worker, arguments) =>
                    {
                        var service = new BackupService(
                            targetService,
                            options,
                            message => LogInfo(message)
                        );
                        arguments.Result = service.CreateBackup(
                            components,
                            filePath,
                            cancellation.Token
                        );
                    },
                    PostWorkCallBack = arguments =>
                    {
                        SetOperationState(false, null);
                        if (arguments.Error != null)
                        {
                            operationSteps.FailOperation(arguments.Error.Message);
                            ContinueAfterBackup(
                                components,
                                new BackupResult
                                {
                                    Status = BackupStatus.Failed,
                                    Error = arguments.Error.Message,
                                    SelectedComponents = components.Count,
                                }
                            );
                            return;
                        }

                        ContinueAfterBackup(components, (BackupResult)arguments.Result);
                    },
                }
            );
        }

        private void ContinueAfterBackup(
            IReadOnlyCollection<LayerAnalysisResult> components,
            BackupResult backup
        )
        {
            if (backup.Status == BackupStatus.Cancelled)
            {
                operationSteps.CancelOperation("Backup and removal were cancelled.");
                return;
            }

            var riskConfirmed = false;
            if (!backup.IsConfirmed)
            {
                var detail = string.IsNullOrWhiteSpace(backup.Error)
                    ? string.Empty
                    : "\r\n\r\nDetails: " + backup.Error;
                if (
                    MessageBox.Show(
                        this,
                        "Backup not confirmed ("
                            + backup.Status
                            + ").\r\n\r\n"
                            + "Removing an Active Layer can permanently discard unmanaged changes. "
                            + "The tool recommends a backup but will not remove your autonomy."
                            + detail
                            + "\r\n\r\nRemove anyway?",
                        "Backup not available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    ) != DialogResult.Yes
                )
                {
                    operationSteps.CancelOperation(
                        "Removal cancelled because the backup was not confirmed."
                    );
                    return;
                }

                if (
                    MessageBox.Show(
                        this,
                        "You are about to remove the Active Layer from "
                            + components.Count
                            + " component(s) without a confirmed backup.\r\n\r\n"
                            + "This operation can be irreversible. Continue?",
                        "Second confirmation required",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Stop
                    ) != DialogResult.Yes
                )
                {
                    operationSteps.CancelOperation("Removal cancelled at final confirmation.");
                    return;
                }

                riskConfirmed = true;
            }

            StartRemoval(components, backup.Status, riskConfirmed);
        }

        private void StartRemoval(
            IReadOnlyCollection<LayerAnalysisResult> components,
            BackupStatus backupStatus,
            bool userConfirmedRisk
        )
        {
            cancellation?.Dispose();
            cancellation = new CancellationTokenSource();
            operationSteps.SetCurrentStep(
                3,
                "Starting removal for " + components.Count + " component(s) in Target..."
            );
            SetOperationState(true, "Removing Active Layers...");
            var targetService = TargetService;
            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = null,
                    Work = (worker, arguments) =>
                    {
                        var service = new ActiveLayerRemovalService(
                            targetService,
                            options,
                            message => LogInfo(message)
                        );
                        arguments.Result = service.Remove(
                            components,
                            backupStatus,
                            userConfirmedRisk,
                            progress =>
                                worker.ReportProgress(
                                    progress.Total == 0
                                        ? 0
                                        : progress.Current * 100 / progress.Total,
                                    progress
                                ),
                            cancellation.Token
                        );
                    },
                    ProgressChanged = arguments =>
                    {
                        var progress = arguments.UserState as RemovalProgress;
                        if (progress == null)
                        {
                            return;
                        }

                        progressBar.Value = Math.Max(
                            0,
                            Math.Min(100, arguments.ProgressPercentage)
                        );
                        if (progress.Stage == RemovalStage.RemovingActiveLayer)
                        {
                            operationSteps.SetCurrentStep(3, progress.Message);
                        }
                        else
                        {
                            operationSteps.SetCurrentStep(4, progress.Message);
                        }

                        statusLabel.Text =
                            "Removing Active Layers: "
                            + progress.Current
                            + "/"
                            + progress.Total
                            + " - "
                            + progress.Message;
                        ApplyRemovalResult(progress.Result);
                    },
                    PostWorkCallBack = arguments =>
                    {
                        SetOperationState(false, null);
                        if (arguments.Error != null)
                        {
                            operationSteps.FailOperation(arguments.Error.Message);
                            ShowError(arguments.Error, "Active Layer removal failed");
                            return;
                        }

                        var results = (
                            (IReadOnlyCollection<RemovalResult>)arguments.Result
                        ).ToList();
                        var removed = results.Count(result =>
                            result.RemovalStatus == RemovalStatus.Removed
                        );
                        var failed = results.Count(result =>
                            result.RemovalStatus == RemovalStatus.Failed
                        );
                        statusLabel.Text =
                            results.Count
                            + " processed; "
                            + removed
                            + " removed; "
                            + failed
                            + " failed.";
                        if (cancellation != null && cancellation.IsCancellationRequested)
                        {
                            operationSteps.CancelOperation(
                                "Removal cancelled. Completed results were preserved."
                            );
                        }
                        else
                        {
                            operationSteps.CompleteOperation(statusLabel.Text);
                        }

                        progressBar.Value = 100;
                        resultsGrid.Refresh();
                        UpdateActionState();
                    },
                }
            );
        }

        private void ApplyRemovalResult(RemovalResult removal)
        {
            if (removal == null)
            {
                return;
            }

            var analysis = analysisResults.FirstOrDefault(result =>
                result.ComponentType == removal.ComponentType
                && result.ComponentId == removal.ComponentId
            );
            if (analysis == null)
            {
                return;
            }

            analysis.Selected = false;
            analysis.Status = removal.ValidationStatus.ToString();
            analysis.Error = removal.Error;
            analysis.DurationMs = removal.DurationMs;
            if (removal.ValidationStatus == ValidationStatus.RemovedAndValidated)
            {
                analysis.HasActiveLayer = false;
                analysis.LayerCount = Math.Max(0, analysis.LayerCount - 1);
            }

            resultsGrid.Refresh();
        }

        private void ApplyFilters()
        {
            var text = nameFilter.Text.Trim();
            var type = componentTypeFilter.SelectedItem as string;
            var status = statusFilter.SelectedItem as string;
            var filtered = analysisResults
                .Where(result => !activeOnly.Checked || result.HasActiveLayer == true)
                .Where(result =>
                    string.IsNullOrWhiteSpace(type)
                    || type == "All component types"
                    || result.ComponentTypeName == type
                )
                .Where(result => MatchesStatus(result, status))
                .Where(result =>
                    text.Length == 0
                    || Contains(result.ComponentName, text)
                    || Contains(result.ComponentTypeName, text)
                    || Contains(result.ComponentId.ToString("D"), text)
                )
                .ToList();

            resultsGrid.DataSource = new BindingList<LayerAnalysisResult>(filtered);
            visibleResultsLabel.Text =
                filtered.Count + " visible / " + analysisResults.Count + " processed";
        }

        private void PopulateFilterValues()
        {
            var selected = componentTypeFilter.SelectedItem as string;
            componentTypeFilter.Items.Clear();
            componentTypeFilter.Items.Add("All component types");
            componentTypeFilter.Items.AddRange(
                analysisResults
                    .Select(result => result.ComponentTypeName)
                    .Distinct()
                    .OrderBy(value => value)
                    .Cast<object>()
                    .ToArray()
            );
            componentTypeFilter.SelectedItem = componentTypeFilter.Items.Contains(selected)
                ? selected
                : "All component types";
        }

        private static bool MatchesStatus(LayerAnalysisResult result, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter) || filter == "All statuses")
            {
                return true;
            }

            if (filter == "Errors")
            {
                return !string.IsNullOrWhiteSpace(result.Error);
            }

            ComponentCorrelationStatus correlation;
            return Enum.TryParse(filter, out correlation)
                && result.CorrelationStatus == correlation;
        }

        private void ClearAnalysis()
        {
            analysisResults.Clear();
            displayedResults.Clear();
            latestMetrics = new AnalysisMetricsSnapshot();
            resultsGrid.DataSource = new BindingList<LayerAnalysisResult>();
            UpdateMetrics(latestMetrics);
            progressBar.Value = 0;
            visibleResultsLabel.Text = "0 visible / 0 processed";
            HideOperationSteps();
            UpdateActionState();
        }

        private void BeginOperationSteps(string title, IReadOnlyCollection<string> steps)
        {
            rootLayout.RowStyles[OperationStepsRowIndex].Height = OperationStepsRowHeight;
            operationSteps.Visible = true;
            operationSteps.BeginOperation(title, steps);
        }

        private void HideOperationSteps()
        {
            operationSteps.ResetSteps();
            operationSteps.Visible = false;
            rootLayout.RowStyles[OperationStepsRowIndex].Height = 0F;
        }

        private static int GetAnalysisStepIndex(AnalysisStage stage)
        {
            switch (stage)
            {
                case AnalysisStage.LoadingSourceComponents:
                    return 0;
                case AnalysisStage.LoadingTargetComponents:
                    return 1;
                case AnalysisStage.CorrelatingComponents:
                    return 2;
                case AnalysisStage.QueryingTargetLayers:
                    return 3;
                case AnalysisStage.FinalizingResults:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stage));
            }
        }

        private void UpdateMetrics(AnalysisMetricsSnapshot metrics)
        {
            processedMetric.Text =
                metrics.ProcessedComponents
                + " / "
                + Math.Max(
                    metrics.TotalSourceComponents,
                    metrics.TotalSourceComponents + metrics.MissingFromSourceSolution
                );
            activeMetric.Text = metrics.ActiveLayers.ToString();
            differenceMetric.Text = (
                metrics.MissingFromTargetSolution
                + metrics.MissingFromSourceSolution
                + metrics.MissingFromTargetEnvironment
            ).ToString();
            errorMetric.Text = metrics.Errors.ToString();
            elapsedMetric.Text = metrics.Elapsed.ToString("hh\\:mm\\:ss");
            batchMetric.Text =
                metrics.Batches
                + " batches / "
                + metrics.Requests
                + " requests / "
                + metrics.Retries
                + " retries / "
                + metrics.Throttlings
                + " throttles / "
                + metrics.Timeouts
                + " timeouts / avg "
                + metrics.AverageBatchDurationMs.ToString("0")
                + "ms";
        }

        private void UpdateConnectionLabels()
        {
            sourceEnvironmentValue.Text = Service == null ? "Not connected" : SourceEnvironmentName;
            targetEnvironmentValue.Text =
                targetConnectionDetail == null ? "Not connected" : TargetEnvironmentName;
        }

        private void UpdateSolutionDetails()
        {
            var source = SelectedSourceSolution;
            sourceSolutionValue.Text =
                source == null
                    ? "-"
                    : source.Version
                        + " | "
                        + (source.IsManaged ? "Managed" : "Unmanaged")
                        + " | "
                        + source.SolutionId.ToString("D");
            targetSolutionValue.Text =
                targetSolution == null
                    ? "Not found in Target"
                    : targetSolution.Version
                        + " | "
                        + (targetSolution.IsManaged ? "Managed" : "Unmanaged")
                        + " | "
                        + targetSolution.SolutionId.ToString("D");
        }

        private void SetOperationState(bool running, string message)
        {
            operationRunning = running;
            cancelButton.Enabled = running;
            if (!string.IsNullOrWhiteSpace(message))
            {
                statusLabel.Text = message;
            }

            UpdateActionState();
        }

        private void UpdateActionState()
        {
            connectTargetButton.Enabled = !operationRunning;
            disconnectTargetButton.Enabled = !operationRunning && targetConnectionDetail != null;
            loadSolutionsButton.Enabled =
                !operationRunning && Service != null && TargetService != null;
            sourceSolutions.Enabled = !operationRunning;
            analyzeButton.Enabled =
                !operationRunning
                && Service != null
                && TargetService != null
                && SelectedSourceSolution != null;
            exportButton.Enabled = !operationRunning && analysisResults.Count > 0;
            prepareRemovalButton.Enabled =
                !operationRunning && analysisResults.Any(result => result.HasActiveLayer == true);
        }

        private static int CalculateProgressPercentage(AnalysisMetricsSnapshot metrics)
        {
            if (metrics == null)
            {
                return 0;
            }

            var total = Math.Max(
                metrics.TotalSourceComponents,
                metrics.TotalSourceComponents + metrics.MissingFromSourceSolution
            );
            return total == 0
                ? 0
                : Math.Max(0, Math.Min(100, metrics.ProcessedComponents * 100 / total));
        }

        private string BuildRemovalSummary(IReadOnlyCollection<LayerAnalysisResult> selected)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Target: " + TargetEnvironmentName);
            builder.AppendLine();
            builder.AppendLine("Selected components: " + selected.Count);
            foreach (var group in selected.GroupBy(result => result.ComponentTypeName))
            {
                builder.AppendLine();
                builder.AppendLine(group.Key);
                foreach (var component in group.Take(25))
                {
                    builder.Append("- ");
                    builder.Append(component.ComponentName);
                    if (!component.ExistsInTargetSolution)
                    {
                        builder.Append(" (not in Target solution)");
                    }

                    builder.AppendLine();
                }
            }

            if (selected.Count > 25)
            {
                builder.AppendLine();
                builder.AppendLine("The list is abbreviated in this confirmation.");
            }

            return builder.ToString();
        }

        private void ShowError(Exception error, string title)
        {
            statusLabel.Text = title + ": " + error.Message;
            MessageBox.Show(this, error.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            LogError(error.ToString());
        }

        private static bool Contains(string value, string text)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private IOrganizationService TargetService
        {
            get
            {
                return targetConnectionDetail == null
                    ? null
                    : targetConnectionDetail.GetCrmServiceClient();
            }
        }

        private SolutionInfo SelectedSourceSolution
        {
            get { return sourceSolutions.GetSelectedItem<SolutionInfo>(); }
        }

        private string SourceEnvironmentName
        {
            get { return ConnectionDetail == null ? "Source" : ConnectionDetail.ConnectionName; }
        }

        private string TargetEnvironmentName
        {
            get
            {
                return targetConnectionDetail == null
                    ? "Target"
                    : targetConnectionDetail.ConnectionName;
            }
        }
    }
}
