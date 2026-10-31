using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Models;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Services;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager
{
    internal partial class SolutionForm : Form
    {
        private List<SolutionMappingChoice> choices = new List<SolutionMappingChoice>();
        private readonly HashSet<SolutionMappingChoice> checkedChoices =
            new HashSet<SolutionMappingChoice>();
        private Func<
            IEnumerable<MappingExportExpectation>,
            List<SolutionComponentItem>
        > buildComponents;
        private List<SolutionComponentItem> selectedComponents = new List<SolutionComponentItem>();
        private bool updating;

        public SolutionForm()
        {
            InitializeComponent();
            picker.Configure(MappingControl.CreatePickerConfiguration("solutions"));
        }

        public SolutionForm(
            IEnumerable<PickerItem> solutions,
            IEnumerable<SolutionMappingChoice> mappingChoices,
            Func<
                IEnumerable<MappingExportExpectation>,
                List<SolutionComponentItem>
            > componentBuilder
        )
            : this()
        {
            if (solutions == null)
            {
                throw new ArgumentNullException(nameof(solutions));
            }
            choices =
                mappingChoices?.ToList() ?? throw new ArgumentNullException(nameof(mappingChoices));
            buildComponents =
                componentBuilder ?? throw new ArgumentNullException(nameof(componentBuilder));
            picker.SetItems(solutions);
            foreach (
                var choice in choices.Where(choice => choice.Available && choice.SelectedByDefault)
            )
            {
                checkedChoices.Add(choice);
            }
            FillGrids();
            RefreshComponents();
        }

        public string SolutionName => picker.GetSelectedItem<PickerItem>()?.Name;
        public List<SolutionComponentItem> SelectedComponents => selectedComponents.ToList();

        private void FillGrids()
        {
            updating = true;
            try
            {
                mappingsGrid.Rows.Clear();
                removalsGrid.Rows.Clear();
                foreach (var choice in choices)
                {
                    if (!choice.IsRemoval && !MatchesSearch(choice))
                    {
                        continue;
                    }
                    var grid = choice.IsRemoval ? removalsGrid : mappingsGrid;
                    var index = grid.Rows.Add(
                        checkedChoices.Contains(choice),
                        choice.Source,
                        choice.Target,
                        choice.Status,
                        choice.Detail
                    );
                    var row = grid.Rows[index];
                    row.Tag = choice;
                    row.ReadOnly = !choice.Available;
                    row.Cells["Detail"].ToolTipText = choice.Detail;
                    row.DefaultCellStyle.BackColor = choice.Available
                        ? Color.White
                        : Color.Gainsboro;
                    row.DefaultCellStyle.SelectionBackColor = row.DefaultCellStyle.BackColor;
                    row.DefaultCellStyle.SelectionForeColor = Color.Black;
                }
                removalsTab.Text = "Remoções (" + choices.Count(choice => choice.IsRemoval) + ")";
            }
            finally
            {
                updating = false;
            }
        }

        private bool MatchesSearch(SolutionMappingChoice choice)
        {
            return (choice.Source + " " + choice.Target + " " + choice.Status).IndexOf(
                    mappingSearch.Text.Trim(),
                    StringComparison.OrdinalIgnoreCase
                ) >= 0;
        }

        private void RefreshComponents()
        {
            if (buildComponents == null)
            {
                return;
            }
            selectedComponents = buildComponents(
                choices
                    .Where(choice => checkedChoices.Contains(choice))
                    .Select(choice => choice.Expectation)
            );
            componentList.Items.Clear();
            foreach (
                var component in selectedComponents.Where(component =>
                    component.ExportExpectation == null
                )
            )
            {
                componentList.Items.Add(component);
            }
            selectionStatus.Text =
                "Mapeamentos selecionados: "
                + checkedChoices.Count(choice => !choice.IsRemoval)
                + "   |   Conferências de remoção: "
                + checkedChoices.Count(choice => choice.IsRemoval)
                + "   |   Componentes necessários: "
                + componentList.Items.Count;
            confirm.Enabled = !string.IsNullOrWhiteSpace(SolutionName) && checkedChoices.Count > 0;
        }

        private void GridDirtyStateChanged(object sender, EventArgs eventArguments)
        {
            var grid = (DataGridView)sender;
            if (grid.IsCurrentCellDirty)
            {
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void GridCellValueChanged(object sender, DataGridViewCellEventArgs eventArguments)
        {
            if (updating || eventArguments.RowIndex < 0 || eventArguments.ColumnIndex != 0)
            {
                return;
            }
            var row = ((DataGridView)sender).Rows[eventArguments.RowIndex];
            var choice = row.Tag as SolutionMappingChoice;
            if (choice == null)
            {
                return;
            }
            if (choice.Available && Convert.ToBoolean(row.Cells[0].Value))
            {
                checkedChoices.Add(choice);
            }
            else
            {
                checkedChoices.Remove(choice);
                updating = true;
                row.Cells[0].Value = false;
                updating = false;
            }
            RefreshComponents();
        }

        private void SearchChanged(object sender, EventArgs eventArguments)
        {
            FillGrids();
        }

        private void PickerSelectionChanged(object sender, EventArgs eventArguments)
        {
            RefreshComponents();
        }

        private void SelectAllClick(object sender, EventArgs eventArguments)
        {
            foreach (var choice in choices.Where(choice => choice.Available && !choice.IsRemoval))
            {
                checkedChoices.Add(choice);
            }
            FillGrids();
            RefreshComponents();
        }

        private void ClearSelectedClick(object sender, EventArgs eventArguments)
        {
            checkedChoices.Clear();
            FillGrids();
            RefreshComponents();
        }

        private void ConfirmClick(object sender, EventArgs eventArguments)
        {
            if (string.IsNullOrWhiteSpace(SolutionName) || checkedChoices.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Selecione uma solution e pelo menos um mapeamento ou uma conferência de remoção."
                );
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
