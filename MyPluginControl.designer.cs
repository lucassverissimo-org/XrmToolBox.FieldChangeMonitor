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
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.configurationGroup = new System.Windows.Forms.GroupBox();
            this.configurationLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblEntity = new System.Windows.Forms.Label();
            this.txtEntityLogicalName = new System.Windows.Forms.TextBox();
            this.btnLoadColumns = new System.Windows.Forms.Button();
            this.lblInterval = new System.Windows.Forms.Label();
            this.nudIntervalSeconds = new System.Windows.Forms.NumericUpDown();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.contentLayout = new System.Windows.Forms.TableLayoutPanel();
            this.columnsGroup = new System.Windows.Forms.GroupBox();
            this.columnsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.columnSearchLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblColumnSearch = new System.Windows.Forms.Label();
            this.txtColumnSearch = new System.Windows.Forms.TextBox();
            this.columnsButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSelectAllColumns = new System.Windows.Forms.Button();
            this.btnClearColumnSelection = new System.Windows.Forms.Button();
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
            this.txtFilterXml = new System.Windows.Forms.TextBox();
            this.bottomLayout = new System.Windows.Forms.TableLayoutPanel();
            this.activeMonitorsGroup = new System.Windows.Forms.GroupBox();
            this.activeMonitorsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.activeMonitorButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnStopSelectedMonitor = new System.Windows.Forms.Button();
            this.lvActiveMonitors = new System.Windows.Forms.ListView();
            this.colActiveEntity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActiveColumns = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActiveInterval = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActiveStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActiveFilter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eventsGroup = new System.Windows.Forms.GroupBox();
            this.lstEvents = new System.Windows.Forms.ListBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolStripMenu.SuspendLayout();
            this.mainLayout.SuspendLayout();
            this.configurationGroup.SuspendLayout();
            this.configurationLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalSeconds)).BeginInit();
            this.contentLayout.SuspendLayout();
            this.columnsGroup.SuspendLayout();
            this.columnsLayout.SuspendLayout();
            this.columnSearchLayout.SuspendLayout();
            this.columnsButtonPanel.SuspendLayout();
            this.filterGroup.SuspendLayout();
            this.filterLayout.SuspendLayout();
            this.conditionBuilderGroup.SuspendLayout();
            this.conditionBuilderLayout.SuspendLayout();
            this.conditionButtonPanel.SuspendLayout();
            this.bottomLayout.SuspendLayout();
            this.activeMonitorsGroup.SuspendLayout();
            this.activeMonitorsLayout.SuspendLayout();
            this.activeMonitorButtonPanel.SuspendLayout();
            this.eventsGroup.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripMenu.Size = new System.Drawing.Size(1180, 27);
            this.toolStripMenu.TabIndex = 0;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(107, 24);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.configurationGroup, 0, 0);
            this.mainLayout.Controls.Add(this.contentLayout, 0, 1);
            this.mainLayout.Controls.Add(this.bottomLayout, 0, 2);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 27);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(12);
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 64F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this.mainLayout.Size = new System.Drawing.Size(1180, 767);
            this.mainLayout.TabIndex = 1;
            // 
            // configurationGroup
            // 
            this.configurationGroup.Controls.Add(this.configurationLayout);
            this.configurationGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configurationGroup.Location = new System.Drawing.Point(15, 15);
            this.configurationGroup.Name = "configurationGroup";
            this.configurationGroup.Padding = new System.Windows.Forms.Padding(12);
            this.configurationGroup.Size = new System.Drawing.Size(1150, 86);
            this.configurationGroup.TabIndex = 0;
            this.configurationGroup.TabStop = false;
            this.configurationGroup.Text = "Configuracao";
            // 
            // configurationLayout
            // 
            this.configurationLayout.ColumnCount = 7;
            this.configurationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.configurationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.configurationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.configurationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.configurationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.configurationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.configurationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.configurationLayout.Controls.Add(this.lblEntity, 0, 0);
            this.configurationLayout.Controls.Add(this.txtEntityLogicalName, 1, 0);
            this.configurationLayout.Controls.Add(this.btnLoadColumns, 2, 0);
            this.configurationLayout.Controls.Add(this.lblInterval, 3, 0);
            this.configurationLayout.Controls.Add(this.nudIntervalSeconds, 4, 0);
            this.configurationLayout.Controls.Add(this.btnStart, 5, 0);
            this.configurationLayout.Controls.Add(this.btnStop, 6, 0);
            this.configurationLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configurationLayout.Location = new System.Drawing.Point(12, 27);
            this.configurationLayout.Name = "configurationLayout";
            this.configurationLayout.RowCount = 1;
            this.configurationLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.configurationLayout.Size = new System.Drawing.Size(1126, 47);
            this.configurationLayout.TabIndex = 0;
            // 
            // lblEntity
            // 
            this.lblEntity.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblEntity.AutoSize = true;
            this.lblEntity.Location = new System.Drawing.Point(3, 15);
            this.lblEntity.Name = "lblEntity";
            this.lblEntity.Size = new System.Drawing.Size(134, 17);
            this.lblEntity.TabIndex = 0;
            this.lblEntity.Text = "Entidade (logical)";
            // 
            // txtEntityLogicalName
            // 
            this.txtEntityLogicalName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEntityLogicalName.Location = new System.Drawing.Point(148, 12);
            this.txtEntityLogicalName.Name = "txtEntityLogicalName";
            this.txtEntityLogicalName.Size = new System.Drawing.Size(340, 22);
            this.txtEntityLogicalName.TabIndex = 1;
            // 
            // btnLoadColumns
            // 
            this.btnLoadColumns.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLoadColumns.Location = new System.Drawing.Point(497, 7);
            this.btnLoadColumns.Name = "btnLoadColumns";
            this.btnLoadColumns.Size = new System.Drawing.Size(138, 32);
            this.btnLoadColumns.TabIndex = 2;
            this.btnLoadColumns.Text = "Carregar colunas";
            this.btnLoadColumns.UseVisualStyleBackColor = true;
            this.btnLoadColumns.Click += new System.EventHandler(this.btnLoadColumns_Click);
            // 
            // lblInterval
            // 
            this.lblInterval.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(644, 15);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(128, 17);
            this.lblInterval.TabIndex = 3;
            this.lblInterval.Text = "Intervalo (segundos)";
            // 
            // nudIntervalSeconds
            // 
            this.nudIntervalSeconds.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudIntervalSeconds.Location = new System.Drawing.Point(789, 12);
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
            this.nudIntervalSeconds.Size = new System.Drawing.Size(70, 22);
            this.nudIntervalSeconds.TabIndex = 4;
            this.nudIntervalSeconds.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnStart
            // 
            this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStart.Location = new System.Drawing.Point(871, 7);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(132, 32);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Adicionar escuta";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStop.Location = new System.Drawing.Point(1024, 7);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(94, 32);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Parar todos";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // contentLayout
            // 
            this.contentLayout.ColumnCount = 2;
            this.contentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.contentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66F));
            this.contentLayout.Controls.Add(this.columnsGroup, 0, 0);
            this.contentLayout.Controls.Add(this.filterGroup, 1, 0);
            this.contentLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLayout.Location = new System.Drawing.Point(15, 107);
            this.contentLayout.Name = "contentLayout";
            this.contentLayout.RowCount = 1;
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayout.Size = new System.Drawing.Size(1150, 414);
            this.contentLayout.TabIndex = 1;
            // 
            // columnsGroup
            // 
            this.columnsGroup.Controls.Add(this.columnsLayout);
            this.columnsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsGroup.Location = new System.Drawing.Point(3, 3);
            this.columnsGroup.Name = "columnsGroup";
            this.columnsGroup.Padding = new System.Windows.Forms.Padding(10);
            this.columnsGroup.Size = new System.Drawing.Size(385, 408);
            this.columnsGroup.TabIndex = 0;
            this.columnsGroup.TabStop = false;
            this.columnsGroup.Text = "Colunas para monitorar";
            // 
            // columnsLayout
            // 
            this.columnsLayout.ColumnCount = 1;
            this.columnsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.columnsLayout.Controls.Add(this.columnSearchLayout, 0, 0);
            this.columnsLayout.Controls.Add(this.columnsButtonPanel, 0, 1);
            this.columnsLayout.Controls.Add(this.clbColumns, 0, 2);
            this.columnsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsLayout.Location = new System.Drawing.Point(10, 25);
            this.columnsLayout.Name = "columnsLayout";
            this.columnsLayout.RowCount = 3;
            this.columnsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.columnsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.columnsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.columnsLayout.Size = new System.Drawing.Size(365, 373);
            this.columnsLayout.TabIndex = 0;
            // 
            // columnSearchLayout
            // 
            this.columnSearchLayout.ColumnCount = 2;
            this.columnSearchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.columnSearchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.columnSearchLayout.Controls.Add(this.lblColumnSearch, 0, 0);
            this.columnSearchLayout.Controls.Add(this.txtColumnSearch, 1, 0);
            this.columnSearchLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnSearchLayout.Location = new System.Drawing.Point(0, 0);
            this.columnSearchLayout.Margin = new System.Windows.Forms.Padding(0);
            this.columnSearchLayout.Name = "columnSearchLayout";
            this.columnSearchLayout.RowCount = 1;
            this.columnSearchLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.columnSearchLayout.Size = new System.Drawing.Size(365, 36);
            this.columnSearchLayout.TabIndex = 0;
            // 
            // lblColumnSearch
            // 
            this.lblColumnSearch.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblColumnSearch.AutoSize = true;
            this.lblColumnSearch.Location = new System.Drawing.Point(3, 9);
            this.lblColumnSearch.Name = "lblColumnSearch";
            this.lblColumnSearch.Size = new System.Drawing.Size(51, 17);
            this.lblColumnSearch.TabIndex = 0;
            this.lblColumnSearch.Text = "Buscar";
            // 
            // txtColumnSearch
            // 
            this.txtColumnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColumnSearch.Location = new System.Drawing.Point(68, 7);
            this.txtColumnSearch.Name = "txtColumnSearch";
            this.txtColumnSearch.Size = new System.Drawing.Size(294, 22);
            this.txtColumnSearch.TabIndex = 1;
            this.txtColumnSearch.TextChanged += new System.EventHandler(this.txtColumnSearch_TextChanged);
            // 
            // columnsButtonPanel
            // 
            this.columnsButtonPanel.Controls.Add(this.btnSelectAllColumns);
            this.columnsButtonPanel.Controls.Add(this.btnClearColumnSelection);
            this.columnsButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsButtonPanel.Location = new System.Drawing.Point(0, 36);
            this.columnsButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.columnsButtonPanel.Name = "columnsButtonPanel";
            this.columnsButtonPanel.Size = new System.Drawing.Size(365, 40);
            this.columnsButtonPanel.TabIndex = 1;
            // 
            // btnSelectAllColumns
            // 
            this.btnSelectAllColumns.Location = new System.Drawing.Point(3, 3);
            this.btnSelectAllColumns.Name = "btnSelectAllColumns";
            this.btnSelectAllColumns.Size = new System.Drawing.Size(115, 30);
            this.btnSelectAllColumns.TabIndex = 0;
            this.btnSelectAllColumns.Text = "Selecionar tudo";
            this.btnSelectAllColumns.UseVisualStyleBackColor = true;
            this.btnSelectAllColumns.Click += new System.EventHandler(this.btnSelectAllColumns_Click);
            // 
            // btnClearColumnSelection
            // 
            this.btnClearColumnSelection.Location = new System.Drawing.Point(124, 3);
            this.btnClearColumnSelection.Name = "btnClearColumnSelection";
            this.btnClearColumnSelection.Size = new System.Drawing.Size(100, 30);
            this.btnClearColumnSelection.TabIndex = 1;
            this.btnClearColumnSelection.Text = "Limpar";
            this.btnClearColumnSelection.UseVisualStyleBackColor = true;
            this.btnClearColumnSelection.Click += new System.EventHandler(this.btnClearColumnSelection_Click);
            // 
            // clbColumns
            // 
            this.clbColumns.CheckOnClick = true;
            this.clbColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbColumns.FormattingEnabled = true;
            this.clbColumns.IntegralHeight = false;
            this.clbColumns.Location = new System.Drawing.Point(3, 79);
            this.clbColumns.Name = "clbColumns";
            this.clbColumns.Size = new System.Drawing.Size(359, 291);
            this.clbColumns.TabIndex = 2;
            this.clbColumns.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbColumns_ItemCheck);
            // 
            // filterGroup
            // 
            this.filterGroup.Controls.Add(this.filterLayout);
            this.filterGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterGroup.Location = new System.Drawing.Point(394, 3);
            this.filterGroup.Name = "filterGroup";
            this.filterGroup.Padding = new System.Windows.Forms.Padding(10);
            this.filterGroup.Size = new System.Drawing.Size(753, 408);
            this.filterGroup.TabIndex = 1;
            this.filterGroup.TabStop = false;
            this.filterGroup.Text = "Construtor de condicoes e Filtro FetchXML";
            // 
            // filterLayout
            // 
            this.filterLayout.ColumnCount = 1;
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterLayout.Controls.Add(this.conditionBuilderGroup, 0, 0);
            this.filterLayout.Controls.Add(this.lblFilterHint, 0, 1);
            this.filterLayout.Controls.Add(this.lvConditions, 0, 2);
            this.filterLayout.Controls.Add(this.txtFilterXml, 0, 3);
            this.filterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterLayout.Location = new System.Drawing.Point(10, 25);
            this.filterLayout.Name = "filterLayout";
            this.filterLayout.RowCount = 4;
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 158F));
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterLayout.Size = new System.Drawing.Size(733, 373);
            this.filterLayout.TabIndex = 0;
            // 
            // conditionBuilderGroup
            // 
            this.conditionBuilderGroup.Controls.Add(this.conditionBuilderLayout);
            this.conditionBuilderGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditionBuilderGroup.Location = new System.Drawing.Point(3, 3);
            this.conditionBuilderGroup.Name = "conditionBuilderGroup";
            this.conditionBuilderGroup.Padding = new System.Windows.Forms.Padding(10);
            this.conditionBuilderGroup.Size = new System.Drawing.Size(727, 152);
            this.conditionBuilderGroup.TabIndex = 0;
            this.conditionBuilderGroup.TabStop = false;
            this.conditionBuilderGroup.Text = "Nova condicao";
            // 
            // conditionBuilderLayout
            // 
            this.conditionBuilderLayout.ColumnCount = 6;
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.conditionBuilderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 232F));
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
            this.conditionBuilderLayout.Location = new System.Drawing.Point(10, 25);
            this.conditionBuilderLayout.Name = "conditionBuilderLayout";
            this.conditionBuilderLayout.RowCount = 4;
            this.conditionBuilderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.conditionBuilderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.conditionBuilderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.conditionBuilderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.conditionBuilderLayout.Size = new System.Drawing.Size(707, 117);
            this.conditionBuilderLayout.TabIndex = 0;
            // 
            // lblFilterType
            // 
            this.lblFilterType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFilterType.AutoSize = true;
            this.lblFilterType.Location = new System.Drawing.Point(3, 7);
            this.lblFilterType.Name = "lblFilterType";
            this.lblFilterType.Size = new System.Drawing.Size(38, 17);
            this.lblFilterType.TabIndex = 0;
            this.lblFilterType.Text = "Tipo";
            // 
            // cboFilterType
            // 
            this.cboFilterType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterType.FormattingEnabled = true;
            this.cboFilterType.Location = new System.Drawing.Point(73, 3);
            this.cboFilterType.Name = "cboFilterType";
            this.cboFilterType.Size = new System.Drawing.Size(99, 24);
            this.cboFilterType.TabIndex = 1;
            this.cboFilterType.SelectedIndexChanged += new System.EventHandler(this.cboFilterType_SelectedIndexChanged);
            // 
            // lblConditionFieldSearch
            // 
            this.lblConditionFieldSearch.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblConditionFieldSearch.AutoSize = true;
            this.lblConditionFieldSearch.Location = new System.Drawing.Point(178, 7);
            this.lblConditionFieldSearch.Name = "lblConditionFieldSearch";
            this.lblConditionFieldSearch.Size = new System.Drawing.Size(51, 17);
            this.lblConditionFieldSearch.TabIndex = 2;
            this.lblConditionFieldSearch.Text = "Buscar";
            // 
            // txtConditionFieldSearch
            // 
            this.txtConditionFieldSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionBuilderLayout.SetColumnSpan(this.txtConditionFieldSearch, 3);
            this.txtConditionFieldSearch.Location = new System.Drawing.Point(258, 4);
            this.txtConditionFieldSearch.Name = "txtConditionFieldSearch";
            this.txtConditionFieldSearch.Size = new System.Drawing.Size(446, 22);
            this.txtConditionFieldSearch.TabIndex = 3;
            this.txtConditionFieldSearch.TextChanged += new System.EventHandler(this.txtConditionFieldSearch_TextChanged);
            // 
            // lblConditionField
            // 
            this.lblConditionField.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblConditionField.AutoSize = true;
            this.lblConditionField.Location = new System.Drawing.Point(3, 38);
            this.lblConditionField.Name = "lblConditionField";
            this.lblConditionField.Size = new System.Drawing.Size(52, 17);
            this.lblConditionField.TabIndex = 4;
            this.lblConditionField.Text = "Campo";
            // 
            // cboConditionAttribute
            // 
            this.cboConditionAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionBuilderLayout.SetColumnSpan(this.cboConditionAttribute, 3);
            this.cboConditionAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConditionAttribute.FormattingEnabled = true;
            this.cboConditionAttribute.Location = new System.Drawing.Point(73, 34);
            this.cboConditionAttribute.Name = "cboConditionAttribute";
            this.cboConditionAttribute.Size = new System.Drawing.Size(294, 24);
            this.cboConditionAttribute.TabIndex = 5;
            this.cboConditionAttribute.SelectedIndexChanged += new System.EventHandler(this.cboConditionAttribute_SelectedIndexChanged);
            // 
            // lblConditionOperator
            // 
            this.lblConditionOperator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblConditionOperator.AutoSize = true;
            this.lblConditionOperator.Location = new System.Drawing.Point(373, 38);
            this.lblConditionOperator.Name = "lblConditionOperator";
            this.lblConditionOperator.Size = new System.Drawing.Size(69, 17);
            this.lblConditionOperator.TabIndex = 6;
            this.lblConditionOperator.Text = "Operador";
            // 
            // cboConditionOperator
            // 
            this.cboConditionOperator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboConditionOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConditionOperator.FormattingEnabled = true;
            this.cboConditionOperator.Location = new System.Drawing.Point(478, 34);
            this.cboConditionOperator.Name = "cboConditionOperator";
            this.cboConditionOperator.Size = new System.Drawing.Size(226, 24);
            this.cboConditionOperator.TabIndex = 7;
            this.cboConditionOperator.SelectedIndexChanged += new System.EventHandler(this.cboConditionOperator_SelectedIndexChanged);
            // 
            // lblConditionValue
            // 
            this.lblConditionValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblConditionValue.AutoSize = true;
            this.lblConditionValue.Location = new System.Drawing.Point(3, 70);
            this.lblConditionValue.Name = "lblConditionValue";
            this.lblConditionValue.Size = new System.Drawing.Size(41, 17);
            this.lblConditionValue.TabIndex = 8;
            this.lblConditionValue.Text = "Valor";
            // 
            // txtConditionValue
            // 
            this.txtConditionValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionBuilderLayout.SetColumnSpan(this.txtConditionValue, 3);
            this.txtConditionValue.Location = new System.Drawing.Point(73, 68);
            this.txtConditionValue.Name = "txtConditionValue";
            this.txtConditionValue.Size = new System.Drawing.Size(294, 22);
            this.txtConditionValue.TabIndex = 9;
            // 
            // btnPickConditionValue
            // 
            this.btnPickConditionValue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPickConditionValue.Location = new System.Drawing.Point(378, 64);
            this.btnPickConditionValue.Name = "btnPickConditionValue";
            this.btnPickConditionValue.Size = new System.Drawing.Size(89, 28);
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
            this.conditionButtonPanel.Location = new System.Drawing.Point(475, 62);
            this.conditionButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.conditionButtonPanel.Name = "conditionButtonPanel";
            this.conditionButtonPanel.Size = new System.Drawing.Size(232, 34);
            this.conditionButtonPanel.TabIndex = 11;
            // 
            // btnAddCondition
            // 
            this.btnAddCondition.Location = new System.Drawing.Point(3, 2);
            this.btnAddCondition.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAddCondition.Name = "btnAddCondition";
            this.btnAddCondition.Size = new System.Drawing.Size(80, 30);
            this.btnAddCondition.TabIndex = 0;
            this.btnAddCondition.Text = "Adicionar";
            this.btnAddCondition.UseVisualStyleBackColor = true;
            this.btnAddCondition.Click += new System.EventHandler(this.btnAddCondition_Click);
            // 
            // btnRemoveCondition
            // 
            this.btnRemoveCondition.Location = new System.Drawing.Point(89, 2);
            this.btnRemoveCondition.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRemoveCondition.Name = "btnRemoveCondition";
            this.btnRemoveCondition.Size = new System.Drawing.Size(72, 30);
            this.btnRemoveCondition.TabIndex = 1;
            this.btnRemoveCondition.Text = "Remover";
            this.btnRemoveCondition.UseVisualStyleBackColor = true;
            this.btnRemoveCondition.Click += new System.EventHandler(this.btnRemoveCondition_Click);
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.Location = new System.Drawing.Point(167, 2);
            this.btnClearFilter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(58, 30);
            this.btnClearFilter.TabIndex = 2;
            this.btnClearFilter.Text = "Limpar";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            // 
            // lblConditionValueHint
            // 
            this.conditionBuilderLayout.SetColumnSpan(this.lblConditionValueHint, 5);
            this.lblConditionValueHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblConditionValueHint.Location = new System.Drawing.Point(73, 96);
            this.lblConditionValueHint.Name = "lblConditionValueHint";
            this.lblConditionValueHint.Size = new System.Drawing.Size(631, 21);
            this.lblConditionValueHint.TabIndex = 12;
            this.lblConditionValueHint.Text = "Carregue uma entidade para criar condicoes.";
            this.lblConditionValueHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFilterHint
            // 
            this.lblFilterHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFilterHint.Location = new System.Drawing.Point(3, 158);
            this.lblFilterHint.Name = "lblFilterHint";
            this.lblFilterHint.Size = new System.Drawing.Size(727, 30);
            this.lblFilterHint.TabIndex = 1;
            this.lblFilterHint.Text = "O XML abaixo e gerado pelas condicoes, mas tambem pode ser editado manualmente.";
            this.lblFilterHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.lvConditions.Location = new System.Drawing.Point(3, 191);
            this.lvConditions.Name = "lvConditions";
            this.lvConditions.Size = new System.Drawing.Size(727, 98);
            this.lvConditions.TabIndex = 2;
            this.lvConditions.UseCompatibleStateImageBehavior = false;
            this.lvConditions.View = System.Windows.Forms.View.Details;
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
            // txtFilterXml
            // 
            this.txtFilterXml.AcceptsReturn = true;
            this.txtFilterXml.AcceptsTab = true;
            this.txtFilterXml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilterXml.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtFilterXml.Location = new System.Drawing.Point(3, 295);
            this.txtFilterXml.Multiline = true;
            this.txtFilterXml.Name = "txtFilterXml";
            this.txtFilterXml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFilterXml.Size = new System.Drawing.Size(727, 75);
            this.txtFilterXml.TabIndex = 3;
            this.txtFilterXml.WordWrap = false;
            // 
            // bottomLayout
            // 
            this.bottomLayout.ColumnCount = 2;
            this.bottomLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.bottomLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.bottomLayout.Controls.Add(this.activeMonitorsGroup, 0, 0);
            this.bottomLayout.Controls.Add(this.eventsGroup, 1, 0);
            this.bottomLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomLayout.Location = new System.Drawing.Point(15, 527);
            this.bottomLayout.Name = "bottomLayout";
            this.bottomLayout.RowCount = 1;
            this.bottomLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bottomLayout.Size = new System.Drawing.Size(1150, 225);
            this.bottomLayout.TabIndex = 2;
            // 
            // activeMonitorsGroup
            // 
            this.activeMonitorsGroup.Controls.Add(this.activeMonitorsLayout);
            this.activeMonitorsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeMonitorsGroup.Location = new System.Drawing.Point(3, 3);
            this.activeMonitorsGroup.Name = "activeMonitorsGroup";
            this.activeMonitorsGroup.Padding = new System.Windows.Forms.Padding(10);
            this.activeMonitorsGroup.Size = new System.Drawing.Size(477, 219);
            this.activeMonitorsGroup.TabIndex = 0;
            this.activeMonitorsGroup.TabStop = false;
            this.activeMonitorsGroup.Text = "Monitoramentos ativos";
            // 
            // activeMonitorsLayout
            // 
            this.activeMonitorsLayout.ColumnCount = 1;
            this.activeMonitorsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.activeMonitorsLayout.Controls.Add(this.activeMonitorButtonPanel, 0, 0);
            this.activeMonitorsLayout.Controls.Add(this.lvActiveMonitors, 0, 1);
            this.activeMonitorsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeMonitorsLayout.Location = new System.Drawing.Point(10, 25);
            this.activeMonitorsLayout.Name = "activeMonitorsLayout";
            this.activeMonitorsLayout.RowCount = 2;
            this.activeMonitorsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.activeMonitorsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.activeMonitorsLayout.Size = new System.Drawing.Size(457, 184);
            this.activeMonitorsLayout.TabIndex = 0;
            // 
            // activeMonitorButtonPanel
            // 
            this.activeMonitorButtonPanel.Controls.Add(this.btnStopSelectedMonitor);
            this.activeMonitorButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeMonitorButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.activeMonitorButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.activeMonitorButtonPanel.Name = "activeMonitorButtonPanel";
            this.activeMonitorButtonPanel.Size = new System.Drawing.Size(457, 38);
            this.activeMonitorButtonPanel.TabIndex = 0;
            // 
            // btnStopSelectedMonitor
            // 
            this.btnStopSelectedMonitor.Location = new System.Drawing.Point(3, 3);
            this.btnStopSelectedMonitor.Name = "btnStopSelectedMonitor";
            this.btnStopSelectedMonitor.Size = new System.Drawing.Size(145, 30);
            this.btnStopSelectedMonitor.TabIndex = 0;
            this.btnStopSelectedMonitor.Text = "Parar selecionado";
            this.btnStopSelectedMonitor.UseVisualStyleBackColor = true;
            this.btnStopSelectedMonitor.Click += new System.EventHandler(this.btnStopSelectedMonitor_Click);
            // 
            // lvActiveMonitors
            // 
            this.lvActiveMonitors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colActiveEntity,
            this.colActiveColumns,
            this.colActiveInterval,
            this.colActiveStatus,
            this.colActiveFilter});
            this.lvActiveMonitors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvActiveMonitors.FullRowSelect = true;
            this.lvActiveMonitors.GridLines = true;
            this.lvActiveMonitors.HideSelection = false;
            this.lvActiveMonitors.Location = new System.Drawing.Point(3, 41);
            this.lvActiveMonitors.Name = "lvActiveMonitors";
            this.lvActiveMonitors.Size = new System.Drawing.Size(451, 140);
            this.lvActiveMonitors.TabIndex = 1;
            this.lvActiveMonitors.UseCompatibleStateImageBehavior = false;
            this.lvActiveMonitors.View = System.Windows.Forms.View.Details;
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
            this.eventsGroup.Controls.Add(this.lstEvents);
            this.eventsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventsGroup.Location = new System.Drawing.Point(486, 3);
            this.eventsGroup.Name = "eventsGroup";
            this.eventsGroup.Padding = new System.Windows.Forms.Padding(10);
            this.eventsGroup.Size = new System.Drawing.Size(661, 219);
            this.eventsGroup.TabIndex = 1;
            this.eventsGroup.TabStop = false;
            this.eventsGroup.Text = "Eventos";
            // 
            // lstEvents
            // 
            this.lstEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEvents.FormattingEnabled = true;
            this.lstEvents.HorizontalScrollbar = true;
            this.lstEvents.IntegralHeight = false;
            this.lstEvents.ItemHeight = 16;
            this.lstEvents.Location = new System.Drawing.Point(10, 25);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(641, 184);
            this.lstEvents.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 794);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1180, 26);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(52, 20);
            this.tsslStatus.Text = "Pronto";
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = System.Drawing.SystemIcons.Information;
            this.notifyIcon.Text = "Monitor Dataverse";
            this.notifyIcon.Visible = false;
            // 
            // MyPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainLayout);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStripMenu);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1180, 820);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.OnCloseTool += new System.EventHandler(this.MyPluginControl_OnCloseTool);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.mainLayout.ResumeLayout(false);
            this.configurationGroup.ResumeLayout(false);
            this.configurationLayout.ResumeLayout(false);
            this.configurationLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalSeconds)).EndInit();
            this.contentLayout.ResumeLayout(false);
            this.columnsGroup.ResumeLayout(false);
            this.columnsLayout.ResumeLayout(false);
            this.columnSearchLayout.ResumeLayout(false);
            this.columnSearchLayout.PerformLayout();
            this.columnsButtonPanel.ResumeLayout(false);
            this.filterGroup.ResumeLayout(false);
            this.filterLayout.ResumeLayout(false);
            this.filterLayout.PerformLayout();
            this.conditionBuilderGroup.ResumeLayout(false);
            this.conditionBuilderLayout.ResumeLayout(false);
            this.conditionBuilderLayout.PerformLayout();
            this.conditionButtonPanel.ResumeLayout(false);
            this.bottomLayout.ResumeLayout(false);
            this.activeMonitorsGroup.ResumeLayout(false);
            this.activeMonitorsLayout.ResumeLayout(false);
            this.activeMonitorButtonPanel.ResumeLayout(false);
            this.eventsGroup.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMenu;
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
        private System.Windows.Forms.TableLayoutPanel contentLayout;
        private System.Windows.Forms.GroupBox columnsGroup;
        private System.Windows.Forms.TableLayoutPanel columnsLayout;
        private System.Windows.Forms.TableLayoutPanel columnSearchLayout;
        private System.Windows.Forms.Label lblColumnSearch;
        private System.Windows.Forms.TextBox txtColumnSearch;
        private System.Windows.Forms.FlowLayoutPanel columnsButtonPanel;
        private System.Windows.Forms.Button btnSelectAllColumns;
        private System.Windows.Forms.Button btnClearColumnSelection;
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
        private System.Windows.Forms.TableLayoutPanel bottomLayout;
        private System.Windows.Forms.GroupBox activeMonitorsGroup;
        private System.Windows.Forms.TableLayoutPanel activeMonitorsLayout;
        private System.Windows.Forms.FlowLayoutPanel activeMonitorButtonPanel;
        private System.Windows.Forms.Button btnStopSelectedMonitor;
        private System.Windows.Forms.ListView lvActiveMonitors;
        private System.Windows.Forms.ColumnHeader colActiveEntity;
        private System.Windows.Forms.ColumnHeader colActiveColumns;
        private System.Windows.Forms.ColumnHeader colActiveInterval;
        private System.Windows.Forms.ColumnHeader colActiveStatus;
        private System.Windows.Forms.ColumnHeader colActiveFilter;
        private System.Windows.Forms.GroupBox eventsGroup;
        private System.Windows.Forms.ListBox lstEvents;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}
