namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls
{
    partial class SolutionPickerControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dropDown?.Dispose();
                this.components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.openButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // valueTextBox
            //
            this.valueTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.valueTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueTextBox.Location = new System.Drawing.Point(0, 0);
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(266, 23);
            this.valueTextBox.TabIndex = 0;
            this.valueTextBox.Text = "Load solutions to begin";
            this.valueTextBox.Click += new System.EventHandler(this.ValueTextBoxClick);
            this.valueTextBox.TextChanged += new System.EventHandler(this.ValueTextBoxTextChanged);
            this.valueTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ValueTextBoxKeyDown);
            //
            // openButton
            //
            this.openButton.AccessibleName = "Open Source solution list";
            this.openButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.openButton.Location = new System.Drawing.Point(266, 0);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(34, 25);
            this.openButton.TabIndex = 1;
            this.openButton.Text = "▼";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.OpenButtonClick);
            //
            // SolutionPickerControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.valueTextBox);
            this.Controls.Add(this.openButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(180, 25);
            this.Name = "SolutionPickerControl";
            this.Size = new System.Drawing.Size(300, 25);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.Button openButton;
    }
}
