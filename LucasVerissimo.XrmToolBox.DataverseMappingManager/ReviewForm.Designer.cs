using System;
using System.Drawing;
using System.Windows.Forms;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager
{
    partial class ReviewForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label summary;
        private Label notice;
        private Label executionStatus;
        private DataGridView changesGrid;
        private FlowLayoutPanel buttons;
        private Button confirm;
        private Button cancel;
        private Button addToSolution;
        private Timer animationTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ReviewForm));
            summary = new Label();
            notice = new Label();
            executionStatus = new Label();
            changesGrid = new DataGridView();
            buttons = new FlowLayoutPanel();
            confirm = new Button();
            cancel = new Button();
            addToSolution = new Button();
            animationTimer = new Timer(components);
            SuspendLayout();
            Text = "Revisar e salvar ajustes";
            Size = new Size(1180, 650);
            MinimumSize = new Size(860, 420);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            Icon = PluginImages.CreateIcon(resources.GetString("ApplicationIcon"));
            Font = new Font("Segoe UI", 9F);
            summary.Name = "summary";
            summary.Dock = DockStyle.Top;
            summary.Height = 44;
            summary.Padding = new Padding(10);
            summary.BackColor = Color.AliceBlue;
            notice.Name = "notice";
            notice.Dock = DockStyle.Bottom;
            notice.Height = 52;
            notice.Padding = new Padding(10, 4, 10, 4);
            notice.Text =
                "Ao salvar, as tabelas envolvidas serão publicadas, incluindo outras customizações pendentes nelas.\r\nAjustes que compartilham destinos são executados juntos; se um falhar, o grupo será revertido.";
            executionStatus.Name = "executionStatus";
            executionStatus.Dock = DockStyle.Bottom;
            executionStatus.Height = 48;
            executionStatus.Padding = new Padding(10, 4, 10, 4);
            executionStatus.Text = "Revise os ajustes abaixo e clique em Salvar para executar.";
            changesGrid.Name = "changesGrid";
            changesGrid.Dock = DockStyle.Fill;
            changesGrid.ReadOnly = true;
            changesGrid.AllowUserToAddRows = false;
            changesGrid.AllowUserToDeleteRows = false;
            changesGrid.AutoGenerateColumns = false;
            changesGrid.RowHeadersVisible = false;
            changesGrid.BackgroundColor = Color.White;
            changesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            changesGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            changesGrid.DefaultCellStyle.SelectionBackColor = Color.White;
            changesGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            changesGrid.RowTemplate.Height = 34;
            changesGrid.CellPainting += ChangesGridCellPainting;
            changesGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Action",
                    HeaderText = "Ação",
                    FillWeight = 80F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            changesGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "OriginalSource",
                    HeaderText = "Origem anterior",
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            changesGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "OriginalTarget",
                    HeaderText = "Destino anterior",
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            changesGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Source",
                    HeaderText = "Origem",
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            changesGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Target",
                    HeaderText = "Destino",
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            changesGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "State",
                    HeaderText = "Estado",
                    FillWeight = 130F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            changesGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Progress",
                    HeaderText = "Progresso",
                    FillWeight = 90F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            changesGrid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Detail",
                    HeaderText = "Detalhes / motivo",
                    FillWeight = 220F,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                }
            );
            buttons.Name = "buttons";
            buttons.Dock = DockStyle.Bottom;
            buttons.Height = 44;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            confirm.Name = "confirm";
            confirm.Text = "Salvar";
            confirm.Size = new Size(110, 30);
            confirm.Click += ConfirmClick;
            cancel.Name = "cancel";
            cancel.Text = "Cancelar";
            cancel.Size = new Size(110, 30);
            cancel.DialogResult = DialogResult.Cancel;
            addToSolution.Name = "addToSolution";
            addToSolution.Text = "Adicionar à solution";
            addToSolution.Size = new Size(170, 30);
            addToSolution.Enabled = false;
            addToSolution.Click += AddToSolutionClick;
            buttons.Controls.Add(confirm);
            buttons.Controls.Add(addToSolution);
            buttons.Controls.Add(cancel);
            animationTimer.Interval = 70;
            animationTimer.Tick += AnimationTimerTick;
            Controls.Add(changesGrid);
            Controls.Add(summary);
            Controls.Add(notice);
            Controls.Add(executionStatus);
            Controls.Add(buttons);
            AcceptButton = confirm;
            CancelButton = cancel;
            FormClosing += ReviewFormClosing;
            ResumeLayout(false);
        }
    }
}
