using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Parsers
{
    internal sealed class LocatedReference
    {
        public string ReferenceType { get; set; }
        public string FoundIn { get; set; }
        public string Snippet { get; set; }
    }

    internal static class TextReferenceParser
    {
        public static LocatedReference Find(
            string value,
            string token,
            string source,
            string referenceType
        )
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(token))
                return null;
            var match = Regex.Match(
                value,
                "(?<![A-Za-z0-9_])" + Regex.Escape(token) + "(?![A-Za-z0-9_])",
                RegexOptions.IgnoreCase
            );
            if (!match.Success)
                return null;
            var start = Math.Max(0, match.Index - 180);
            var length = Math.Min(value.Length - start, match.Length + 360);
            return new LocatedReference
            {
                ReferenceType = referenceType,
                FoundIn = source,
                Snippet = value.Substring(start, length),
            };
        }
    }

    internal static class FilteringAttributesParser
    {
        public static bool Contains(string list, string attribute)
        {
            return !string.IsNullOrWhiteSpace(list)
                && list.Split(',')
                    .Select(x => x.Trim())
                    .Any(x => string.Equals(x, attribute, StringComparison.OrdinalIgnoreCase));
        }
    }

    internal static class PowerAutomateParser
    {
        private static readonly string[] TableKeys =
        {
            "subscriptionRequest/entityname",
            "subscriptionRequest/entityName",
            "entityname",
            "entityName",
            "tablename",
            "tableName",
        };

        public static IReadOnlyCollection<LocatedReference> Find(
            string json,
            string table,
            string tableEntitySetName,
            string column,
            bool columnSearch
        )
        {
            var found = new List<LocatedReference>();
            if (string.IsNullOrWhiteSpace(json))
                return found;
            try
            {
                var serializer = new JavaScriptSerializer
                {
                    MaxJsonLength = int.MaxValue,
                    RecursionLimit = 256,
                };

                var definition = serializer.DeserializeObject(json);
                if (columnSearch)
                {
                    found.AddRange(
                        FindColumnReferencesForTable(
                            definition,
                            string.Empty,
                            table,
                            tableEntitySetName,
                            column
                        )
                    );
                }
                else
                {
                    foreach (var property in EnumerateProperties(definition, string.Empty))
                    {
                        if (
                            IsTablePropertyPath(property.Path)
                            && MatchesTable(property.Value, table, tableEntitySetName)
                        )
                        {
                            found.Add(Make(property, ClassifyTableReference(property.Path)));
                        }
                    }
                }
            }
            catch
            {
                if (columnSearch)
                {
                    return found;
                }

                var hit = TextReferenceParser.Find(json, table, "clientdata", "Table Reference");
                if (hit == null && !string.IsNullOrWhiteSpace(tableEntitySetName))
                {
                    hit = TextReferenceParser.Find(
                        json,
                        tableEntitySetName,
                        "clientdata",
                        "Table Reference"
                    );
                }
                if (hit != null)
                    found.Add(hit);
            }
            return found
                .GroupBy(x => x.ReferenceType + "|" + x.Snippet)
                .Select(x => x.First())
                .ToList();
        }

        private static IEnumerable<LocatedReference> FindColumnReferencesForTable(
            object value,
            string path,
            string tableLogicalName,
            string tableEntitySetName,
            string columnLogicalName
        )
        {
            var dictionary = value as IDictionary<string, object>;
            if (dictionary != null)
            {
                var containsSelectedTable = dictionary.Any(pair =>
                    IsTablePropertyName(pair.Key)
                    && MatchesTable(
                        ConvertJsonValue(pair.Value),
                        tableLogicalName,
                        tableEntitySetName
                    )
                );

                if (containsSelectedTable)
                {
                    foreach (var property in EnumerateProperties(dictionary, path))
                    {
                        if (ContainsColumnReference(property, columnLogicalName))
                        {
                            yield return Make(property, Classify(property.Path));
                        }
                    }
                }

                foreach (var pair in dictionary)
                {
                    var childPath = string.IsNullOrWhiteSpace(path)
                        ? pair.Key
                        : path + "." + pair.Key;

                    foreach (
                        var reference in FindColumnReferencesForTable(
                            pair.Value,
                            childPath,
                            tableLogicalName,
                            tableEntitySetName,
                            columnLogicalName
                        )
                    )
                    {
                        yield return reference;
                    }
                }

                yield break;
            }

            var array = value as object[];
            if (array == null)
            {
                yield break;
            }

            for (var index = 0; index < array.Length; index++)
            {
                foreach (
                    var reference in FindColumnReferencesForTable(
                        array[index],
                        path + "[" + index + "]",
                        tableLogicalName,
                        tableEntitySetName,
                        columnLogicalName
                    )
                )
                {
                    yield return reference;
                }
            }
        }

        private static IEnumerable<JsonProperty> EnumerateProperties(object value, string path)
        {
            var dictionary = value as IDictionary<string, object>;
            if (dictionary != null)
            {
                foreach (var pair in dictionary)
                {
                    var childPath = string.IsNullOrWhiteSpace(path)
                        ? pair.Key
                        : path + "." + pair.Key;
                    foreach (var nested in EnumerateProperties(pair.Value, childPath))
                        yield return nested;
                }
                yield break;
            }
            var array = value as object[];
            if (array != null)
            {
                for (var i = 0; i < array.Length; i++)
                    foreach (var nested in EnumerateProperties(array[i], path + "[" + i + "]"))
                        yield return nested;
                yield break;
            }
            yield return new JsonProperty
            {
                Path = path,
                Value =
                    value == null
                        ? string.Empty
                        : Convert.ToString(
                            value,
                            System.Globalization.CultureInfo.InvariantCulture
                        ),
            };
        }

        private static LocatedReference Make(JsonProperty p, string type)
        {
            return new LocatedReference
            {
                ReferenceType = type,
                FoundIn = "clientdata: " + p.Path,
                Snippet = p.Path + ": " + p.Value,
            };
        }

        private static string Classify(string path)
        {
            var p = path.ToLowerInvariant();
            if (p.Contains("triggercondition"))
                return "Trigger Condition";
            if (p.Contains("filter"))
                return "Filter Rows";
            if (p.Contains("select"))
                return "Select Columns";
            if (p.Contains("trigger"))
                return "Trigger";
            if (p.Contains("update"))
                return "Update Row";
            if (p.Contains("create"))
                return "Create Row";
            if (p.Contains("list") || p.Contains("getitems"))
                return "List Rows";
            if (p.Contains("condition"))
                return "Condition";
            return "Field Reference";
        }

        private static bool MatchesTable(string value, string logicalName, string entitySetName)
        {
            return string.Equals(value, logicalName, StringComparison.OrdinalIgnoreCase)
                || (
                    !string.IsNullOrWhiteSpace(entitySetName)
                    && string.Equals(value, entitySetName, StringComparison.OrdinalIgnoreCase)
                );
        }

        private static bool IsTablePropertyName(string propertyName)
        {
            return TableKeys.Any(tableKey =>
                string.Equals(propertyName, tableKey, StringComparison.OrdinalIgnoreCase)
            );
        }

        private static bool IsTablePropertyPath(string propertyPath)
        {
            return TableKeys.Any(tableKey =>
                propertyPath.EndsWith(tableKey, StringComparison.OrdinalIgnoreCase)
            );
        }

        private static bool ContainsColumnReference(JsonProperty property, string columnLogicalName)
        {
            return TextReferenceParser.Find(
                    property.Value,
                    columnLogicalName,
                    "clientdata",
                    "Field Reference"
                ) != null
                || TextReferenceParser.Find(
                    property.Path,
                    columnLogicalName,
                    "clientdata",
                    "Field Reference"
                ) != null;
        }

        private static string ConvertJsonValue(object value)
        {
            return value == null
                ? string.Empty
                : Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string ClassifyTableReference(string path)
        {
            var isTriggerReference =
                path.IndexOf("trigger", StringComparison.OrdinalIgnoreCase) >= 0
                || path.IndexOf("subscriptionRequest", StringComparison.OrdinalIgnoreCase) >= 0;

            return isTriggerReference ? "Dataverse Trigger Table" : "Dataverse Action Table";
        }

        private sealed class JsonProperty
        {
            public string Path { get; set; }
            public string Value { get; set; }
        }
    }

    internal static class XmlReferenceParser
    {
        public static IReadOnlyCollection<LocatedReference> FindViewReferences(
            string xml,
            string column,
            string source
        )
        {
            var result = new List<LocatedReference>();
            if (string.IsNullOrWhiteSpace(xml))
                return result;
            try
            {
                var doc = XDocument.Parse(xml);
                foreach (
                    var e in doc.Descendants()
                        .Where(e =>
                            e.Attributes()
                                .Any(a =>
                                    string.Equals(
                                        a.Value,
                                        column,
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                )
                        )
                )
                {
                    var type =
                        e.Name.LocalName == "attribute" || e.Name.LocalName == "cell"
                            ? "Displayed Column"
                        : e.Name.LocalName == "condition" ? "Filter"
                        : e.Name.LocalName == "order" ? "Sort"
                        : e.Ancestors().Any(a => a.Name.LocalName == "link-entity") ? "Relationship"
                        : "XML Reference";
                    result.Add(
                        new LocatedReference
                        {
                            ReferenceType = type,
                            FoundIn = source,
                            Snippet = e.ToString(SaveOptions.DisableFormatting),
                        }
                    );
                }
            }
            catch
            {
                var hit = TextReferenceParser.Find(xml, column, source, "XML Reference");
                if (hit != null)
                    result.Add(hit);
            }
            return result;
        }
    }
}
