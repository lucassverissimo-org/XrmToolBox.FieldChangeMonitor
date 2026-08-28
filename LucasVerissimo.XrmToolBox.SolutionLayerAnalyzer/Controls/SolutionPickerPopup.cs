using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls
{
    internal partial class SolutionPickerPopup : UserControl
    {
        private readonly List<SolutionInfo> solutions = new List<SolutionInfo>();

        public SolutionPickerPopup()
        {
            InitializeComponent();
        }

        public event EventHandler SolutionAccepted;

        public SolutionInfo SelectedSolution
        {
            get { return solutionsGrid.CurrentRow?.DataBoundItem as SolutionInfo; }
        }

        public void SetSolutions(IEnumerable<SolutionInfo> availableSolutions)
        {
            if (availableSolutions == null)
            {
                throw new ArgumentNullException(nameof(availableSolutions));
            }

            solutions.Clear();
            solutions.AddRange(
                availableSolutions
                    .Where(solution => solution != null)
                    .OrderBy(solution => solution.FriendlyName)
                    .ThenBy(solution => solution.UniqueName)
            );
            searchTextBox.Clear();
            filterTimer.Stop();
            ApplyFilter();
        }

        public void SelectSolution(SolutionInfo solution)
        {
            solutionsGrid.ClearSelection();
            if (solution == null)
            {
                return;
            }

            foreach (DataGridViewRow row in solutionsGrid.Rows)
            {
                var rowSolution = row.DataBoundItem as SolutionInfo;
                if (rowSolution != null && rowSolution.SolutionId == solution.SolutionId)
                {
                    row.Selected = true;
                    solutionsGrid.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        public void SetSearchText(string searchText)
        {
            var value = searchText ?? string.Empty;
            if (string.Equals(searchTextBox.Text, value, StringComparison.Ordinal))
            {
                return;
            }

            searchTextBox.Text = value;
        }

        public void FocusSearch()
        {
            BeginInvoke(
                new Action(() =>
                {
                    searchTextBox.Focus();
                    searchTextBox.SelectAll();
                })
            );
        }

        private void SearchTextBoxTextChanged(object sender, EventArgs eventArguments)
        {
            filterTimer.Stop();
            filterTimer.Start();
        }

        private void FilterTimerTick(object sender, EventArgs eventArguments)
        {
            filterTimer.Stop();
            ApplyFilter();
        }

        private void SearchTextBoxKeyDown(object sender, KeyEventArgs eventArguments)
        {
            if (eventArguments.KeyCode != Keys.Down || solutionsGrid.Rows.Count == 0)
            {
                return;
            }

            eventArguments.Handled = true;
            eventArguments.SuppressKeyPress = true;
            solutionsGrid.Focus();
            solutionsGrid.CurrentCell = solutionsGrid.Rows[0].Cells[0];
            solutionsGrid.Rows[0].Selected = true;
        }

        private void SolutionsGridCellDoubleClick(
            object sender,
            DataGridViewCellEventArgs eventArguments
        )
        {
            if (eventArguments.RowIndex >= 0)
            {
                AcceptSelection();
            }
        }

        private void SolutionsGridKeyDown(object sender, KeyEventArgs eventArguments)
        {
            if (eventArguments.KeyCode != Keys.Enter)
            {
                return;
            }

            eventArguments.Handled = true;
            eventArguments.SuppressKeyPress = true;
            AcceptSelection();
        }

        private void SelectButtonClick(object sender, EventArgs eventArguments)
        {
            AcceptSelection();
        }

        private void ApplyFilter()
        {
            var search = searchTextBox.Text.Trim();
            var filteredSolutions = solutions
                .Where(solution =>
                    string.IsNullOrWhiteSpace(search)
                    || Contains(solution.FriendlyName, search)
                    || Contains(solution.UniqueName, search)
                    || Contains(solution.Version, search)
                )
                .ToList();

            solutionsGrid.DataSource = new BindingList<SolutionInfo>(filteredSolutions);
            resultCountLabel.Text =
                filteredSolutions.Count + " of " + solutions.Count + " solutions";
            selectButton.Enabled = filteredSolutions.Count > 0;

            if (solutionsGrid.Rows.Count > 0)
            {
                solutionsGrid.Rows[0].Selected = true;
            }
        }

        private void AcceptSelection()
        {
            if (SelectedSolution != null)
            {
                SolutionAccepted?.Invoke(this, EventArgs.Empty);
            }
        }

        private static bool Contains(string value, string search)
        {
            return value?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
