namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Controls
{
    partial class ScannerSelectionControl
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
            this.businessRulesCheckBox = new System.Windows.Forms.CheckBox();
            this.businessRulesStatus = new System.Windows.Forms.Label();
            this.formsCheckBox = new System.Windows.Forms.CheckBox();
            this.formsStatus = new System.Windows.Forms.Label();
            this.powerAutomateCheckBox = new System.Windows.Forms.CheckBox();
            this.powerAutomateStatus = new System.Windows.Forms.Label();
            this.viewsCheckBox = new System.Windows.Forms.CheckBox();
            this.viewsStatus = new System.Windows.Forms.Label();
            this.classicWorkflowsCheckBox = new System.Windows.Forms.CheckBox();
            this.classicWorkflowsStatus = new System.Windows.Forms.Label();
            this.pluginStepsCheckBox = new System.Windows.Forms.CheckBox();
            this.pluginStepsStatus = new System.Windows.Forms.Label();
            this.businessProcessFlowsCheckBox = new System.Windows.Forms.CheckBox();
            this.businessProcessFlowsStatus = new System.Windows.Forms.Label();
            this.webResourcesCheckBox = new System.Windows.Forms.CheckBox();
            this.webResourcesStatus = new System.Windows.Forms.Label();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.statusToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.rootLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 4;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.rootLayout.Controls.Add(this.businessRulesCheckBox, 0, 0);
            this.rootLayout.Controls.Add(this.businessRulesStatus, 1, 0);
            this.rootLayout.Controls.Add(this.formsCheckBox, 2, 0);
            this.rootLayout.Controls.Add(this.formsStatus, 3, 0);
            this.rootLayout.Controls.Add(this.powerAutomateCheckBox, 0, 1);
            this.rootLayout.Controls.Add(this.powerAutomateStatus, 1, 1);
            this.rootLayout.Controls.Add(this.viewsCheckBox, 2, 1);
            this.rootLayout.Controls.Add(this.viewsStatus, 3, 1);
            this.rootLayout.Controls.Add(this.classicWorkflowsCheckBox, 0, 2);
            this.rootLayout.Controls.Add(this.classicWorkflowsStatus, 1, 2);
            this.rootLayout.Controls.Add(this.pluginStepsCheckBox, 2, 2);
            this.rootLayout.Controls.Add(this.pluginStepsStatus, 3, 2);
            this.rootLayout.Controls.Add(this.businessProcessFlowsCheckBox, 0, 3);
            this.rootLayout.Controls.Add(this.businessProcessFlowsStatus, 1, 3);
            this.rootLayout.Controls.Add(this.webResourcesCheckBox, 2, 3);
            this.rootLayout.Controls.Add(this.webResourcesStatus, 3, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.rootLayout.Size = new System.Drawing.Size(760, 92);
            //
            // scanner check boxes
            //
            ConfigureCheckBox(this.businessRulesCheckBox, "businessRulesCheckBox", "Business Rules", 0);
            ConfigureCheckBox(this.formsCheckBox, "formsCheckBox", "Forms", 1);
            ConfigureCheckBox(this.powerAutomateCheckBox, "powerAutomateCheckBox", "Power Automate", 2);
            ConfigureCheckBox(this.viewsCheckBox, "viewsCheckBox", "Views", 3);
            ConfigureCheckBox(this.classicWorkflowsCheckBox, "classicWorkflowsCheckBox", "Classic Workflows", 4);
            ConfigureCheckBox(this.pluginStepsCheckBox, "pluginStepsCheckBox", "Plugin Steps", 5);
            ConfigureCheckBox(this.businessProcessFlowsCheckBox, "businessProcessFlowsCheckBox", "Business Process Flows", 6);
            ConfigureCheckBox(this.webResourcesCheckBox, "webResourcesCheckBox", "Web Resources", 7);
            //
            // status labels
            //
            ConfigureStatusLabel(this.businessRulesStatus, "businessRulesStatus");
            ConfigureStatusLabel(this.formsStatus, "formsStatus");
            ConfigureStatusLabel(this.powerAutomateStatus, "powerAutomateStatus");
            ConfigureStatusLabel(this.viewsStatus, "viewsStatus");
            ConfigureStatusLabel(this.classicWorkflowsStatus, "classicWorkflowsStatus");
            ConfigureStatusLabel(this.pluginStepsStatus, "pluginStepsStatus");
            ConfigureStatusLabel(this.businessProcessFlowsStatus, "businessProcessFlowsStatus");
            ConfigureStatusLabel(this.webResourcesStatus, "webResourcesStatus");
            //
            // animationTimer
            //
            this.animationTimer.Interval = 120;
            this.animationTimer.Tick += new System.EventHandler(this.AnimationTimerTick);
            //
            // ScannerSelectionControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rootLayout);
            this.Name = "ScannerSelectionControl";
            this.Size = new System.Drawing.Size(760, 92);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        private static void ConfigureCheckBox(
            System.Windows.Forms.CheckBox checkBox,
            string name,
            string text,
            int tabIndex
        )
        {
            checkBox.AutoSize = true;
            checkBox.Checked = true;
            checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox.Dock = System.Windows.Forms.DockStyle.Fill;
            checkBox.Name = name;
            checkBox.TabIndex = tabIndex;
            checkBox.Text = text;
            checkBox.UseVisualStyleBackColor = true;
        }

        private static void ConfigureStatusLabel(
            System.Windows.Forms.Label label,
            string name
        )
        {
            label.AutoEllipsis = true;
            label.Dock = System.Windows.Forms.DockStyle.Fill;
            label.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            label.Name = name;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.CheckBox businessRulesCheckBox;
        private System.Windows.Forms.Label businessRulesStatus;
        private System.Windows.Forms.CheckBox formsCheckBox;
        private System.Windows.Forms.Label formsStatus;
        private System.Windows.Forms.CheckBox powerAutomateCheckBox;
        private System.Windows.Forms.Label powerAutomateStatus;
        private System.Windows.Forms.CheckBox viewsCheckBox;
        private System.Windows.Forms.Label viewsStatus;
        private System.Windows.Forms.CheckBox classicWorkflowsCheckBox;
        private System.Windows.Forms.Label classicWorkflowsStatus;
        private System.Windows.Forms.CheckBox pluginStepsCheckBox;
        private System.Windows.Forms.Label pluginStepsStatus;
        private System.Windows.Forms.CheckBox businessProcessFlowsCheckBox;
        private System.Windows.Forms.Label businessProcessFlowsStatus;
        private System.Windows.Forms.CheckBox webResourcesCheckBox;
        private System.Windows.Forms.Label webResourcesStatus;
        private System.Windows.Forms.Timer animationTimer;
        private System.Windows.Forms.ToolTip statusToolTip;
    }
}
