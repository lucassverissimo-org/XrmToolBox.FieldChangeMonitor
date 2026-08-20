using LucasVerissimo.XrmToolBox.ToolManager.Models;

namespace LucasVerissimo.XrmToolBox.ToolManager;

internal partial class ToolManagementForm : Form
{
    private readonly ToolSummary tool;
    private readonly ReleaseRunner releaseRunner;
    private bool releasePrepared;

    public ToolManagementForm(string repositoryRoot, ToolSummary tool)
    {
        this.tool = tool ?? throw new ArgumentNullException(nameof(tool));
        releaseRunner = new ReleaseRunner(repositoryRoot);
        InitializeComponent();
        PopulateToolInformation();
        incrementComboBox.SelectedIndex = 0;
    }

    private void PopulateToolInformation()
    {
        Text = $"Gerenciar — {tool.Name}";
        nameValueLabel.Text = tool.Name;
        packageIdValueLabel.Text = tool.PackageId;
        localVersionValueLabel.Text = tool.LocalVersion;
        publishedVersionValueLabel.Text = tool.PublishedVersion;
        statusValueLabel.Text = tool.Status;
        versionTextBox.Text = VersionService.Suggest(tool.LocalVersion, VersionIncrement.Patch);
    }

    private void IncrementComboBox_SelectedIndexChanged(object? sender, EventArgs eventArgs)
    {
        VersionIncrement increment = incrementComboBox.SelectedIndex switch
        {
            1 => VersionIncrement.Minor,
            2 => VersionIncrement.Major,
            3 => VersionIncrement.Custom,
            _ => VersionIncrement.Patch,
        };

        versionTextBox.ReadOnly = increment != VersionIncrement.Custom;
        if (increment != VersionIncrement.Custom)
        {
            versionTextBox.Text = VersionService.Suggest(tool.LocalVersion, increment);
        }

        InvalidatePreparedRelease();
    }

    private void ReleaseInput_Changed(object? sender, EventArgs eventArgs)
    {
        InvalidatePreparedRelease();
    }

    private async void PrepareButton_Click(object? sender, EventArgs eventArgs)
    {
        await RunReleaseAsync(publish: false);
    }

    private async void PublishButton_Click(object? sender, EventArgs eventArgs)
    {
        if (!releasePrepared)
        {
            MessageBox.Show(
                this,
                "Prepare e valide o pacote antes de publicá-lo.",
                "Publicar",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            return;
        }

        using PublishConfirmationForm confirmationForm = new(tool.Name, versionTextBox.Text.Trim());
        if (confirmationForm.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        string? apiKey = GetApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return;
        }

        ReleaseResult result = await ExecuteReleaseAsync(publish: true, apiKey);
        if (!result.Succeeded && result.AuthenticationFailed)
        {
            apiKey = RequestApiKey(
                "A autenticação no NuGet falhou. A chave pode estar expirada, revogada ou sem permissão para este pacote."
            );
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                await ExecuteReleaseAsync(publish: true, apiKey);
            }
        }
    }

    private void OpenPackageButton_Click(object? sender, EventArgs eventArgs)
    {
        if (!string.IsNullOrWhiteSpace(tool.Manifest.NuGetUrl))
        {
            BrowserService.Open(tool.Manifest.NuGetUrl);
        }
    }

    private async Task RunReleaseAsync(bool publish)
    {
        if (!ValidateReleaseInput())
        {
            return;
        }

        ReleaseResult result = await ExecuteReleaseAsync(publish, apiKey: null);
        releasePrepared = result.Succeeded && !publish;
        publishButton.Enabled = releasePrepared;
    }

    private async Task<ReleaseResult> ExecuteReleaseAsync(bool publish, string? apiKey)
    {
        SetBusy(true);
        outputTextBox.Clear();
        AppendLog(publish ? "Iniciando publicação..." : "Preparando e validando o release...");

        try
        {
            ReleaseResult result = await releaseRunner.RunAsync(
                tool.Manifest,
                versionTextBox.Text.Trim(),
                releaseNotesTextBox.Text.Trim(),
                publish,
                apiKey,
                AppendLog
            );

            if (result.Succeeded)
            {
                AppendLog(publish ? "Publicação concluída." : "Pacote preparado e validado.");
                MessageBox.Show(
                    this,
                    publish
                        ? "A versão foi publicada no NuGet com sucesso."
                        : "O pacote foi gerado e todas as validações foram concluídas.",
                    publish ? "Publicação concluída" : "Release preparado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                AppendLog("O processo terminou com erro.");
                MessageBox.Show(
                    this,
                    "O processo não foi concluído. Consulte o log para identificar a validação que falhou.",
                    publish ? "Falha na publicação" : "Falha na preparação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            return result;
        }
        catch (Exception exception)
        {
            AppendLog(exception.Message);
            MessageBox.Show(
                this,
                exception.Message,
                "Gerenciar release",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            return new ReleaseResult(false, false, exception.Message);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private bool ValidateReleaseInput()
    {
        if (!VersionService.IsFourPartVersion(versionTextBox.Text.Trim()))
        {
            MessageBox.Show(
                this,
                "Informe a versão com quatro segmentos, por exemplo 1.2.1.0.",
                "Versão inválida",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            versionTextBox.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(releaseNotesTextBox.Text))
        {
            MessageBox.Show(
                this,
                "Informe as notas da versão.",
                "Notas da versão",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            releaseNotesTextBox.Focus();
            return false;
        }

        return true;
    }

    private string? GetApiKey()
    {
        string? apiKey = WindowsCredentialService.ReadNuGetApiKey();
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            return apiKey;
        }

        return RequestApiKey("Informe uma API key do NuGet para publicar o pacote.");
    }

    private string? RequestApiKey(string message)
    {
        using ApiKeyForm apiKeyForm = new(message);
        if (apiKeyForm.ShowDialog(this) != DialogResult.OK)
        {
            return null;
        }

        WindowsCredentialService.SaveNuGetApiKey(apiKeyForm.ApiKey);
        return apiKeyForm.ApiKey;
    }

    private void AppendLog(string message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => AppendLog(message));
            return;
        }

        outputTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private void InvalidatePreparedRelease()
    {
        releasePrepared = false;
        publishButton.Enabled = false;
    }

    private void SetBusy(bool busy)
    {
        UseWaitCursor = busy;
        prepareButton.Enabled = !busy;
        publishButton.Enabled = !busy && releasePrepared;
        incrementComboBox.Enabled = !busy;
        versionTextBox.Enabled = !busy;
        releaseNotesTextBox.Enabled = !busy;
        progressBar.Style = busy ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
        progressBar.Visible = busy;
    }
}
