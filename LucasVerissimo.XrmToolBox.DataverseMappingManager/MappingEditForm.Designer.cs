using System.Drawing;
using System.Windows.Forms;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager
{
    partial class MappingEditForm
    {
        private System.ComponentModel.IContainer components;
        private TableLayoutPanel layout;
        private Label original;
        private Label sourceLabel;
        private Label targetLabel;
        private ComboBox source;
        private ComboBox target;
        private Label validation;
        private FlowLayoutPanel actions;
        private Button apply;
        private Button cancel;

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
            layout = new TableLayoutPanel();
            original = new Label();
            sourceLabel = new Label();
            targetLabel = new Label();
            source = new ComboBox();
            target = new ComboBox();
            validation = new Label();
            actions = new FlowLayoutPanel();
            apply = new Button();
            cancel = new Button();
            SuspendLayout();
            Text = "Alterar mapeamento";
            Icon = PluginImages.CreateIcon(
                new System.ComponentModel.ComponentResourceManager(
                    typeof(MappingEditForm)
                ).GetString("ApplicationIcon")
            );
            Font = new Font("Segoe UI", 9F);
            ClientSize = new Size(680, 290);
            MinimumSize = new Size(560, 330);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(16);
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowCount = 5;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            original.Dock = DockStyle.Fill;
            original.AutoEllipsis = true;
            sourceLabel.Text = "Origem";
            sourceLabel.Dock = DockStyle.Fill;
            targetLabel.Text = "Destino";
            targetLabel.Dock = DockStyle.Fill;
            source.Dock = DockStyle.Top;
            source.DropDownStyle = ComboBoxStyle.DropDownList;
            source.AutoCompleteSource = AutoCompleteSource.ListItems;
            source.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            source.FormattingEnabled = true;
            source.Format += FormatColumn;
            source.SelectedIndexChanged += SelectionChanged;
            target.Dock = DockStyle.Top;
            target.DropDownStyle = ComboBoxStyle.DropDownList;
            target.AutoCompleteSource = AutoCompleteSource.ListItems;
            target.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            target.FormattingEnabled = true;
            target.Format += FormatColumn;
            target.SelectedIndexChanged += SelectionChanged;
            validation.Dock = DockStyle.Fill;
            actions.Dock = DockStyle.Fill;
            actions.FlowDirection = FlowDirection.RightToLeft;
            actions.WrapContents = false;
            apply.Text = "Aplicar ajuste";
            apply.Size = new Size(145, 32);
            apply.Enabled = false;
            apply.Click += ApplyClick;
            cancel.Text = "Cancelar";
            cancel.Size = new Size(100, 32);
            cancel.DialogResult = DialogResult.Cancel;
            actions.Controls.Add(apply);
            actions.Controls.Add(cancel);
            layout.Controls.Add(original, 0, 0);
            layout.SetColumnSpan(original, 2);
            layout.Controls.Add(sourceLabel, 0, 1);
            layout.Controls.Add(source, 1, 1);
            layout.Controls.Add(targetLabel, 0, 2);
            layout.Controls.Add(target, 1, 2);
            layout.Controls.Add(validation, 0, 3);
            layout.SetColumnSpan(validation, 2);
            layout.Controls.Add(actions, 0, 4);
            layout.SetColumnSpan(actions, 2);
            Controls.Add(layout);
            AcceptButton = apply;
            CancelButton = cancel;
            ResumeLayout(false);
        }
    }
}
