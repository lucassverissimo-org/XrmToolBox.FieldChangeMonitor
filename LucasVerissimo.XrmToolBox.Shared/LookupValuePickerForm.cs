using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.Shared.WinForms
{
    public sealed class LookupValuePickerForm : Form
    {
        private readonly IOrganizationService service;
        private readonly LookupAttributeMetadata lookupAttribute;
        private readonly Dictionary<string, EntityMetadata> metadataCache = new Dictionary<
            string,
            EntityMetadata
        >(StringComparer.OrdinalIgnoreCase);
        private readonly ComboBox cboTarget;
        private readonly TextBox txtSearch;
        private readonly Button btnSearch;
        private readonly ListView lvRecords;
        private readonly Button btnOk;
        private readonly Button btnCancel;

        public LookupValuePickerForm(
            IOrganizationService service,
            LookupAttributeMetadata lookupAttribute
        )
        {
            this.service = service;
            this.lookupAttribute = lookupAttribute;

            Text = "Selecionar registro";
            Width = 760;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.Sizable;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10),
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

            var searchLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
            };
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));

            var lblTarget = new System.Windows.Forms.Label
            {
                Text = "Tabela",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };

            cboTarget = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };

            var lblSearch = new System.Windows.Forms.Label
            {
                Text = "Busca",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };

            txtSearch = new TextBox { Dock = DockStyle.Fill };
            txtSearch.KeyDown += txtSearch_KeyDown;

            btnSearch = new Button { Text = "Buscar", Dock = DockStyle.Fill };
            btnSearch.Click += (sender, args) => SearchRecords();

            searchLayout.Controls.Add(lblTarget, 0, 0);
            searchLayout.Controls.Add(cboTarget, 1, 0);
            searchLayout.Controls.Add(lblSearch, 2, 0);
            searchLayout.Controls.Add(txtSearch, 3, 0);
            searchLayout.Controls.Add(btnSearch, 4, 0);

            lvRecords = new ListView
            {
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false,
                MultiSelect = false,
                View = View.Details,
            };
            lvRecords.Columns.Add("Nome", 420);
            lvRecords.Columns.Add("Id", 280);
            lvRecords.DoubleClick += (sender, args) => ConfirmSelection();

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
            };

            btnOk = new Button
            {
                Text = "OK",
                Width = 90,
                Height = 30,
            };
            btnOk.Click += (sender, args) => ConfirmSelection();

            btnCancel = new Button
            {
                Text = "Cancelar",
                Width = 90,
                Height = 30,
                DialogResult = DialogResult.Cancel,
            };

            buttonPanel.Controls.Add(btnOk);
            buttonPanel.Controls.Add(btnCancel);
            layout.Controls.Add(searchLayout, 0, 0);
            layout.Controls.Add(lvRecords, 0, 1);
            layout.Controls.Add(buttonPanel, 0, 2);
            Controls.Add(layout);

            AcceptButton = btnSearch;
            CancelButton = btnCancel;

            PopulateTargets();
        }

        public string SelectedValue { get; private set; }

        private void PopulateTargets()
        {
            cboTarget.Items.Clear();

            if (lookupAttribute.Targets != null)
            {
                foreach (var target in lookupAttribute.Targets)
                {
                    cboTarget.Items.Add(target);
                }
            }

            if (cboTarget.Items.Count > 0)
            {
                cboTarget.SelectedIndex = 0;
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SearchRecords();
            }
        }

        private void SearchRecords()
        {
            if (cboTarget.SelectedItem == null)
            {
                MessageBox.Show(
                    this,
                    "O lookup nao possui tabela alvo disponivel.",
                    "Tabela indisponivel",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var target = cboTarget.SelectedItem.ToString();

            try
            {
                var metadata = GetEntityMetadata(target);
                var primaryId = metadata.PrimaryIdAttribute;
                var primaryName = metadata.PrimaryNameAttribute;
                var columns = string.IsNullOrWhiteSpace(primaryName)
                    ? new ColumnSet(primaryId)
                    : new ColumnSet(primaryId, primaryName);

                var query = new QueryExpression(target)
                {
                    ColumnSet = columns,
                    NoLock = true,
                    TopCount = 50,
                };

                var searchText = txtSearch.Text.Trim();
                if (
                    !string.IsNullOrWhiteSpace(searchText)
                    && !string.IsNullOrWhiteSpace(primaryName)
                )
                {
                    query.Criteria.AddCondition(
                        primaryName,
                        ConditionOperator.Like,
                        "%" + searchText + "%"
                    );
                }

                var result = service.RetrieveMultiple(query);
                lvRecords.BeginUpdate();
                lvRecords.Items.Clear();

                foreach (var entity in result.Entities)
                {
                    var name =
                        !string.IsNullOrWhiteSpace(primaryName) && entity.Contains(primaryName)
                            ? Convert.ToString(entity[primaryName])
                            : entity.Id.ToString("D");
                    var item = new ListViewItem(name);
                    item.SubItems.Add(entity.Id.ToString("D"));
                    item.Tag = entity.Id.ToString("D");
                    lvRecords.Items.Add(item);
                }

                lvRecords.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Erro ao buscar registros",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private EntityMetadata GetEntityMetadata(string logicalName)
        {
            EntityMetadata metadata;
            if (metadataCache.TryGetValue(logicalName, out metadata))
            {
                return metadata;
            }

            var response = (RetrieveEntityResponse)
                service.Execute(
                    new RetrieveEntityRequest
                    {
                        LogicalName = logicalName,
                        EntityFilters = EntityFilters.Entity,
                    }
                );

            metadataCache[logicalName] = response.EntityMetadata;
            return response.EntityMetadata;
        }

        private void ConfirmSelection()
        {
            if (lvRecords.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Selecione um registro.",
                    "Registro obrigatorio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            SelectedValue = lvRecords.SelectedItems[0].Tag as string;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
