namespace LucasVerissimo.XrmToolBox.Shared.Controls
{
    partial class GridPickerPopup
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
            this.itemsGrid = new System.Windows.Forms.DataGridView();
            this.footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.resultCountLabel = new System.Windows.Forms.Label();
            this.selectButton = new System.Windows.Forms.Button();
            this.filterTimer = new System.Windows.Forms.Timer(this.components);
            this.rootLayout.SuspendLayout();
            this.searchLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemsGrid)).BeginInit();
            this.footerLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.BackColor = System.Drawing.Color.White;
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.searchLayout, 0, 0);
            this.rootLayout.Controls.Add(this.itemsGrid, 0, 1);
            this.rootLayout.Controls.Add(this.footerLayout, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(8);
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.rootLayout.Size = new System.Drawing.Size(680, 340);
            //
            // searchLayout
            //
            this.searchLayout.ColumnCount = 2;
            this.searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.searchLayout.Controls.Add(this.searchLabel, 0, 0);
            this.searchLayout.Controls.Add(this.searchTextBox, 1, 0);
            this.searchLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLayout.Name = "searchLayout";
            //
            // searchLabel
            //
            this.searchLabel.AutoSize = true;
            this.searchLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLabel.Text = "Search";
            this.searchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // searchTextBox
            //
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.TabIndex = 0;
            this.searchTextBox.TextChanged += new System.EventHandler(this.SearchTextBoxTextChanged);
            this.searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchTextBoxKeyDown);
            //
            // itemsGrid
            //
            this.itemsGrid.AllowUserToAddRows = false;
            this.itemsGrid.AllowUserToDeleteRows = false;
            this.itemsGrid.AllowUserToResizeRows = false;
            this.itemsGrid.AutoGenerateColumns = false;
            this.itemsGrid.BackgroundColor = System.Drawing.Color.White;
            this.itemsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.itemsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsGrid.MultiSelect = false;
            this.itemsGrid.Name = "itemsGrid";
            this.itemsGrid.ReadOnly = true;
            this.itemsGrid.RowHeadersVisible = false;
            this.itemsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.itemsGrid.TabIndex = 1;
            this.itemsGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ItemsGridCellDoubleClick);
            this.itemsGrid.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ItemsGridColumnHeaderMouseClick);
            this.itemsGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemsGridKeyDown);
            //
            // footerLayout
            //
            this.footerLayout.ColumnCount = 2;
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.footerLayout.Controls.Add(this.resultCountLabel, 0, 0);
            this.footerLayout.Controls.Add(this.selectButton, 1, 0);
            this.footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerLayout.Name = "footerLayout";
            //
            // resultCountLabel
            //
            this.resultCountLabel.AutoSize = true;
            this.resultCountLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultCountLabel.Text = "0 of 0 items";
            this.resultCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // selectButton
            //
            this.selectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectButton.Name = "selectButton";
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
            // GridPickerPopup
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "GridPickerPopup";
            this.Size = new System.Drawing.Size(680, 340);
            this.rootLayout.ResumeLayout(false);
            this.searchLayout.ResumeLayout(false);
            this.searchLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemsGrid)).EndInit();
            this.footerLayout.ResumeLayout(false);
            this.footerLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel searchLayout;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.DataGridView itemsGrid;
        private System.Windows.Forms.TableLayoutPanel footerLayout;
        private System.Windows.Forms.Label resultCountLabel;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Timer filterTimer;
    }
}
