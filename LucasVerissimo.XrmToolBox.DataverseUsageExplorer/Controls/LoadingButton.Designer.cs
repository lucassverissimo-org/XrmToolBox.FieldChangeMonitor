namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Controls
{
    partial class LoadingButton
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.animationTimer.Interval = 120;
            this.animationTimer.Tick += new System.EventHandler(this.AnimationTimerTick);
        }

        private System.Windows.Forms.Timer animationTimer;
    }
}
