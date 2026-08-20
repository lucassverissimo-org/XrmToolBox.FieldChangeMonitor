#nullable enable

namespace LucasVerissimo.XrmToolBox.ToolManager;

partial class ApiKeyForm
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel layout = null!;
    private Label messageLabel = null!;
    private TextBox apiKeyTextBox = null!;
    private CheckBox showApiKeyCheckBox = null!;
    private LinkLabel openNuGetKeysLinkLabel = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

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
        layout = new TableLayoutPanel();
        messageLabel = new Label();
        apiKeyTextBox = new TextBox();
        showApiKeyCheckBox = new CheckBox();
        openNuGetKeysLinkLabel = new LinkLabel();
        buttonsPanel = new FlowLayoutPanel();
        saveButton = new Button();
        cancelButton = new Button();
        layout.SuspendLayout();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        // layout
        layout.ColumnCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(messageLabel, 0, 0);
        layout.Controls.Add(apiKeyTextBox, 0, 1);
        layout.Controls.Add(showApiKeyCheckBox, 0, 2);
        layout.Controls.Add(openNuGetKeysLinkLabel, 0, 3);
        layout.Controls.Add(buttonsPanel, 0, 4);
        layout.Dock = DockStyle.Fill;
        layout.Location = new Point(16, 16);
        layout.Name = "layout";
        layout.RowCount = 5;
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        // controls
        messageLabel.Dock = DockStyle.Fill;
        messageLabel.Name = "messageLabel";
        apiKeyTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        apiKeyTextBox.Name = "apiKeyTextBox";
        apiKeyTextBox.UseSystemPasswordChar = true;
        showApiKeyCheckBox.AutoSize = true;
        showApiKeyCheckBox.Name = "showApiKeyCheckBox";
        showApiKeyCheckBox.Text = "Mostrar chave";
        showApiKeyCheckBox.CheckedChanged += ShowApiKeyCheckBox_CheckedChanged;
        openNuGetKeysLinkLabel.AutoSize = true;
        openNuGetKeysLinkLabel.Name = "openNuGetKeysLinkLabel";
        openNuGetKeysLinkLabel.Text = "Gerenciar API keys no NuGet.org";
        openNuGetKeysLinkLabel.LinkClicked += OpenNuGetKeysLinkLabel_LinkClicked;
        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Dock = DockStyle.Fill;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonsPanel.Name = "buttonsPanel";
        buttonsPanel.Padding = new Padding(0, 8, 0, 0);
        saveButton.AutoSize = true;
        saveButton.Name = "saveButton";
        saveButton.Text = "Salvar e continuar";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += SaveButton_Click;
        cancelButton.AutoSize = true;
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Name = "cancelButton";
        cancelButton.Text = "Cancelar";
        cancelButton.UseVisualStyleBackColor = true;
        // ApiKeyForm
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(534, 241);
        Controls.Add(layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ApiKeyForm";
        Padding = new Padding(16);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Credencial do NuGet";
        layout.ResumeLayout(false);
        layout.PerformLayout();
        buttonsPanel.ResumeLayout(false);
        buttonsPanel.PerformLayout();
        ResumeLayout(false);
    }
}
