namespace XrmTool_bravo
{
    partial class MyPluginControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tslTitle = new System.Windows.Forms.ToolStripLabel();
            this.tslSubtitle = new System.Windows.Forms.ToolStripLabel();
            this.tslSpring = new System.Windows.Forms.ToolStripLabel();
            this.tslConnection = new System.Windows.Forms.ToolStripLabel();
            this.tslActiveMonitors = new System.Windows.Forms.ToolStripLabel();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.contentLayout = new System.Windows.Forms.TableLayoutPanel();
            this.configurationGroup = new System.Windows.Forms.GroupBox();
            this.configurationLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblEntity = new System.Windows.Forms.Label();
            this.txtEntityLogicalName = new System.Windows.Forms.TextBox();
            this.btnLoadColumns = new System.Windows.Forms.Button();
            this.lblInterval = new System.Windows.Forms.Label();
            this.nudIntervalSeconds = new System.Windows.Forms.NumericUpDown();
            this.columnsGroup = new System.Windows.Forms.GroupBox();
            this.columnsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.columnSearchLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblColumnSearch = new System.Windows.Forms.Label();
            this.txtColumnSearch = new System.Windows.Forms.TextBox();
            this.columnsButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSelectAllColumns = new System.Windows.Forms.Button();
            this.btnClearColumnSelection = new System.Windows.Forms.Button();
            this.lblSelectedCount = new System.Windows.Forms.Label();
            this.clbColumns = new System.Windows.Forms.CheckedListBox();
            this.filterGroup = new System.Windows.Forms.GroupBox();
            this.filterLayout = new System.Windows.Forms.TableLayoutPanel();
            this.conditionBuilderGroup = new System.Windows.Forms.GroupBox();
            this.conditionBuilderLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblFilterType = new System.Windows.Forms.Label();
            this.cboFilterType = new System.Windows.Forms.ComboBox();
            this.lblConditionFieldSearch = new System.Windows.Forms.Label();
            this.txtConditionFieldSearch = new System.Windows.Forms.TextBox();
            this.lblConditionField = new System.Windows.Forms.Label();
            this.cboConditionAttribute = new System.Windows.Forms.ComboBox();
            this.lblConditionOperator = new System.Windows.Forms.Label();
            this.cboConditionOperator = new System.Windows.Forms.ComboBox();
            this.lblConditionValue = new System.Windows.Forms.Label();
            this.txtConditionValue = new System.Windows.Forms.TextBox();
            this.btnPickConditionValue = new System.Windows.Forms.Button();
            this.conditionButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddCondition = new System.Windows.Forms.Button();
            this.btnRemoveCondition = new System.Windows.Forms.Button();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.lblConditionValueHint = new System.Windows.Forms.Label();
            this.lblFilterHint = new System.Windows.Forms.Label();
            this.lvConditions = new System.Windows.Forms.ListView();
            this.colConditionField = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colConditionOperator = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colConditionValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnToggleAdvanced = new System.Windows.Forms.Button();
            this.advancedButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSaveFilterXml = new System.Windows.Forms.Button();
            this.txtFilterXml = new System.Windows.Forms.TextBox();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblConfigurationReady = new System.Windows.Forms.Label();
            this.lblConfigurationSummary = new System.Windows.Forms.Label();
            this.lblMonitorName = new System.Windows.Forms.Label();
            this.txtMonitorName = new System.Windows.Forms.TextBox();
            this.btnCancelEdit = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.bottomLayout = new System.Windows.Forms.TableLayoutPanel();
            this.activeMonitorsGroup = new System.Windows.Forms.GroupBox();
            this.activeMonitorsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.activeMonitorButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnStopSelectedMonitor = new System.Windows.Forms.Button();
            this.btnRemoveSelectedMonitors = new System.Windows.Forms.Button();
            this.btnPauseSelectedMonitors = new System.Windows.Forms.Button();
            this.btnSelectAllMonitors = new System.Windows.Forms.Button();
            this.btnExportMonitors = new System.Windows.Forms.Button();
            this.btnImportMonitors = new System.Windows.Forms.Button();
            this.btnEditMonitor = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lvActiveMonitors = new System.Windows.Forms.ListView();
            this.colActiveEntity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActiveName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActiveColumns = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActiveInterval = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActiveStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActiveFilter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eventsGroup = new System.Windows.Forms.GroupBox();
            this.eventsTabs = new System.Windows.Forms.TabControl();
            this.recentChangesTab = new System.Windows.Forms.TabPage();
            this.technicalLogTab = new System.Windows.Forms.TabPage();
            this.lvRecentChanges = new System.Windows.Forms.ListView();
            this.colChangeModifiedOn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChangeRecordId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChangeModifiedBy = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChangeRecordName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChangeField = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChangeValues = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChangeMonitor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstEvents = new System.Windows.Forms.ListBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolStripMenu.SuspendLayout();
            this.mainLayout.SuspendLayout();
            this.contentLayout.SuspendLayout();
            this.configurationGroup.SuspendLayout();
            this.configurationLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalSeconds)).BeginInit();
            this.columnsGroup.SuspendLayout();
            this.columnsLayout.SuspendLayout();
            this.columnSearchLayout.SuspendLayout();
            this.columnsButtonPanel.SuspendLayout();
            this.filterGroup.SuspendLayout();
            this.filterLayout.SuspendLayout();
            this.conditionBuilderGroup.SuspendLayout();
            this.conditionBuilderLayout.SuspendLayout();
            this.conditionButtonPanel.SuspendLayout();
            this.advancedButtonsPanel.SuspendLayout();
            this.summaryPanel.SuspendLayout();
            this.bottomLayout.SuspendLayout();
            this.activeMonitorsGroup.SuspendLayout();
            this.activeMonitorsLayout.SuspendLayout();
            this.activeMonitorButtonPanel.SuspendLayout();
            this.eventsGroup.SuspendLayout();
            this.eventsTabs.SuspendLayout();
            this.recentChangesTab.SuspendLayout();
            this.technicalLogTab.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.AutoSize = false;
            this.toolStripMenu.BackColor = System.Drawing.Color.White;
            this.toolStripMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslTitle,
            this.tslSubtitle,
            this.tslSpring,
            this.tslConnection,
            this.tslActiveMonitors,
            this.tsbClose});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(9, 0, 6, 0);
            this.toolStripMenu.Size = new System.Drawing.Size(885, 42);
            this.toolStripMenu.TabIndex = 0;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tslTitle
            // 
            this.tslTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 13F);
            this.tslTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(45)))));
            this.tslTitle.Name = "tslTitle";
            this.tslTitle.Size = new System.Drawing.Size(193, 39);
            this.tslTitle.Text = "Field Change Monitor";
            // 
            // tslSubtitle
            // 
            this.tslSubtitle.ForeColor = System.Drawing.Color.DimGray;
            this.tslSubtitle.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.tslSubtitle.Name = "tslSubtitle";
            this.tslSubtitle.Size = new System.Drawing.Size(248, 39);
            this.tslSubtitle.Text = "Monitore alteracoes em campos do Dataverse";
            // 
            // tslSpring
            // 
            this.tslSpring.AutoSize = false;
            this.tslSpring.Name = "tslSpring";
            this.tslSpring.Size = new System.Drawing.Size(40, 49);
            // 
            // tslConnection
            // 
            this.tslConnection.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tslConnection.ForeColor = System.Drawing.Color.ForestGreen;
            this.tslConnection.Margin = new System.Windows.Forms.Padding(0, 1, 16, 2);
            this.tslConnection.Name = "tslConnection";
            this.tslConnection.Size = new System.Drawing.Size(120, 39);
            this.tslConnection.Text = "Aguardando conexao";
            // 
            // tslActiveMonitors
            // 
            this.tslActiveMonitors.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tslActiveMonitors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(127)))), ((int)(((byte)(140)))));
            this.tslActiveMonitors.Margin = new System.Windows.Forms.Padding(0, 1, 16, 2);
            this.tslActiveMonitors.Name = "tslActiveMonitors";
            this.tslActiveMonitors.Size = new System.Drawing.Size(142, 39);
            this.tslActiveMonitors.Text = "Monitoramentos ativos: 0";
            // 
            // tsbClose
            // 
            this.tsbClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(107, 49);
            this.tsbClose.Text = "Fechar ferramenta";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.contentLayout, 0, 0);
            this.mainLayout.Controls.Add(this.summaryPanel, 0, 1);
            this.mainLayout.Controls.Add(this.bottomLayout, 0, 2);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 42);
            this.mainLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(9, 10, 9, 10);
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 62F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38F));
            this.mainLayout.Size = new System.Drawing.Size(885, 602);
            this.mainLayout.TabIndex = 1;
            // 
            // contentLayout
            // 
            this.contentLayout.ColumnCount = 2;
            this.contentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.contentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.contentLayout.Controls.Add(this.configurationGroup, 0, 0);
            this.contentLayout.Controls.Add(this.filterGroup, 1, 0);
            this.contentLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLayout.Location = new System.Drawing.Point(11, 12);
            this.contentLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.contentLayout.Name = "contentLayout";
            this.contentLayout.RowCount = 1;
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayout.Size = new System.Drawing.Size(863, 318);
            this.contentLayout.TabIndex = 1;
            // 
            // configurationGroup
            // 
            this.configurationGroup.Controls.Add(this.configurationLayout);
            this.configurationGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configurationGroup.Location = new System.Drawing.Point(2, 2);
            this.configurationGroup.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.configurationGroup.Name = "configurationGroup";
            this.configurationGroup.Padding = new System.Windows.Forms.Padding(9, 10, 9, 10);
            this.configurationGroup.Size = new System.Drawing.Size(211, 314);
            this.configurationGroup.TabIndex = 0;
            this.configurationGroup.TabStop = false;
            this.configurationGroup.Text = "1  Tabela, frequência e campos monitorados";
            // 
            // configurationLayout
            // 
            this.configurationLayout.ColumnCount = 2;
            this.configurationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.configurationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.configurationLayout.Controls.Add(this.lblEntity, 0, 0);
            this.configurationLayout.Controls.Add(this.txtEntityLogicalName, 0, 1);
            this.configurationLayout.Controls.Add(this.btnLoadColumns, 1, 1);
            this.configurationLayout.Controls.Add(this.lblInterval, 0, 2);
            this.configurationLayout.Controls.Add(this.nudIntervalSeconds, 0, 3);
            this.configurationLayout.Controls.Add(this.columnsGroup, 0, 4);
            this.configurationLayout.SetColumnSpan(this.columnsGroup, 2);
            this.configurationLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configurationLayout.Location = new System.Drawing.Point(9, 23);
            this.configurationLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.configurationLayout.Name = "configurationLayout";
            this.configurationLayout.RowCount = 5;
            this.configurationLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.configurationLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.configurationLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.configurationLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.configurationLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.configurationLayout.Size = new System.Drawing.Size(193, 281);
            this.configurationLayout.TabIndex = 0;
            // 
            // lblEntity
            // 
            this.lblEntity.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblEntity.AutoSize = true;
            this.configurationLayout.SetColumnSpan(this.lblEntity, 2);
            this.lblEntity.Location = new System.Drawing.Point(2, 5);
            this.lblEntity.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEntity.Name = "lblEntity";
            this.lblEntity.Size = new System.Drawing.Size(88, 13);
            this.lblEntity.TabIndex = 0;
            this.lblEntity.Text = "Entidade (logical)";
            // 
            // txtEntityLogicalName
            // 
            this.txtEntityLogicalName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEntityLogicalName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEntityLogicalName.Location = new System.Drawing.Point(2, 31);
            this.txtEntityLogicalName.Margin = new System.Windows.Forms.Padding(2, 7, 4, 7);
            this.txtEntityLogicalName.Name = "txtEntityLogicalName";
            this.txtEntityLogicalName.Size = new System.Drawing.Size(86, 20);
            this.txtEntityLogicalName.TabIndex = 1;
            this.txtEntityLogicalName.TextChanged += new System.EventHandler(this.ConfigurationValueChanged);
            // 
            // btnLoadColumns
            // 
            this.btnLoadColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoadColumns.Location = new System.Drawing.Point(94, 29);
            this.btnLoadColumns.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.btnLoadColumns.Name = "btnLoadColumns";
            this.btnLoadColumns.Size = new System.Drawing.Size(97, 29);
            this.btnLoadColumns.TabIndex = 2;
            this.btnLoadColumns.Text = "Carregar colunas";
            this.btnLoadColumns.UseVisualStyleBackColor = true;
            this.btnLoadColumns.Click += new System.EventHandler(this.btnLoadColumns_Click);
            // 
            // lblInterval
            // 
            this.lblInterval.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblInterval.AutoSize = true;
            this.configurationLayout.SetColumnSpan(this.lblInterval, 2);
            this.lblInterval.Location = new System.Drawing.Point(2, 70);
            this.lblInterval.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(103, 13);
            this.lblInterval.TabIndex = 3;
            this.lblInterval.Text = "Intervalo (segundos)";
            // 
            // nudIntervalSeconds
            // 
            this.nudIntervalSeconds.Dock = System.Windows.Forms.DockStyle.Left;
            this.nudIntervalSeconds.Location = new System.Drawing.Point(2, 98);
            this.nudIntervalSeconds.Margin = new System.Windows.Forms.Padding(2, 7, 2, 7);
            this.nudIntervalSeconds.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.nudIntervalSeconds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudIntervalSeconds.Name = "nudIntervalSeconds";
            this.nudIntervalSeconds.Size = new System.Drawing.Size(52, 20);
            this.nudIntervalSeconds.TabIndex = 4;
            this.nudIntervalSeconds.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudIntervalSeconds.ValueChanged += new System.EventHandler(this.ConfigurationValueChanged);
            // 
            // columnsGroup
            // 
            this.columnsGroup.Controls.Add(this.columnsLayout);
            this.columnsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsGroup.Location = new System.Drawing.Point(217, 2);
            this.columnsGroup.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.columnsGroup.Name = "columnsGroup";
            this.columnsGroup.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.columnsGroup.Size = new System.Drawing.Size(272, 314);
            this.columnsGroup.TabIndex = 0;
            this.columnsGroup.TabStop = false;
            this.columnsGroup.Text = "Campos monitorados";
            // 
            // columnsLayout
            // 
            this.columnsLayout.ColumnCount = 1;
            this.columnsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.columnsLayout.Controls.Add(this.columnSearchLayout, 0, 0);
            this.columnsLayout.Controls.Add(this.columnsButtonPanel, 0, 1);
            this.columnsLayout.Controls.Add(this.clbColumns, 0, 2);
            this.columnsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsLayout.Location = new System.Drawing.Point(8, 21);
            this.columnsLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.columnsLayout.Name = "columnsLayout";
            this.columnsLayout.RowCount = 3;
            this.columnsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.columnsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.columnsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.columnsLayout.Size = new System.Drawing.Size(256, 285);
            this.columnsLayout.TabIndex = 0;
            // 
            // columnSearchLayout
            // 
            this.columnSearchLayout.ColumnCount = 2;
            this.columnSearchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.columnSearchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.columnSearchLayout.Controls.Add(this.lblColumnSearch, 0, 0);
            this.columnSearchLayout.Controls.Add(this.txtColumnSearch, 1, 0);
            this.columnSearchLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnSearchLayout.Location = new System.Drawing.Point(0, 0);
            this.columnSearchLayout.Margin = new System.Windows.Forms.Padding(0);
            this.columnSearchLayout.Name = "columnSearchLayout";
            this.columnSearchLayout.RowCount = 1;
            this.columnSearchLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.columnSearchLayout.Size = new System.Drawing.Size(256, 29);
            this.columnSearchLayout.TabIndex = 0;
            // 
            // lblColumnSearch
            // 
            this.lblColumnSearch.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblColumnSearch.AutoSize = true;
            this.lblColumnSearch.Location = new System.Drawing.Point(2, 8);
            this.lblColumnSearch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblColumnSearch.Name = "lblColumnSearch";
            this.lblColumnSearch.Size = new System.Drawing.Size(40, 13);
            this.lblColumnSearch.TabIndex = 0;
            this.lblColumnSearch.Text = "Buscar";
            // 
            // txtColumnSearch
            // 
            this.txtColumnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColumnSearch.Location = new System.Drawing.Point(51, 4);
            this.txtColumnSearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtColumnSearch.Name = "txtColumnSearch";
            this.txtColumnSearch.Size = new System.Drawing.Size(203, 20);
            this.txtColumnSearch.TabIndex = 1;
            this.txtColumnSearch.TextChanged += new System.EventHandler(this.txtColumnSearch_TextChanged);
            // 
            // columnsButtonPanel
            // 
            this.columnsButtonPanel.Controls.Add(this.btnSelectAllColumns);
            this.columnsButtonPanel.Controls.Add(this.btnClearColumnSelection);
            this.columnsButtonPanel.Controls.Add(this.lblSelectedCount);
            this.columnsButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsButtonPanel.Location = new System.Drawing.Point(0, 29);
            this.columnsButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.columnsButtonPanel.Name = "columnsButtonPanel";
            this.columnsButtonPanel.Size = new System.Drawing.Size(256, 32);
            this.columnsButtonPanel.TabIndex = 1;
            // 
            // btnSelectAllColumns
            // 
            this.btnSelectAllColumns.Location = new System.Drawing.Point(2, 2);
            this.btnSelectAllColumns.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSelectAllColumns.Name = "btnSelectAllColumns";
            this.btnSelectAllColumns.Size = new System.Drawing.Size(86, 24);
            this.btnSelectAllColumns.TabIndex = 0;
            this.btnSelectAllColumns.Text = "Selecionar tudo";
            this.btnSelectAllColumns.UseVisualStyleBackColor = true;
            this.btnSelectAllColumns.Click += new System.EventHandler(this.btnSelectAllColumns_Click);
            // 
            // btnClearColumnSelection
            // 
            this.btnClearColumnSelection.Location = new System.Drawing.Point(92, 2);
            this.btnClearColumnSelection.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClearColumnSelection.Name = "btnClearColumnSelection";
            this.btnClearColumnSelection.Size = new System.Drawing.Size(75, 24);
            this.btnClearColumnSelection.TabIndex = 1;
            this.btnClearColumnSelection.Text = "Limpar";
            this.btnClearColumnSelection.UseVisualStyleBackColor = true;
            this.btnClearColumnSelection.Click += new System.EventHandler(this.btnClearColumnSelection_Click);
            // 
            // lblSelectedCount
            // 
            this.lblSelectedCount.AutoSize = true;
            this.lblSelectedCount.Location = new System.Drawing.Point(9, 35);
            this.lblSelectedCount.Margin = new System.Windows.Forms.Padding(9, 7, 2, 0);
            this.lblSelectedCount.Name = "lblSelectedCount";
            this.lblSelectedCount.Size = new System.Drawing.Size(84, 13);
            this.lblSelectedCount.TabIndex = 2;
            this.lblSelectedCount.Text = "0 selecionado(s)";
            // 
            // clbColumns
            // 
            this.clbColumns.CheckOnClick = true;
            this.clbColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbColumns.FormattingEnabled = true;
            this.clbColumns.IntegralHeight = false;
            this.clbColumns.Location = new System.Drawing.Point(2, 63);
            this.clbColumns.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.clbColumns.Name = "clbColumns";
            this.clbColumns.Size = new System.Drawing.Size(252, 220);
            this.clbColumns.TabIndex = 2;
            this.clbColumns.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbColumns_ItemCheck);
            // 
            // filterGroup
            // 
            this.filterGroup.Controls.Add(this.filterLayout);
            this.filterGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterGroup.Location = new System.Drawing.Point(493, 2);
            this.filterGroup.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.filterGroup.Name = "filterGroup";
            this.filterGroup.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.filterGroup.Size = new System.Drawing.Size(368, 314);
            this.filterGroup.TabIndex = 1;
            this.filterGroup.TabStop = false;
            this.filterGroup.Text = "3  Filtro (opcional)";
            // 
            // filterLayout
            // 
            this.filterLayout.ColumnCount = 1;
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterLayout.Controls.Add(this.conditionBuilderGroup, 0, 0);
            this.filterLayout.Controls.Add(this.lblFilterHint, 0, 1);
            this.filterLayout.Controls.Add(this.lvConditions, 0, 2);
            this.filterLayout.Controls.Add(this.advancedButtonsPanel, 0, 3);
            this.filterLayout.Controls.Add(this.txtFilterXml, 0, 4);
            this.filterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterLayout.Location = new System.Drawing.Point(8, 21);
            this.filterLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.filterLayout.Name = "filterLayout";
            this.filterLayout.RowCount = 5;
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.filterLayout.Size = new System.Drawing.Size(352, 285);
            this.filterLayout.TabIndex = 0;
            // 
            // conditionBuilderGroup
            // 
            this.conditionBuilderGroup.Controls.Add(this.conditionBuilderLayout);
            this.conditionBuilderGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditionBuilderGroup.Location = new System.Drawing.Point(2, 2);
            this.conditionBuilderGroup.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.conditionBuilderGroup.Name = "conditionBuilderGroup";
            this.conditionBuilderGroup.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.conditionBuilderGroup.Size = new System.Drawing.Size(348, 124);
            this.conditionBuilderGroup.TabIndex = 0;
            this.conditionBuilderGroup.TabStop = false;
            this.conditionBuilderGroup.Text = "Nova condicao";
            // 
            // conditionBuilderLayout
            // 
            this.conditionBuilderLayout.ColumnCount = 6;
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 174F));
            this.conditionBuilderLayout.Controls.Add(this.lblFilterType, 0, 0);
            this.conditionBuilderLayout.Controls.Add(this.cboFilterType, 1, 0);
            this.conditionBuilderLayout.Controls.Add(this.lblConditionFieldSearch, 2, 0);
            this.conditionBuilderLayout.Controls.Add(this.txtConditionFieldSearch, 3, 0);
            this.conditionBuilderLayout.Controls.Add(this.lblConditionField, 0, 1);
            this.conditionBuilderLayout.Controls.Add(this.cboConditionAttribute, 1, 1);
            this.conditionBuilderLayout.Controls.Add(this.lblConditionOperator, 4, 1);
            this.conditionBuilderLayout.Controls.Add(this.cboConditionOperator, 5, 1);
            this.conditionBuilderLayout.Controls.Add(this.lblConditionValue, 0, 2);
            this.conditionBuilderLayout.Controls.Add(this.txtConditionValue, 1, 2);
            this.conditionBuilderLayout.Controls.Add(this.btnPickConditionValue, 4, 2);
            this.conditionBuilderLayout.Controls.Add(this.conditionButtonPanel, 5, 2);
            this.conditionBuilderLayout.Controls.Add(this.lblConditionValueHint, 1, 3);
            this.conditionBuilderLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditionBuilderLayout.Location = new System.Drawing.Point(8, 21);
            this.conditionBuilderLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.conditionBuilderLayout.Name = "conditionBuilderLayout";
            this.conditionBuilderLayout.RowCount = 4;
            this.conditionBuilderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.conditionBuilderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.conditionBuilderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.conditionBuilderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.conditionBuilderLayout.Size = new System.Drawing.Size(332, 95);
            this.conditionBuilderLayout.TabIndex = 0;
            // 
            // lblFilterType
            // 
            this.lblFilterType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFilterType.AutoSize = true;
            this.lblFilterType.Location = new System.Drawing.Point(2, 6);
            this.lblFilterType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFilterType.Name = "lblFilterType";
            this.lblFilterType.Size = new System.Drawing.Size(28, 13);
            this.lblFilterType.TabIndex = 0;
            this.lblFilterType.Text = "Tipo";
            // 
            // cboFilterType
            // 
            this.cboFilterType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterType.FormattingEnabled = true;
            this.cboFilterType.Location = new System.Drawing.Point(54, 2);
            this.cboFilterType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboFilterType.Name = "cboFilterType";
            this.cboFilterType.Size = new System.Drawing.Size(75, 21);
            this.cboFilterType.TabIndex = 1;
            this.cboFilterType.SelectedIndexChanged += new System.EventHandler(this.cboFilterType_SelectedIndexChanged);
            // 
            // lblConditionFieldSearch
            // 
            this.lblConditionFieldSearch.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblConditionFieldSearch.AutoSize = true;
            this.lblConditionFieldSearch.Location = new System.Drawing.Point(133, 6);
            this.lblConditionFieldSearch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConditionFieldSearch.Name = "lblConditionFieldSearch";
            this.lblConditionFieldSearch.Size = new System.Drawing.Size(40, 13);
            this.lblConditionFieldSearch.TabIndex = 2;
            this.lblConditionFieldSearch.Text = "Buscar";
            // 
            // txtConditionFieldSearch
            // 
            this.txtConditionFieldSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionBuilderLayout.SetColumnSpan(this.txtConditionFieldSearch, 3);
            this.txtConditionFieldSearch.Location = new System.Drawing.Point(193, 2);
            this.txtConditionFieldSearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtConditionFieldSearch.Name = "txtConditionFieldSearch";
            this.txtConditionFieldSearch.Size = new System.Drawing.Size(137, 20);
            this.txtConditionFieldSearch.TabIndex = 3;
            this.txtConditionFieldSearch.TextChanged += new System.EventHandler(this.txtConditionFieldSearch_TextChanged);
            // 
            // lblConditionField
            // 
            this.lblConditionField.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblConditionField.AutoSize = true;
            this.lblConditionField.Location = new System.Drawing.Point(2, 31);
            this.lblConditionField.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConditionField.Name = "lblConditionField";
            this.lblConditionField.Size = new System.Drawing.Size(40, 13);
            this.lblConditionField.TabIndex = 4;
            this.lblConditionField.Text = "Campo";
            // 
            // cboConditionAttribute
            // 
            this.cboConditionAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionBuilderLayout.SetColumnSpan(this.cboConditionAttribute, 3);
            this.cboConditionAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConditionAttribute.FormattingEnabled = true;
            this.cboConditionAttribute.Location = new System.Drawing.Point(54, 27);
            this.cboConditionAttribute.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboConditionAttribute.Name = "cboConditionAttribute";
            this.cboConditionAttribute.Size = new System.Drawing.Size(23, 21);
            this.cboConditionAttribute.TabIndex = 5;
            this.cboConditionAttribute.SelectedIndexChanged += new System.EventHandler(this.cboConditionAttribute_SelectedIndexChanged);
            // 
            // lblConditionOperator
            // 
            this.lblConditionOperator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblConditionOperator.AutoSize = true;
            this.lblConditionOperator.Location = new System.Drawing.Point(81, 31);
            this.lblConditionOperator.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConditionOperator.Name = "lblConditionOperator";
            this.lblConditionOperator.Size = new System.Drawing.Size(51, 13);
            this.lblConditionOperator.TabIndex = 6;
            this.lblConditionOperator.Text = "Operador";
            // 
            // cboConditionOperator
            // 
            this.cboConditionOperator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboConditionOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConditionOperator.FormattingEnabled = true;
            this.cboConditionOperator.Location = new System.Drawing.Point(160, 27);
            this.cboConditionOperator.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboConditionOperator.Name = "cboConditionOperator";
            this.cboConditionOperator.Size = new System.Drawing.Size(170, 21);
            this.cboConditionOperator.TabIndex = 7;
            this.cboConditionOperator.SelectedIndexChanged += new System.EventHandler(this.cboConditionOperator_SelectedIndexChanged);
            // 
            // lblConditionValue
            // 
            this.lblConditionValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblConditionValue.AutoSize = true;
            this.lblConditionValue.Location = new System.Drawing.Point(2, 57);
            this.lblConditionValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConditionValue.Name = "lblConditionValue";
            this.lblConditionValue.Size = new System.Drawing.Size(31, 13);
            this.lblConditionValue.TabIndex = 8;
            this.lblConditionValue.Text = "Valor";
            // 
            // txtConditionValue
            // 
            this.txtConditionValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionBuilderLayout.SetColumnSpan(this.txtConditionValue, 3);
            this.txtConditionValue.Location = new System.Drawing.Point(54, 54);
            this.txtConditionValue.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtConditionValue.Name = "txtConditionValue";
            this.txtConditionValue.Size = new System.Drawing.Size(23, 20);
            this.txtConditionValue.TabIndex = 9;
            // 
            // btnPickConditionValue
            // 
            this.btnPickConditionValue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPickConditionValue.Location = new System.Drawing.Point(85, 52);
            this.btnPickConditionValue.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPickConditionValue.Name = "btnPickConditionValue";
            this.btnPickConditionValue.Size = new System.Drawing.Size(67, 23);
            this.btnPickConditionValue.TabIndex = 10;
            this.btnPickConditionValue.Text = "Selecionar";
            this.btnPickConditionValue.UseVisualStyleBackColor = true;
            this.btnPickConditionValue.Click += new System.EventHandler(this.btnPickConditionValue_Click);
            // 
            // conditionButtonPanel
            // 
            this.conditionButtonPanel.Controls.Add(this.btnAddCondition);
            this.conditionButtonPanel.Controls.Add(this.btnRemoveCondition);
            this.conditionButtonPanel.Controls.Add(this.btnClearFilter);
            this.conditionButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditionButtonPanel.Location = new System.Drawing.Point(158, 50);
            this.conditionButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.conditionButtonPanel.Name = "conditionButtonPanel";
            this.conditionButtonPanel.Size = new System.Drawing.Size(174, 28);
            this.conditionButtonPanel.TabIndex = 11;
            // 
            // btnAddCondition
            // 
            this.btnAddCondition.Location = new System.Drawing.Point(2, 2);
            this.btnAddCondition.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAddCondition.Name = "btnAddCondition";
            this.btnAddCondition.Size = new System.Drawing.Size(60, 24);
            this.btnAddCondition.TabIndex = 0;
            this.btnAddCondition.Text = "Adicionar";
            this.btnAddCondition.UseVisualStyleBackColor = true;
            this.btnAddCondition.Click += new System.EventHandler(this.btnAddCondition_Click);
            // 
            // btnRemoveCondition
            // 
            this.btnRemoveCondition.Location = new System.Drawing.Point(66, 2);
            this.btnRemoveCondition.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnRemoveCondition.Name = "btnRemoveCondition";
            this.btnRemoveCondition.Size = new System.Drawing.Size(54, 24);
            this.btnRemoveCondition.TabIndex = 1;
            this.btnRemoveCondition.Text = "Remover";
            this.btnRemoveCondition.UseVisualStyleBackColor = true;
            this.btnRemoveCondition.Click += new System.EventHandler(this.btnRemoveCondition_Click);
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.Location = new System.Drawing.Point(124, 2);
            this.btnClearFilter.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(44, 24);
            this.btnClearFilter.TabIndex = 2;
            this.btnClearFilter.Text = "Limpar";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            // 
            // lblConditionValueHint
            // 
            this.conditionBuilderLayout.SetColumnSpan(this.lblConditionValueHint, 5);
            this.lblConditionValueHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblConditionValueHint.Location = new System.Drawing.Point(54, 78);
            this.lblConditionValueHint.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConditionValueHint.Name = "lblConditionValueHint";
            this.lblConditionValueHint.Size = new System.Drawing.Size(276, 17);
            this.lblConditionValueHint.TabIndex = 12;
            this.lblConditionValueHint.Text = "Carregue uma entidade para criar condicoes.";
            this.lblConditionValueHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFilterHint
            // 
            this.lblFilterHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFilterHint.Location = new System.Drawing.Point(2, 128);
            this.lblFilterHint.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFilterHint.Name = "lblFilterHint";
            this.lblFilterHint.Size = new System.Drawing.Size(348, 24);
            this.lblFilterHint.TabIndex = 1;
            this.lblFilterHint.Text = "O XML abaixo e gerado pelas condicoes, mas tambem pode ser editado manualmente.";
            this.lblFilterHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFilterHint.Visible = false;
            // 
            // lvConditions
            // 
            this.lvConditions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colConditionField,
            this.colConditionOperator,
            this.colConditionValue});
            this.lvConditions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvConditions.FullRowSelect = true;
            this.lvConditions.GridLines = true;
            this.lvConditions.HideSelection = false;
            this.lvConditions.Location = new System.Drawing.Point(2, 154);
            this.lvConditions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lvConditions.Name = "lvConditions";
            this.lvConditions.Size = new System.Drawing.Size(348, 98);
            this.lvConditions.TabIndex = 2;
            this.lvConditions.UseCompatibleStateImageBehavior = false;
            this.lvConditions.View = System.Windows.Forms.View.Details;
            this.lvConditions.SizeChanged += new System.EventHandler(this.ListView_SizeChanged);
            // 
            // colConditionField
            // 
            this.colConditionField.Text = "Campo";
            this.colConditionField.Width = 250;
            // 
            // colConditionOperator
            // 
            this.colConditionOperator.Text = "Operador";
            this.colConditionOperator.Width = 190;
            // 
            // colConditionValue
            // 
            this.colConditionValue.Text = "Valor";
            this.colConditionValue.Width = 260;
            // 
            // btnToggleAdvanced
            // 
            this.btnToggleAdvanced.AutoSize = true;
            this.btnToggleAdvanced.Dock = System.Windows.Forms.DockStyle.None;
            this.btnToggleAdvanced.FlatAppearance.BorderSize = 0;
            this.btnToggleAdvanced.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleAdvanced.Location = new System.Drawing.Point(2, 256);
            this.btnToggleAdvanced.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnToggleAdvanced.Name = "btnToggleAdvanced";
            this.btnToggleAdvanced.Size = new System.Drawing.Size(348, 27);
            this.btnToggleAdvanced.TabIndex = 3;
            this.btnToggleAdvanced.Text = "›  Opções avançadas • Editar FetchXML";
            this.btnToggleAdvanced.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnToggleAdvanced.UseVisualStyleBackColor = true;
            this.btnToggleAdvanced.Click += new System.EventHandler(this.btnToggleAdvanced_Click);
            // 
            // advancedButtonsPanel
            // 
            this.advancedButtonsPanel.Controls.Add(this.btnToggleAdvanced);
            this.advancedButtonsPanel.Controls.Add(this.btnSaveFilterXml);
            this.advancedButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advancedButtonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.advancedButtonsPanel.Name = "advancedButtonsPanel";
            this.advancedButtonsPanel.WrapContents = false;
            // 
            // btnSaveFilterXml
            // 
            this.btnSaveFilterXml.AutoSize = true;
            this.btnSaveFilterXml.Name = "btnSaveFilterXml";
            this.btnSaveFilterXml.Text = "Salvar FetchXML";
            this.btnSaveFilterXml.UseVisualStyleBackColor = true;
            this.btnSaveFilterXml.Visible = false;
            this.btnSaveFilterXml.Click += new System.EventHandler(this.btnSaveFilterXml_Click);
            // 
            // txtFilterXml
            // 
            this.txtFilterXml.AcceptsReturn = true;
            this.txtFilterXml.AcceptsTab = true;
            this.txtFilterXml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilterXml.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtFilterXml.Location = new System.Drawing.Point(2, 287);
            this.txtFilterXml.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtFilterXml.Multiline = true;
            this.txtFilterXml.Name = "txtFilterXml";
            this.txtFilterXml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFilterXml.Size = new System.Drawing.Size(348, 1);
            this.txtFilterXml.TabIndex = 3;
            this.txtFilterXml.Visible = false;
            this.txtFilterXml.WordWrap = false;
            // 
            // summaryPanel
            // 
            this.summaryPanel.BackColor = System.Drawing.Color.White;
            this.summaryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.summaryPanel.Controls.Add(this.lblConfigurationReady);
            this.summaryPanel.Controls.Add(this.lblConfigurationSummary);
            this.summaryPanel.Controls.Add(this.lblMonitorName);
            this.summaryPanel.Controls.Add(this.txtMonitorName);
            this.summaryPanel.Controls.Add(this.btnCancelEdit);
            this.summaryPanel.Controls.Add(this.btnStart);
            this.summaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryPanel.Location = new System.Drawing.Point(11, 338);
            this.summaryPanel.Margin = new System.Windows.Forms.Padding(2, 6, 2, 6);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(863, 50);
            this.summaryPanel.TabIndex = 2;
            this.summaryPanel.SizeChanged += new System.EventHandler(this.summaryPanel_SizeChanged);
            // 
            // lblConfigurationReady
            // 
            this.lblConfigurationReady.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblConfigurationReady.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lblConfigurationReady.Location = new System.Drawing.Point(300, 5);
            this.lblConfigurationReady.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConfigurationReady.Name = "lblConfigurationReady";
            this.lblConfigurationReady.Padding = new System.Windows.Forms.Padding(0);
            this.lblConfigurationReady.Size = new System.Drawing.Size(360, 26);
            this.lblConfigurationReady.TabIndex = 0;
            this.lblConfigurationReady.Text = "Configure o monitoramento";
            // 
            // lblConfigurationSummary
            // 
            this.lblConfigurationSummary.AutoEllipsis = true;
            this.lblConfigurationSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConfigurationSummary.Location = new System.Drawing.Point(300, 34);
            this.lblConfigurationSummary.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConfigurationSummary.Name = "lblConfigurationSummary";
            this.lblConfigurationSummary.Padding = new System.Windows.Forms.Padding(0);
            this.lblConfigurationSummary.Size = new System.Drawing.Size(390, 30);
            this.lblConfigurationSummary.TabIndex = 1;
            this.lblConfigurationSummary.Text = "Selecione uma tabela";
            // 
            // lblMonitorName
            // 
            this.lblMonitorName.AutoSize = true;
            this.lblMonitorName.Location = new System.Drawing.Point(14, 7);
            this.lblMonitorName.Name = "lblMonitorName";
            this.lblMonitorName.Text = "Nome do monitoramento *";
            // 
            // txtMonitorName
            // 
            this.txtMonitorName.Location = new System.Drawing.Point(14, 29);
            this.txtMonitorName.Name = "txtMonitorName";
            this.txtMonitorName.Size = new System.Drawing.Size(265, 23);
            this.txtMonitorName.TabIndex = 0;
            this.txtMonitorName.TextChanged += new System.EventHandler(this.ConfigurationValueChanged);
            // 
            // btnCancelEdit
            // 
            this.btnCancelEdit.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancelEdit.Name = "btnCancelEdit";
            this.btnCancelEdit.Size = new System.Drawing.Size(110, 48);
            this.btnCancelEdit.Text = "Cancelar edicao";
            this.btnCancelEdit.UseVisualStyleBackColor = true;
            this.btnCancelEdit.Visible = false;
            this.btnCancelEdit.Click += new System.EventHandler(this.btnCancelEdit_Click);
            // 
            // btnStart
            // 
            this.btnStart.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnStart.Location = new System.Drawing.Point(696, 0);
            this.btnStart.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(165, 48);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Iniciar monitoramento";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // bottomLayout
            // 
            this.bottomLayout.ColumnCount = 2;
            this.bottomLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.bottomLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.bottomLayout.Controls.Add(this.activeMonitorsGroup, 0, 0);
            this.bottomLayout.Controls.Add(this.eventsGroup, 1, 0);
            this.bottomLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomLayout.Location = new System.Drawing.Point(11, 396);
            this.bottomLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bottomLayout.Name = "bottomLayout";
            this.bottomLayout.RowCount = 1;
            this.bottomLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bottomLayout.Size = new System.Drawing.Size(863, 194);
            this.bottomLayout.TabIndex = 2;
            // 
            // activeMonitorsGroup
            // 
            this.activeMonitorsGroup.Controls.Add(this.activeMonitorsLayout);
            this.activeMonitorsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeMonitorsGroup.Location = new System.Drawing.Point(2, 2);
            this.activeMonitorsGroup.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.activeMonitorsGroup.Name = "activeMonitorsGroup";
            this.activeMonitorsGroup.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.activeMonitorsGroup.Size = new System.Drawing.Size(358, 190);
            this.activeMonitorsGroup.TabIndex = 0;
            this.activeMonitorsGroup.TabStop = false;
            this.activeMonitorsGroup.Text = "Em execução";
            // 
            // activeMonitorsLayout
            // 
            this.activeMonitorsLayout.ColumnCount = 1;
            this.activeMonitorsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.activeMonitorsLayout.Controls.Add(this.activeMonitorButtonPanel, 0, 0);
            this.activeMonitorsLayout.Controls.Add(this.lvActiveMonitors, 0, 1);
            this.activeMonitorsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeMonitorsLayout.Location = new System.Drawing.Point(8, 21);
            this.activeMonitorsLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.activeMonitorsLayout.Name = "activeMonitorsLayout";
            this.activeMonitorsLayout.RowCount = 2;
            this.activeMonitorsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.activeMonitorsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.activeMonitorsLayout.Size = new System.Drawing.Size(342, 161);
            this.activeMonitorsLayout.TabIndex = 0;
            // 
            // activeMonitorButtonPanel
            // 
            this.activeMonitorButtonPanel.Controls.Add(this.btnSelectAllMonitors);
            this.activeMonitorButtonPanel.Controls.Add(this.btnEditMonitor);
            this.activeMonitorButtonPanel.Controls.Add(this.btnPauseSelectedMonitors);
            this.activeMonitorButtonPanel.Controls.Add(this.btnRemoveSelectedMonitors);
            this.activeMonitorButtonPanel.Controls.Add(this.btnExportMonitors);
            this.activeMonitorButtonPanel.Controls.Add(this.btnImportMonitors);
            this.activeMonitorButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeMonitorButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.activeMonitorButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.activeMonitorButtonPanel.Name = "activeMonitorButtonPanel";
            this.activeMonitorButtonPanel.Size = new System.Drawing.Size(342, 31);
            this.activeMonitorButtonPanel.TabIndex = 0;
            // 
            // btnStopSelectedMonitor
            // 
            this.btnStopSelectedMonitor.Location = new System.Drawing.Point(2, 2);
            this.btnStopSelectedMonitor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStopSelectedMonitor.Name = "btnStopSelectedMonitor";
            this.btnStopSelectedMonitor.Size = new System.Drawing.Size(109, 24);
            this.btnStopSelectedMonitor.TabIndex = 0;
            this.btnStopSelectedMonitor.Text = "Parar selecionado";
            this.btnStopSelectedMonitor.UseVisualStyleBackColor = true;
            this.btnStopSelectedMonitor.Click += new System.EventHandler(this.btnStopSelectedMonitor_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStop.Location = new System.Drawing.Point(115, 2);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(70, 26);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Parar todos";
            this.btnStop.Visible = false;
            // 
            // btnSelectAllMonitors
            // 
            this.btnSelectAllMonitors.AutoSize = true;
            this.btnSelectAllMonitors.Name = "btnSelectAllMonitors";
            this.btnSelectAllMonitors.Text = "Selecionar todos";
            this.btnSelectAllMonitors.UseVisualStyleBackColor = true;
            this.btnSelectAllMonitors.Click += new System.EventHandler(this.btnSelectAllMonitors_Click);
            // 
            // btnPauseSelectedMonitors
            // 
            this.btnPauseSelectedMonitors.AutoSize = true;
            this.btnPauseSelectedMonitors.Name = "btnPauseSelectedMonitors";
            this.btnPauseSelectedMonitors.Text = "Pausar / continuar";
            this.btnPauseSelectedMonitors.UseVisualStyleBackColor = true;
            this.btnPauseSelectedMonitors.Click += new System.EventHandler(this.btnPauseSelectedMonitors_Click);
            // 
            // btnRemoveSelectedMonitors
            // 
            this.btnRemoveSelectedMonitors.AutoSize = true;
            this.btnRemoveSelectedMonitors.Name = "btnRemoveSelectedMonitors";
            this.btnRemoveSelectedMonitors.Text = "Remover";
            this.btnRemoveSelectedMonitors.UseVisualStyleBackColor = true;
            this.btnRemoveSelectedMonitors.Click += new System.EventHandler(this.btnRemoveSelectedMonitors_Click);
            // 
            // btnExportMonitors
            // 
            this.btnExportMonitors.AutoSize = true;
            this.btnExportMonitors.Name = "btnExportMonitors";
            this.btnExportMonitors.Text = "Exportar selecionados";
            this.btnExportMonitors.UseVisualStyleBackColor = true;
            this.btnExportMonitors.Click += new System.EventHandler(this.btnExportMonitors_Click);
            // 
            // btnImportMonitors
            // 
            this.btnImportMonitors.AutoSize = true;
            this.btnImportMonitors.Name = "btnImportMonitors";
            this.btnImportMonitors.Text = "Importar";
            this.btnImportMonitors.UseVisualStyleBackColor = true;
            this.btnImportMonitors.Click += new System.EventHandler(this.btnImportMonitors_Click);
            // 
            // btnEditMonitor
            // 
            this.btnEditMonitor.AutoSize = true;
            this.btnEditMonitor.Name = "btnEditMonitor";
            this.btnEditMonitor.Text = "Editar";
            this.btnEditMonitor.UseVisualStyleBackColor = true;
            this.btnEditMonitor.Click += new System.EventHandler(this.btnEditMonitor_Click);
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lvActiveMonitors
            // 
            this.lvActiveMonitors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colActiveName,
            this.colActiveEntity,
            this.colActiveColumns,
            this.colActiveInterval,
            this.colActiveStatus,
            this.colActiveFilter});
            this.lvActiveMonitors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvActiveMonitors.FullRowSelect = true;
            this.lvActiveMonitors.GridLines = true;
            this.lvActiveMonitors.HideSelection = false;
            this.lvActiveMonitors.Location = new System.Drawing.Point(2, 33);
            this.lvActiveMonitors.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lvActiveMonitors.Name = "lvActiveMonitors";
            this.lvActiveMonitors.Size = new System.Drawing.Size(338, 126);
            this.lvActiveMonitors.TabIndex = 1;
            this.lvActiveMonitors.UseCompatibleStateImageBehavior = false;
            this.lvActiveMonitors.View = System.Windows.Forms.View.Details;
            this.lvActiveMonitors.SizeChanged += new System.EventHandler(this.ListView_SizeChanged);
            this.lvActiveMonitors.DoubleClick += new System.EventHandler(this.lvActiveMonitors_DoubleClick);
            // 
            // colActiveName
            // 
            this.colActiveName.Text = "Monitoramento";
            this.colActiveName.Width = 150;
            // 
            // colActiveEntity
            // 
            this.colActiveEntity.Text = "Entidade";
            this.colActiveEntity.Width = 110;
            // 
            // colActiveColumns
            // 
            this.colActiveColumns.Text = "Colunas";
            this.colActiveColumns.Width = 160;
            // 
            // colActiveInterval
            // 
            this.colActiveInterval.Text = "Seg.";
            this.colActiveInterval.Width = 50;
            // 
            // colActiveStatus
            // 
            this.colActiveStatus.Text = "Status";
            this.colActiveStatus.Width = 120;
            // 
            // colActiveFilter
            // 
            this.colActiveFilter.Text = "Filtro";
            this.colActiveFilter.Width = 220;
            // 
            // eventsGroup
            // 
            this.eventsGroup.Controls.Add(this.eventsTabs);
            this.eventsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventsGroup.Location = new System.Drawing.Point(364, 2);
            this.eventsGroup.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.eventsGroup.Name = "eventsGroup";
            this.eventsGroup.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.eventsGroup.Size = new System.Drawing.Size(497, 190);
            this.eventsGroup.TabIndex = 1;
            this.eventsGroup.TabStop = false;
            // 
            // eventsTabs
            // 
            this.eventsTabs.Controls.Add(this.recentChangesTab);
            this.eventsTabs.Controls.Add(this.technicalLogTab);
            this.eventsTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventsTabs.Name = "eventsTabs";
            this.eventsTabs.SelectedIndex = 0;
            // 
            // recentChangesTab
            // 
            this.recentChangesTab.Controls.Add(this.lvRecentChanges);
            this.recentChangesTab.Name = "recentChangesTab";
            this.recentChangesTab.Padding = new System.Windows.Forms.Padding(3);
            this.recentChangesTab.Text = "Alteracoes";
            this.recentChangesTab.UseVisualStyleBackColor = true;
            // 
            // technicalLogTab
            // 
            this.technicalLogTab.Controls.Add(this.lstEvents);
            this.technicalLogTab.Name = "technicalLogTab";
            this.technicalLogTab.Padding = new System.Windows.Forms.Padding(3);
            this.technicalLogTab.Text = "Log tecnico";
            this.technicalLogTab.UseVisualStyleBackColor = true;
            // 
            // lvRecentChanges
            // 
            this.lvRecentChanges.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colChangeModifiedOn,
            this.colChangeRecordId,
            this.colChangeModifiedBy,
            this.colChangeRecordName,
            this.colChangeField,
            this.colChangeValues,
            this.colChangeMonitor});
            this.lvRecentChanges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRecentChanges.FullRowSelect = true;
            this.lvRecentChanges.GridLines = true;
            this.lvRecentChanges.HideSelection = false;
            this.lvRecentChanges.Name = "lvRecentChanges";
            this.lvRecentChanges.UseCompatibleStateImageBehavior = false;
            this.lvRecentChanges.View = System.Windows.Forms.View.Details;
            this.lvRecentChanges.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvRecentChanges_MouseClick);
            this.lvRecentChanges.SizeChanged += new System.EventHandler(this.ListView_SizeChanged);
            this.colChangeModifiedOn.Text = "ModifiedOn";
            this.colChangeModifiedOn.Width = 135;
            this.colChangeRecordId.Text = "ID do registro (clique para abrir)";
            this.colChangeRecordId.Width = 245;
            this.colChangeModifiedBy.Text = "ModifiedBy";
            this.colChangeModifiedBy.Width = 150;
            this.colChangeRecordName.Text = "Registro";
            this.colChangeRecordName.Width = 150;
            this.colChangeField.Text = "Campo";
            this.colChangeField.Width = 130;
            this.colChangeValues.Text = "Alteracao";
            this.colChangeValues.Width = 280;
            this.colChangeMonitor.Text = "Monitoramento";
            this.colChangeMonitor.Width = 150;
            this.eventsGroup.Text = "Alterações recentes";
            // 
            // lstEvents
            // 
            this.lstEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEvents.FormattingEnabled = true;
            this.lstEvents.HorizontalScrollbar = true;
            this.lstEvents.IntegralHeight = false;
            this.lstEvents.Location = new System.Drawing.Point(8, 21);
            this.lstEvents.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(481, 161);
            this.lstEvents.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 644);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip.Size = new System.Drawing.Size(885, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(43, 17);
            this.tsslStatus.Text = "Pronto";
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = System.Drawing.SystemIcons.Information;
            this.notifyIcon.Text = "Monitor Dataverse";
            // 
            // MyPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainLayout);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(885, 666);
            this.OnCloseTool += new System.EventHandler(this.MyPluginControl_OnCloseTool);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.mainLayout.ResumeLayout(false);
            this.contentLayout.ResumeLayout(false);
            this.configurationGroup.ResumeLayout(false);
            this.configurationLayout.ResumeLayout(false);
            this.configurationLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalSeconds)).EndInit();
            this.columnsGroup.ResumeLayout(false);
            this.columnsLayout.ResumeLayout(false);
            this.columnSearchLayout.ResumeLayout(false);
            this.columnSearchLayout.PerformLayout();
            this.columnsButtonPanel.ResumeLayout(false);
            this.columnsButtonPanel.PerformLayout();
            this.filterGroup.ResumeLayout(false);
            this.filterLayout.ResumeLayout(false);
            this.filterLayout.PerformLayout();
            this.conditionBuilderGroup.ResumeLayout(false);
            this.conditionBuilderLayout.ResumeLayout(false);
            this.conditionBuilderLayout.PerformLayout();
            this.conditionButtonPanel.ResumeLayout(false);
            this.advancedButtonsPanel.ResumeLayout(false);
            this.advancedButtonsPanel.PerformLayout();
            this.summaryPanel.ResumeLayout(false);
            this.bottomLayout.ResumeLayout(false);
            this.activeMonitorsGroup.ResumeLayout(false);
            this.activeMonitorsLayout.ResumeLayout(false);
            this.activeMonitorButtonPanel.ResumeLayout(false);
            this.eventsGroup.ResumeLayout(false);
            this.eventsTabs.ResumeLayout(false);
            this.recentChangesTab.ResumeLayout(false);
            this.technicalLogTab.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripLabel tslTitle;
        private System.Windows.Forms.ToolStripLabel tslSubtitle;
        private System.Windows.Forms.ToolStripLabel tslSpring;
        private System.Windows.Forms.ToolStripLabel tslConnection;
        private System.Windows.Forms.ToolStripLabel tslActiveMonitors;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.GroupBox configurationGroup;
        private System.Windows.Forms.TableLayoutPanel configurationLayout;
        private System.Windows.Forms.Label lblEntity;
        private System.Windows.Forms.TextBox txtEntityLogicalName;
        private System.Windows.Forms.Button btnLoadColumns;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.NumericUpDown nudIntervalSeconds;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Panel summaryPanel;
        private System.Windows.Forms.Label lblConfigurationReady;
        private System.Windows.Forms.Label lblConfigurationSummary;
        private System.Windows.Forms.Label lblMonitorName;
        private System.Windows.Forms.TextBox txtMonitorName;
        private System.Windows.Forms.Button btnCancelEdit;
        private System.Windows.Forms.TableLayoutPanel contentLayout;
        private System.Windows.Forms.GroupBox columnsGroup;
        private System.Windows.Forms.TableLayoutPanel columnsLayout;
        private System.Windows.Forms.TableLayoutPanel columnSearchLayout;
        private System.Windows.Forms.Label lblColumnSearch;
        private System.Windows.Forms.TextBox txtColumnSearch;
        private System.Windows.Forms.FlowLayoutPanel columnsButtonPanel;
        private System.Windows.Forms.Button btnSelectAllColumns;
        private System.Windows.Forms.Button btnClearColumnSelection;
        private System.Windows.Forms.Label lblSelectedCount;
        private System.Windows.Forms.CheckedListBox clbColumns;
        private System.Windows.Forms.GroupBox filterGroup;
        private System.Windows.Forms.TableLayoutPanel filterLayout;
        private System.Windows.Forms.GroupBox conditionBuilderGroup;
        private System.Windows.Forms.TableLayoutPanel conditionBuilderLayout;
        private System.Windows.Forms.Label lblFilterType;
        private System.Windows.Forms.ComboBox cboFilterType;
        private System.Windows.Forms.Label lblConditionFieldSearch;
        private System.Windows.Forms.TextBox txtConditionFieldSearch;
        private System.Windows.Forms.Label lblConditionField;
        private System.Windows.Forms.ComboBox cboConditionAttribute;
        private System.Windows.Forms.Label lblConditionOperator;
        private System.Windows.Forms.ComboBox cboConditionOperator;
        private System.Windows.Forms.Label lblConditionValue;
        private System.Windows.Forms.TextBox txtConditionValue;
        private System.Windows.Forms.Button btnPickConditionValue;
        private System.Windows.Forms.FlowLayoutPanel conditionButtonPanel;
        private System.Windows.Forms.Button btnAddCondition;
        private System.Windows.Forms.Button btnRemoveCondition;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Label lblConditionValueHint;
        private System.Windows.Forms.Label lblFilterHint;
        private System.Windows.Forms.ListView lvConditions;
        private System.Windows.Forms.ColumnHeader colConditionField;
        private System.Windows.Forms.ColumnHeader colConditionOperator;
        private System.Windows.Forms.ColumnHeader colConditionValue;
        private System.Windows.Forms.TextBox txtFilterXml;
        private System.Windows.Forms.Button btnToggleAdvanced;
        private System.Windows.Forms.FlowLayoutPanel advancedButtonsPanel;
        private System.Windows.Forms.Button btnSaveFilterXml;
        private System.Windows.Forms.TableLayoutPanel bottomLayout;
        private System.Windows.Forms.GroupBox activeMonitorsGroup;
        private System.Windows.Forms.TableLayoutPanel activeMonitorsLayout;
        private System.Windows.Forms.FlowLayoutPanel activeMonitorButtonPanel;
        private System.Windows.Forms.Button btnStopSelectedMonitor;
        private System.Windows.Forms.Button btnRemoveSelectedMonitors;
        private System.Windows.Forms.Button btnPauseSelectedMonitors;
        private System.Windows.Forms.Button btnSelectAllMonitors;
        private System.Windows.Forms.Button btnExportMonitors;
        private System.Windows.Forms.Button btnImportMonitors;
        private System.Windows.Forms.Button btnEditMonitor;
        private System.Windows.Forms.ListView lvActiveMonitors;
        private System.Windows.Forms.ColumnHeader colActiveEntity;
        private System.Windows.Forms.ColumnHeader colActiveName;
        private System.Windows.Forms.ColumnHeader colActiveColumns;
        private System.Windows.Forms.ColumnHeader colActiveInterval;
        private System.Windows.Forms.ColumnHeader colActiveStatus;
        private System.Windows.Forms.ColumnHeader colActiveFilter;
        private System.Windows.Forms.GroupBox eventsGroup;
        private System.Windows.Forms.TabControl eventsTabs;
        private System.Windows.Forms.TabPage recentChangesTab;
        private System.Windows.Forms.TabPage technicalLogTab;
        private System.Windows.Forms.ListView lvRecentChanges;
        private System.Windows.Forms.ColumnHeader colChangeModifiedOn;
        private System.Windows.Forms.ColumnHeader colChangeRecordId;
        private System.Windows.Forms.ColumnHeader colChangeModifiedBy;
        private System.Windows.Forms.ColumnHeader colChangeRecordName;
        private System.Windows.Forms.ColumnHeader colChangeField;
        private System.Windows.Forms.ColumnHeader colChangeValues;
        private System.Windows.Forms.ColumnHeader colChangeMonitor;
        private System.Windows.Forms.ListBox lstEvents;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}
