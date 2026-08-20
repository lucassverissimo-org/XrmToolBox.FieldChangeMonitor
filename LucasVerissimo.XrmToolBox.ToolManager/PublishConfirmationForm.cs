namespace LucasVerissimo.XrmToolBox.ToolManager;

internal partial class PublishConfirmationForm : Form
{
    private readonly string expectedVersion;

    public PublishConfirmationForm(string toolName, string version)
    {
        expectedVersion = version;
        InitializeComponent();
        messageLabel.Text =
            $"A publicação de {toolName} {version} no NuGet é irreversível. "
            + "Digite exatamente a versão para confirmar:";
    }

    private void ConfirmationTextBox_TextChanged(object? sender, EventArgs eventArgs)
    {
        publishButton.Enabled = string.Equals(
            confirmationTextBox.Text.Trim(),
            expectedVersion,
            StringComparison.Ordinal
        );
    }
}
