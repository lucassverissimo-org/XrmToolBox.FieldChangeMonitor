using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace XrmTool_bravo
{
    internal sealed class OptionSetValuePickerForm : Form
    {
        private readonly bool allowMultiple;
        private readonly ListView lvOptions;
        private readonly Button btnOk;
        private readonly Button btnCancel;

        public OptionSetValuePickerForm(AttributeMetadata attribute, bool allowMultiple)
        {
            this.allowMultiple = allowMultiple;
            SelectedValues = new List<string>();

            Text = "Selecionar valor";
            Width = 520;
            Height = 420;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.Sizable;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

            lvOptions = new ListView
            {
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false,
                MultiSelect = allowMultiple,
                View = View.Details
            };
            lvOptions.Columns.Add("Rotulo", 300);
            lvOptions.Columns.Add("Valor", 120);
            lvOptions.DoubleClick += (sender, args) => ConfirmSelection();

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };

            btnOk = new Button
            {
                Text = "OK",
                Width = 90,
                Height = 30
            };
            btnOk.Click += (sender, args) => ConfirmSelection();

            btnCancel = new Button
            {
                Text = "Cancelar",
                Width = 90,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };

            buttonPanel.Controls.Add(btnOk);
            buttonPanel.Controls.Add(btnCancel);
            layout.Controls.Add(lvOptions, 0, 0);
            layout.Controls.Add(buttonPanel, 0, 1);
            Controls.Add(layout);

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            PopulateOptions(attribute);
        }

        public List<string> SelectedValues { get; private set; }

        private void PopulateOptions(AttributeMetadata attribute)
        {
            var booleanAttribute = attribute as BooleanAttributeMetadata;
            if (booleanAttribute != null && booleanAttribute.OptionSet != null)
            {
                AddOption(booleanAttribute.OptionSet.TrueOption, "1");
                AddOption(booleanAttribute.OptionSet.FalseOption, "0");
                return;
            }

            var enumAttribute = attribute as EnumAttributeMetadata;
            if (enumAttribute == null || enumAttribute.OptionSet == null)
            {
                return;
            }

            foreach (var option in enumAttribute.OptionSet.Options.Where(option => option.Value.HasValue).OrderBy(option => option.Value.Value))
            {
                AddOption(option, option.Value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        private void AddOption(OptionMetadata option, string value)
        {
            if (option == null || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var label = GetOptionLabel(option);
            if (string.IsNullOrWhiteSpace(label))
            {
                label = value;
            }

            var item = new ListViewItem(label);
            item.SubItems.Add(value);
            item.Tag = value;
            lvOptions.Items.Add(item);
        }

        private void ConfirmSelection()
        {
            if (lvOptions.SelectedItems.Count == 0)
            {
                MessageBox.Show(this, "Selecione ao menos um valor.", "Valor obrigatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedValues = lvOptions.SelectedItems
                .Cast<ListViewItem>()
                .Select(item => item.Tag as string)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            if (!allowMultiple && SelectedValues.Count > 1)
            {
                SelectedValues = SelectedValues.Take(1).ToList();
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private static string GetOptionLabel(OptionMetadata option)
        {
            if (option == null || option.Label == null)
            {
                return null;
            }

            if (option.Label.UserLocalizedLabel != null)
            {
                return option.Label.UserLocalizedLabel.Label;
            }

            return option.Label.LocalizedLabels.Count > 0 ? option.Label.LocalizedLabels[0].Label : null;
        }
    }
}
