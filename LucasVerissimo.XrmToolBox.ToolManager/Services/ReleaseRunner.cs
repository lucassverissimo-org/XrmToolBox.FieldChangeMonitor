using System.Diagnostics;
using LucasVerissimo.XrmToolBox.ToolManager.Models;

namespace LucasVerissimo.XrmToolBox.ToolManager;

internal sealed class ReleaseRunner
{
    private readonly string repositoryRoot;

    public ReleaseRunner(string repositoryRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repositoryRoot);
        this.repositoryRoot = repositoryRoot;
    }

    public async Task<ReleaseResult> RunAsync(
        ToolManifest manifest,
        string version,
        string releaseNotes,
        bool publish,
        string? apiKey,
        Action<string> writeLog,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentNullException.ThrowIfNull(writeLog);

        string scriptPath = Path.Combine(repositoryRoot, "scripts", "publish-tool.ps1");
        ProcessStartInfo startInfo = new()
        {
            FileName = "powershell.exe",
            WorkingDirectory = repositoryRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);
        startInfo.ArgumentList.Add("-ManifestPath");
        startInfo.ArgumentList.Add(manifest.ManifestPath);
        startInfo.ArgumentList.Add("-Version");
        startInfo.ArgumentList.Add(version);
        startInfo.ArgumentList.Add("-ReleaseNotes");
        startInfo.ArgumentList.Add(releaseNotes);

        if (publish)
        {
            startInfo.ArgumentList.Add("-Publish");
            startInfo.Environment["NUGET_API_KEY"] = apiKey ?? string.Empty;
        }

        using Process process = new() { StartInfo = startInfo };
        List<string> output = new();

        process.OutputDataReceived += (_, eventArgs) => AddOutput(eventArgs.Data, output, writeLog);
        process.ErrorDataReceived += (_, eventArgs) => AddOutput(eventArgs.Data, output, writeLog);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);
        string combinedOutput = string.Join(Environment.NewLine, output);

        return new ReleaseResult(
            process.ExitCode == 0,
            IsAuthenticationFailure(combinedOutput),
            combinedOutput
        );
    }

    private static void AddOutput(string? line, List<string> output, Action<string> writeLog)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        lock (output)
        {
            output.Add(line);
        }

        writeLog(line);
    }

    private static bool IsAuthenticationFailure(string output)
    {
        string[] indicators =
        {
            "401",
            "403",
            "api key",
            "unauthorized",
            "forbidden",
            "authentication",
        };

        return indicators.Any(indicator =>
            output.Contains(indicator, StringComparison.OrdinalIgnoreCase)
        );
    }
}

internal sealed record ReleaseResult(bool Succeeded, bool AuthenticationFailed, string Output);
