namespace LucasVerissimo.XrmToolBox.ToolManager;

internal static class RepositoryLocator
{
    public static string FindRepositoryRoot(string startingDirectory)
    {
        DirectoryInfo? directory = new(startingDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "LucasVerissimo.XrmToolBox.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Não foi possível localizar a raiz do repositório a partir da pasta da aplicação."
        );
    }
}
