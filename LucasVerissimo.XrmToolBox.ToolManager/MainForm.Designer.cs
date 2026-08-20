#nullable enable

namespace LucasVerissimo.XrmToolBox.ToolManager;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private ToolStrip actionsToolStrip = null!;
    private ToolStripButton newToolButton = null!;
    private ToolStripButton manageButton = null!;
    private ToolStripSeparator firstSeparator = null!;
    private ToolStripButton openGitButton = null!;
    private ToolStripButton openNuGetButton = null!;
    private ToolStripSeparator secondSeparator = null!;
    private ToolStripButton refreshButton = null!;
    private DataGridView toolsGrid = null!;
    private DataGridViewTextBoxColumn nameColumn = null!;
    private DataGridViewTextBoxColumn localVersionColumn = null!;
    private DataGridViewTextBoxColumn publishedVersionColumn = null!;
    private DataGridViewTextBoxColumn targetFrameworkColumn = null!;
    private DataGridViewTextBoxColumn statusColumn = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel statusLabel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            toolsBindingSource.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        actionsToolStrip = new ToolStrip();
        newToolButton = new ToolStripButton();
        manageButton = new ToolStripButton();
        firstSeparator = new ToolStripSeparator();
        openGitButton = new ToolStripButton();
        openNuGetButton = new ToolStripButton();
        secondSeparator = new ToolStripSeparator();
        refreshButton = new ToolStripButton();
        toolsGrid = new DataGridView();
        nameColumn = new DataGridViewTextBoxColumn();
        localVersionColumn = new DataGridViewTextBoxColumn();
        publishedVersionColumn = new DataGridViewTextBoxColumn();
        targetFrameworkColumn = new DataGridViewTextBoxColumn();
        statusColumn = new DataGridViewTextBoxColumn();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        actionsToolStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)toolsGrid).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // actionsToolStrip
        actionsToolStrip.GripStyle = ToolStripGripStyle.Hidden;
        actionsToolStrip.ImageScalingSize = new Size(24, 24);
        actionsToolStrip.Items.AddRange(
            new ToolStripItem[]
            {
                newToolButton,
                manageButton,
                firstSeparator,
                openGitButton,
                openNuGetButton,
                secondSeparator,
                refreshButton,
            }
        );
        actionsToolStrip.Location = new Point(0, 0);
        actionsToolStrip.Name = "actionsToolStrip";
        actionsToolStrip.Padding = new Padding(8, 6, 8, 6);
        actionsToolStrip.Size = new Size(1064, 43);
        // newToolButton
        newToolButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        newToolButton.Name = "newToolButton";
        newToolButton.Size = new Size(101, 28);
        newToolButton.Text = "Nova ferramenta";
        newToolButton.Click += NewToolButton_Click;
        // manageButton
        manageButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        manageButton.Name = "manageButton";
        manageButton.Size = new Size(68, 28);
        manageButton.Text = "Gerenciar";
        manageButton.Click += ManageButton_Click;
        // openGitButton
        openGitButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        openGitButton.Name = "openGitButton";
        openGitButton.Size = new Size(61, 28);
        openGitButton.Text = "Abrir Git";
        openGitButton.Click += OpenGitButton_Click;
        // openNuGetButton
        openNuGetButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        openNuGetButton.Name = "openNuGetButton";
        openNuGetButton.Size = new Size(79, 28);
        openNuGetButton.Text = "Abrir NuGet";
        openNuGetButton.Click += OpenNuGetButton_Click;
        // refreshButton
        refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(62, 28);
        refreshButton.Text = "Atualizar";
        refreshButton.Click += RefreshButton_Click;
        // toolsGrid
        toolsGrid.AllowUserToAddRows = false;
        toolsGrid.AllowUserToDeleteRows = false;
        toolsGrid.AllowUserToOrderColumns = true;
        toolsGrid.AutoGenerateColumns = false;
        toolsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        toolsGrid.BackgroundColor = SystemColors.Window;
        toolsGrid.BorderStyle = BorderStyle.None;
        toolsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        toolsGrid.Columns.AddRange(
            nameColumn,
            localVersionColumn,
            publishedVersionColumn,
            targetFrameworkColumn,
            statusColumn
        );
        toolsGrid.Dock = DockStyle.Fill;
        toolsGrid.Location = new Point(0, 43);
        toolsGrid.MultiSelect = true;
        toolsGrid.Name = "toolsGrid";
        toolsGrid.ReadOnly = true;
        toolsGrid.RowHeadersVisible = false;
        toolsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        toolsGrid.Size = new Size(1064, 558);
        toolsGrid.TabIndex = 0;
        toolsGrid.CellDoubleClick += ToolsGrid_CellDoubleClick;
        // columns
        nameColumn.DataPropertyName = "Name";
        nameColumn.FillWeight = 160F;
        nameColumn.HeaderText = "Ferramenta";
        nameColumn.Name = "nameColumn";
        nameColumn.ReadOnly = true;
        localVersionColumn.DataPropertyName = "LocalVersion";
        localVersionColumn.HeaderText = "Versão local";
        localVersionColumn.Name = "localVersionColumn";
        localVersionColumn.ReadOnly = true;
        publishedVersionColumn.DataPropertyName = "PublishedVersion";
        publishedVersionColumn.HeaderText = "Versão no NuGet";
        publishedVersionColumn.Name = "publishedVersionColumn";
        publishedVersionColumn.ReadOnly = true;
        targetFrameworkColumn.DataPropertyName = "TargetFramework";
        targetFrameworkColumn.HeaderText = "Framework";
        targetFrameworkColumn.Name = "targetFrameworkColumn";
        targetFrameworkColumn.ReadOnly = true;
        statusColumn.DataPropertyName = "Status";
        statusColumn.HeaderText = "Situação";
        statusColumn.Name = "statusColumn";
        statusColumn.ReadOnly = true;
        // statusStrip
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 601);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1064, 22);
        statusLabel.Name = "statusLabel";
        statusLabel.Text = "Pronto.";
        // MainForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1064, 623);
        Controls.Add(toolsGrid);
        Controls.Add(actionsToolStrip);
        Controls.Add(statusStrip);
        MinimumSize = new Size(850, 480);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Gerenciador de Ferramentas XrmToolBox";
        actionsToolStrip.ResumeLayout(false);
        actionsToolStrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)toolsGrid).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
