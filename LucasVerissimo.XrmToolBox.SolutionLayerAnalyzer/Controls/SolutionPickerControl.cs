using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls
{
    public partial class SolutionPickerControl : UserControl
    {
        private readonly SolutionPickerPopup popup;
        private readonly ToolStripDropDown dropDown;
        private readonly ToolStripControlHost popupHost;
        private SolutionInfo selectedSolution;
        private bool updatingValueText;

        public SolutionPickerControl()
        {
            InitializeComponent();

            popup = new SolutionPickerPopup();
            popup.SolutionAccepted += PopupSolutionAccepted;

            popupHost = new ToolStripControlHost(popup)
            {
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
            };

            dropDown = new ToolStripDropDown
            {
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = new Padding(1),
            };
            dropDown.Items.Add(popupHost);
        }

        public event EventHandler SelectedSolutionChanged;

        public SolutionInfo SelectedSolution
        {
            get { return selectedSolution; }
        }

        public void SetSolutions(IEnumerable<SolutionInfo> solutions)
        {
            if (solutions == null)
            {
                throw new ArgumentNullException(nameof(solutions));
            }

            var availableSolutions = solutions.Where(solution => solution != null).ToList();
            popup.SetSolutions(availableSolutions);

            if (
                selectedSolution != null
                && availableSolutions.All(solution =>
                    solution.SolutionId != selectedSolution.SolutionId
                )
            )
            {
                SetSelectedSolution(null);
            }

            if (selectedSolution != null)
            {
                UpdateValueText(
                    selectedSolution.FriendlyName + "  —  " + selectedSolution.UniqueName
                );
            }
            else if (availableSolutions.Count == 0)
            {
                UpdateValueText("No solutions loaded");
            }
            else
            {
                UpdateValueText("Select a solution...");
            }
        }

        public void ClearSolutions()
        {
            popup.SetSolutions(Array.Empty<SolutionInfo>());
            SetSelectedSolution(null);
            UpdateValueText("Load solutions to begin");
        }

        protected override void OnEnabledChanged(EventArgs eventArguments)
        {
            base.OnEnabledChanged(eventArguments);
            valueTextBox.BackColor = Enabled ? SystemColors.Window : SystemColors.Control;
        }

        private void OpenButtonClick(object sender, EventArgs eventArguments)
        {
            ShowSolutionList();
        }

        private void ValueTextBoxClick(object sender, EventArgs eventArguments)
        {
            ShowSolutionList();
        }

        private void ValueTextBoxTextChanged(object sender, EventArgs eventArguments)
        {
            if (updatingValueText)
            {
                return;
            }

            if (selectedSolution != null)
            {
                selectedSolution = null;
                SelectedSolutionChanged?.Invoke(this, EventArgs.Empty);
            }

            popup.SetSearchText(valueTextBox.Text);
            ShowSolutionList(false);
        }

        private void ValueTextBoxKeyDown(object sender, KeyEventArgs eventArguments)
        {
            if (
                eventArguments.KeyCode == Keys.Enter
                || (eventArguments.Alt && eventArguments.KeyCode == Keys.Down)
            )
            {
                eventArguments.Handled = true;
                eventArguments.SuppressKeyPress = true;
                ShowSolutionList();
            }
        }

        private void ShowSolutionList(bool focusSearch = true)
        {
            if (!Enabled || dropDown.Visible)
            {
                return;
            }

            var popupWidth = Math.Max(760, Width);
            var popupSize = new Size(popupWidth, 340);
            popup.Size = popupSize;
            popupHost.Size = popupSize;
            dropDown.Size = new Size(popupSize.Width + 2, popupSize.Height + 2);
            popup.SelectSolution(selectedSolution);
            dropDown.Show(this, new Point(0, Height));
            if (focusSearch)
            {
                popup.FocusSearch();
            }
            else
            {
                valueTextBox.Focus();
            }
        }

        private void PopupSolutionAccepted(object sender, EventArgs eventArguments)
        {
            var solution = popup.SelectedSolution;
            if (solution == null)
            {
                return;
            }

            SetSelectedSolution(solution);
            dropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
        }

        private void SetSelectedSolution(SolutionInfo solution)
        {
            if (
                selectedSolution != null
                && solution != null
                && selectedSolution.SolutionId == solution.SolutionId
            )
            {
                return;
            }

            if (selectedSolution == null && solution == null)
            {
                return;
            }

            selectedSolution = solution;
            UpdateValueText(
                solution == null
                    ? "Select a solution..."
                    : solution.FriendlyName + "  —  " + solution.UniqueName
            );
            valueTextBox.SelectionStart = 0;
            valueTextBox.SelectionLength = 0;
            SelectedSolutionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateValueText(string text)
        {
            updatingValueText = true;
            try
            {
                valueTextBox.Text = text;
            }
            finally
            {
                updatingValueText = false;
            }
        }
    }
}
