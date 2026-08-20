namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer
{
    partial class UsageExplorerControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                cancellation?.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.criteriaLayout = new System.Windows.Forms.TableLayoutPanel();
            this.searchModePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.searchByLabel = new System.Windows.Forms.Label();
            this.byTable = new System.Windows.Forms.RadioButton();
            this.byColumn = new System.Windows.Forms.RadioButton();
            this.tableStep = new System.Windows.Forms.GroupBox();
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.tableHintLabel = new System.Windows.Forms.Label();
            this.tables = new LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Controls.SearchableMetadataComboBox();
            this.loadTables = new System.Windows.Forms.Button();
            this.selectedTableInfo = new System.Windows.Forms.Label();
            this.stepArrowLabel = new System.Windows.Forms.Label();
            this.columnStep = new System.Windows.Forms.GroupBox();
            this.columnLayout = new System.Windows.Forms.TableLayoutPanel();
            this.columnHintLabel = new System.Windows.Forms.Label();
            this.columns = new LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Controls.SearchableMetadataComboBox();
            this.selectedColumnInfo = new System.Windows.Forms.Label();
            this.searchAreaLayout = new System.Windows.Forms.TableLayoutPanel();
            this.searchInLabel = new System.Windows.Forms.Label();
            this.scannerList = new System.Windows.Forms.CheckedListBox();
            this.actionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.scan = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.resultPanel = new System.Windows.Forms.TableLayoutPanel();
            this.filtersPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.filterLabel = new System.Windows.Forms.Label();
            this.componentFilter = new System.Windows.Forms.ComboBox();
            this.search = new System.Windows.Forms.TextBox();
            this.openComponent = new System.Windows.Forms.Button();
            this.summary = new System.Windows.Forms.Label();
            this.grid = new System.Windows.Forms.DataGridView();
            this.componentTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.referenceTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.foundInColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.confidenceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modifiedOnColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.Label();
            this.rootLayout.SuspendLayout();
            this.criteriaLayout.SuspendLayout();
            this.searchModePanel.SuspendLayout();
            this.tableStep.SuspendLayout();
            this.tableLayout.SuspendLayout();
            this.columnStep.SuspendLayout();
            this.columnLayout.SuspendLayout();
            this.searchAreaLayout.SuspendLayout();
            this.actionsPanel.SuspendLayout();
            this.resultPanel.SuspendLayout();
            this.filtersPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.titleLabel, 0, 0);
            this.rootLayout.Controls.Add(this.criteriaLayout, 0, 1);
            this.rootLayout.Controls.Add(this.resultPanel, 0, 2);
            this.rootLayout.Controls.Add(this.status, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(12);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.rootLayout.Size = new System.Drawing.Size(1200, 720);
            this.rootLayout.TabIndex = 0;
            //
            // titleLabel
            //
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(35, 62, 93);
            this.titleLabel.Location = new System.Drawing.Point(12, 12);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(273, 30);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Dataverse Usage Explorer";
            //
            // criteriaLayout
            //
            this.criteriaLayout.AutoSize = true;
            this.criteriaLayout.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.criteriaLayout.ColumnCount = 3;
            this.criteriaLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.criteriaLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.criteriaLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.criteriaLayout.Controls.Add(this.searchModePanel, 0, 0);
            this.criteriaLayout.Controls.Add(this.tableStep, 0, 1);
            this.criteriaLayout.Controls.Add(this.stepArrowLabel, 1, 1);
            this.criteriaLayout.Controls.Add(this.columnStep, 2, 1);
            this.criteriaLayout.Controls.Add(this.searchAreaLayout, 0, 2);
            this.criteriaLayout.Controls.Add(this.actionsPanel, 2, 2);
            this.criteriaLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.criteriaLayout.Location = new System.Drawing.Point(15, 55);
            this.criteriaLayout.Name = "criteriaLayout";
            this.criteriaLayout.Padding = new System.Windows.Forms.Padding(10);
            this.criteriaLayout.RowCount = 3;
            this.criteriaLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.criteriaLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.criteriaLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.criteriaLayout.Size = new System.Drawing.Size(1170, 237);
            this.criteriaLayout.TabIndex = 1;
            this.criteriaLayout.SetColumnSpan(this.searchModePanel, 3);
            this.criteriaLayout.SetColumnSpan(this.searchAreaLayout, 2);
            //
            // searchModePanel
            //
            this.searchModePanel.AutoSize = true;
            this.searchModePanel.Controls.Add(this.searchByLabel);
            this.searchModePanel.Controls.Add(this.byTable);
            this.searchModePanel.Controls.Add(this.byColumn);
            this.searchModePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchModePanel.Location = new System.Drawing.Point(10, 10);
            this.searchModePanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.searchModePanel.Name = "searchModePanel";
            this.searchModePanel.Size = new System.Drawing.Size(1150, 27);
            this.searchModePanel.TabIndex = 0;
            //
            // searchByLabel
            //
            this.searchByLabel.AutoSize = true;
            this.searchByLabel.Padding = new System.Windows.Forms.Padding(0, 5, 4, 0);
            this.searchByLabel.Size = new System.Drawing.Size(66, 20);
            this.searchByLabel.Text = "Search by:";
            //
            // byTable
            //
            this.byTable.AutoSize = true;
            this.byTable.Checked = true;
            this.byTable.Name = "byTable";
            this.byTable.Size = new System.Drawing.Size(52, 19);
            this.byTable.TabIndex = 0;
            this.byTable.TabStop = true;
            this.byTable.Text = "Table";
            this.byTable.UseVisualStyleBackColor = true;
            this.byTable.CheckedChanged += new System.EventHandler(this.ByTableCheckedChanged);
            //
            // byColumn
            //
            this.byColumn.AutoSize = true;
            this.byColumn.Name = "byColumn";
            this.byColumn.Size = new System.Drawing.Size(67, 19);
            this.byColumn.TabIndex = 1;
            this.byColumn.Text = "Column";
            this.byColumn.UseVisualStyleBackColor = true;
            this.byColumn.CheckedChanged += new System.EventHandler(this.ByColumnCheckedChanged);
            //
            // tableStep
            //
            this.tableStep.AutoSize = true;
            this.tableStep.Controls.Add(this.tableLayout);
            this.tableStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableStep.Location = new System.Drawing.Point(13, 48);
            this.tableStep.Name = "tableStep";
            this.tableStep.Padding = new System.Windows.Forms.Padding(10);
            this.tableStep.Size = new System.Drawing.Size(548, 102);
            this.tableStep.TabIndex = 1;
            this.tableStep.TabStop = false;
            this.tableStep.Text = "1. Choose the table";
            //
            // tableLayout
            //
            this.tableLayout.AutoSize = true;
            this.tableLayout.ColumnCount = 2;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableLayout.Controls.Add(this.tableHintLabel, 0, 0);
            this.tableLayout.Controls.Add(this.tables, 0, 1);
            this.tableLayout.Controls.Add(this.loadTables, 1, 1);
            this.tableLayout.Controls.Add(this.selectedTableInfo, 0, 2);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(10, 26);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 3;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableLayout.Size = new System.Drawing.Size(528, 66);
            this.tableLayout.TabIndex = 0;
            this.tableLayout.SetColumnSpan(this.tableHintLabel, 2);
            this.tableLayout.SetColumnSpan(this.selectedTableInfo, 2);
            //
            // tableHintLabel
            //
            this.tableHintLabel.AutoSize = true;
            this.tableHintLabel.ForeColor = System.Drawing.Color.DimGray;
            this.tableHintLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.tableHintLabel.Text = "Open the list or type part of the display or logical name";
            //
            // tables
            //
            this.tables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tables.Name = "tables";
            this.tables.Size = new System.Drawing.Size(437, 23);
            this.tables.TabIndex = 0;
            this.tables.SelectedIndexChanged += new System.EventHandler(this.TablesSelectedIndexChanged);
            //
            // loadTables
            //
            this.loadTables.AutoSize = true;
            this.loadTables.Enabled = false;
            this.loadTables.Name = "loadTables";
            this.loadTables.Size = new System.Drawing.Size(85, 25);
            this.loadTables.TabIndex = 1;
            this.loadTables.Text = "Load tables";
            this.loadTables.UseVisualStyleBackColor = true;
            this.loadTables.Click += new System.EventHandler(this.LoadTablesClick);
            //
            // selectedTableInfo
            //
            this.selectedTableInfo.AutoSize = true;
            this.selectedTableInfo.ForeColor = System.Drawing.Color.DimGray;
            this.selectedTableInfo.Padding = new System.Windows.Forms.Padding(4, 7, 0, 2);
            this.selectedTableInfo.Text = "No table selected";
            //
            // stepArrowLabel
            //
            this.stepArrowLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.stepArrowLabel.AutoSize = true;
            this.stepArrowLabel.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.stepArrowLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.stepArrowLabel.Text = "➜";
            //
            // columnStep
            //
            this.columnStep.AutoSize = true;
            this.columnStep.Controls.Add(this.columnLayout);
            this.columnStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnStep.Enabled = false;
            this.columnStep.Location = new System.Drawing.Point(609, 48);
            this.columnStep.Name = "columnStep";
            this.columnStep.Padding = new System.Windows.Forms.Padding(10);
            this.columnStep.Size = new System.Drawing.Size(548, 102);
            this.columnStep.TabIndex = 2;
            this.columnStep.TabStop = false;
            this.columnStep.Text = "2. Choose a column from this table";
            //
            // columnLayout
            //
            this.columnLayout.AutoSize = true;
            this.columnLayout.ColumnCount = 1;
            this.columnLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.columnLayout.Controls.Add(this.columnHintLabel, 0, 0);
            this.columnLayout.Controls.Add(this.columns, 0, 1);
            this.columnLayout.Controls.Add(this.selectedColumnInfo, 0, 2);
            this.columnLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnLayout.Location = new System.Drawing.Point(10, 26);
            this.columnLayout.Name = "columnLayout";
            this.columnLayout.RowCount = 3;
            this.columnLayout.Size = new System.Drawing.Size(528, 66);
            this.columnLayout.TabIndex = 0;
            //
            // columnHintLabel
            //
            this.columnHintLabel.AutoSize = true;
            this.columnHintLabel.ForeColor = System.Drawing.Color.DimGray;
            this.columnHintLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.columnHintLabel.Text = "The list is limited to columns from the selected table";
            //
            // columns
            //
            this.columns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columns.Name = "columns";
            this.columns.Size = new System.Drawing.Size(528, 23);
            this.columns.TabIndex = 0;
            this.columns.SelectedIndexChanged += new System.EventHandler(this.ColumnsSelectedIndexChanged);
            //
            // selectedColumnInfo
            //
            this.selectedColumnInfo.AutoSize = true;
            this.selectedColumnInfo.ForeColor = System.Drawing.Color.DimGray;
            this.selectedColumnInfo.Padding = new System.Windows.Forms.Padding(4, 7, 0, 2);
            this.selectedColumnInfo.Text = "No column selected";
            //
            // searchAreaLayout
            //
            this.searchAreaLayout.AutoSize = true;
            this.searchAreaLayout.ColumnCount = 2;
            this.searchAreaLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.searchAreaLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.searchAreaLayout.Controls.Add(this.searchInLabel, 0, 0);
            this.searchAreaLayout.Controls.Add(this.scannerList, 1, 0);
            this.searchAreaLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchAreaLayout.Location = new System.Drawing.Point(10, 163);
            this.searchAreaLayout.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.searchAreaLayout.Name = "searchAreaLayout";
            this.searchAreaLayout.RowCount = 1;
            this.searchAreaLayout.Size = new System.Drawing.Size(596, 64);
            this.searchAreaLayout.TabIndex = 3;
            //
            // searchInLabel
            //
            this.searchInLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.searchInLabel.AutoSize = true;
            this.searchInLabel.Margin = new System.Windows.Forms.Padding(0, 5, 10, 0);
            this.searchInLabel.Text = "Search in:";
            //
            // scannerList
            //
            this.scannerList.CheckOnClick = true;
            this.scannerList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerList.FormattingEnabled = true;
            this.scannerList.Items.AddRange(new object[] { "Business Rules", "Power Automate", "Classic Workflows", "Business Process Flows", "Forms", "Views", "Plugin Steps" });
            this.scannerList.SetItemChecked(0, true);
            this.scannerList.SetItemChecked(1, true);
            this.scannerList.SetItemChecked(2, true);
            this.scannerList.SetItemChecked(3, true);
            this.scannerList.SetItemChecked(4, true);
            this.scannerList.SetItemChecked(5, true);
            this.scannerList.SetItemChecked(6, true);
            this.scannerList.Name = "scannerList";
            this.scannerList.Size = new System.Drawing.Size(530, 64);
            this.scannerList.TabIndex = 0;
            //
            // actionsPanel
            //
            this.actionsPanel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.actionsPanel.AutoSize = true;
            this.actionsPanel.Controls.Add(this.scan);
            this.actionsPanel.Controls.Add(this.cancel);
            this.actionsPanel.Location = new System.Drawing.Point(961, 175);
            this.actionsPanel.Margin = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.actionsPanel.Name = "actionsPanel";
            this.actionsPanel.Size = new System.Drawing.Size(196, 38);
            this.actionsPanel.TabIndex = 4;
            //
            // scan
            //
            this.scan.Enabled = false;
            this.scan.Name = "scan";
            this.scan.Size = new System.Drawing.Size(90, 32);
            this.scan.TabIndex = 0;
            this.scan.Text = "Scan";
            this.scan.UseVisualStyleBackColor = true;
            this.scan.Click += new System.EventHandler(this.ScanClick);
            //
            // cancel
            //
            this.cancel.Enabled = false;
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(90, 32);
            this.cancel.TabIndex = 1;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.CancelClick);
            //
            // resultPanel
            //
            this.resultPanel.ColumnCount = 1;
            this.resultPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.resultPanel.Controls.Add(this.filtersPanel, 0, 0);
            this.resultPanel.Controls.Add(this.grid, 0, 1);
            this.resultPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultPanel.Location = new System.Drawing.Point(12, 302);
            this.resultPanel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.resultPanel.Name = "resultPanel";
            this.resultPanel.RowCount = 2;
            this.resultPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.resultPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.resultPanel.Size = new System.Drawing.Size(1176, 385);
            this.resultPanel.TabIndex = 2;
            //
            // filtersPanel
            //
            this.filtersPanel.AutoSize = true;
            this.filtersPanel.Controls.Add(this.filterLabel);
            this.filtersPanel.Controls.Add(this.componentFilter);
            this.filtersPanel.Controls.Add(this.search);
            this.filtersPanel.Controls.Add(this.openComponent);
            this.filtersPanel.Controls.Add(this.summary);
            this.filtersPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filtersPanel.Name = "filtersPanel";
            this.filtersPanel.Size = new System.Drawing.Size(1176, 31);
            this.filtersPanel.TabIndex = 0;
            //
            // filterLabel
            //
            this.filterLabel.AutoSize = true;
            this.filterLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.filterLabel.Text = "Filter:";
            //
            // componentFilter
            //
            this.componentFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.componentFilter.FormattingEnabled = true;
            this.componentFilter.Items.AddRange(new object[] { "All component types" });
            this.componentFilter.Name = "componentFilter";
            this.componentFilter.Size = new System.Drawing.Size(160, 23);
            this.componentFilter.TabIndex = 0;
            this.componentFilter.SelectedIndexChanged += new System.EventHandler(this.ComponentFilterSelectedIndexChanged);
            //
            // search
            //
            this.search.Name = "search";
            this.search.Size = new System.Drawing.Size(220, 23);
            this.search.TabIndex = 1;
            this.search.TextChanged += new System.EventHandler(this.SearchTextChanged);
            //
            // openComponent
            //
            this.openComponent.AutoSize = true;
            this.openComponent.Enabled = false;
            this.openComponent.Name = "openComponent";
            this.openComponent.Size = new System.Drawing.Size(112, 25);
            this.openComponent.TabIndex = 2;
            this.openComponent.Text = "Open Component";
            this.openComponent.UseVisualStyleBackColor = true;
            this.openComponent.Click += new System.EventHandler(this.OpenComponentClick);
            //
            // summary
            //
            this.summary.AutoSize = true;
            this.summary.Padding = new System.Windows.Forms.Padding(12, 5, 0, 0);
            //
            // grid
            //
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AutoGenerateColumns = false;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.componentTypeColumn, this.nameColumn, this.tableColumn, this.statusColumn, this.referenceTypeColumn, this.foundInColumn, this.confidenceColumn, this.modifiedOnColumn, this.idColumn });
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(1176, 354);
            this.grid.TabIndex = 1;
            this.grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridCellDoubleClick);
            this.grid.SelectionChanged += new System.EventHandler(this.GridSelectionChanged);
            //
            // grid columns
            //
            this.componentTypeColumn.DataPropertyName = "ComponentType";
            this.componentTypeColumn.HeaderText = "Component Type";
            this.componentTypeColumn.Name = "componentTypeColumn";
            this.componentTypeColumn.ReadOnly = true;
            this.componentTypeColumn.Width = 120;
            this.nameColumn.DataPropertyName = "Name";
            this.nameColumn.HeaderText = "Name";
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.ReadOnly = true;
            this.nameColumn.Width = 190;
            this.tableColumn.DataPropertyName = "TableLogicalName";
            this.tableColumn.HeaderText = "Table";
            this.tableColumn.Name = "tableColumn";
            this.tableColumn.ReadOnly = true;
            this.tableColumn.Width = 140;
            this.statusColumn.DataPropertyName = "Status";
            this.statusColumn.HeaderText = "Status";
            this.statusColumn.Name = "statusColumn";
            this.statusColumn.ReadOnly = true;
            this.statusColumn.Width = 90;
            this.referenceTypeColumn.DataPropertyName = "ReferenceType";
            this.referenceTypeColumn.HeaderText = "Reference Type";
            this.referenceTypeColumn.Name = "referenceTypeColumn";
            this.referenceTypeColumn.ReadOnly = true;
            this.referenceTypeColumn.Width = 145;
            this.foundInColumn.DataPropertyName = "FoundIn";
            this.foundInColumn.HeaderText = "Found In";
            this.foundInColumn.Name = "foundInColumn";
            this.foundInColumn.ReadOnly = true;
            this.foundInColumn.Width = 180;
            this.confidenceColumn.DataPropertyName = "Confidence";
            this.confidenceColumn.HeaderText = "Confidence";
            this.confidenceColumn.Name = "confidenceColumn";
            this.confidenceColumn.ReadOnly = true;
            this.confidenceColumn.Width = 90;
            this.modifiedOnColumn.DataPropertyName = "ModifiedOn";
            this.modifiedOnColumn.HeaderText = "Modified On";
            this.modifiedOnColumn.Name = "modifiedOnColumn";
            this.modifiedOnColumn.ReadOnly = true;
            this.modifiedOnColumn.Width = 120;
            this.idColumn.DataPropertyName = "ComponentId";
            this.idColumn.HeaderText = "Id";
            this.idColumn.Name = "idColumn";
            this.idColumn.ReadOnly = true;
            this.idColumn.Width = 220;
            //
            // status
            //
            this.status.AutoSize = true;
            this.status.ForeColor = System.Drawing.Color.DimGray;
            this.status.Text = "Connect to Dataverse to begin.";
            //
            // UsageExplorerControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Name = "UsageExplorerControl";
            this.Size = new System.Drawing.Size(1200, 720);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.criteriaLayout.ResumeLayout(false);
            this.criteriaLayout.PerformLayout();
            this.searchModePanel.ResumeLayout(false);
            this.searchModePanel.PerformLayout();
            this.tableStep.ResumeLayout(false);
            this.tableStep.PerformLayout();
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            this.columnStep.ResumeLayout(false);
            this.columnStep.PerformLayout();
            this.columnLayout.ResumeLayout(false);
            this.columnLayout.PerformLayout();
            this.searchAreaLayout.ResumeLayout(false);
            this.searchAreaLayout.PerformLayout();
            this.actionsPanel.ResumeLayout(false);
            this.resultPanel.ResumeLayout(false);
            this.resultPanel.PerformLayout();
            this.filtersPanel.ResumeLayout(false);
            this.filtersPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.TableLayoutPanel criteriaLayout;
        private System.Windows.Forms.FlowLayoutPanel searchModePanel;
        private System.Windows.Forms.Label searchByLabel;
        private System.Windows.Forms.RadioButton byTable;
        private System.Windows.Forms.RadioButton byColumn;
        private System.Windows.Forms.GroupBox tableStep;
        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Label tableHintLabel;
        private Controls.SearchableMetadataComboBox tables;
        private System.Windows.Forms.Button loadTables;
        private System.Windows.Forms.Label selectedTableInfo;
        private System.Windows.Forms.Label stepArrowLabel;
        private System.Windows.Forms.GroupBox columnStep;
        private System.Windows.Forms.TableLayoutPanel columnLayout;
        private System.Windows.Forms.Label columnHintLabel;
        private Controls.SearchableMetadataComboBox columns;
        private System.Windows.Forms.Label selectedColumnInfo;
        private System.Windows.Forms.TableLayoutPanel searchAreaLayout;
        private System.Windows.Forms.Label searchInLabel;
        private System.Windows.Forms.CheckedListBox scannerList;
        private System.Windows.Forms.FlowLayoutPanel actionsPanel;
        private System.Windows.Forms.Button scan;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.TableLayoutPanel resultPanel;
        private System.Windows.Forms.FlowLayoutPanel filtersPanel;
        private System.Windows.Forms.Label filterLabel;
        private System.Windows.Forms.ComboBox componentFilter;
        private System.Windows.Forms.TextBox search;
        private System.Windows.Forms.Button openComponent;
        private System.Windows.Forms.Label summary;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn componentTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tableColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn referenceTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn foundInColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn confidenceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn modifiedOnColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn idColumn;
        private System.Windows.Forms.Label status;
    }
}
