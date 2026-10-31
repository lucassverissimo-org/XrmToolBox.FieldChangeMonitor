using System;
using System.Linq;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Models;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Services;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager
{
    internal partial class MappingEditForm : Form
    {
        private readonly MappingWorkspace workspace;
        private readonly MappingRow mapping;

        public MappingEditForm()
        {
            InitializeComponent();
        }

        internal MappingEditForm(MappingWorkspace workspace, MappingRow mapping)
            : this()
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
            if (!workspace.Rows.Contains(mapping) || mapping.Protected || mapping.Removed)
            {
                throw new ArgumentException("Escolha um mapeamento editável.", nameof(mapping));
            }
            original.Text = "Mapeamento atual: " + mapping.Description;
            source.Items.AddRange(
                workspace
                    .Source.Attributes.OrderBy(item => item.LogicalName)
                    .Select(item => new ColumnItem { Metadata = item })
                    .ToArray()
            );
            target.Items.AddRange(
                workspace
                    .Target.Attributes.OrderBy(item => item.LogicalName)
                    .Select(item => new ColumnItem { Metadata = item })
                    .ToArray()
            );
            source.SelectedItem = source
                .Items.Cast<ColumnItem>()
                .FirstOrDefault(item => item.Name == mapping.Source);
            target.SelectedItem = target
                .Items.Cast<ColumnItem>()
                .FirstOrDefault(item => item.Name == mapping.Target);
            UpdateValidation();
        }

        private void SelectionChanged(object sender, EventArgs eventArguments)
        {
            UpdateValidation();
        }

        private void UpdateValidation()
        {
            if (workspace == null || mapping == null)
            {
                apply.Enabled = false;
                return;
            }
            var origin = source.SelectedItem as ColumnItem;
            var destination = target.SelectedItem as ColumnItem;
            var reason = MappingRules.Incompatibility(origin?.Metadata, destination?.Metadata);
            if (
                reason.Length == 0
                && workspace.Rows.Any(row =>
                    row != mapping && !row.Removed && row.Target == destination.Name
                )
            )
            {
                reason = "O destino já pertence a outro mapeamento.";
            }
            if (
                reason.Length == 0
                && origin.Name == mapping.Source
                && destination.Name == mapping.Target
            )
            {
                reason = "Escolha outra origem ou outro destino.";
            }
            validation.Text =
                reason.Length == 0 ? "O ajuste ficará pendente até Revisar e salvar." : reason;
            apply.Enabled = reason.Length == 0;
        }

        private void ApplyClick(object sender, EventArgs eventArguments)
        {
            UpdateValidation();
            if (!apply.Enabled)
            {
                return;
            }
            mapping.Source = ((ColumnItem)source.SelectedItem).Name;
            mapping.Target = ((ColumnItem)target.SelectedItem).Name;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FormatColumn(object sender, ListControlConvertEventArgs eventArguments)
        {
            if (eventArguments.ListItem is ColumnItem item)
            {
                eventArguments.Value = item.Name + " — " + item.Label;
            }
        }
    }
}
