namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls
{
    partial class SolutionPickerPopup
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
            this.searchLayout = new System.Windows.Forms.TableLayoutPanel();
            this.searchLabel = new System.Windows.Forms.Label();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.solutionsGrid = new System.Windows.Forms.DataGridView();
            this.displayNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uniqueNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.versionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.managedColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.resultCountLabel = new System.Windows.Forms.Label();
            this.selectButton = new System.Windows.Forms.Button();
            this.filterTimer = new System.Windows.Forms.Timer(this.components);
            this.rootLayout.SuspendLayout();
            this.searchLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.solutionsGrid)).BeginInit();
            this.footerLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.BackColor = System.Drawing.Color.White;
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.searchLayout, 0, 0);
            this.rootLayout.Controls.Add(this.solutionsGrid, 0, 1);
            this.rootLayout.Controls.Add(this.footerLayout, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(8);
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.rootLayout.Size = new System.Drawing.Size(760, 340);
            this.rootLayout.TabIndex = 0;
            //
            // searchLayout
            //
            this.searchLayout.ColumnCount = 2;
            this.searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.searchLayout.Controls.Add(this.searchLabel, 0, 0);
            this.searchLayout.Controls.Add(this.searchTextBox, 1, 0);
            this.searchLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLayout.Location = new System.Drawing.Point(11, 11);
            this.searchLayout.Name = "searchLayout";
            this.searchLayout.RowCount = 1;
            this.searchLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.searchLayout.Size = new System.Drawing.Size(738, 30);
            this.searchLayout.TabIndex = 0;
            this.searchLabel.AutoSize = true;
            this.searchLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLabel.Text = "Search";
            this.searchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTextBox.Location = new System.Drawing.Point(65, 3);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(670, 23);
            this.searchTextBox.TabIndex = 0;
            this.searchTextBox.TextChanged += new System.EventHandler(this.SearchTextBoxTextChanged);
            this.searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchTextBoxKeyDown);
            //
            // solutionsGrid
            //
            this.solutionsGrid.AllowUserToAddRows = false;
            this.solutionsGrid.AllowUserToDeleteRows = false;
            this.solutionsGrid.AllowUserToResizeRows = false;
            this.solutionsGrid.AutoGenerateColumns = false;
            this.solutionsGrid.BackgroundColor = System.Drawing.Color.White;
            this.solutionsGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.solutionsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.solutionsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.displayNameColumn, this.uniqueNameColumn, this.versionColumn, this.managedColumn });
            this.solutionsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.solutionsGrid.Location = new System.Drawing.Point(11, 47);
            this.solutionsGrid.MultiSelect = false;
            this.solutionsGrid.Name = "solutionsGrid";
            this.solutionsGrid.ReadOnly = true;
            this.solutionsGrid.RowHeadersVisible = false;
            this.solutionsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.solutionsGrid.Size = new System.Drawing.Size(738, 240);
            this.solutionsGrid.TabIndex = 1;
            this.solutionsGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SolutionsGridCellDoubleClick);
            this.solutionsGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SolutionsGridKeyDown);
            //
            // columns
            //
            this.displayNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.displayNameColumn.DataPropertyName = "FriendlyName";
            this.displayNameColumn.FillWeight = 155F;
            this.displayNameColumn.HeaderText = "Display Name";
            this.displayNameColumn.Name = "displayNameColumn";
            this.displayNameColumn.ReadOnly = true;
            this.uniqueNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.uniqueNameColumn.DataPropertyName = "UniqueName";
            this.uniqueNameColumn.FillWeight = 145F;
            this.uniqueNameColumn.HeaderText = "Logical / Unique Name";
            this.uniqueNameColumn.Name = "uniqueNameColumn";
            this.uniqueNameColumn.ReadOnly = true;
            this.versionColumn.DataPropertyName = "Version";
            this.versionColumn.HeaderText = "Version";
            this.versionColumn.Name = "versionColumn";
            this.versionColumn.ReadOnly = true;
            this.versionColumn.Width = 90;
            this.managedColumn.DataPropertyName = "IsManaged";
            this.managedColumn.HeaderText = "Managed";
            this.managedColumn.Name = "managedColumn";
            this.managedColumn.ReadOnly = true;
            this.managedColumn.Width = 72;
            //
            // footerLayout
            //
            this.footerLayout.ColumnCount = 2;
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.footerLayout.Controls.Add(this.resultCountLabel, 0, 0);
            this.footerLayout.Controls.Add(this.selectButton, 1, 0);
            this.footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerLayout.Location = new System.Drawing.Point(11, 293);
            this.footerLayout.Name = "footerLayout";
            this.footerLayout.RowCount = 1;
            this.footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.Size = new System.Drawing.Size(738, 36);
            this.footerLayout.TabIndex = 2;
            this.resultCountLabel.AutoSize = true;
            this.resultCountLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultCountLabel.Text = "0 of 0 solutions";
            this.resultCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectButton.Location = new System.Drawing.Point(631, 3);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(104, 30);
            this.selectButton.TabIndex = 2;
            this.selectButton.Text = "Select";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.SelectButtonClick);
            //
            // filterTimer
            //
            this.filterTimer.Interval = 250;
            this.filterTimer.Tick += new System.EventHandler(this.FilterTimerTick);
            //
            // SolutionPickerPopup
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "SolutionPickerPopup";
            this.Size = new System.Drawing.Size(760, 340);
            this.rootLayout.ResumeLayout(false);
            this.searchLayout.ResumeLayout(false);
            this.searchLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.solutionsGrid)).EndInit();
            this.footerLayout.ResumeLayout(false);
            this.footerLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel searchLayout;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.DataGridView solutionsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn displayNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn uniqueNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn versionColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn managedColumn;
        private System.Windows.Forms.TableLayoutPanel footerLayout;
        private System.Windows.Forms.Label resultCountLabel;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Timer filterTimer;
    }
}
