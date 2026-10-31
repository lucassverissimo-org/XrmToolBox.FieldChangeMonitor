using System;
using System.Drawing;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.Shared.Controls;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager
{
    partial class MappingControl
    {
        private System.ComponentModel.IContainer components = null;
        private Font activeStepFont;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                activeStepFont?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private TableLayoutPanel layout;
        private FlowLayoutPanel steps;
        private Button tablesStep;
        private Button mappingStep;
        private Button reviewStep;
        private Label stepPosition;
        private Panel pages;
        private Panel tablesPage;
        private TableLayoutPanel tablesLayout;
        private Label tablesTitle;
        private Label sourcePickerLabel;
        private Label targetPickerLabel;
        private FlowLayoutPanel loadActions;
        private FlowLayoutPanel continueActions;
        private Button loadTables;
        private GridPickerControl sourcePicker;
        private GridPickerControl targetPicker;
        private Button viewMappings;
        private TableLayoutPanel mappingPage;
        private TableLayoutPanel contextBar;
        private Label loadedPair;
        private Button moreActions;
        private ContextMenuStrip moreMenu;
        private ToolStripMenuItem publish;
        private ToolStripMenuItem solution;
        private ToolStripMenuItem helpMenu;
        private TableLayoutPanel selectionBar;
        private Label selectionSummary;
        private Button clearChecks;
        private Button addMapping;
        private TableLayoutPanel lists;
        private TableLayoutPanel sourcePanel;
        private TableLayoutPanel targetPanel;
        private TableLayoutPanel mappingPanel;
        private Label sourceLabel;
        private Label targetLabel;
        private Label mappingLabel;
        private TextBox sourceSearch;
        private TextBox targetSearch;
        private TextBox mappingSearch;
        private DataGridView sourceGrid;
        private DataGridView targetGrid;
        private DataGridView mappingGrid;
        private CheckBox compatibleOnly;
        private FlowLayoutPanel sourceActions;
        private FlowLayoutPanel targetActions;
        private Button refreshSource;
        private Button refreshTarget;
        private Button refreshMappings;
        private FlowLayoutPanel mappingActions;
        private Button editMapping;
        private Button removeMapping;
        private Button undoMapping;
        private TableLayoutPanel footer;
        private Label status;
        private FlowLayoutPanel footerActions;
        private Button saveChanges;
        private Button undoAll;
        private ToolTip tips;
        private ImageList actionImages;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            activeStepFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            layout = new TableLayoutPanel();
            steps = new FlowLayoutPanel();
            tablesStep = new Button();
            mappingStep = new Button();
            reviewStep = new Button();
            stepPosition = new Label();
            pages = new Panel();
            tablesPage = new Panel();
            tablesLayout = new TableLayoutPanel();
            tablesTitle = new Label();
            sourcePickerLabel = new Label();
            targetPickerLabel = new Label();
            loadActions = new FlowLayoutPanel();
            continueActions = new FlowLayoutPanel();
            loadTables = new Button();
            sourcePicker = new GridPickerControl();
            targetPicker = new GridPickerControl();
            viewMappings = new Button();
            mappingPage = new TableLayoutPanel();
            contextBar = new TableLayoutPanel();
            loadedPair = new Label();
            moreActions = new Button();
            moreMenu = new ContextMenuStrip(components);
            publish = new ToolStripMenuItem();
            solution = new ToolStripMenuItem();
            helpMenu = new ToolStripMenuItem();
            selectionBar = new TableLayoutPanel();
            selectionSummary = new Label();
            clearChecks = new Button();
            addMapping = new Button();
            lists = new TableLayoutPanel();
            sourcePanel = new TableLayoutPanel();
            targetPanel = new TableLayoutPanel();
            mappingPanel = new TableLayoutPanel();
            sourceLabel = new Label();
            targetLabel = new Label();
            mappingLabel = new Label();
            sourceSearch = new TextBox();
            targetSearch = new TextBox();
            mappingSearch = new TextBox();
            sourceGrid = new DataGridView();
            targetGrid = new DataGridView();
            mappingGrid = new DataGridView();
            compatibleOnly = new CheckBox();
            sourceActions = new FlowLayoutPanel();
            targetActions = new FlowLayoutPanel();
            refreshSource = new Button();
            refreshTarget = new Button();
            refreshMappings = new Button();
            mappingActions = new FlowLayoutPanel();
            editMapping = new Button();
            removeMapping = new Button();
            undoMapping = new Button();
            footer = new TableLayoutPanel();
            status = new Label();
            footerActions = new FlowLayoutPanel();
            saveChanges = new Button();
            undoAll = new Button();
            tips = new ToolTip(components);
            actionImages = new ImageList(components);
            layout.Name = "layout";
            steps.Name = "steps";
            tablesStep.Name = "tablesStep";
            mappingStep.Name = "mappingStep";
            reviewStep.Name = "reviewStep";
            stepPosition.Name = "stepPosition";
            pages.Name = "pages";
            tablesPage.Name = "tablesPage";
            tablesLayout.Name = "tablesLayout";
            tablesTitle.Name = "tablesTitle";
            sourcePickerLabel.Name = "sourcePickerLabel";
            targetPickerLabel.Name = "targetPickerLabel";
            loadActions.Name = "loadActions";
            continueActions.Name = "continueActions";
            loadTables.Name = "loadTables";
            sourcePicker.Name = "sourcePicker";
            targetPicker.Name = "targetPicker";
            viewMappings.Name = "viewMappings";
            mappingPage.Name = "mappingPage";
            contextBar.Name = "contextBar";
            loadedPair.Name = "loadedPair";
            moreActions.Name = "moreActions";
            moreMenu.Name = "moreMenu";
            publish.Name = "publish";
            solution.Name = "solution";
            helpMenu.Name = "helpMenu";
            selectionBar.Name = "selectionBar";
            selectionSummary.Name = "selectionSummary";
            clearChecks.Name = "clearChecks";
            addMapping.Name = "addMapping";
            lists.Name = "lists";
            sourcePanel.Name = "sourcePanel";
            targetPanel.Name = "targetPanel";
            mappingPanel.Name = "mappingPanel";
            sourceLabel.Name = "sourceLabel";
            targetLabel.Name = "targetLabel";
            mappingLabel.Name = "mappingLabel";
            sourceSearch.Name = "sourceSearch";
            targetSearch.Name = "targetSearch";
            mappingSearch.Name = "mappingSearch";
            sourceGrid.Name = "sourceGrid";
            targetGrid.Name = "targetGrid";
            mappingGrid.Name = "mappingGrid";
            compatibleOnly.Name = "compatibleOnly";
            mappingActions.Name = "mappingActions";
            editMapping.Name = "editMapping";
            removeMapping.Name = "removeMapping";
            undoMapping.Name = "undoMapping";
            footer.Name = "footer";
            status.Name = "status";
            footerActions.Name = "footerActions";
            saveChanges.Name = "saveChanges";
            undoAll.Name = "undoAll";
            SuspendLayout();
            Name = "MappingControl";
            Size = new Size(1280, 780);
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 9F);
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
            actionImages.ColorDepth = ColorDepth.Depth32Bit;
            actionImages.ImageSize = new Size(20, 20);
            actionImages.TransparentColor = Color.Transparent;
            actionImages.Images.Add("tables", PluginImages.CreateActionImage("\uE80A"));
            actionImages.Images.Add("refresh", PluginImages.CreateActionImage("\uE72C"));
            actionImages.Images.Add("next", PluginImages.CreateActionImage("\uE72A"));
            actionImages.Images.Add("compare", PluginImages.CreateActionImage("\uE8AB"));
            actionImages.Images.Add("save", PluginImages.CreateActionImage("\uE74E"));
            actionImages.Images.Add("add", PluginImages.CreateActionImage("\uE710"));
            actionImages.Images.Add("edit", PluginImages.CreateActionImage("\uE70F"));
            actionImages.Images.Add("delete", PluginImages.CreateActionImage("\uE74D"));
            actionImages.Images.Add("undo", PluginImages.CreateActionImage("\uE7A7"));
            actionImages.Images.Add("clear", PluginImages.CreateActionImage("\uE894"));
            actionImages.Images.Add("more", PluginImages.CreateActionImage("\uE712"));
            actionImages.Images.Add("help", PluginImages.CreateActionImage("\uE897"));
            actionImages.Images.Add("solution", PluginImages.CreateActionImage("\uE7B8"));
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(12);
            layout.ColumnCount = 1;
            layout.RowCount = 3;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            steps.Dock = DockStyle.Fill;
            steps.WrapContents = false;
            steps.AutoScroll = true;
            tablesStep.Text = "Tabelas";
            tablesStep.Size = new Size(155, 34);
            tablesStep.Click += TablesStepClick;
            mappingStep.Text = "Mapear campos";
            mappingStep.Size = new Size(190, 34);
            mappingStep.Click += MappingStepClick;
            reviewStep.Text = "Revisar e salvar";
            reviewStep.Size = new Size(195, 34);
            reviewStep.Click += ReviewStepClick;
            stepPosition.AutoSize = true;
            stepPosition.Margin = new Padding(16, 11, 0, 0);
            steps.Controls.Add(tablesStep);
            steps.Controls.Add(mappingStep);
            steps.Controls.Add(reviewStep);
            steps.Controls.Add(stepPosition);
            pages.Dock = DockStyle.Fill;
            tablesPage.Dock = DockStyle.Fill;
            tablesLayout.Dock = DockStyle.Top;
            tablesLayout.Height = 270;
            tablesLayout.Padding = new Padding(12, 18, 12, 0);
            tablesLayout.ColumnCount = 2;
            tablesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tablesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tablesLayout.RowCount = 5;
            tablesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tablesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tablesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tablesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tablesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            tablesTitle.Text = "Escolha as tabelas";
            tablesTitle.Font = activeStepFont;
            tablesTitle.Dock = DockStyle.Fill;
            tablesLayout.Controls.Add(tablesTitle, 0, 0);
            tablesLayout.SetColumnSpan(tablesTitle, 2);
            loadActions.Dock = DockStyle.Fill;
            loadTables.Text = "Carregar tabelas";
            loadTables.Size = new Size(175, 32);
            loadTables.Click += LoadTablesClick;
            loadActions.Controls.Add(loadTables);
            tablesLayout.Controls.Add(loadActions, 0, 1);
            tablesLayout.SetColumnSpan(loadActions, 2);
            sourcePickerLabel.Text = "Tabela de origem";
            sourcePickerLabel.Dock = DockStyle.Fill;
            targetPickerLabel.Text = "Tabela de destino";
            targetPickerLabel.Dock = DockStyle.Fill;
            sourcePicker.Dock = DockStyle.Fill;
            sourcePicker.Margin = new Padding(3, 3, 16, 3);
            sourcePicker.SelectedItemChanged += SourcePickerChanged;
            targetPicker.Dock = DockStyle.Fill;
            targetPicker.SelectedItemChanged += TargetPickerChanged;
            tablesLayout.Controls.Add(sourcePickerLabel, 0, 2);
            tablesLayout.Controls.Add(targetPickerLabel, 1, 2);
            tablesLayout.Controls.Add(sourcePicker, 0, 3);
            tablesLayout.Controls.Add(targetPicker, 1, 3);
            continueActions.Dock = DockStyle.Fill;
            continueActions.FlowDirection = FlowDirection.RightToLeft;
            continueActions.Padding = new Padding(0, 12, 0, 0);
            viewMappings.Text = "Continuar para mapeamentos";
            viewMappings.Size = new Size(265, 34);
            viewMappings.Click += ViewMappingsClick;
            continueActions.Controls.Add(viewMappings);
            tablesLayout.Controls.Add(continueActions, 0, 4);
            tablesLayout.SetColumnSpan(continueActions, 2);
            tablesPage.Controls.Add(tablesLayout);
            mappingPage.Dock = DockStyle.Fill;
            mappingPage.ColumnCount = 1;
            mappingPage.RowCount = 3;
            mappingPage.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            mappingPage.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            mappingPage.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mappingPage.Visible = false;
            contextBar.Dock = DockStyle.Fill;
            contextBar.ColumnCount = 2;
            contextBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            contextBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
            contextBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            loadedPair.Dock = DockStyle.Fill;
            loadedPair.TextAlign = ContentAlignment.MiddleLeft;
            loadedPair.Font = activeStepFont;
            loadedPair.AutoEllipsis = true;
            moreActions.Text = "Mais ações";
            moreActions.Dock = DockStyle.Fill;
            moreActions.Click += MoreActionsClick;
            contextBar.Controls.Add(loadedPair, 0, 0);
            contextBar.Controls.Add(moreActions, 1, 0);
            solution.Text = "Adicionar à solution";
            solution.Click += SolutionClick;
            publish.Text = "Publicar alterações pendentes";
            publish.Click += PublishClick;
            helpMenu.Text = "Ajuda";
            helpMenu.Click += HelpClick;
            moreMenu.Items.Add(solution);
            moreMenu.Items.Add(publish);
            moreMenu.Items.Add(new ToolStripSeparator());
            moreMenu.Items.Add(helpMenu);
            selectionBar.Dock = DockStyle.Fill;
            selectionBar.ColumnCount = 1;
            selectionBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            selectionBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
            selectionBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F));
            selectionSummary.Dock = DockStyle.Fill;
            selectionSummary.TextAlign = ContentAlignment.MiddleLeft;
            selectionSummary.AutoEllipsis = true;
            clearChecks.Text = "Limpar seleções";
            clearChecks.Size = new Size(155, 34);
            clearChecks.Click += ClearChecksClick;
            addMapping.Text = "Criar mapeamento";
            addMapping.Size = new Size(180, 34);
            addMapping.Click += AddMappingClick;
            selectionBar.Controls.Add(selectionSummary, 0, 0);
            selectionBar.Controls.Add(clearChecks, 1, 0);
            selectionBar.Controls.Add(addMapping, 2, 0);
            lists.Dock = DockStyle.Fill;
            lists.ColumnCount = 3;
            lists.RowCount = 1;
            lists.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            lists.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            lists.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            mappingPage.Controls.Add(contextBar, 0, 0);
            mappingPage.Controls.Add(selectionBar, 0, 1);
            mappingPage.Controls.Add(lists, 0, 2);
            compatibleOnly.Text = "Somente compatíveis";
            compatibleOnly.AutoSize = true;
            compatibleOnly.CheckedChanged += FilterChanged;
            mappingActions.Dock = DockStyle.Fill;
            mappingActions.WrapContents = false;
            mappingActions.AutoScroll = true;
            editMapping.Text = "Alterar";
            editMapping.Size = new Size(76, 30);
            editMapping.Click += EditMappingClick;
            removeMapping.Text = "Remover";
            removeMapping.Size = new Size(76, 30);
            removeMapping.Click += RemoveMappingClick;
            undoMapping.Text = "Desfazer";
            undoMapping.Size = new Size(76, 30);
            undoMapping.Click += UndoMappingClick;
            editMapping.Margin = new Padding(0, 3, 3, 3);
            removeMapping.Margin = new Padding(0, 3, 3, 3);
            undoMapping.Margin = new Padding(0, 3, 3, 3);
            sourceActions.Dock = DockStyle.Fill;
            sourceActions.WrapContents = false;
            targetActions.Dock = DockStyle.Fill;
            targetActions.WrapContents = false;
            refreshSource.Name = "refreshSource";
            refreshSource.Size = new Size(32, 30);
            refreshSource.ImageList = actionImages;
            refreshSource.ImageKey = "refresh";
            refreshSource.AccessibleName = "Atualizar colunas de origem";
            refreshSource.UseVisualStyleBackColor = true;
            refreshSource.Click += RefreshSourceClick;
            refreshTarget.Name = "refreshTarget";
            refreshTarget.Size = new Size(32, 30);
            refreshTarget.ImageList = actionImages;
            refreshTarget.ImageKey = "refresh";
            refreshTarget.AccessibleName = "Atualizar colunas de destino";
            refreshTarget.UseVisualStyleBackColor = true;
            refreshTarget.Click += RefreshTargetClick;
            refreshMappings.Name = "refreshMappings";
            refreshMappings.Size = new Size(32, 30);
            refreshMappings.Margin = new Padding(0, 3, 3, 3);
            refreshMappings.ImageList = actionImages;
            refreshMappings.ImageKey = "refresh";
            refreshMappings.AccessibleName = "Atualizar mapeamentos";
            refreshMappings.UseVisualStyleBackColor = true;
            refreshMappings.Click += RefreshMappingsClick;
            sourceActions.Controls.Add(refreshSource);
            targetActions.Controls.Add(refreshTarget);
            targetActions.Controls.Add(compatibleOnly);
            mappingActions.Controls.Add(refreshMappings);
            mappingActions.Controls.Add(editMapping);
            mappingActions.Controls.Add(removeMapping);
            mappingActions.Controls.Add(undoMapping);
            sourcePanel.Dock = DockStyle.Fill;
            sourcePanel.Margin = new Padding(3, 8, 6, 0);
            sourcePanel.ColumnCount = 1;
            sourcePanel.RowCount = 4;
            sourcePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            sourcePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            sourcePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            sourcePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            sourceLabel.Text = "Origem";
            sourceLabel.Font = activeStepFont;
            sourceLabel.Dock = DockStyle.Fill;
            sourceLabel.AutoEllipsis = true;
            sourceSearch.Dock = DockStyle.Fill;
            sourceSearch.AccessibleName = "Buscar colunas de origem";
            sourceSearch.TextChanged += FilterChanged;
            sourceGrid.Dock = DockStyle.Fill;
            sourceGrid.ReadOnly = true;
            sourceGrid.AllowUserToAddRows = false;
            sourceGrid.AllowUserToDeleteRows = false;
            sourceGrid.AllowUserToResizeRows = false;
            sourceGrid.MultiSelect = false;
            sourceGrid.RowHeadersVisible = false;
            sourceGrid.AutoGenerateColumns = false;
            sourceGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            sourceGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            sourceGrid.BackgroundColor = SystemColors.Window;
            sourceGrid.ForeColor = SystemColors.WindowText;
            sourceGrid.RowTemplate.Height = 23;
            sourceGrid.RowTemplate.MinimumHeight = 23;
            sourceGrid.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            sourceGrid.CellContentClick += GridCheckboxClick;
            sourceGrid.KeyDown += GridCheckboxKeyDown;
            sourcePanel.Controls.Add(sourceLabel, 0, 0);
            sourcePanel.Controls.Add(sourceSearch, 0, 1);
            sourcePanel.Controls.Add(sourceGrid, 0, 2);
            lists.Controls.Add(sourcePanel, 0, 0);
            sourceGrid.Columns.Add(
                new DataGridViewCheckBoxColumn
                {
                    Name = "Checked",
                    HeaderText = "Marcar",
                    Width = 46,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    ToolTipText = "Marcar este registro (ou pressione Espaço na caixa)",
                }
            );
            sourceGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Name",
                    HeaderText = "Nome lógico",
                    FillWeight = 105F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            sourceGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Label",
                    HeaderText = "Nome",
                    FillWeight = 95F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            sourceGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Type",
                    HeaderText = "Tipo",
                    FillWeight = 65F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            sourceGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Reason",
                    HeaderText = "Situação",
                    FillWeight = 135F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            targetPanel.Dock = DockStyle.Fill;
            targetPanel.Margin = new Padding(3, 8, 6, 0);
            targetPanel.ColumnCount = 1;
            targetPanel.RowCount = 4;
            targetPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            targetPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            targetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            targetPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            targetLabel.Text = "Destino";
            targetLabel.Font = activeStepFont;
            targetLabel.Dock = DockStyle.Fill;
            targetLabel.AutoEllipsis = true;
            targetSearch.Dock = DockStyle.Fill;
            targetSearch.AccessibleName = "Buscar colunas de destino";
            targetSearch.TextChanged += FilterChanged;
            targetGrid.Dock = DockStyle.Fill;
            targetGrid.ReadOnly = true;
            targetGrid.AllowUserToAddRows = false;
            targetGrid.AllowUserToDeleteRows = false;
            targetGrid.AllowUserToResizeRows = false;
            targetGrid.MultiSelect = false;
            targetGrid.RowHeadersVisible = false;
            targetGrid.AutoGenerateColumns = false;
            targetGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            targetGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            targetGrid.BackgroundColor = SystemColors.Window;
            targetGrid.ForeColor = SystemColors.WindowText;
            targetGrid.RowTemplate.Height = 23;
            targetGrid.RowTemplate.MinimumHeight = 23;
            targetGrid.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            targetGrid.CellContentClick += GridCheckboxClick;
            targetGrid.KeyDown += GridCheckboxKeyDown;
            targetPanel.Controls.Add(targetLabel, 0, 0);
            targetPanel.Controls.Add(targetSearch, 0, 1);
            targetPanel.Controls.Add(targetGrid, 0, 2);
            lists.Controls.Add(targetPanel, 1, 0);
            targetGrid.Columns.Add(
                new DataGridViewCheckBoxColumn
                {
                    Name = "Checked",
                    HeaderText = "Marcar",
                    Width = 46,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    ToolTipText = "Marcar este registro (ou pressione Espaço na caixa)",
                }
            );
            targetGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Name",
                    HeaderText = "Nome lógico",
                    FillWeight = 105F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            targetGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Label",
                    HeaderText = "Nome",
                    FillWeight = 95F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            targetGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Type",
                    HeaderText = "Tipo",
                    FillWeight = 65F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            targetGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Reason",
                    HeaderText = "Situação",
                    FillWeight = 135F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            mappingPanel.Dock = DockStyle.Fill;
            mappingPanel.Margin = new Padding(3, 8, 6, 0);
            mappingPanel.ColumnCount = 1;
            mappingPanel.RowCount = 4;
            mappingPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            mappingPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            mappingPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mappingPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            mappingLabel.Text = "Mapeamentos";
            mappingLabel.Font = activeStepFont;
            mappingLabel.Dock = DockStyle.Fill;
            mappingLabel.AutoEllipsis = true;
            mappingSearch.Dock = DockStyle.Fill;
            mappingSearch.AccessibleName = "Buscar mapeamentos";
            mappingSearch.TextChanged += FilterChanged;
            mappingGrid.Dock = DockStyle.Fill;
            mappingGrid.ReadOnly = true;
            mappingGrid.AllowUserToAddRows = false;
            mappingGrid.AllowUserToDeleteRows = false;
            mappingGrid.AllowUserToResizeRows = false;
            mappingGrid.MultiSelect = false;
            mappingGrid.RowHeadersVisible = false;
            mappingGrid.AutoGenerateColumns = false;
            mappingGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            mappingGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            mappingGrid.BackgroundColor = SystemColors.Window;
            mappingGrid.ForeColor = SystemColors.WindowText;
            mappingGrid.RowTemplate.Height = 23;
            mappingGrid.RowTemplate.MinimumHeight = 23;
            mappingGrid.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            mappingGrid.CellContentClick += GridCheckboxClick;
            mappingGrid.KeyDown += GridCheckboxKeyDown;
            mappingPanel.Controls.Add(mappingLabel, 0, 0);
            mappingPanel.Controls.Add(mappingSearch, 0, 1);
            mappingPanel.Controls.Add(mappingGrid, 0, 2);
            lists.Controls.Add(mappingPanel, 2, 0);
            mappingGrid.Columns.Add(
                new DataGridViewCheckBoxColumn
                {
                    Name = "Checked",
                    HeaderText = "Marcar",
                    Width = 46,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    ToolTipText = "Marcar este registro (ou pressione Espaço na caixa)",
                }
            );
            mappingGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Source",
                    HeaderText = "Origem",
                    FillWeight = 110F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            mappingGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Target",
                    HeaderText = "Destino",
                    FillWeight = 110F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            mappingGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Change",
                    HeaderText = "Ajuste",
                    FillWeight = 85F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            mappingGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Protection",
                    HeaderText = "Restrição",
                    FillWeight = 95F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            sourcePanel.Controls.Add(sourceActions, 0, 3);
            targetPanel.Controls.Add(targetActions, 0, 3);
            mappingPanel.Controls.Add(mappingActions, 0, 3);
            footer.Dock = DockStyle.Fill;
            footer.ColumnCount = 1;
            footer.RowCount = 2;
            footer.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            footer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            status.Dock = DockStyle.Fill;
            status.TextAlign = ContentAlignment.MiddleLeft;
            status.AutoEllipsis = true;
            status.Text = "Conecte ao Dataverse e carregue as tabelas.";
            footerActions.Dock = DockStyle.Right;
            footerActions.WrapContents = false;
            footerActions.AutoSize = true;
            footerActions.FlowDirection = FlowDirection.LeftToRight;
            undoAll.Text = "Descartar ajustes";
            undoAll.Size = new Size(170, 34);
            undoAll.Click += UndoAllClick;
            saveChanges.Text = "Revisar e salvar";
            saveChanges.Size = new Size(215, 34);
            saveChanges.Font = activeStepFont;
            saveChanges.Click += SaveChangesClick;
            footerActions.Controls.Add(undoAll);
            footerActions.Controls.Add(clearChecks);
            footerActions.Controls.Add(addMapping);
            footerActions.Controls.Add(saveChanges);
            footer.Controls.Add(status, 0, 0);
            footer.Controls.Add(footerActions, 0, 1);
            pages.Controls.Add(tablesPage);
            pages.Controls.Add(mappingPage);
            layout.Controls.Add(steps, 0, 0);
            layout.Controls.Add(pages, 0, 1);
            layout.Controls.Add(footer, 0, 2);
            tablesStep.ImageList = actionImages;
            tablesStep.ImageKey = "tables";
            tablesStep.TextImageRelation = TextImageRelation.ImageBeforeText;
            tablesStep.UseVisualStyleBackColor = true;
            mappingStep.ImageList = actionImages;
            mappingStep.ImageKey = "compare";
            mappingStep.TextImageRelation = TextImageRelation.ImageBeforeText;
            mappingStep.UseVisualStyleBackColor = true;
            reviewStep.ImageList = actionImages;
            reviewStep.ImageKey = "save";
            reviewStep.TextImageRelation = TextImageRelation.ImageBeforeText;
            reviewStep.UseVisualStyleBackColor = true;
            loadTables.ImageList = actionImages;
            loadTables.ImageKey = "refresh";
            loadTables.TextImageRelation = TextImageRelation.ImageBeforeText;
            loadTables.UseVisualStyleBackColor = true;
            viewMappings.ImageList = actionImages;
            viewMappings.ImageKey = "next";
            viewMappings.TextImageRelation = TextImageRelation.ImageBeforeText;
            viewMappings.UseVisualStyleBackColor = true;
            moreActions.ImageList = actionImages;
            moreActions.ImageKey = "more";
            moreActions.TextImageRelation = TextImageRelation.ImageBeforeText;
            moreActions.UseVisualStyleBackColor = true;
            clearChecks.ImageList = actionImages;
            clearChecks.ImageKey = "clear";
            clearChecks.TextImageRelation = TextImageRelation.ImageBeforeText;
            clearChecks.UseVisualStyleBackColor = true;
            addMapping.ImageList = actionImages;
            addMapping.ImageKey = "add";
            addMapping.TextImageRelation = TextImageRelation.ImageBeforeText;
            addMapping.UseVisualStyleBackColor = true;
            editMapping.ImageList = actionImages;
            editMapping.ImageKey = "edit";
            editMapping.TextImageRelation = TextImageRelation.ImageBeforeText;
            editMapping.UseVisualStyleBackColor = true;
            removeMapping.TextImageRelation = TextImageRelation.ImageBeforeText;
            removeMapping.UseVisualStyleBackColor = true;
            undoMapping.TextImageRelation = TextImageRelation.ImageBeforeText;
            undoMapping.UseVisualStyleBackColor = true;
            undoAll.ImageList = actionImages;
            undoAll.ImageKey = "undo";
            undoAll.TextImageRelation = TextImageRelation.ImageBeforeText;
            undoAll.UseVisualStyleBackColor = true;
            saveChanges.ImageList = actionImages;
            saveChanges.ImageKey = "save";
            saveChanges.TextImageRelation = TextImageRelation.ImageBeforeText;
            saveChanges.UseVisualStyleBackColor = true;
            solution.Image = actionImages.Images["solution"];
            publish.Image = actionImages.Images["save"];
            helpMenu.Image = actionImages.Images["help"];
            tips.SetToolTip(
                refreshSource,
                "Atualizar somente as colunas de origem; preserva ajustes e filtros"
            );
            tips.SetToolTip(
                refreshTarget,
                "Atualizar somente as colunas de destino; preserva ajustes e filtros"
            );
            tips.SetToolTip(
                refreshMappings,
                "Consultar mapeamentos no Dataverse; preserva ajustes locais se o ambiente não mudou"
            );
            tips.SetToolTip(clearChecks, "Desmarcar as três listas");
            tips.SetToolTip(addMapping, "Criar um ajuste local com a origem e o destino marcados");
            tips.SetToolTip(
                editMapping,
                "Abrir o mapeamento marcado para escolher outra origem ou outro destino"
            );
            tips.SetToolTip(removeMapping, "Marcar o mapeamento escolhido para remoção");
            tips.SetToolTip(undoMapping, "Desfazer o ajuste do mapeamento escolhido");
            tips.SetToolTip(saveChanges, "Revisar os ajustes antes de salvar e publicar");
            tips.SetToolTip(sourceSearch, "Buscar por nome lógico, nome ou tipo");
            tips.SetToolTip(targetSearch, "Buscar por nome lógico, nome ou tipo");
            tips.SetToolTip(mappingSearch, "Buscar por origem, destino ou ajuste");
            Controls.Add(layout);
            ResumeLayout(false);
        }
    }
}
