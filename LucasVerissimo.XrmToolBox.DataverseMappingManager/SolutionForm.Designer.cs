using System;
using System.Drawing;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.Shared.Controls;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager
{
    partial class SolutionForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private TableLayoutPanel layout;
        private GridPickerControl picker;
        private Label explanation;
        private TabControl sections;
        private TabPage mappingsTab;
        private TabPage componentsTab;
        private TabPage removalsTab;
        private TableLayoutPanel mappingsLayout;
        private TableLayoutPanel removalsLayout;
        private TextBox mappingSearch;
        private DataGridView mappingsGrid;
        private DataGridView removalsGrid;
        private ListBox componentList;
        private Label componentNotice;
        private Label removalNotice;
        private Label selectionStatus;
        private FlowLayoutPanel selectionButtons;
        private Button selectAll;
        private Button clearSelected;
        private Button confirm;
        private Button cancel;
        private FlowLayoutPanel buttons;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(
                typeof(SolutionForm)
            );
            layout = new TableLayoutPanel();
            picker = new GridPickerControl();
            explanation = new Label();
            sections = new TabControl();
            mappingsTab = new TabPage();
            componentsTab = new TabPage();
            removalsTab = new TabPage();
            mappingsLayout = new TableLayoutPanel();
            removalsLayout = new TableLayoutPanel();
            mappingSearch = new TextBox();
            mappingsGrid = new DataGridView();
            removalsGrid = new DataGridView();
            componentList = new ListBox();
            componentNotice = new Label();
            removalNotice = new Label();
            selectionStatus = new Label();
            selectionButtons = new FlowLayoutPanel();
            selectAll = new Button();
            clearSelected = new Button();
            confirm = new Button();
            cancel = new Button();
            buttons = new FlowLayoutPanel();
            layout.Name = "layout";
            picker.Name = "picker";
            explanation.Name = "explanation";
            sections.Name = "sections";
            mappingsTab.Name = "mappingsTab";
            componentsTab.Name = "componentsTab";
            removalsTab.Name = "removalsTab";
            mappingsLayout.Name = "mappingsLayout";
            removalsLayout.Name = "removalsLayout";
            mappingSearch.Name = "mappingSearch";
            mappingsGrid.Name = "mappingsGrid";
            removalsGrid.Name = "removalsGrid";
            componentList.Name = "componentList";
            componentNotice.Name = "componentNotice";
            removalNotice.Name = "removalNotice";
            selectionStatus.Name = "selectionStatus";
            selectionButtons.Name = "selectionButtons";
            selectAll.Name = "selectAll";
            clearSelected.Name = "clearSelected";
            confirm.Name = "confirm";
            cancel.Name = "cancel";
            buttons.Name = "buttons";
            SuspendLayout();
            Text = "Adicionar componentes à solution";
            Size = new Size(1120, 760);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            Font = new Font("Segoe UI", 9F);
            Icon = PluginImages.CreateIcon(resources.GetString("ApplicationIcon"));
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 5;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            explanation.Dock = DockStyle.Fill;
            explanation.Padding = new Padding(10);
            explanation.BackColor = Color.AliceBlue;
            explanation.Text =
                "Escolha a solution e os mapeamentos que deseja transportar. Os adicionados ou alterados e salvos nesta sessão já vêm marcados.\r\nOs demais ficam disponíveis para seleção. Ajustes locais precisam ser salvos primeiro. A lista inclui todos os vínculos carregados, mesmo os ocultos por filtros na tela principal.\r\nA inclusão será conferida no XML de uma exportação não gerenciada (exige permissão de exportação). O Dataverse pode exportar outros vínculos junto.";
            picker.Dock = DockStyle.Fill;
            picker.SelectedItemChanged += PickerSelectionChanged;
            sections.Dock = DockStyle.Fill;
            sections.Controls.Add(mappingsTab);
            sections.Controls.Add(componentsTab);
            sections.Controls.Add(removalsTab);
            mappingsTab.Text = "Mapeamentos a transportar";
            componentsTab.Text = "Componentes necessários";
            removalsTab.Text = "Remoções";
            mappingsLayout.Dock = DockStyle.Fill;
            mappingsLayout.ColumnCount = 1;
            mappingsLayout.RowCount = 3;
            mappingsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            mappingsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mappingsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            mappingSearch.Dock = DockStyle.Fill;
            mappingSearch.AccessibleName = "Buscar mapeamentos por origem, destino ou situação";
            mappingSearch.TextChanged += SearchChanged;
            selectionButtons.Dock = DockStyle.Fill;
            selectAll.Text = "Marcar todos disponíveis";
            selectAll.Size = new Size(185, 30);
            selectAll.Click += SelectAllClick;
            clearSelected.Text = "Limpar seleção";
            clearSelected.Size = new Size(140, 30);
            clearSelected.Click += ClearSelectedClick;
            selectionButtons.Controls.Add(selectAll);
            selectionButtons.Controls.Add(clearSelected);
            mappingsLayout.Controls.Add(mappingSearch, 0, 0);
            mappingsLayout.Controls.Add(mappingsGrid, 0, 1);
            mappingsLayout.Controls.Add(selectionButtons, 0, 2);
            mappingsTab.Controls.Add(mappingsLayout);
            componentList.Dock = DockStyle.Fill;
            componentList.HorizontalScrollbar = true;
            componentNotice.Dock = DockStyle.Top;
            componentNotice.Height = 56;
            componentNotice.Padding = new Padding(10);
            componentNotice.Text =
                "Calculados automaticamente a partir das marcações. Incluem as duas tabelas, os relacionamentos do par e somente as colunas envolvidas nos vínculos selecionados. As tabelas não incluem todos os subcomponentes.";
            componentsTab.Controls.Add(componentList);
            componentsTab.Controls.Add(componentNotice);
            removalsLayout.Dock = DockStyle.Fill;
            removalsLayout.ColumnCount = 1;
            removalsLayout.RowCount = 2;
            removalsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 94F));
            removalsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            removalNotice.Dock = DockStyle.Fill;
            removalNotice.Padding = new Padding(10);
            removalNotice.BackColor = Color.LemonChiffon;
            removalNotice.Text =
                "Remoções são separadas da inclusão. Conferir ausência no ZIP não executa nem comprova exclusão no destino.\r\nGerenciado: remover no desenvolvimento da solution de origem e aplicar upgrade no destino, após revisar dependências e camadas. Um update comum não exclui componentes.\r\nNão gerenciado: planejar remoção explícita no destino. Mapeamentos de sistema não podem ser excluídos individualmente. As conferências abaixo são opcionais e começam desmarcadas.";
            removalsLayout.Controls.Add(removalNotice, 0, 0);
            removalsLayout.Controls.Add(removalsGrid, 0, 1);
            removalsTab.Controls.Add(removalsLayout);
            mappingsGrid.Dock = DockStyle.Fill;
            mappingsGrid.AllowUserToAddRows = false;
            mappingsGrid.AllowUserToDeleteRows = false;
            mappingsGrid.AutoGenerateColumns = false;
            mappingsGrid.RowHeadersVisible = false;
            mappingsGrid.MultiSelect = false;
            mappingsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            mappingsGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            mappingsGrid.BackgroundColor = Color.White;
            mappingsGrid.DefaultCellStyle.SelectionBackColor = Color.White;
            mappingsGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            mappingsGrid.CurrentCellDirtyStateChanged += GridDirtyStateChanged;
            mappingsGrid.CellValueChanged += GridCellValueChanged;
            mappingsGrid.Columns.Add(
                new DataGridViewCheckBoxColumn
                {
                    Name = "Include",
                    HeaderText = "Incluir",
                    Width = 58,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            mappingsGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Source",
                    HeaderText = "Origem (nome lógico)",
                    FillWeight = 100F,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            mappingsGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Target",
                    HeaderText = "Destino (nome lógico)",
                    FillWeight = 100F,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            mappingsGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Situação",
                    FillWeight = 120F,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            mappingsGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Detail",
                    HeaderText = "Detalhes",
                    FillWeight = 220F,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            removalsGrid.Dock = DockStyle.Fill;
            removalsGrid.AllowUserToAddRows = false;
            removalsGrid.AllowUserToDeleteRows = false;
            removalsGrid.AutoGenerateColumns = false;
            removalsGrid.RowHeadersVisible = false;
            removalsGrid.MultiSelect = false;
            removalsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            removalsGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            removalsGrid.BackgroundColor = Color.White;
            removalsGrid.DefaultCellStyle.SelectionBackColor = Color.White;
            removalsGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            removalsGrid.CurrentCellDirtyStateChanged += GridDirtyStateChanged;
            removalsGrid.CellValueChanged += GridCellValueChanged;
            removalsGrid.Columns.Add(
                new DataGridViewCheckBoxColumn
                {
                    Name = "Include",
                    HeaderText = "Conferir ausência",
                    Width = 115,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            removalsGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Source",
                    HeaderText = "Origem (nome lógico)",
                    FillWeight = 100F,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            removalsGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Target",
                    HeaderText = "Destino (nome lógico)",
                    FillWeight = 100F,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            removalsGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Situação",
                    FillWeight = 120F,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            removalsGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Detail",
                    HeaderText = "Detalhes",
                    FillWeight = 220F,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            selectionStatus.Dock = DockStyle.Fill;
            selectionStatus.Padding = new Padding(10);
            buttons.Dock = DockStyle.Fill;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            confirm.Text = "Adicionar e conferir";
            confirm.Size = new Size(155, 30);
            confirm.Enabled = false;
            confirm.Click += ConfirmClick;
            cancel.Text = "Cancelar";
            cancel.Size = new Size(110, 30);
            cancel.DialogResult = DialogResult.Cancel;
            buttons.Controls.Add(confirm);
            buttons.Controls.Add(cancel);
            layout.Controls.Add(explanation, 0, 0);
            layout.Controls.Add(picker, 0, 1);
            layout.Controls.Add(sections, 0, 2);
            layout.Controls.Add(selectionStatus, 0, 3);
            layout.Controls.Add(buttons, 0, 4);
            Controls.Add(layout);
            AcceptButton = confirm;
            CancelButton = cancel;
            ResumeLayout(false);
        }
    }
}
