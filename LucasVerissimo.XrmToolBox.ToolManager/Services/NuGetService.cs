using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace LucasVerissimo.XrmToolBox.ToolManager;

internal sealed class NuGetService
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(15) };

    public async Task<string?> GetLatestVersionAsync(
        string packageId,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);

        string normalizedId = packageId.ToLowerInvariant();
        string url = $"https://api.nuget.org/v3-flatcontainer/{normalizedId}/index.json";

        try
        {
            VersionIndex? index = await HttpClient.GetFromJsonAsync<VersionIndex>(
                url,
                cancellationToken
            );
            return index?.Versions.LastOrDefault();
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
    }

    private sealed class VersionIndex
    {
        [JsonPropertyName("versions")]
        public string[] Versions { get; set; } = Array.Empty<string>();
    }
}
