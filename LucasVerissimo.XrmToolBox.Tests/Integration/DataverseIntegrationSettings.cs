using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace LucasVerissimo.XrmToolBox.Tests.Integration
{
    internal sealed class DataverseIntegrationSettings
    {
        private const string SettingsFileName = "local.settings.json";
        private const string SettingsPathEnvironmentVariable = "XRMTOOLBOX_TEST_SETTINGS";

        public string SourceConnectionString { get; set; }

        public string TargetConnectionString { get; set; }

        public string SolutionUniqueName { get; set; }

        public static bool TryLoad(out DataverseIntegrationSettings settings, out string reason)
        {
            var settingsPath = FindSettingsPath();
            if (settingsPath == null)
            {
                settings = null;
                reason =
                    "Integration settings were not found. Copy local.settings.example.json to local.settings.json.";
                return false;
            }

            try
            {
                var serializer = new JavaScriptSerializer();
                var root = serializer.Deserialize<IntegrationSettingsRoot>(
                    File.ReadAllText(settingsPath)
                );
                settings = root == null ? null : root.Dataverse;
            }
            catch (Exception exception)
            {
                settings = null;
                reason =
                    "The integration settings file is invalid: " + exception.GetType().Name + ".";
                return false;
            }

            var missingNames = GetMissingSettingNames(settings);
            if (missingNames.Count > 0)
            {
                settings = null;
                reason =
                    "Complete these integration settings: " + string.Join(", ", missingNames) + ".";
                return false;
            }

            reason = null;
            return true;
        }

        private static string FindSettingsPath()
        {
            var configuredPath = Environment.GetEnvironmentVariable(
                SettingsPathEnvironmentVariable
            );
            if (!string.IsNullOrWhiteSpace(configuredPath))
            {
                var fullConfiguredPath = Path.GetFullPath(configuredPath);
                return File.Exists(fullConfiguredPath) ? fullConfiguredPath : null;
            }

            foreach (var startDirectory in GetSearchStartDirectories())
            {
                var currentDirectory = new DirectoryInfo(startDirectory);
                while (currentDirectory != null)
                {
                    var directCandidate = Path.Combine(currentDirectory.FullName, SettingsFileName);
                    if (File.Exists(directCandidate))
                    {
                        return directCandidate;
                    }

                    var repositoryCandidate = Path.Combine(
                        currentDirectory.FullName,
                        "LucasVerissimo.XrmToolBox.Tests",
                        SettingsFileName
                    );
                    if (File.Exists(repositoryCandidate))
                    {
                        return repositoryCandidate;
                    }

                    currentDirectory = currentDirectory.Parent;
                }
            }

            return null;
        }

        private static IEnumerable<string> GetSearchStartDirectories()
        {
            yield return AppDomain.CurrentDomain.BaseDirectory;
            yield return Environment.CurrentDirectory;
        }

        private static List<string> GetMissingSettingNames(DataverseIntegrationSettings settings)
        {
            var missingNames = new List<string>();
            if (settings == null || string.IsNullOrWhiteSpace(settings.SourceConnectionString))
            {
                missingNames.Add("Dataverse.SourceConnectionString");
            }

            if (settings == null || string.IsNullOrWhiteSpace(settings.TargetConnectionString))
            {
                missingNames.Add("Dataverse.TargetConnectionString");
            }

            if (settings == null || string.IsNullOrWhiteSpace(settings.SolutionUniqueName))
            {
                missingNames.Add("Dataverse.SolutionUniqueName");
            }

            return missingNames;
        }

        private sealed class IntegrationSettingsRoot
        {
            public DataverseIntegrationSettings Dataverse { get; set; }
        }
    }
}
