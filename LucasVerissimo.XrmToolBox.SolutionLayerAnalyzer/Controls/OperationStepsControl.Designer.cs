namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls
{
    partial class OperationStepsControl
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
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.detailLabel = new System.Windows.Forms.Label();
            this.stepOneStatus = new System.Windows.Forms.Label();
            this.stepOneLabel = new System.Windows.Forms.Label();
            this.stepTwoStatus = new System.Windows.Forms.Label();
            this.stepTwoLabel = new System.Windows.Forms.Label();
            this.stepThreeStatus = new System.Windows.Forms.Label();
            this.stepThreeLabel = new System.Windows.Forms.Label();
            this.stepFourStatus = new System.Windows.Forms.Label();
            this.stepFourLabel = new System.Windows.Forms.Label();
            this.stepFiveStatus = new System.Windows.Forms.Label();
            this.stepFiveLabel = new System.Windows.Forms.Label();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.rootLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 2;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.titleLabel, 0, 0);
            this.rootLayout.Controls.Add(this.detailLabel, 0, 1);
            this.rootLayout.Controls.Add(this.stepOneStatus, 0, 2);
            this.rootLayout.Controls.Add(this.stepOneLabel, 1, 2);
            this.rootLayout.Controls.Add(this.stepTwoStatus, 0, 3);
            this.rootLayout.Controls.Add(this.stepTwoLabel, 1, 3);
            this.rootLayout.Controls.Add(this.stepThreeStatus, 0, 4);
            this.rootLayout.Controls.Add(this.stepThreeLabel, 1, 4);
            this.rootLayout.Controls.Add(this.stepFourStatus, 0, 5);
            this.rootLayout.Controls.Add(this.stepFourLabel, 1, 5);
            this.rootLayout.Controls.Add(this.stepFiveStatus, 0, 6);
            this.rootLayout.Controls.Add(this.stepFiveLabel, 1, 6);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(9, 5, 9, 5);
            this.rootLayout.RowCount = 7;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.rootLayout.Size = new System.Drawing.Size(700, 150);
            this.rootLayout.TabIndex = 0;
            //
            // titleLabel
            //
            this.titleLabel.AutoEllipsis = true;
            this.rootLayout.SetColumnSpan(this.titleLabel, 2);
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(8, 91, 126);
            this.titleLabel.Text = "Operation progress";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // detailLabel
            //
            this.detailLabel.AutoEllipsis = true;
            this.rootLayout.SetColumnSpan(this.detailLabel, 2);
            this.detailLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailLabel.ForeColor = System.Drawing.Color.DimGray;
            this.detailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // step status labels
            //
            this.ConfigureStatusLabel(this.stepOneStatus);
            this.ConfigureStatusLabel(this.stepTwoStatus);
            this.ConfigureStatusLabel(this.stepThreeStatus);
            this.ConfigureStatusLabel(this.stepFourStatus);
            this.ConfigureStatusLabel(this.stepFiveStatus);
            //
            // step description labels
            //
            this.ConfigureStepLabel(this.stepOneLabel, "Step 1");
            this.ConfigureStepLabel(this.stepTwoLabel, "Step 2");
            this.ConfigureStepLabel(this.stepThreeLabel, "Step 3");
            this.ConfigureStepLabel(this.stepFourLabel, "Step 4");
            this.ConfigureStepLabel(this.stepFiveLabel, "Step 5");
            //
            // animationTimer
            //
            this.animationTimer.Interval = 160;
            this.animationTimer.Tick += new System.EventHandler(this.AnimationTimerTick);
            //
            // OperationStepsControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 251);
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "OperationStepsControl";
            this.Size = new System.Drawing.Size(700, 150);
            this.rootLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void ConfigureStatusLabel(System.Windows.Forms.Label label)
        {
            label.Dock = System.Windows.Forms.DockStyle.Fill;
            label.Font = new System.Drawing.Font("Segoe UI Symbol", 11F, System.Drawing.FontStyle.Bold);
            label.ForeColor = System.Drawing.Color.Gray;
            label.Text = "○";
            label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }

        private void ConfigureStepLabel(System.Windows.Forms.Label label, string text)
        {
            label.AutoEllipsis = true;
            label.Dock = System.Windows.Forms.DockStyle.Fill;
            label.ForeColor = System.Drawing.Color.DimGray;
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label detailLabel;
        private System.Windows.Forms.Label stepOneStatus;
        private System.Windows.Forms.Label stepOneLabel;
        private System.Windows.Forms.Label stepTwoStatus;
        private System.Windows.Forms.Label stepTwoLabel;
        private System.Windows.Forms.Label stepThreeStatus;
        private System.Windows.Forms.Label stepThreeLabel;
        private System.Windows.Forms.Label stepFourStatus;
        private System.Windows.Forms.Label stepFourLabel;
        private System.Windows.Forms.Label stepFiveStatus;
        private System.Windows.Forms.Label stepFiveLabel;
        private System.Windows.Forms.Timer animationTimer;
    }
}
