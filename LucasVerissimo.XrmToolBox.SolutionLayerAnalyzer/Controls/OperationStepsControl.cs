using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls
{
    internal partial class OperationStepsControl : UserControl
    {
        private static readonly string[] LoadingFrames = { "◐", "◓", "◑", "◒" };
        private Label[] statusLabels;
        private Label[] stepLabels;
        private string[] stepDescriptions = new string[0];
        private int currentStepIndex = -1;
        private int loadingFrameIndex;

        public OperationStepsControl()
        {
            InitializeComponent();
            statusLabels = new[]
            {
                stepOneStatus,
                stepTwoStatus,
                stepThreeStatus,
                stepFourStatus,
                stepFiveStatus,
            };
            stepLabels = new[]
            {
                stepOneLabel,
                stepTwoLabel,
                stepThreeLabel,
                stepFourLabel,
                stepFiveLabel,
            };
            ResetSteps();
        }

        public void BeginOperation(string title, IReadOnlyCollection<string> steps)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("The operation title is required.", nameof(title));
            }

            if (steps == null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            if (steps.Count != stepLabels.Length || steps.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException(
                    "Exactly five non-empty operation steps are required.",
                    nameof(steps)
                );
            }

            titleLabel.Text = title;
            detailLabel.Text = "Starting...";
            stepDescriptions = steps.ToArray();
            currentStepIndex = -1;
            loadingFrameIndex = 0;

            for (var index = 0; index < stepLabels.Length; index++)
            {
                stepLabels[index].Text = stepDescriptions[index];
                SetWaiting(index);
            }

            animationTimer.Stop();
        }

        public void SetCurrentStep(int stepIndex, string detail)
        {
            ValidateStepIndex(stepIndex);
            currentStepIndex = stepIndex;
            detailLabel.Text = string.IsNullOrWhiteSpace(detail)
                ? stepDescriptions[stepIndex]
                : detail;

            for (var index = 0; index < stepLabels.Length; index++)
            {
                if (index < stepIndex)
                {
                    SetCompleted(index);
                }
                else if (index == stepIndex)
                {
                    SetRunning(index);
                }
                else
                {
                    SetWaiting(index);
                }
            }

            animationTimer.Start();
        }

        public void CompleteOperation(string detail)
        {
            currentStepIndex = -1;
            animationTimer.Stop();
            detailLabel.Text = string.IsNullOrWhiteSpace(detail) ? "Completed." : detail;
            for (var index = 0; index < stepLabels.Length; index++)
            {
                SetCompleted(index);
            }
        }

        public void FailOperation(string detail)
        {
            animationTimer.Stop();
            detailLabel.Text = string.IsNullOrWhiteSpace(detail) ? "Operation failed." : detail;
            if (currentStepIndex >= 0 && currentStepIndex < statusLabels.Length)
            {
                statusLabels[currentStepIndex].Text = "✕";
                statusLabels[currentStepIndex].ForeColor = Color.Firebrick;
                stepLabels[currentStepIndex].ForeColor = Color.Firebrick;
            }
        }

        public void CancelOperation(string detail)
        {
            animationTimer.Stop();
            detailLabel.Text = string.IsNullOrWhiteSpace(detail) ? "Operation cancelled." : detail;
            if (currentStepIndex >= 0 && currentStepIndex < statusLabels.Length)
            {
                statusLabels[currentStepIndex].Text = "■";
                statusLabels[currentStepIndex].ForeColor = Color.DarkOrange;
                stepLabels[currentStepIndex].ForeColor = Color.DarkOrange;
            }
        }

        public void ResetSteps()
        {
            animationTimer.Stop();
            titleLabel.Text = "Operation progress";
            detailLabel.Text = string.Empty;
            currentStepIndex = -1;
            stepDescriptions = new string[stepLabels.Length];
            for (var index = 0; index < stepLabels.Length; index++)
            {
                stepDescriptions[index] = "Step " + (index + 1);
                stepLabels[index].Text = stepDescriptions[index];
                SetWaiting(index);
            }
        }

        private void AnimationTimerTick(object sender, EventArgs e)
        {
            if (currentStepIndex < 0 || currentStepIndex >= statusLabels.Length)
            {
                animationTimer.Stop();
                return;
            }

            loadingFrameIndex = (loadingFrameIndex + 1) % LoadingFrames.Length;
            statusLabels[currentStepIndex].Text = LoadingFrames[loadingFrameIndex];
        }

        private void SetWaiting(int index)
        {
            statusLabels[index].Text = "○";
            statusLabels[index].ForeColor = Color.Gray;
            stepLabels[index].ForeColor = Color.DimGray;
        }

        private void SetRunning(int index)
        {
            statusLabels[index].Text = LoadingFrames[loadingFrameIndex];
            statusLabels[index].ForeColor = Color.FromArgb(8, 127, 140);
            stepLabels[index].ForeColor = Color.FromArgb(8, 91, 126);
        }

        private void SetCompleted(int index)
        {
            statusLabels[index].Text = "✓";
            statusLabels[index].ForeColor = Color.ForestGreen;
            stepLabels[index].ForeColor = Color.FromArgb(45, 90, 55);
        }

        private void ValidateStepIndex(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= stepLabels.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(stepIndex));
            }
        }
    }
}
