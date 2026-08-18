using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LucasVerissimo.XrmToolBox.FieldChangeMonitor
{
    public partial class MyPluginControl
    {
        private readonly Color modernAccent = Color.FromArgb(8, 127, 140);
        private Panel modernPageHost;
        private Control modernShell;
        private Panel wizardPage;
        private Panel monitorsPage;
        private Panel historyPage;
        private Panel settingsPage;
        private Panel wizardContent;
        private readonly List<Panel> wizardSteps = new List<Panel>();
        private readonly List<Label> wizardStepLabels = new List<Label>();
        private int currentWizardStep;
        private DataGridView modernMonitorsGrid;
        private DataGridView modernRecentGrid;
        private Label modernSelectionLabel;
        private Button modernBulkPauseButton;
        private Button modernBulkContinueButton;
        private NumericUpDown modernMaximumHistory;
        private CheckBox modernConfirmOpenRecord;
        private CheckBox modernRestoreMonitors;
        private Label modernReviewSummary;
        private ToolTip modernToolTip;
        private Action modernColumnsLoadedCallback;

        private void InitializeModernInterface()
        {
            mainLayout.Visible = false;
            toolStripMenu.Visible = true;
            tslSubtitle.Visible = false;
            tslActiveMonitors.Visible = true;

            modernToolTip = new ToolTip();
            var shell = new TableLayoutPanel
            {
                Dock = DockStyle.None,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.FromArgb(247, 249, 250),
                Padding = new Padding(0)
            };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 205F));
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var navigation = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(12, 20, 12, 12),
                BackColor = Color.White
            };
            navigation.Controls.Add(CreateNavigationButton("＋  Novo monitoramento", ResetAndShowNewWizard));
            navigation.Controls.Add(CreateNavigationButton("▣  Monitoramentos", ShowMonitorsPage));
            navigation.Controls.Add(CreateNavigationButton("◷  Histórico", ShowHistoryPage));
            navigation.Controls.Add(CreateNavigationButton("⚙  Configurações", ShowSettingsPage));

            modernPageHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(247, 249, 250) };
            shell.Controls.Add(navigation, 0, 0);
            shell.Controls.Add(modernPageHost, 1, 0);
            Controls.Add(shell);
            modernShell = shell;
            Resize += (sender, args) => UpdateModernShellBounds();
            HandleCreated += (sender, args) => UpdateModernShellBounds();
            UpdateModernShellBounds();
            shell.BringToFront();
            statusStrip.BringToFront();
            toolStripMenu.BringToFront();

            BuildWizardPage();
            BuildMonitorsPage();
            BuildHistoryPage();
            BuildSettingsPage();
            ShowMonitorsPage();
        }

        private void UpdateModernShellBounds()
        {
            if (modernShell == null) return;
            var top = toolStripMenu.Visible ? toolStripMenu.Bottom : 0;
            var bottom = statusStrip.Visible ? statusStrip.Top : ClientSize.Height;
            modernShell.Bounds = new Rectangle(0, top, ClientSize.Width, Math.Max(0, bottom - top));
        }

        private Button CreateNavigationButton(string text, Action action)
        {
            var button = new Button
            {
                Text = text,
                Width = 178,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(35, 45, 50),
                Margin = new Padding(0, 0, 0, 8)
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += (sender, args) => action();
            return button;
        }

        private static Label CreatePageTitle(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 42,
                Font = new Font("Segoe UI Semibold", 18F),
                ForeColor = Color.FromArgb(28, 36, 40),
                Padding = new Padding(0, 6, 0, 0)
            };
        }

        private static Label CreateHint(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 34,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(85, 95, 100)
            };
        }

        private void BuildWizardPage()
        {
            wizardPage = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28, 18, 28, 18), AutoScroll = true };
            wizardPage.Controls.Add(CreateWizardFooter());
            wizardContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 10) };
            wizardPage.Controls.Add(wizardContent);
            wizardPage.Controls.Add(CreateWizardProgress());
            wizardPage.Controls.Add(CreateHint("Complete as etapas abaixo para configurar o monitoramento."));
            wizardPage.Controls.Add(CreatePageTitle("Criar monitoramento"));
            modernPageHost.Controls.Add(wizardPage);

            wizardSteps.Add(CreateEntityStep());
            wizardSteps.Add(CreateFieldsStep());
            wizardSteps.Add(CreateFilterStep());
            wizardSteps.Add(CreateReviewStep());
            foreach (var step in wizardSteps)
            {
                step.Dock = DockStyle.Fill;
                step.Visible = false;
                wizardContent.Controls.Add(step);
            }
        }

        private Control CreateWizardProgress()
        {
            var panel = new TableLayoutPanel { Dock = DockStyle.Top, Height = 56, ColumnCount = 4 };
            var names = new[] { "1  Tabela", "2  Campos", "3  Filtros", "4  Revisar" };
            for (var index = 0; index < names.Length; index++)
            {
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                var label = new Label
                {
                    Text = names[index],
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI Semibold", 10F),
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(index == 0 ? 0 : 4, 7, index == names.Length - 1 ? 0 : 4, 7)
                };
                wizardStepLabels.Add(label);
                panel.Controls.Add(label, index, 0);
            }
            return panel;
        }

        private Control CreateWizardFooter()
        {
            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 10, 0, 0)
            };
            var next = CreateActionButton("Continuar", true);
            next.Click += (sender, args) => MoveWizardNext();
            var back = CreateActionButton("Voltar", false);
            back.Click += (sender, args) => ShowWizard(Math.Max(0, currentWizardStep - 1));
            var cancel = CreateActionButton("Cancelar", false);
            cancel.Click += (sender, args) =>
            {
                if (editingMonitor != null) CancelMonitorEditing(true);
                ShowMonitorsPage();
            };
            next.Name = "modernWizardNext";
            back.Name = "modernWizardBack";
            footer.Controls.Add(next);
            footer.Controls.Add(cancel);
            footer.Controls.Add(back);
            return footer;
        }

        private Button CreateActionButton(string text, bool primary)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                MinimumSize = new Size(112, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = primary ? modernAccent : Color.White,
                ForeColor = primary ? Color.White : Color.FromArgb(45, 55, 60),
                Margin = new Padding(8, 0, 0, 0)
            };
            button.FlatAppearance.BorderColor = primary ? modernAccent : Color.FromArgb(180, 188, 193);
            return button;
        }

        private Panel CreateEntityStep()
        {
            var page = new Panel { Padding = new Padding(8, 12, 8, 8) };
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            var form = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 6, Padding = new Padding(0, 4, 0, 0) };
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            AddFormLabel(form, "Tabela do Dataverse", 0);
            txtEntityLogicalName.Dock = DockStyle.Fill;
            form.Controls.Add(txtEntityLogicalName, 0, 1);
            btnSearchEntities.Text = "Buscar tabelas";
            btnSearchEntities.Dock = DockStyle.Fill;
            form.Controls.Add(btnSearchEntities, 1, 1);
            btnLoadColumns.Text = "Carregar campos";
            btnLoadColumns.Dock = DockStyle.Fill;
            form.Controls.Add(btnLoadColumns, 2, 1);
            AddFormLabel(form, "Nome do monitoramento", 2);
            txtMonitorName.Dock = DockStyle.Fill;
            form.Controls.Add(txtMonitorName, 0, 3);
            form.SetColumnSpan(txtMonitorName, 3);
            AddFormLabel(form, "Intervalo de consulta (segundos)", 4);
            nudIntervalSeconds.Dock = DockStyle.Left;
            nudIntervalSeconds.Width = 120;
            form.Controls.Add(nudIntervalSeconds, 0, 5);
            layout.Controls.Add(CreateHint("Selecione a tabela e dê um nome fácil de reconhecer. Os campos serão carregados automaticamente ao continuar."), 0, 0);
            layout.Controls.Add(form, 0, 1);
            page.Controls.Add(layout);
            return page;
        }

        private static void AddFormLabel(TableLayoutPanel panel, string text, int row)
        {
            var label = new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft, Font = new Font("Segoe UI Semibold", 9F) };
            panel.Controls.Add(label, 0, row);
            panel.SetColumnSpan(label, panel.ColumnCount);
        }

        private Panel CreateFieldsStep()
        {
            var page = new Panel { Padding = new Padding(0, 8, 0, 0) };
            columnsGroup.Dock = DockStyle.Fill;
            columnsGroup.Text = "Campos monitorados — pesquise pelo nome exibido ou lógico";
            page.Controls.Add(columnsGroup);
            return page;
        }

        private Panel CreateFilterStep()
        {
            var page = new Panel { Padding = new Padding(0, 8, 0, 0), AutoScroll = true };
            filterGroup.Dock = DockStyle.Top;
            filterGroup.Height = 590;
            filterGroup.Text = "Filtros opcionais";
            btnToggleAdvanced.Visible = true;
            page.Controls.Add(filterGroup);
            return page;
        }

        private Panel CreateReviewStep()
        {
            var page = new Panel { Padding = new Padding(8, 12, 8, 8) };
            modernReviewSummary = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20),
                AutoEllipsis = true
            };
            page.Controls.Add(modernReviewSummary);
            return page;
        }

        private void ShowWizard(int step)
        {
            ShowModernPage(wizardPage);
            currentWizardStep = Math.Max(0, Math.Min(3, step));
            for (var index = 0; index < wizardSteps.Count; index++)
            {
                wizardSteps[index].Visible = index == currentWizardStep;
                wizardStepLabels[index].BackColor = index <= currentWizardStep ? Color.FromArgb(220, 242, 244) : Color.White;
                wizardStepLabels[index].ForeColor = index <= currentWizardStep ? modernAccent : Color.FromArgb(90, 100, 105);
            }
            wizardSteps[currentWizardStep].BringToFront();
            var next = FindControl<Button>(wizardPage, "modernWizardNext");
            var back = FindControl<Button>(wizardPage, "modernWizardBack");
            if (next != null) next.Text = currentWizardStep == 3 ? (editingMonitor == null ? "Iniciar monitoramento" : "Salvar alterações") : "Continuar";
            if (back != null) back.Enabled = currentWizardStep > 0;
            if (currentWizardStep == 3) UpdateModernReview();
        }

        private static T FindControl<T>(Control root, string name) where T : Control
        {
            return root.Controls.Find(name, true).OfType<T>().FirstOrDefault();
        }

        private void MoveWizardNext()
        {
            if (currentWizardStep == 0)
            {
                if (string.IsNullOrWhiteSpace(GetSelectedEntityLogicalName()) || string.IsNullOrWhiteSpace(txtMonitorName.Text))
                {
                    MessageBox.Show("Selecione uma tabela e informe o nome do monitoramento.", "Dados obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (currentEntityMetadata == null || !string.Equals(currentEntityLogicalName, GetSelectedEntityLogicalName(), StringComparison.OrdinalIgnoreCase))
                {
                    if (Service == null)
                    {
                        MessageBox.Show("Conecte-se a um ambiente antes de continuar.", "Conexão obrigatória", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    modernColumnsLoadedCallback = () => ShowWizard(1);
                    LoadColumns();
                    return;
                }
            }
            else if (currentWizardStep == 1 && checkedMonitoredColumns.Count == 0)
            {
                MessageBox.Show("Selecione ao menos um campo para monitorar.", "Campos obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (currentWizardStep < 3)
            {
                ShowWizard(currentWizardStep + 1);
                return;
            }

            if (editingMonitor != null)
            {
                var monitorBeingEdited = editingMonitor;
                SaveMonitorEdits();
                if (editingMonitor == null)
                {
                    ActivateMonitor(monitorBeingEdited);
                    ShowMonitorsPage();
                }
                return;
            }

            var previousCount = activeMonitors.Count;
            StartMonitoring();
            if (activeMonitors.Count > previousCount)
            {
                ShowMonitorsPage();
            }
        }

        private void ActivateMonitor(ActiveMonitor monitor)
        {
            if (monitor == null)
            {
                return;
            }

            monitor.IsPaused = false;
            monitor.NeedsBaselineReset = true;
            UpdateActiveMonitorStatus(monitor, "Iniciando");
            StartMonitorTask(monitor);
            PersistMonitorConfigurations();
            SetMonitoringControls(false);
        }

        private void UpdateModernReview()
        {
            var filterText = filterConditions.Count == 0
                ? "Todos os registros"
                : string.Join("; ", filterConditions.Select(item =>
                    $"{item.AttributeDisplayName} {item.OperatorDisplayName} {string.Join(", ", item.Values ?? new List<string>())}"));
            modernReviewSummary.Text =
                "RESUMO DA CONFIGURAÇÃO\r\n\r\n" +
                $"Nome:  {txtMonitorName.Text.Trim()}\r\n\r\n" +
                $"Tabela:  {GetSelectedEntityLogicalName()}\r\n\r\n" +
                $"Intervalo:  {nudIntervalSeconds.Value:0} segundos\r\n\r\n" +
                $"Campos monitorados ({checkedMonitoredColumns.Count}):\r\n  {string.Join(", ", checkedMonitoredColumns.OrderBy(item => item))}\r\n\r\n" +
                $"Filtro:  {filterText}";
        }

        private void BuildMonitorsPage()
        {
            monitorsPage = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 16, 24, 16) };
            var newButton = CreateActionButton("＋  Novo monitoramento", true);
            newButton.Dock = DockStyle.Right;
            newButton.Width = 190;
            newButton.Click += (sender, args) => ResetAndShowNewWizard();
            var heading = new Panel { Dock = DockStyle.Top, Height = 62 };
            heading.Controls.Add(newButton);
            heading.Controls.Add(CreateHint("Acompanhe, pause ou edite os monitoramentos deste ambiente."));
            heading.Controls.Add(CreatePageTitle("Monitoramentos"));

            chkWindowsPopups.Text = "Receber popups do Windows — desmarque para manter apenas o registro no histórico";
            chkWindowsPopups.Dock = DockStyle.Top;
            chkWindowsPopups.Height = 34;

            var toolbar = CreateMonitorToolbar();
            modernMonitorsGrid = CreateMonitorGrid();
            var monitorArea = new Panel { Dock = DockStyle.Top, Height = 300, BackColor = Color.White, Padding = new Padding(0) };
            monitorArea.Controls.Add(modernMonitorsGrid);
            monitorArea.Controls.Add(toolbar);

            modernRecentGrid = CreateRecentGrid();
            var recentLabel = new Label { Text = "Alterações recentes", Dock = DockStyle.Top, Height = 36, Font = new Font("Segoe UI Semibold", 12F), Padding = new Padding(0, 10, 0, 0) };
            monitorsPage.Controls.Add(modernRecentGrid);
            monitorsPage.Controls.Add(recentLabel);
            monitorsPage.Controls.Add(monitorArea);
            monitorsPage.Controls.Add(chkWindowsPopups);
            monitorsPage.Controls.Add(heading);
            modernPageHost.Controls.Add(monitorsPage);
        }

        private Control CreateMonitorToolbar()
        {
            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, WrapContents = false, Padding = new Padding(6, 6, 6, 4), BackColor = Color.White };
            var selectAll = new CheckBox { Text = "Selecionar todos", AutoSize = true, Margin = new Padding(4, 7, 12, 0) };
            selectAll.CheckedChanged += (sender, args) =>
            {
                foreach (DataGridViewRow row in modernMonitorsGrid.Rows) row.Cells["Selected"].Value = selectAll.Checked;
                UpdateModernSelectionState();
            };
            modernSelectionLabel = new Label { Text = "0 selecionados", AutoSize = true, Margin = new Padding(0, 9, 12, 0), Font = new Font("Segoe UI Semibold", 9F) };
            modernBulkPauseButton = CreateIconButton("Ⅱ", "Pausar selecionados", () => SetSelectedMonitorsPaused(true));
            modernBulkContinueButton = CreateIconButton("▶", "Continuar selecionados", () => SetSelectedMonitorsPaused(false));
            var spacer = new Label { AutoSize = false, Width = 18, Height = 30 };
            var import = CreateActionButton("Importar", false);
            import.Click += (sender, args) => ImportMonitors();
            var export = CreateActionButton("Exportar selecionados", false);
            export.Click += (sender, args) => ExportModernSelectedMonitors();
            toolbar.Controls.Add(selectAll);
            toolbar.Controls.Add(modernSelectionLabel);
            toolbar.Controls.Add(modernBulkPauseButton);
            toolbar.Controls.Add(modernBulkContinueButton);
            toolbar.Controls.Add(spacer);
            toolbar.Controls.Add(import);
            toolbar.Controls.Add(export);
            return toolbar;
        }

        private Button CreateIconButton(string icon, string tooltip, Action action)
        {
            var button = new Button { Text = icon, Width = 38, Height = 30, FlatStyle = FlatStyle.Flat, BackColor = Color.White, Margin = new Padding(3, 1, 3, 1) };
            button.FlatAppearance.BorderColor = Color.FromArgb(185, 193, 198);
            button.Click += (sender, args) => action();
            modernToolTip.SetToolTip(button, tooltip);
            button.AccessibleName = tooltip;
            return button;
        }

        private DataGridView CreateMonitorGrid()
        {
            var grid = CreateBaseGrid();
            grid.Dock = DockStyle.Fill;
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Selected", Width = 34 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Nome", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 155 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Entity", HeaderText = "Tabela", Width = 105 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fields", HeaderText = "Campos", Width = 72 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Records", HeaderText = "Registros", Width = 76 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Interval", HeaderText = "Intervalo", Width = 72 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 105 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "LastQuery", HeaderText = "Última consulta", Width = 120 });
            grid.Columns.Add(CreateGridButton("Pause", "Ações", 38));
            grid.Columns.Add(CreateGridButton("Edit", string.Empty, 38));
            grid.Columns.Add(CreateGridButton("Remove", string.Empty, 38));
            grid.CellContentClick += ModernMonitorsGrid_CellContentClick;
            grid.CurrentCellDirtyStateChanged += (sender, args) =>
            {
                if (grid.IsCurrentCellDirty) grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            grid.CellValueChanged += (sender, args) => { if (args.RowIndex >= 0 && grid.Columns[args.ColumnIndex].Name == "Selected") UpdateModernSelectionState(); };
            grid.CellToolTipTextNeeded += (sender, args) =>
            {
                if (args.RowIndex < 0) return;
                var name = grid.Columns[args.ColumnIndex].Name;
                args.ToolTipText = name == "Pause" ? (((ActiveMonitor)grid.Rows[args.RowIndex].Tag).IsPaused ? "Continuar" : "Pausar") : name == "Edit" ? "Editar" : name == "Remove" ? "Remover" : null;
            };
            return grid;
        }

        private static DataGridViewButtonColumn CreateGridButton(string name, string header, int width)
        {
            return new DataGridViewButtonColumn { Name = name, HeaderText = header, Width = width, FlatStyle = FlatStyle.Flat, UseColumnTextForButtonValue = false };
        }

        private static DataGridView CreateBaseGrid()
        {
            return new DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
                RowTemplate = { Height = 34 },
                ColumnHeadersHeight = 34,
                EnableHeadersVisualStyles = false
            };
        }

        private DataGridView CreateRecentGrid()
        {
            var grid = CreateBaseGrid();
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            foreach (var header in new[] { "ModifiedOn", "Evento", "ID do registro", "ModifiedBy", "Registro", "Campo", "Alteração" }) grid.Columns.Add(header, header);
            return grid;
        }

        private void ModernMonitorsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var monitor = modernMonitorsGrid.Rows[e.RowIndex].Tag as ActiveMonitor;
            if (monitor == null) return;
            var column = modernMonitorsGrid.Columns[e.ColumnIndex].Name;
            if (column == "Pause") SetMonitorsPaused(new[] { monitor }, !monitor.IsPaused);
            else if (column == "Edit") EditModernMonitor(monitor);
            else if (column == "Remove") RemoveModernMonitor(monitor);
        }

        private void SetSelectedMonitorsPaused(bool paused)
        {
            SetMonitorsPaused(GetModernSelectedMonitors(), paused);
        }

        private void SetMonitorsPaused(IEnumerable<ActiveMonitor> monitors, bool paused)
        {
            var changed = monitors.Where(item => item != null && item.IsPaused != paused).ToList();
            foreach (var monitor in changed)
            {
                monitor.IsPaused = paused;
                if (!paused)
                {
                    monitor.NeedsBaselineReset = false;
                    StartMonitorTask(monitor);
                }
                UpdateActiveMonitorStatus(monitor, paused ? "Pausado" : "Retomando");
                AddLog($"{monitor.DisplayName}: {(paused ? "pausado" : "retomado")}.");
            }
            PersistMonitorConfigurations();
            SetMonitoringControls(false);
        }

        private List<ActiveMonitor> GetModernSelectedMonitors()
        {
            return modernMonitorsGrid.Rows.Cast<DataGridViewRow>()
                .Where(row => Convert.ToBoolean(row.Cells["Selected"].Value ?? false))
                .Select(row => row.Tag as ActiveMonitor).Where(item => item != null).ToList();
        }

        private void UpdateModernSelectionState()
        {
            if (modernMonitorsGrid == null) return;
            var selected = GetModernSelectedMonitors();
            modernSelectionLabel.Text = $"{selected.Count} selecionado(s)";
            modernBulkPauseButton.Enabled = selected.Any(item => !item.IsPaused);
            modernBulkContinueButton.Enabled = selected.Any(item => item.IsPaused);
        }

        private void ExportModernSelectedMonitors()
        {
            ExportMonitors(GetModernSelectedMonitors());
        }

        private void EditModernMonitor(ActiveMonitor monitor)
        {
            BeginEditingMonitor(monitor);
            if (editingMonitor == monitor)
            {
                ShowWizard(0);
            }
        }

        private void RemoveModernMonitor(ActiveMonitor monitor)
        {
            RemoveMonitors(new[] { monitor });
        }

        private void RefreshModernMonitorGrid()
        {
            if (modernMonitorsGrid == null) return;
            var selectedIds = new HashSet<Guid>(GetModernSelectedMonitors().Select(item => item.Id));
            modernMonitorsGrid.Rows.Clear();
            lock (monitorsLock)
            {
                foreach (var monitor in activeMonitors)
                {
                    var rowIndex = modernMonitorsGrid.Rows.Add(selectedIds.Contains(monitor.Id), monitor.DisplayName,
                        monitor.Configuration.EntityLogicalName, $"{monitor.Configuration.MonitoredColumns.Count} campos",
                        monitor.MonitoredRecordCount.ToString(), $"{monitor.Configuration.IntervalSeconds} s",
                        monitor.IsPaused ? "Ⅱ  Pausado" : "▶  Ativo",
                        monitor.LastQueryOn.HasValue ? monitor.LastQueryOn.Value.ToString("HH:mm:ss") : "—",
                        monitor.IsPaused ? "▶" : "Ⅱ", "✎", "⌫");
                    modernMonitorsGrid.Rows[rowIndex].Tag = monitor;
                }
            }
            UpdateModernSelectionState();
        }

        private void RefreshModernRecentGrid()
        {
            if (modernRecentGrid == null) return;
            modernRecentGrid.Rows.Clear();
            foreach (ListViewItem item in lvRecentChanges.Items.Cast<ListViewItem>().Take(20))
            {
                modernRecentGrid.Rows.Add(item.SubItems.Cast<ListViewItem.ListViewSubItem>().Take(7).Select(sub => sub.Text).ToArray());
            }
        }

        private void ResetAndShowNewWizard()
        {
            if (editingMonitor != null) CancelMonitorEditing(false);
            txtMonitorName.Clear();
            txtEntityLogicalName.Text = string.Empty;
            currentEntityMetadata = null;
            currentEntityLogicalName = null;
            checkedMonitoredColumns.Clear();
            clbColumns.Items.Clear();
            filterConditions.Clear();
            RefreshConditionList();
            txtFilterXml.Clear();
            ShowWizard(0);
        }

        private void ShowMonitorsPage()
        {
            ShowModernPage(monitorsPage);
            RefreshModernMonitorGrid();
            RefreshModernRecentGrid();
        }

        private void BuildHistoryPage()
        {
            historyPage = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 16, 24, 16) };
            var tabs = new TabControl { Dock = DockStyle.Fill };
            var changes = new TabPage("Alterações");
            var log = new TabPage("Log técnico");
            lvRecentChanges.Dock = DockStyle.Fill;
            lstEvents.Dock = DockStyle.Fill;
            changes.Controls.Add(lvRecentChanges);
            log.Controls.Add(lstEvents);
            tabs.TabPages.Add(changes);
            tabs.TabPages.Add(log);
            historyPage.Controls.Add(tabs);
            historyPage.Controls.Add(CreateHint("Consulte as alterações detectadas pelos monitoramentos deste ambiente."));
            historyPage.Controls.Add(CreatePageTitle("Histórico de alterações"));
            modernPageHost.Controls.Add(historyPage);
        }

        private void ShowHistoryPage()
        {
            ShowModernPage(historyPage);
        }

        private void BuildSettingsPage()
        {
            settingsPage = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 16, 24, 16), AutoScroll = true };
            var content = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
            var storage = new GroupBox { Text = "Armazenamento", Width = 760, Height = 145, Padding = new Padding(14) };
            var storageFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            var maxRow = new FlowLayoutPanel { AutoSize = true, Width = 720 };
            maxRow.Controls.Add(new Label { Text = "Quantidade máxima de alterações por ambiente", AutoSize = true, Margin = new Padding(0, 8, 16, 0) });
            modernMaximumHistory = new NumericUpDown { Minimum = 10, Maximum = 5000, Value = DefaultMaximumRecentChanges, Width = 90 };
            maxRow.Controls.Add(modernMaximumHistory);
            var openFolder = CreateActionButton("Abrir pasta de configurações", false);
            openFolder.Click += (sender, args) => OpenSettingsFolder();
            var clearData = CreateActionButton("Limpar histórico", false);
            clearData.Click += (sender, args) => ClearModernHistory();
            var storageButtons = new FlowLayoutPanel { AutoSize = true, Width = 720 };
            storageButtons.Controls.Add(openFolder);
            storageButtons.Controls.Add(clearData);
            storageFlow.Controls.Add(maxRow);
            storageFlow.Controls.Add(new Label { Text = "Os dados são armazenados localmente nas configurações do XrmToolBox.", AutoSize = true, Margin = new Padding(0, 8, 0, 8) });
            storageFlow.Controls.Add(storageButtons);
            storage.Controls.Add(storageFlow);

            var behavior = new GroupBox { Text = "Comportamento", Width = 760, Height = 115, Padding = new Padding(14) };
            var behaviorFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            modernRestoreMonitors = new CheckBox { Text = "Restaurar monitoramentos ao abrir a ferramenta", AutoSize = true };
            modernConfirmOpenRecord = new CheckBox { Text = "Confirmar antes de abrir um registro no navegador", AutoSize = true };
            behaviorFlow.Controls.Add(modernRestoreMonitors);
            behaviorFlow.Controls.Add(modernConfirmOpenRecord);
            behavior.Controls.Add(behaviorFlow);

            var save = CreateActionButton("Salvar configurações", true);
            save.Click += (sender, args) => SaveModernSettings();
            content.Controls.Add(storage);
            content.Controls.Add(behavior);
            content.Controls.Add(save);
            settingsPage.Controls.Add(content);
            settingsPage.Controls.Add(CreateHint("Defina o comportamento geral da ferramenta neste computador."));
            settingsPage.Controls.Add(CreatePageTitle("Configurações"));
            modernPageHost.Controls.Add(settingsPage);
        }

        private void ApplyModernSettings()
        {
            if (mySettings == null || modernMaximumHistory == null) return;
            modernMaximumHistory.Value = Math.Max(modernMaximumHistory.Minimum, Math.Min(modernMaximumHistory.Maximum,
                mySettings.MaximumRecentChanges <= 0 ? DefaultMaximumRecentChanges : mySettings.MaximumRecentChanges));
            modernConfirmOpenRecord.Checked = mySettings.ConfirmBeforeOpeningRecord;
            modernRestoreMonitors.Checked = mySettings.RestoreMonitorsOnStartup;
        }

        private void SaveModernSettings()
        {
            if (mySettings == null) return;
            mySettings.MaximumRecentChanges = Convert.ToInt32(modernMaximumHistory.Value);
            mySettings.ConfirmBeforeOpeningRecord = modernConfirmOpenRecord.Checked;
            mySettings.RestoreMonitorsOnStartup = modernRestoreMonitors.Checked;
            SettingsManager.Instance.Save(GetType(), mySettings);
            SetStatus("Configurações salvas.");
        }

        private int GetMaximumRecentChanges()
        {
            return mySettings == null || mySettings.MaximumRecentChanges <= 0 ? DefaultMaximumRecentChanges : mySettings.MaximumRecentChanges;
        }

        private void OpenSettingsFolder()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MscrmTools", "XrmToolBox", "Settings");
            Directory.CreateDirectory(folder);
            Process.Start(new ProcessStartInfo(folder) { UseShellExecute = true });
        }

        private void ClearModernHistory()
        {
            if (MessageBox.Show("Deseja limpar o histórico deste ambiente?", "Limpar histórico", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            lvRecentChanges.Items.Clear();
            PersistRecentChanges();
            RefreshModernRecentGrid();
        }

        private void ShowSettingsPage()
        {
            ApplyModernSettings();
            ShowModernPage(settingsPage);
        }

        private void ShowModernPage(Control page)
        {
            foreach (Control control in modernPageHost.Controls) control.Visible = false;
            page.Visible = true;
            page.BringToFront();
        }
    }
}
