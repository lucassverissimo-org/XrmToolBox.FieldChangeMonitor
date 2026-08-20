#nullable enable

namespace LucasVerissimo.XrmToolBox.ToolManager;

partial class ToolManagementForm
{
    private System.ComponentModel.IContainer? components = null;
    private TabControl tabs = null!;
    private TabPage overviewTab = null!;
    private TableLayoutPanel overviewLayout = null!;
    private Label nameLabel = null!;
    private Label nameValueLabel = null!;
    private Label packageIdLabel = null!;
    private Label packageIdValueLabel = null!;
    private Label localVersionLabel = null!;
    private Label localVersionValueLabel = null!;
    private Label publishedVersionLabel = null!;
    private Label publishedVersionValueLabel = null!;
    private Label statusLabel = null!;
    private Label statusValueLabel = null!;
    private Button openPackageButton = null!;
    private TabPage releaseTab = null!;
    private TableLayoutPanel releaseLayout = null!;
    private Label incrementLabel = null!;
    private ComboBox incrementComboBox = null!;
    private Label versionLabel = null!;
    private TextBox versionTextBox = null!;
    private Label releaseNotesLabel = null!;
    private TextBox releaseNotesTextBox = null!;
    private FlowLayoutPanel releaseButtonsPanel = null!;
    private Button prepareButton = null!;
    private Button publishButton = null!;
    private ProgressBar progressBar = null!;
    private Label outputLabel = null!;
    private TextBox outputTextBox = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        tabs = new TabControl();
        overviewTab = new TabPage();
        overviewLayout = new TableLayoutPanel();
        nameLabel = new Label();
        nameValueLabel = new Label();
        packageIdLabel = new Label();
        packageIdValueLabel = new Label();
        localVersionLabel = new Label();
        localVersionValueLabel = new Label();
        publishedVersionLabel = new Label();
        publishedVersionValueLabel = new Label();
        statusLabel = new Label();
        statusValueLabel = new Label();
        openPackageButton = new Button();
        releaseTab = new TabPage();
        releaseLayout = new TableLayoutPanel();
        incrementLabel = new Label();
        incrementComboBox = new ComboBox();
        versionLabel = new Label();
        versionTextBox = new TextBox();
        releaseNotesLabel = new Label();
        releaseNotesTextBox = new TextBox();
        releaseButtonsPanel = new FlowLayoutPanel();
        prepareButton = new Button();
        publishButton = new Button();
        progressBar = new ProgressBar();
        outputLabel = new Label();
        outputTextBox = new TextBox();
        tabs.SuspendLayout();
        overviewTab.SuspendLayout();
        overviewLayout.SuspendLayout();
        releaseTab.SuspendLayout();
        releaseLayout.SuspendLayout();
        releaseButtonsPanel.SuspendLayout();
        SuspendLayout();
        // tabs
        tabs.Controls.Add(overviewTab);
        tabs.Controls.Add(releaseTab);
        tabs.Dock = DockStyle.Fill;
        tabs.Location = new Point(0, 0);
        tabs.Name = "tabs";
        tabs.SelectedIndex = 0;
        tabs.Size = new Size(884, 661);
        // overviewTab
        overviewTab.Controls.Add(overviewLayout);
        overviewTab.Location = new Point(4, 24);
        overviewTab.Name = "overviewTab";
        overviewTab.Padding = new Padding(20);
        overviewTab.Text = "Visão geral";
        overviewTab.UseVisualStyleBackColor = true;
        // overviewLayout
        overviewLayout.ColumnCount = 2;
        overviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        overviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        overviewLayout.Controls.Add(nameLabel, 0, 0);
        overviewLayout.Controls.Add(nameValueLabel, 1, 0);
        overviewLayout.Controls.Add(packageIdLabel, 0, 1);
        overviewLayout.Controls.Add(packageIdValueLabel, 1, 1);
        overviewLayout.Controls.Add(localVersionLabel, 0, 2);
        overviewLayout.Controls.Add(localVersionValueLabel, 1, 2);
        overviewLayout.Controls.Add(publishedVersionLabel, 0, 3);
        overviewLayout.Controls.Add(publishedVersionValueLabel, 1, 3);
        overviewLayout.Controls.Add(statusLabel, 0, 4);
        overviewLayout.Controls.Add(statusValueLabel, 1, 4);
        overviewLayout.Controls.Add(openPackageButton, 1, 5);
        overviewLayout.Dock = DockStyle.Top;
        overviewLayout.Location = new Point(20, 20);
        overviewLayout.Name = "overviewLayout";
        overviewLayout.RowCount = 6;
        overviewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        overviewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        overviewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        overviewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        overviewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        overviewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        overviewLayout.Size = new Size(836, 238);
        // overview labels
        ConfigureCaption(nameLabel, "Ferramenta:");
        ConfigureValue(nameValueLabel);
        ConfigureCaption(packageIdLabel, "Package ID:");
        ConfigureValue(packageIdValueLabel);
        ConfigureCaption(localVersionLabel, "Versão local:");
        ConfigureValue(localVersionValueLabel);
        ConfigureCaption(publishedVersionLabel, "Versão publicada:");
        ConfigureValue(publishedVersionValueLabel);
        ConfigureCaption(statusLabel, "Situação:");
        ConfigureValue(statusValueLabel);
        // openPackageButton
        openPackageButton.Anchor = AnchorStyles.Left;
        openPackageButton.AutoSize = true;
        openPackageButton.Name = "openPackageButton";
        openPackageButton.Text = "Abrir no NuGet";
        openPackageButton.UseVisualStyleBackColor = true;
        openPackageButton.Click += OpenPackageButton_Click;
        // releaseTab
        releaseTab.Controls.Add(releaseLayout);
        releaseTab.Location = new Point(4, 24);
        releaseTab.Name = "releaseTab";
        releaseTab.Padding = new Padding(16);
        releaseTab.Text = "Preparar e publicar";
        releaseTab.UseVisualStyleBackColor = true;
        // releaseLayout
        releaseLayout.ColumnCount = 2;
        releaseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
        releaseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        releaseLayout.Controls.Add(incrementLabel, 0, 0);
        releaseLayout.Controls.Add(incrementComboBox, 1, 0);
        releaseLayout.Controls.Add(versionLabel, 0, 1);
        releaseLayout.Controls.Add(versionTextBox, 1, 1);
        releaseLayout.Controls.Add(releaseNotesLabel, 0, 2);
        releaseLayout.Controls.Add(releaseNotesTextBox, 1, 2);
        releaseLayout.Controls.Add(releaseButtonsPanel, 1, 3);
        releaseLayout.Controls.Add(progressBar, 1, 4);
        releaseLayout.Controls.Add(outputLabel, 0, 5);
        releaseLayout.Controls.Add(outputTextBox, 0, 6);
        releaseLayout.Dock = DockStyle.Fill;
        releaseLayout.Location = new Point(16, 16);
        releaseLayout.Name = "releaseLayout";
        releaseLayout.RowCount = 7;
        releaseLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        releaseLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        releaseLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 125F));
        releaseLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        releaseLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        releaseLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        releaseLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        releaseLayout.SetColumnSpan(outputLabel, 2);
        releaseLayout.SetColumnSpan(outputTextBox, 2);
        // release inputs
        ConfigureCaption(incrementLabel, "Incremento:");
        incrementComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        incrementComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        incrementComboBox.Items.AddRange(new object[] { "Patch", "Minor", "Major", "Personalizada" });
        incrementComboBox.Name = "incrementComboBox";
        incrementComboBox.SelectedIndexChanged += IncrementComboBox_SelectedIndexChanged;
        ConfigureCaption(versionLabel, "Nova versão:");
        versionTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        versionTextBox.Name = "versionTextBox";
        versionTextBox.TextChanged += ReleaseInput_Changed;
        ConfigureCaption(releaseNotesLabel, "Notas da versão:");
        releaseNotesLabel.TextAlign = ContentAlignment.TopLeft;
        releaseNotesTextBox.AcceptsReturn = true;
        releaseNotesTextBox.Dock = DockStyle.Fill;
        releaseNotesTextBox.Multiline = true;
        releaseNotesTextBox.Name = "releaseNotesTextBox";
        releaseNotesTextBox.ScrollBars = ScrollBars.Vertical;
        releaseNotesTextBox.TextChanged += ReleaseInput_Changed;
        // buttons
        releaseButtonsPanel.Controls.Add(prepareButton);
        releaseButtonsPanel.Controls.Add(publishButton);
        releaseButtonsPanel.Dock = DockStyle.Fill;
        releaseButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
        releaseButtonsPanel.Name = "releaseButtonsPanel";
        releaseButtonsPanel.Padding = new Padding(0, 7, 0, 0);
        prepareButton.AutoSize = true;
        prepareButton.Name = "prepareButton";
        prepareButton.Text = "Preparar e validar";
        prepareButton.UseVisualStyleBackColor = true;
        prepareButton.Click += PrepareButton_Click;
        publishButton.AutoSize = true;
        publishButton.Enabled = false;
        publishButton.Name = "publishButton";
        publishButton.Text = "Publicar no NuGet";
        publishButton.UseVisualStyleBackColor = true;
        publishButton.Click += PublishButton_Click;
        progressBar.Dock = DockStyle.Fill;
        progressBar.Name = "progressBar";
        progressBar.Visible = false;
        outputLabel.AutoSize = true;
        outputLabel.Dock = DockStyle.Fill;
        outputLabel.Font = new Font(outputLabel.Font, FontStyle.Bold);
        outputLabel.Name = "outputLabel";
        outputLabel.Text = "Log da validação";
        outputLabel.TextAlign = ContentAlignment.BottomLeft;
        outputTextBox.BackColor = Color.FromArgb(30, 30, 30);
        outputTextBox.Dock = DockStyle.Fill;
        outputTextBox.Font = new Font("Consolas", 9F);
        outputTextBox.ForeColor = Color.Gainsboro;
        outputTextBox.Multiline = true;
        outputTextBox.Name = "outputTextBox";
        outputTextBox.ReadOnly = true;
        outputTextBox.ScrollBars = ScrollBars.Both;
        outputTextBox.WordWrap = false;
        // ToolManagementForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(884, 661);
        Controls.Add(tabs);
        MinimumSize = new Size(720, 560);
        Name = "ToolManagementForm";
        StartPosition = FormStartPosition.CenterParent;
        tabs.ResumeLayout(false);
        overviewTab.ResumeLayout(false);
        overviewLayout.ResumeLayout(false);
        overviewLayout.PerformLayout();
        releaseTab.ResumeLayout(false);
        releaseLayout.ResumeLayout(false);
        releaseLayout.PerformLayout();
        releaseButtonsPanel.ResumeLayout(false);
        releaseButtonsPanel.PerformLayout();
        ResumeLayout(false);
    }

    private static void ConfigureCaption(Label label, string text)
    {
        label.AutoSize = true;
        label.Dock = DockStyle.Fill;
        label.Font = new Font(label.Font, FontStyle.Bold);
        label.Text = text;
        label.TextAlign = ContentAlignment.MiddleLeft;
    }

    private static void ConfigureValue(Label label)
    {
        label.AutoEllipsis = true;
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleLeft;
    }
}
