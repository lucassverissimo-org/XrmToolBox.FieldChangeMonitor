using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Controls
{
    internal partial class ScannerSelectionControl : UserControl
    {
        private readonly Dictionary<string, ScannerItem> items;
        private readonly string[] spinnerFrames = { "◐", "◓", "◑", "◒" };
        private int spinnerFrameIndex;

        public ScannerSelectionControl()
        {
            InitializeComponent();
            items = CreateItems();
        }

        public IReadOnlyCollection<string> CheckedItems
        {
            get
            {
                return items
                    .Values.Where(item => item.CheckBox.Checked)
                    .Select(item => item.Name)
                    .ToList();
            }
        }

        public void BeginScan(IEnumerable<string> selectedScannerNames)
        {
            var selected = new HashSet<string>(
                selectedScannerNames ?? Array.Empty<string>(),
                StringComparer.OrdinalIgnoreCase
            );

            foreach (var item in items.Values)
            {
                item.CheckBox.Enabled = false;
                SetState(
                    item,
                    selected.Contains(item.Name) ? ScannerItemState.Pending : ScannerItemState.Idle,
                    null
                );
            }
        }

        public void MarkStarted(string scannerName)
        {
            SetState(Find(scannerName), ScannerItemState.Running, "Preparing...");
            EnsureAnimationState();
        }

        public void MarkProgress(string scannerName, int current, int total)
        {
            SetState(Find(scannerName), ScannerItemState.Running, current + "/" + total);
            EnsureAnimationState();
        }

        public void MarkCompleted(string scannerName, int referenceCount)
        {
            SetState(
                Find(scannerName),
                ScannerItemState.Completed,
                referenceCount + " reference(s) found"
            );
            EnsureAnimationState();
        }

        public void MarkFailed(string scannerName, string errorMessage)
        {
            SetState(Find(scannerName), ScannerItemState.Failed, errorMessage);
            EnsureAnimationState();
        }

        public void FinishCancellation()
        {
            foreach (var item in items.Values)
            {
                if (item.State == ScannerItemState.Running)
                {
                    SetState(item, ScannerItemState.Cancelled, item.Detail);
                }
                else if (item.State == ScannerItemState.Pending)
                {
                    SetState(item, ScannerItemState.Idle, null);
                }
            }

            EndScan();
        }

        public void EndScan()
        {
            animationTimer.Stop();
            foreach (var item in items.Values)
            {
                item.CheckBox.Enabled = true;
            }
        }

        private Dictionary<string, ScannerItem> CreateItems()
        {
            return new[]
            {
                CreateItem("Business Rules", businessRulesCheckBox, businessRulesStatus),
                CreateItem("Power Automate", powerAutomateCheckBox, powerAutomateStatus),
                CreateItem("Classic Workflows", classicWorkflowsCheckBox, classicWorkflowsStatus),
                CreateItem(
                    "Business Process Flows",
                    businessProcessFlowsCheckBox,
                    businessProcessFlowsStatus
                ),
                CreateItem("Forms", formsCheckBox, formsStatus),
                CreateItem("Views", viewsCheckBox, viewsStatus),
                CreateItem("Plugin Steps", pluginStepsCheckBox, pluginStepsStatus),
                CreateItem("Web Resources", webResourcesCheckBox, webResourcesStatus),
            }.ToDictionary(item => item.Name, StringComparer.OrdinalIgnoreCase);
        }

        private ScannerItem CreateItem(string name, CheckBox checkBox, Label statusLabel)
        {
            return new ScannerItem
            {
                Name = name,
                CheckBox = checkBox,
                StatusLabel = statusLabel,
            };
        }

        private ScannerItem Find(string scannerName)
        {
            if (!items.TryGetValue(scannerName, out var item))
            {
                throw new InvalidOperationException("Unknown scanner: " + scannerName);
            }

            return item;
        }

        private void SetState(ScannerItem item, ScannerItemState state, string detail)
        {
            item.State = state;
            item.Detail = detail;
            item.StatusLabel.Text = GetStatusText(item);
            item.StatusLabel.ForeColor = GetStatusColor(state);
            statusToolTip.SetToolTip(item.StatusLabel, GetToolTip(item));
        }

        private string GetStatusText(ScannerItem item)
        {
            switch (item.State)
            {
                case ScannerItemState.Pending:
                    return "○";
                case ScannerItemState.Running:
                    return spinnerFrames[spinnerFrameIndex];
                case ScannerItemState.Completed:
                    return "✓";
                case ScannerItemState.Failed:
                    return "!";
                case ScannerItemState.Cancelled:
                    return "■";
                default:
                    return string.Empty;
            }
        }

        private static Color GetStatusColor(ScannerItemState state)
        {
            switch (state)
            {
                case ScannerItemState.Pending:
                    return Color.SlateGray;
                case ScannerItemState.Running:
                    return Color.RoyalBlue;
                case ScannerItemState.Completed:
                    return Color.SeaGreen;
                case ScannerItemState.Failed:
                    return Color.Firebrick;
                case ScannerItemState.Cancelled:
                    return Color.DarkOrange;
                default:
                    return SystemColors.ControlText;
            }
        }

        private static string GetToolTip(ScannerItem item)
        {
            var stateName = item.State.ToString();
            return string.IsNullOrWhiteSpace(item.Detail)
                ? stateName
                : stateName + ": " + item.Detail;
        }

        private void EnsureAnimationState()
        {
            if (items.Values.Any(item => item.State == ScannerItemState.Running))
            {
                animationTimer.Start();
            }
            else
            {
                animationTimer.Stop();
            }
        }

        private void AnimationTimerTick(object sender, EventArgs eventArguments)
        {
            spinnerFrameIndex = (spinnerFrameIndex + 1) % spinnerFrames.Length;
            foreach (var item in items.Values.Where(item => item.State == ScannerItemState.Running))
            {
                item.StatusLabel.Text = GetStatusText(item);
            }
        }

        private sealed class ScannerItem
        {
            public string Name { get; set; }

            public CheckBox CheckBox { get; set; }

            public Label StatusLabel { get; set; }

            public ScannerItemState State { get; set; }

            public string Detail { get; set; }
        }

        private enum ScannerItemState
        {
            Idle,
            Pending,
            Running,
            Completed,
            Failed,
            Cancelled,
        }
    }
}
