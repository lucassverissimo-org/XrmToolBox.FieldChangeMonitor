namespace LucasVerissimo.XrmToolBox.ToolManager;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        string repositoryRoot = RepositoryLocator.FindRepositoryRoot(AppContext.BaseDirectory);
        ToolCatalogService catalogService = new(repositoryRoot, new NuGetService());
        Application.Run(new MainForm(repositoryRoot, catalogService));
    }
}
