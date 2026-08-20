using System.Text.Json;
using System.Text.RegularExpressions;
using LucasVerissimo.XrmToolBox.ToolManager.Models;

namespace LucasVerissimo.XrmToolBox.ToolManager;

internal sealed class ToolCatalogService
{
    private static readonly Regex AssemblyVersionPattern = new(
        "AssemblyVersion\\(\"(?<version>[^\"]+)\"\\)",
        RegexOptions.Compiled
    );

    private readonly string repositoryRoot;
    private readonly NuGetService nuGetService;

    public ToolCatalogService(string repositoryRoot, NuGetService nuGetService)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repositoryRoot);
        this.repositoryRoot = Path.GetFullPath(repositoryRoot);
        this.nuGetService = nuGetService ?? throw new ArgumentNullException(nameof(nuGetService));
    }

    public async Task<IReadOnlyList<ToolSummary>> LoadAsync(
        CancellationToken cancellationToken = default
    )
    {
        string[] manifestPaths = Directory.GetFiles(
            repositoryRoot,
            "tool-release.json",
            SearchOption.AllDirectories
        );

        List<Task<ToolSummary>> loadTasks = manifestPaths
            .Select(path => LoadToolAsync(path, cancellationToken))
            .ToList();

        ToolSummary[] tools = await Task.WhenAll(loadTasks);
        return tools.OrderBy(tool => tool.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    private async Task<ToolSummary> LoadToolAsync(
        string manifestPath,
        CancellationToken cancellationToken
    )
    {
        string json = await File.ReadAllTextAsync(manifestPath, cancellationToken);
        ToolManifest manifest =
            JsonSerializer.Deserialize<ToolManifest>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? throw new InvalidDataException($"Manifesto inválido: {manifestPath}");

        manifest.ManifestPath = manifestPath;
        manifest.ToolDirectory = Path.GetDirectoryName(manifestPath)!;
        ValidateManifest(manifest);

        string localVersion = ReadAssemblyVersion(manifest);
        string? publishedVersion = await nuGetService.GetLatestVersionAsync(
            manifest.PackageId,
            cancellationToken
        );

        return new ToolSummary
        {
            Manifest = manifest,
            LocalVersion = localVersion,
            PublishedVersion = publishedVersion ?? "Indisponível",
            Status = GetStatus(localVersion, publishedVersion),
        };
    }

    private static void ValidateManifest(ToolManifest manifest)
    {
        if (string.IsNullOrWhiteSpace(manifest.Name))
        {
            throw new InvalidDataException(
                $"O manifesto '{manifest.ManifestPath}' não possui nome."
            );
        }

        if (string.IsNullOrWhiteSpace(manifest.PackageId))
        {
            throw new InvalidDataException(
                $"O manifesto '{manifest.ManifestPath}' não possui Package ID."
            );
        }

        string projectPath = manifest.ResolveFromToolDirectory(manifest.ProjectPath);
        string nuspecPath = manifest.ResolveFromToolDirectory(manifest.NuspecPath);
        string assemblyInfoPath = manifest.ResolveFromToolDirectory(manifest.AssemblyInfoPath);

        if (!File.Exists(projectPath) || !File.Exists(nuspecPath) || !File.Exists(assemblyInfoPath))
        {
            throw new InvalidDataException(
                $"O manifesto '{manifest.ManifestPath}' referencia arquivos que não existem."
            );
        }
    }

    private static string ReadAssemblyVersion(ToolManifest manifest)
    {
        string path = manifest.ResolveFromToolDirectory(manifest.AssemblyInfoPath);
        string content = File.ReadAllText(path);
        Match match = AssemblyVersionPattern.Match(content);
        return match.Success ? match.Groups["version"].Value : "Não encontrada";
    }

    private static string GetStatus(string localVersion, string? publishedVersion)
    {
        if (publishedVersion is null)
        {
            return "NuGet indisponível";
        }

        if (!Version.TryParse(localVersion, out Version? local))
        {
            return "Versão local inválida";
        }

        if (!Version.TryParse(publishedVersion, out Version? published))
        {
            return "Versão publicada inválida";
        }

        int comparison = local.CompareTo(published);
        if (comparison > 0)
        {
            return "Nova versão local";
        }

        if (comparison < 0)
        {
            return "Projeto desatualizado";
        }

        return "Atualizada";
    }
}
