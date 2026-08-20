using System.Diagnostics;

namespace LucasVerissimo.XrmToolBox.ToolManager;

internal static class BrowserService
{
    public static void Open(string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
    }
}
