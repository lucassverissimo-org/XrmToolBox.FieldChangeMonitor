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
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.detailsTextBox = new System.Windows.Forms.TextBox();
            this.linkLayout = new System.Windows.Forms.TableLayoutPanel();
            this.componentLinkCaption = new System.Windows.Forms.Label();
            this.componentLinkLabel = new System.Windows.Forms.LinkLabel();
            this.navigationMessageLabel = new System.Windows.Forms.Label();
            this.rootLayout.SuspendLayout();
            this.linkLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.detailsTextBox, 0, 0);
            this.rootLayout.Controls.Add(this.linkLayout, 0, 1);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.Size = new System.Drawing.Size(760, 560);
            this.rootLayout.TabIndex = 0;
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
            this.detailsTextBox.Size = new System.Drawing.Size(760, 512);
            this.detailsTextBox.TabIndex = 0;
            //
            // linkLayout
            //
            this.linkLayout.ColumnCount = 2;
            this.linkLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.linkLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.linkLayout.Controls.Add(this.componentLinkCaption, 0, 0);
            this.linkLayout.Controls.Add(this.componentLinkLabel, 1, 0);
            this.linkLayout.Controls.Add(this.navigationMessageLabel, 1, 0);
            this.linkLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLayout.Location = new System.Drawing.Point(3, 515);
            this.linkLayout.Name = "linkLayout";
            this.linkLayout.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.linkLayout.RowCount = 1;
            this.linkLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.linkLayout.Size = new System.Drawing.Size(754, 42);
            this.linkLayout.TabIndex = 1;
            //
            // componentLinkCaption
            //
            this.componentLinkCaption.AutoSize = true;
            this.componentLinkCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentLinkCaption.Location = new System.Drawing.Point(11, 6);
            this.componentLinkCaption.Name = "componentLinkCaption";
            this.componentLinkCaption.Size = new System.Drawing.Size(106, 30);
            this.componentLinkCaption.TabIndex = 0;
            this.componentLinkCaption.Text = "Component action:";
            this.componentLinkCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // componentLinkLabel
            //
            this.componentLinkLabel.AutoEllipsis = true;
            this.componentLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentLinkLabel.Location = new System.Drawing.Point(123, 6);
            this.componentLinkLabel.Name = "componentLinkLabel";
            this.componentLinkLabel.Size = new System.Drawing.Size(620, 30);
            this.componentLinkLabel.TabIndex = 1;
            this.componentLinkLabel.TabStop = true;
            this.componentLinkLabel.Text = "Open component";
            this.componentLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.componentLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ComponentLinkLabelLinkClicked);
            //
            // navigationMessageLabel
            //
            this.navigationMessageLabel.AutoEllipsis = true;
            this.navigationMessageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationMessageLabel.Location = new System.Drawing.Point(123, 6);
            this.navigationMessageLabel.Name = "navigationMessageLabel";
            this.navigationMessageLabel.Size = new System.Drawing.Size(620, 30);
            this.navigationMessageLabel.TabIndex = 2;
            this.navigationMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // UsageDetailsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 560);
            this.Controls.Add(this.rootLayout);
            this.Name = "UsageDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Usage reference details";
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.linkLayout.ResumeLayout(false);
            this.linkLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TextBox detailsTextBox;
        private System.Windows.Forms.TableLayoutPanel linkLayout;
        private System.Windows.Forms.Label componentLinkCaption;
        private System.Windows.Forms.LinkLabel componentLinkLabel;
        private System.Windows.Forms.Label navigationMessageLabel;
    }
}
