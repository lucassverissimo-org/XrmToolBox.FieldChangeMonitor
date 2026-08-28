namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer
{
    partial class SolutionLayerAnalyzerControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.connectionLayout = new System.Windows.Forms.TableLayoutPanel();
            this.sourceEnvironmentCaption = new System.Windows.Forms.Label();
            this.sourceEnvironmentValue = new System.Windows.Forms.Label();
            this.targetEnvironmentCaption = new System.Windows.Forms.Label();
            this.targetEnvironmentValue = new System.Windows.Forms.Label();
            this.connectTargetButton = new System.Windows.Forms.Button();
            this.disconnectTargetButton = new System.Windows.Forms.Button();
            this.solutionCaption = new System.Windows.Forms.Label();
            this.sourceSolutions = new LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls.SolutionPickerControl();
            this.loadSolutionsButton = new System.Windows.Forms.Button();
            this.sourceSolutionCaption = new System.Windows.Forms.Label();
            this.sourceSolutionValue = new System.Windows.Forms.Label();
            this.targetSolutionCaption = new System.Windows.Forms.Label();
            this.targetSolutionValue = new System.Windows.Forms.Label();
            this.analyzeButton = new System.Windows.Forms.Button();
            this.operationSteps = new LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls.OperationStepsControl();
            this.metricsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.processedCaption = new System.Windows.Forms.Label();
            this.processedMetric = new System.Windows.Forms.Label();
            this.activeCaption = new System.Windows.Forms.Label();
            this.activeMetric = new System.Windows.Forms.Label();
            this.differenceCaption = new System.Windows.Forms.Label();
            this.differenceMetric = new System.Windows.Forms.Label();
            this.errorCaption = new System.Windows.Forms.Label();
            this.errorMetric = new System.Windows.Forms.Label();
            this.elapsedCaption = new System.Windows.Forms.Label();
            this.elapsedMetric = new System.Windows.Forms.Label();
            this.batchMetric = new System.Windows.Forms.Label();
            this.filterPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.activeOnly = new System.Windows.Forms.CheckBox();
            this.componentTypeFilter = new System.Windows.Forms.ComboBox();
            this.statusFilter = new System.Windows.Forms.ComboBox();
            this.nameFilterLabel = new System.Windows.Forms.Label();
            this.nameFilter = new System.Windows.Forms.TextBox();
            this.visibleResultsLabel = new System.Windows.Forms.Label();
            this.resultsGrid = new System.Windows.Forms.DataGridView();
            this.selectColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.activeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.componentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sourceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.targetSolutionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.targetEnvironmentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.correlationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.layerCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultStatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.actionPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.prepareRemovalButton = new System.Windows.Forms.Button();
            this.statusPanel = new System.Windows.Forms.TableLayoutPanel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.rootLayout.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.connectionLayout.SuspendLayout();
            this.metricsLayout.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).BeginInit();
            this.actionPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerPanel, 0, 0);
            this.rootLayout.Controls.Add(this.connectionLayout, 0, 1);
            this.rootLayout.Controls.Add(this.operationSteps, 0, 2);
            this.rootLayout.Controls.Add(this.metricsLayout, 0, 3);
            this.rootLayout.Controls.Add(this.filterPanel, 0, 4);
            this.rootLayout.Controls.Add(this.resultsGrid, 0, 5);
            this.rootLayout.Controls.Add(this.actionPanel, 0, 6);
            this.rootLayout.Controls.Add(this.statusPanel, 0, 7);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(14);
            this.rootLayout.RowCount = 8;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 158F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.rootLayout.Size = new System.Drawing.Size(1260, 760);
            this.rootLayout.TabIndex = 0;
            //
            // headerPanel
            //
            this.headerPanel.Controls.Add(this.subtitleLabel);
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerPanel.Location = new System.Drawing.Point(17, 17);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1226, 62);
            this.headerPanel.TabIndex = 0;
            //
            // subtitleLabel
            //
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.ForeColor = System.Drawing.Color.DimGray;
            this.subtitleLabel.Location = new System.Drawing.Point(2, 39);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(477, 15);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "Compare solution composition, inspect Active Layers, and remove selected layers safely.";
            //
            // titleLabel
            //
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(8, 91, 126);
            this.titleLabel.Location = new System.Drawing.Point(-1, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(279, 32);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Solution Layer Analyzer";
            //
            // connectionLayout
            //
            this.connectionLayout.BackColor = System.Drawing.Color.FromArgb(247, 249, 251);
            this.connectionLayout.ColumnCount = 5;
            this.connectionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.connectionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.connectionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.connectionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.connectionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.connectionLayout.Controls.Add(this.sourceEnvironmentCaption, 0, 0);
            this.connectionLayout.Controls.Add(this.sourceEnvironmentValue, 1, 0);
            this.connectionLayout.Controls.Add(this.targetEnvironmentCaption, 0, 1);
            this.connectionLayout.Controls.Add(this.targetEnvironmentValue, 1, 1);
            this.connectionLayout.Controls.Add(this.connectTargetButton, 2, 1);
            this.connectionLayout.Controls.Add(this.disconnectTargetButton, 3, 1);
            this.connectionLayout.Controls.Add(this.solutionCaption, 0, 2);
            this.connectionLayout.Controls.Add(this.sourceSolutions, 1, 2);
            this.connectionLayout.Controls.Add(this.loadSolutionsButton, 2, 2);
            this.connectionLayout.Controls.Add(this.analyzeButton, 4, 2);
            this.connectionLayout.Controls.Add(this.sourceSolutionCaption, 0, 3);
            this.connectionLayout.Controls.Add(this.sourceSolutionValue, 1, 3);
            this.connectionLayout.Controls.Add(this.targetSolutionCaption, 0, 4);
            this.connectionLayout.Controls.Add(this.targetSolutionValue, 1, 4);
            this.connectionLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectionLayout.Location = new System.Drawing.Point(17, 85);
            this.connectionLayout.Name = "connectionLayout";
            this.connectionLayout.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.connectionLayout.RowCount = 5;
            this.connectionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.connectionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.connectionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.connectionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.connectionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.connectionLayout.Size = new System.Drawing.Size(1226, 152);
            this.connectionLayout.TabIndex = 1;
            //
            // captions and values
            //
            this.sourceEnvironmentCaption.AutoSize = true;
            this.sourceEnvironmentCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceEnvironmentCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.sourceEnvironmentCaption.Text = "Source Environment";
            this.sourceEnvironmentCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.sourceEnvironmentValue.AutoEllipsis = true;
            this.sourceEnvironmentValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceEnvironmentValue.Text = "Not connected";
            this.sourceEnvironmentValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.targetEnvironmentCaption.AutoSize = true;
            this.targetEnvironmentCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetEnvironmentCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.targetEnvironmentCaption.Text = "Target Environment";
            this.targetEnvironmentCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.targetEnvironmentValue.AutoEllipsis = true;
            this.targetEnvironmentValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetEnvironmentValue.Text = "Not connected";
            this.targetEnvironmentValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.solutionCaption.AutoSize = true;
            this.solutionCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.solutionCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.solutionCaption.Text = "Source Solution";
            this.solutionCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.sourceSolutionCaption.AutoSize = true;
            this.sourceSolutionCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceSolutionCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.sourceSolutionCaption.Text = "Source details";
            this.sourceSolutionCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.sourceSolutionValue.AutoEllipsis = true;
            this.sourceSolutionValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectionLayout.SetColumnSpan(this.sourceSolutionValue, 4);
            this.sourceSolutionValue.Text = "-";
            this.sourceSolutionValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.targetSolutionCaption.AutoSize = true;
            this.targetSolutionCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetSolutionCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.targetSolutionCaption.Text = "Target details";
            this.targetSolutionCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.targetSolutionValue.AutoEllipsis = true;
            this.targetSolutionValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectionLayout.SetColumnSpan(this.targetSolutionValue, 4);
            this.targetSolutionValue.Text = "-";
            this.targetSolutionValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // connection buttons and selector
            //
            this.connectTargetButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectTargetButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.connectTargetButton.Text = "Connect Target";
            this.connectTargetButton.UseVisualStyleBackColor = true;
            this.connectTargetButton.Click += new System.EventHandler(this.ConnectTargetClick);
            this.disconnectTargetButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.disconnectTargetButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.disconnectTargetButton.Text = "Disconnect Target";
            this.disconnectTargetButton.UseVisualStyleBackColor = true;
            this.disconnectTargetButton.Click += new System.EventHandler(this.DisconnectTargetClick);
            this.sourceSolutions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceSolutions.Location = new System.Drawing.Point(158, 65);
            this.sourceSolutions.Name = "sourceSolutions";
            this.sourceSolutions.Size = new System.Drawing.Size(682, 28);
            this.sourceSolutions.TabIndex = 4;
            this.sourceSolutions.SelectedSolutionChanged += new System.EventHandler(this.SourceSolutionChanged);
            this.loadSolutionsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadSolutionsButton.Text = "Load Solutions";
            this.loadSolutionsButton.UseVisualStyleBackColor = true;
            this.loadSolutionsButton.Click += new System.EventHandler(this.LoadSolutionsClick);
            this.analyzeButton.BackColor = System.Drawing.Color.FromArgb(8, 127, 140);
            this.analyzeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analyzeButton.FlatAppearance.BorderSize = 0;
            this.analyzeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.analyzeButton.ForeColor = System.Drawing.Color.White;
            this.analyzeButton.Text = "Analyze";
            this.analyzeButton.UseVisualStyleBackColor = false;
            this.analyzeButton.Click += new System.EventHandler(this.AnalyzeClick);
            //
            // operationSteps
            //
            this.operationSteps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationSteps.Location = new System.Drawing.Point(17, 243);
            this.operationSteps.Name = "operationSteps";
            this.operationSteps.Size = new System.Drawing.Size(1226, 1);
            this.operationSteps.TabIndex = 2;
            this.operationSteps.Visible = false;
            //
            // metricsLayout
            //
            this.metricsLayout.ColumnCount = 6;
            this.metricsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.metricsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.metricsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.metricsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.metricsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.metricsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.metricsLayout.Controls.Add(this.processedCaption, 0, 0);
            this.metricsLayout.Controls.Add(this.activeCaption, 1, 0);
            this.metricsLayout.Controls.Add(this.differenceCaption, 2, 0);
            this.metricsLayout.Controls.Add(this.errorCaption, 3, 0);
            this.metricsLayout.Controls.Add(this.elapsedCaption, 4, 0);
            this.metricsLayout.Controls.Add(this.processedMetric, 0, 1);
            this.metricsLayout.Controls.Add(this.activeMetric, 1, 1);
            this.metricsLayout.Controls.Add(this.differenceMetric, 2, 1);
            this.metricsLayout.Controls.Add(this.errorMetric, 3, 1);
            this.metricsLayout.Controls.Add(this.elapsedMetric, 4, 1);
            this.metricsLayout.Controls.Add(this.batchMetric, 5, 1);
            this.metricsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metricsLayout.Location = new System.Drawing.Point(17, 243);
            this.metricsLayout.Name = "metricsLayout";
            this.metricsLayout.RowCount = 2;
            this.metricsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.metricsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.metricsLayout.Size = new System.Drawing.Size(1226, 70);
            this.metricsLayout.TabIndex = 2;
            this.processedCaption.Text = "Progress";
            this.activeCaption.Text = "Active Layers";
            this.differenceCaption.Text = "ALM Differences";
            this.errorCaption.Text = "Errors";
            this.elapsedCaption.Text = "Elapsed";
            this.processedMetric.Text = "0 / 0";
            this.activeMetric.Text = "0";
            this.differenceMetric.Text = "0";
            this.errorMetric.Text = "0";
            this.elapsedMetric.Text = "00:00:00";
            this.batchMetric.Text = "0 batches / 0 retries / 0 throttles";
            this.ConfigureMetricLabels();
            //
            // filterPanel
            //
            this.filterPanel.Controls.Add(this.activeOnly);
            this.filterPanel.Controls.Add(this.componentTypeFilter);
            this.filterPanel.Controls.Add(this.statusFilter);
            this.filterPanel.Controls.Add(this.nameFilterLabel);
            this.filterPanel.Controls.Add(this.nameFilter);
            this.filterPanel.Controls.Add(this.visibleResultsLabel);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterPanel.Location = new System.Drawing.Point(17, 319);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Padding = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.filterPanel.Size = new System.Drawing.Size(1226, 40);
            this.filterPanel.TabIndex = 3;
            this.activeOnly.AutoSize = true;
            this.activeOnly.Margin = new System.Windows.Forms.Padding(0, 6, 16, 0);
            this.activeOnly.Text = "Active Layers only";
            this.activeOnly.CheckedChanged += new System.EventHandler(this.FilterChanged);
            this.componentTypeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.componentTypeFilter.Items.AddRange(new object[] { "All component types" });
            this.componentTypeFilter.Margin = new System.Windows.Forms.Padding(0, 2, 10, 0);
            this.componentTypeFilter.Size = new System.Drawing.Size(185, 23);
            this.componentTypeFilter.SelectedIndexChanged += new System.EventHandler(this.FilterChanged);
            this.statusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.statusFilter.Items.AddRange(new object[] { "All statuses", "Matched", "MissingFromTargetSolution", "MissingFromSourceSolution", "MissingFromTargetEnvironment", "Errors" });
            this.statusFilter.Margin = new System.Windows.Forms.Padding(0, 2, 10, 0);
            this.statusFilter.Size = new System.Drawing.Size(220, 23);
            this.statusFilter.SelectedIndexChanged += new System.EventHandler(this.FilterChanged);
            this.nameFilterLabel.AutoSize = true;
            this.nameFilterLabel.Margin = new System.Windows.Forms.Padding(0, 6, 7, 0);
            this.nameFilterLabel.Text = "Name or ID";
            this.nameFilter.Margin = new System.Windows.Forms.Padding(0, 2, 10, 0);
            this.nameFilter.Size = new System.Drawing.Size(245, 23);
            this.nameFilter.TextChanged += new System.EventHandler(this.FilterChanged);
            this.visibleResultsLabel.AutoSize = true;
            this.visibleResultsLabel.Margin = new System.Windows.Forms.Padding(4, 6, 0, 0);
            this.visibleResultsLabel.Text = "0 visible / 0 processed";
            //
            // resultsGrid
            //
            this.resultsGrid.AllowUserToAddRows = false;
            this.resultsGrid.AllowUserToDeleteRows = false;
            this.resultsGrid.AllowUserToOrderColumns = true;
            this.resultsGrid.AutoGenerateColumns = false;
            this.resultsGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.resultsGrid.BackgroundColor = System.Drawing.Color.White;
            this.resultsGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.resultsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.selectColumn, this.activeColumn, this.typeColumn, this.componentColumn, this.sourceColumn, this.targetSolutionColumn, this.targetEnvironmentColumn, this.correlationColumn, this.layerCountColumn, this.resultStatusColumn, this.errorColumn });
            this.resultsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsGrid.Location = new System.Drawing.Point(17, 365);
            this.resultsGrid.Name = "resultsGrid";
            this.resultsGrid.RowHeadersVisible = false;
            this.resultsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.resultsGrid.Size = new System.Drawing.Size(1226, 280);
            this.resultsGrid.TabIndex = 4;
            this.resultsGrid.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.ResultsGridCellBeginEdit);
            this.resultsGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.ResultsGridCellFormatting);
            this.resultsGrid.CurrentCellDirtyStateChanged += new System.EventHandler(this.ResultsGridCurrentCellDirtyStateChanged);
            this.selectColumn.DataPropertyName = "Selected";
            this.selectColumn.HeaderText = "Select";
            this.selectColumn.Width = 48;
            this.activeColumn.DataPropertyName = "ActiveLayerDisplay";
            this.activeColumn.HeaderText = "Active";
            this.activeColumn.ReadOnly = true;
            this.activeColumn.Width = 55;
            this.typeColumn.DataPropertyName = "ComponentTypeName";
            this.typeColumn.HeaderText = "Type";
            this.typeColumn.ReadOnly = true;
            this.typeColumn.Width = 105;
            this.componentColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.componentColumn.DataPropertyName = "ComponentName";
            this.componentColumn.HeaderText = "Component";
            this.componentColumn.MinimumWidth = 160;
            this.componentColumn.ReadOnly = true;
            this.sourceColumn.DataPropertyName = "SourceSolutionDisplay";
            this.sourceColumn.HeaderText = "Source";
            this.sourceColumn.ReadOnly = true;
            this.sourceColumn.Width = 58;
            this.targetSolutionColumn.DataPropertyName = "TargetSolutionDisplay";
            this.targetSolutionColumn.HeaderText = "Target Solution";
            this.targetSolutionColumn.ReadOnly = true;
            this.targetSolutionColumn.Width = 92;
            this.targetEnvironmentColumn.DataPropertyName = "TargetEnvironmentDisplay";
            this.targetEnvironmentColumn.HeaderText = "Target Env";
            this.targetEnvironmentColumn.ReadOnly = true;
            this.targetEnvironmentColumn.Width = 78;
            this.correlationColumn.DataPropertyName = "CorrelationStatus";
            this.correlationColumn.HeaderText = "Correlation";
            this.correlationColumn.ReadOnly = true;
            this.correlationColumn.Width = 185;
            this.layerCountColumn.DataPropertyName = "LayerCount";
            this.layerCountColumn.HeaderText = "Active Layers";
            this.layerCountColumn.ReadOnly = true;
            this.layerCountColumn.Width = 55;
            this.resultStatusColumn.DataPropertyName = "Status";
            this.resultStatusColumn.HeaderText = "Status";
            this.resultStatusColumn.ReadOnly = true;
            this.resultStatusColumn.Width = 155;
            this.errorColumn.DataPropertyName = "Error";
            this.errorColumn.HeaderText = "Error";
            this.errorColumn.ReadOnly = true;
            this.errorColumn.Width = 220;
            //
            // actionPanel
            //
            this.actionPanel.Controls.Add(this.cancelButton);
            this.actionPanel.Controls.Add(this.exportButton);
            this.actionPanel.Controls.Add(this.prepareRemovalButton);
            this.actionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.actionPanel.Location = new System.Drawing.Point(17, 651);
            this.actionPanel.Name = "actionPanel";
            this.actionPanel.Padding = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.actionPanel.Size = new System.Drawing.Size(1226, 42);
            this.actionPanel.TabIndex = 5;
            this.cancelButton.Enabled = false;
            this.cancelButton.Size = new System.Drawing.Size(98, 30);
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelClick);
            this.exportButton.Size = new System.Drawing.Size(98, 30);
            this.exportButton.Text = "Export CSV";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportClick);
            this.prepareRemovalButton.BackColor = System.Drawing.Color.FromArgb(176, 94, 0);
            this.prepareRemovalButton.FlatAppearance.BorderSize = 0;
            this.prepareRemovalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.prepareRemovalButton.ForeColor = System.Drawing.Color.White;
            this.prepareRemovalButton.Size = new System.Drawing.Size(148, 30);
            this.prepareRemovalButton.Text = "Prepare Removal";
            this.prepareRemovalButton.UseVisualStyleBackColor = false;
            this.prepareRemovalButton.Click += new System.EventHandler(this.PrepareRemovalClick);
            //
            // statusPanel
            //
            this.statusPanel.ColumnCount = 1;
            this.statusPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusPanel.Controls.Add(this.statusLabel, 0, 0);
            this.statusPanel.Controls.Add(this.progressBar, 0, 1);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusPanel.Location = new System.Drawing.Point(17, 699);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.RowCount = 2;
            this.statusPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.statusPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.statusPanel.Size = new System.Drawing.Size(1226, 44);
            this.statusPanel.TabIndex = 6;
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.Text = "Connect the Source environment to begin.";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            //
            // SolutionLayerAnalyzerControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "SolutionLayerAnalyzerControl";
            this.Size = new System.Drawing.Size(1260, 760);
            this.rootLayout.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.connectionLayout.ResumeLayout(false);
            this.connectionLayout.PerformLayout();
            this.metricsLayout.ResumeLayout(false);
            this.metricsLayout.PerformLayout();
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).EndInit();
            this.actionPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void ConfigureMetricLabels()
        {
            foreach (System.Windows.Forms.Label caption in new[] { this.processedCaption, this.activeCaption, this.differenceCaption, this.errorCaption, this.elapsedCaption })
            {
                caption.Dock = System.Windows.Forms.DockStyle.Fill;
                caption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                caption.ForeColor = System.Drawing.Color.DimGray;
                caption.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            }

            foreach (System.Windows.Forms.Label value in new[] { this.processedMetric, this.activeMetric, this.differenceMetric, this.errorMetric, this.elapsedMetric, this.batchMetric })
            {
                value.Dock = System.Windows.Forms.DockStyle.Fill;
                value.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
                value.ForeColor = System.Drawing.Color.FromArgb(35, 94, 139);
                value.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            }
        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.TableLayoutPanel connectionLayout;
        private System.Windows.Forms.Label sourceEnvironmentCaption;
        private System.Windows.Forms.Label sourceEnvironmentValue;
        private System.Windows.Forms.Label targetEnvironmentCaption;
        private System.Windows.Forms.Label targetEnvironmentValue;
        private System.Windows.Forms.Button connectTargetButton;
        private System.Windows.Forms.Button disconnectTargetButton;
        private System.Windows.Forms.Label solutionCaption;
        private LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls.SolutionPickerControl sourceSolutions;
        private System.Windows.Forms.Button loadSolutionsButton;
        private System.Windows.Forms.Label sourceSolutionCaption;
        private System.Windows.Forms.Label sourceSolutionValue;
        private System.Windows.Forms.Label targetSolutionCaption;
        private System.Windows.Forms.Label targetSolutionValue;
        private System.Windows.Forms.Button analyzeButton;
        private LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls.OperationStepsControl operationSteps;
        private System.Windows.Forms.TableLayoutPanel metricsLayout;
        private System.Windows.Forms.Label processedCaption;
        private System.Windows.Forms.Label processedMetric;
        private System.Windows.Forms.Label activeCaption;
        private System.Windows.Forms.Label activeMetric;
        private System.Windows.Forms.Label differenceCaption;
        private System.Windows.Forms.Label differenceMetric;
        private System.Windows.Forms.Label errorCaption;
        private System.Windows.Forms.Label errorMetric;
        private System.Windows.Forms.Label elapsedCaption;
        private System.Windows.Forms.Label elapsedMetric;
        private System.Windows.Forms.Label batchMetric;
        private System.Windows.Forms.FlowLayoutPanel filterPanel;
        private System.Windows.Forms.CheckBox activeOnly;
        private System.Windows.Forms.ComboBox componentTypeFilter;
        private System.Windows.Forms.ComboBox statusFilter;
        private System.Windows.Forms.Label nameFilterLabel;
        private System.Windows.Forms.TextBox nameFilter;
        private System.Windows.Forms.Label visibleResultsLabel;
        private System.Windows.Forms.DataGridView resultsGrid;
        private System.Windows.Forms.DataGridViewCheckBoxColumn selectColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn activeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn componentColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sourceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn targetSolutionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn targetEnvironmentColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn correlationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn layerCountColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultStatusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn errorColumn;
        private System.Windows.Forms.FlowLayoutPanel actionPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button prepareRemovalButton;
        private System.Windows.Forms.TableLayoutPanel statusPanel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
