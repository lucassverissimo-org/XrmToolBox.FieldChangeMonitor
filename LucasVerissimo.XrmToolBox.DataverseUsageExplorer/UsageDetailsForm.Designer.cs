namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer
{
    partial class UsageDetailsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.detailsTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            //
            // detailsTextBox
            //
            this.detailsTextBox.BackColor = System.Drawing.Color.White;
            this.detailsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsTextBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.detailsTextBox.Location = new System.Drawing.Point(0, 0);
            this.detailsTextBox.Multiline = true;
            this.detailsTextBox.Name = "detailsTextBox";
            this.detailsTextBox.ReadOnly = true;
            this.detailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.detailsTextBox.Size = new System.Drawing.Size(760, 560);
            this.detailsTextBox.TabIndex = 0;
            //
            // UsageDetailsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 560);
            this.Controls.Add(this.detailsTextBox);
            this.Name = "UsageDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Usage reference details";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox detailsTextBox;
    }
}
