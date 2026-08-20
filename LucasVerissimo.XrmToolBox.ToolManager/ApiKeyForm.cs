namespace LucasVerissimo.XrmToolBox.ToolManager;

internal partial class ApiKeyForm : Form
{
    public ApiKeyForm(string message)
    {
        InitializeComponent();
        messageLabel.Text = message;
    }

    public string ApiKey => apiKeyTextBox.Text.Trim();

    private void ShowApiKeyCheckBox_CheckedChanged(object? sender, EventArgs eventArgs)
    {
        apiKeyTextBox.UseSystemPasswordChar = !showApiKeyCheckBox.Checked;
    }

    private void SaveButton_Click(object? sender, EventArgs eventArgs)
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            MessageBox.Show(
                this,
                "Informe uma API key.",
                "Credencial do NuGet",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void OpenNuGetKeysLinkLabel_LinkClicked(
        object? sender,
        LinkLabelLinkClickedEventArgs eventArgs
    )
    {
        BrowserService.Open("https://www.nuget.org/account/apikeys");
    }
}
