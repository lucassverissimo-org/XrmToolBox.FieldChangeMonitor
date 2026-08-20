using LucasVerissimo.XrmToolBox.ToolManager.Models;

namespace LucasVerissimo.XrmToolBox.ToolManager;

internal partial class MainForm : Form
{
    private const string RepositoryGitUrl =
        "https://github.com/lucassverissimo-org/XrmToolBox.FieldChangeMonitor";
    private const string RootNuGetUrl =
        "https://www.nuget.org/packages?q=LucasVerissimo.XrmToolBox";

    private readonly string repositoryRoot;
    private readonly ToolCatalogService catalogService;
    private readonly BindingSource toolsBindingSource = new();

    public MainForm(string repositoryRoot, ToolCatalogService catalogService)
    {
        this.repositoryRoot = repositoryRoot;
        this.catalogService = catalogService;
        InitializeComponent();
    }

    protected override async void OnShown(EventArgs eventArgs)
    {
        base.OnShown(eventArgs);
        await RefreshToolsAsync();
    }

    private async void RefreshButton_Click(object? sender, EventArgs eventArgs)
    {
        await RefreshToolsAsync();
    }

    private void NewToolButton_Click(object? sender, EventArgs eventArgs)
    {
        MessageBox.Show(
            this,
            "O assistente para criar uma nova ferramenta ainda não foi implementado.",
            "Nova ferramenta",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    private void OpenGitButton_Click(object? sender, EventArgs eventArgs)
    {
        OpenSelectedUrls(tool => tool.Manifest.GitUrl, RepositoryGitUrl, "Git");
    }

    private void OpenNuGetButton_Click(object? sender, EventArgs eventArgs)
    {
        OpenSelectedUrls(tool => tool.Manifest.NuGetUrl, RootNuGetUrl, "NuGet");
    }

    private void ManageButton_Click(object? sender, EventArgs eventArgs)
    {
        ToolSummary? selectedTool = GetSelectedTools().FirstOrDefault();
        if (selectedTool is null)
        {
            MessageBox.Show(
                this,
                "Selecione uma ferramenta para gerenciar.",
                "Gerenciar ferramenta",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            return;
        }

        using ToolManagementForm form = new(repositoryRoot, selectedTool);
        form.ShowDialog(this);
    }

    private void ToolsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs eventArgs)
    {
        if (eventArgs.RowIndex >= 0)
        {
            ManageButton_Click(sender, EventArgs.Empty);
        }
    }

    private async Task RefreshToolsAsync()
    {
        SetBusy(true, "Carregando ferramentas e consultando o NuGet...");
        try
        {
            IReadOnlyList<ToolSummary> tools = await catalogService.LoadAsync();
            toolsBindingSource.DataSource = tools;
            toolsGrid.DataSource = toolsBindingSource;
            statusLabel.Text = $"{tools.Count} ferramenta(s) encontrada(s).";
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Falha ao carregar as ferramentas.";
            MessageBox.Show(
                this,
                exception.Message,
                "Minhas Ferramentas",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        finally
        {
            SetBusy(false, statusLabel.Text ?? string.Empty);
        }
    }

    private IReadOnlyList<ToolSummary> GetSelectedTools()
    {
        return toolsGrid
            .SelectedRows.Cast<DataGridViewRow>()
            .Select(row => row.DataBoundItem)
            .OfType<ToolSummary>()
            .DistinctBy(tool => tool.PackageId)
            .ToList();
    }

    private void OpenSelectedUrls(
        Func<ToolSummary, string> getUrl,
        string fallbackUrl,
        string destinationName
    )
    {
        IReadOnlyList<ToolSummary> selectedTools = GetSelectedTools();
        if (selectedTools.Count == 0)
        {
            BrowserService.Open(fallbackUrl);
            return;
        }

        List<string> urls = selectedTools
            .Select(getUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (urls.Count == 0)
        {
            MessageBox.Show(
                this,
                $"As ferramentas selecionadas não possuem uma URL de {destinationName} configurada.",
                $"Abrir {destinationName}",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        if (urls.Count > 5)
        {
            DialogResult confirmation = MessageBox.Show(
                this,
                $"Serão abertas {urls.Count} guias no navegador. Deseja continuar?",
                $"Abrir {destinationName}",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (confirmation != DialogResult.Yes)
            {
                return;
            }
        }

        foreach (string url in urls)
        {
            BrowserService.Open(url);
        }
    }

    private void SetBusy(bool busy, string status)
    {
        UseWaitCursor = busy;
        refreshButton.Enabled = !busy;
        manageButton.Enabled = !busy;
        statusLabel.Text = status;
    }
}
