using LucasVerissimo.XrmToolBox.ToolManager.Models;

namespace LucasVerissimo.XrmToolBox.ToolManager;

internal static class VersionService
{
    public static string Suggest(string currentVersion, VersionIncrement increment)
    {
        if (!Version.TryParse(currentVersion, out Version? version))
        {
            throw new ArgumentException("A versão atual é inválida.", nameof(currentVersion));
        }

        return increment switch
        {
            VersionIncrement.Major => $"{version.Major + 1}.0.0.0",
            VersionIncrement.Minor => $"{version.Major}.{version.Minor + 1}.0.0",
            VersionIncrement.Patch =>
                $"{version.Major}.{version.Minor}.{Math.Max(version.Build, 0) + 1}.0",
            _ => Normalize(version),
        };
    }

    public static bool IsFourPartVersion(string value)
    {
        return Version.TryParse(value, out Version? version) && version.Revision >= 0;
    }

    private static string Normalize(Version version)
    {
        return $"{version.Major}.{version.Minor}.{Math.Max(version.Build, 0)}.{Math.Max(version.Revision, 0)}";
    }
}
