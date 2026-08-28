using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Services
{
    internal sealed class CsvExportContext
    {
        public string SourceEnvironment { get; set; }

        public string TargetEnvironment { get; set; }

        public SolutionInfo SourceSolution { get; set; }

        public SolutionInfo TargetSolution { get; set; }
    }

    internal sealed class CsvExportService
    {
        public void Export(
            string filePath,
            CsvExportContext context,
            IEnumerable<LayerAnalysisResult> results
        )
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("The CSV file path is required.", nameof(filePath));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            var columns = new[]
            {
                "SourceEnvironment",
                "TargetEnvironment",
                "SolutionUniqueName",
                "SolutionVersionSource",
                "SolutionVersionTarget",
                "ComponentType",
                "ComponentTypeName",
                "ComponentId",
                "ComponentName",
                "ExistsInSourceSolution",
                "ExistsInTargetSolution",
                "ExistsInTargetEnvironment",
                "CorrelationStatus",
                "HasActiveLayer",
                "LayerCount",
                "Status",
                "Error",
                "DurationMs",
            };

            using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            {
                writer.WriteLine(string.Join(",", columns));
                foreach (var result in results)
                {
                    var values = new[]
                    {
                        context.SourceEnvironment,
                        context.TargetEnvironment,
                        context.SourceSolution?.UniqueName,
                        context.SourceSolution?.Version,
                        context.TargetSolution?.Version,
                        result.ComponentType.ToString(),
                        result.ComponentTypeName,
                        result.ComponentId.ToString("D"),
                        result.ComponentName,
                        result.ExistsInSourceSolution.ToString(),
                        result.ExistsInTargetSolution.ToString(),
                        result.ExistsInTargetEnvironment.ToString(),
                        result.CorrelationStatus.ToString(),
                        result.HasActiveLayer.HasValue
                            ? result.HasActiveLayer.Value.ToString()
                            : string.Empty,
                        result.LayerCount.ToString(),
                        result.Status,
                        result.Error,
                        result.DurationMs.ToString(),
                    };
                    writer.WriteLine(string.Join(",", values.Select(Escape)));
                }
            }
        }

        private static string Escape(string value)
        {
            var text = value ?? string.Empty;
            if (text.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0)
            {
                return text;
            }

            return "\"" + text.Replace("\"", "\"\"") + "\"";
        }
    }
}
