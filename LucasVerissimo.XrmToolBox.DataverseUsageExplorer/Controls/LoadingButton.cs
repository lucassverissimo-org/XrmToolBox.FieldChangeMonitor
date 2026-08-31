using System;
using System.Windows.Forms;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Controls
{
    internal partial class LoadingButton : Button
    {
        private readonly string[] frames = { "◐", "◓", "◑", "◒" };
        private int frameIndex;
        private string idleText = "Scan";

        public LoadingButton()
        {
            InitializeComponent();
        }

        public bool IsLoading { get; private set; }

        public void StartLoading()
        {
            if (IsLoading)
            {
                return;
            }

            idleText = Text;
            IsLoading = true;
            Enabled = false;
            frameIndex = 0;
            UpdateLoadingText();
            animationTimer.Start();
        }

        public void StopLoading()
        {
            animationTimer.Stop();
            IsLoading = false;
            Text = idleText;
            Enabled = true;
        }

        private void AnimationTimerTick(object sender, EventArgs eventArguments)
        {
            frameIndex = (frameIndex + 1) % frames.Length;
            UpdateLoadingText();
        }

        private void UpdateLoadingText()
        {
            Text = frames[frameIndex] + "  Scanning...";
        }
    }
}
