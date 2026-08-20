using System.Text.Json.Serialization;

namespace LucasVerissimo.XrmToolBox.ToolManager.Models;

internal sealed class ToolManifest
{
    [JsonIgnore]
    public string ManifestPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string ToolDirectory { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string PackageId { get; set; } = string.Empty;

    public string ProjectPath { get; set; } = string.Empty;

    public string NuspecPath { get; set; } = string.Empty;

    public string AssemblyInfoPath { get; set; } = string.Empty;

    public string PackageScriptPath { get; set; } = string.Empty;

    public string GitUrl { get; set; } = string.Empty;

    public string NuGetUrl { get; set; } = string.Empty;

    public string TargetFramework { get; set; } = string.Empty;

    public string[] ValidationScripts { get; set; } = Array.Empty<string>();

    public string ResolveFromToolDirectory(string path)
    {
        return Path.GetFullPath(Path.Combine(ToolDirectory, path));
    }
}

internal sealed class ToolSummary
{
    public required ToolManifest Manifest { get; init; }

    public string Name => Manifest.Name;

    public string PackageId => Manifest.PackageId;

    public string LocalVersion { get; init; } = string.Empty;

    public string PublishedVersion { get; init; } = string.Empty;

    public string TargetFramework => Manifest.TargetFramework;

    public string Status { get; init; } = string.Empty;
}

internal enum VersionIncrement
{
    Patch,
    Minor,
    Major,
    Custom,
}
