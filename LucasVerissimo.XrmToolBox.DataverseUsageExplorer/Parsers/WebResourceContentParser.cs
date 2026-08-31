using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Parsers
{
    internal static class WebResourceContentParser
    {
        private const int SnippetContextLength = 100;

        public static string Decode(string encodedContent)
        {
            if (string.IsNullOrWhiteSpace(encodedContent))
            {
                return string.Empty;
            }

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(encodedContent));
            }
            catch (FormatException)
            {
                return encodedContent;
            }
        }

        public static string FindIdentifier(string content, string identifier)
        {
            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(identifier))
            {
                return null;
            }

            var pattern = "(?<![A-Za-z0-9_])" + Regex.Escape(identifier) + "(?![A-Za-z0-9_])";
            var match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return null;
            }

            return CreateSnippet(content, match.Index, match.Length);
        }

        public static IReadOnlyCollection<string> ExtractFormWebResourceNames(string formXml)
        {
            if (string.IsNullOrWhiteSpace(formXml))
            {
                return Array.Empty<string>();
            }

            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            ExtractXmlReferences(formXml, names);
            ExtractFileNameReferences(formXml, names);
            return names.OrderBy(name => name).ToList();
        }

        private static void ExtractXmlReferences(string formXml, ISet<string> names)
        {
            try
            {
                var document = XDocument.Parse(formXml);
                foreach (var element in document.Descendants())
                {
                    if (IsLibraryElement(element))
                    {
                        AddName(names, GetAttributeValue(element, "name"));
                    }

                    AddName(names, GetAttributeValue(element, "libraryName"));

                    if (IsWebResourceValueElement(element))
                    {
                        AddName(names, element.Value);
                    }
                }
            }
            catch (XmlException)
            {
                // Malformed form XML is still inspected by the file-name fallback below.
            }
        }

        private static void ExtractFileNameReferences(string formXml, ISet<string> names)
        {
            const string pattern =
                @"[A-Za-z0-9_.-]+_/[^""'<>\s]+\.(?:js|html?|css)(?:\?[^""'<>\s]*)?";

            foreach (Match match in Regex.Matches(formXml, pattern, RegexOptions.IgnoreCase))
            {
                AddName(names, match.Value);
            }
        }

        private static bool IsLibraryElement(XElement element)
        {
            return string.Equals(
                element.Name.LocalName,
                "Library",
                StringComparison.OrdinalIgnoreCase
            );
        }

        private static bool IsWebResourceValueElement(XElement element)
        {
            var localName = element.Name.LocalName;
            return string.Equals(localName, "Url", StringComparison.OrdinalIgnoreCase)
                || string.Equals(localName, "WebResource", StringComparison.OrdinalIgnoreCase)
                || string.Equals(localName, "WebResourceName", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetAttributeValue(XElement element, string attributeName)
        {
            return element
                .Attributes()
                .FirstOrDefault(attribute =>
                    string.Equals(
                        attribute.Name.LocalName,
                        attributeName,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                ?.Value;
        }

        private static void AddName(ISet<string> names, string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return;
            }

            var normalized = candidate.Trim();
            const string webResourcePrefix = "$webresource:";
            if (normalized.StartsWith(webResourcePrefix, StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(webResourcePrefix.Length);
            }

            var queryIndex = normalized.IndexOf('?');
            if (queryIndex >= 0)
            {
                normalized = normalized.Substring(0, queryIndex);
            }

            if (
                normalized.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
                || normalized.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                || normalized.EndsWith(".htm", StringComparison.OrdinalIgnoreCase)
                || normalized.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
            )
            {
                names.Add(normalized);
            }
        }

        private static string CreateSnippet(string content, int matchIndex, int matchLength)
        {
            var start = Math.Max(0, matchIndex - SnippetContextLength);
            var end = Math.Min(content.Length, matchIndex + matchLength + SnippetContextLength);
            var snippet = content.Substring(start, end - start);

            return Regex.Replace(snippet, "\\s+", " ").Trim();
        }
    }
}
