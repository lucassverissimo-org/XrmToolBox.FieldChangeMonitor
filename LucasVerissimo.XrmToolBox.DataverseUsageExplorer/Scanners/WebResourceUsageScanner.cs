using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Parsers;
using LucasVerissimo.XrmToolBox.Shared.BusinessLogic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Scanners
{
    internal sealed class WebResourceUsageScanner : IUsageScanner
    {
        private const int HtmlWebResourceType = 1;
        private const int JavaScriptWebResourceType = 3;

        public string Name
        {
            get { return "Web Resource"; }
        }

        public IReadOnlyCollection<UsageReference> Scan(
            UsageSearchContext context,
            CancellationToken token,
            Action<string, int, int> progress
        )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var queryService = new DataverseQueryService(context.Service);
            var webResources = queryService.RetrieveAll(CreateQuery(), token);
            var formLinks = GetFormLinks(context, queryService, token);
            var references = new List<UsageReference>();
            var searchIdentifier = GetSearchIdentifier(context);

            var current = 0;
            foreach (var webResource in webResources)
            {
                token.ThrowIfCancellationRequested();
                progress(Name, ++current, webResources.Count);

                var technicalName = webResource.GetAttributeValue<string>("name");
                if (
                    string.IsNullOrWhiteSpace(technicalName)
                    || !formLinks.TryGetValue(technicalName, out var linkedForms)
                    || linkedForms.Count == 0
                )
                {
                    continue;
                }

                var content = WebResourceContentParser.Decode(
                    webResource.GetAttributeValue<string>("content")
                );
                var snippet = WebResourceContentParser.FindIdentifier(content, searchIdentifier);

                if (snippet != null)
                {
                    references.Add(CreateReference(webResource, context, snippet, linkedForms));
                }
            }

            return references;
        }

        private static IReadOnlyDictionary<string, IReadOnlyCollection<FormLink>> GetFormLinks(
            UsageSearchContext context,
            DataverseQueryService queryService,
            CancellationToken token
        )
        {
            var forms = queryService
                .RetrieveAll(CreateFormQuery(context), token)
                .Where(form => MatchesTable(form, context));
            var links = new Dictionary<string, List<FormLink>>(StringComparer.OrdinalIgnoreCase);

            foreach (var form in forms)
            {
                token.ThrowIfCancellationRequested();
                var resourceNames = WebResourceContentParser.ExtractFormWebResourceNames(
                    form.GetAttributeValue<string>("formxml")
                );
                foreach (var resourceName in resourceNames)
                {
                    if (!links.TryGetValue(resourceName, out var linkedForms))
                    {
                        linkedForms = new List<FormLink>();
                        links.Add(resourceName, linkedForms);
                    }

                    linkedForms.Add(new FormLink { Id = form.Id, Name = GetFormName(form) });
                }
            }

            return links.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyCollection<FormLink>)pair.Value,
                StringComparer.OrdinalIgnoreCase
            );
        }

        private static QueryExpression CreateFormQuery(UsageSearchContext context)
        {
            var query = new QueryExpression("systemform")
            {
                ColumnSet = new ColumnSet("name", "formxml", "objecttypecode"),
            };
            query.Criteria.AddCondition(
                "objecttypecode",
                ConditionOperator.Equal,
                context.TableLogicalName
            );
            return query;
        }

        private static bool MatchesTable(Entity form, UsageSearchContext context)
        {
            var objectTypeCode = form.Attributes.Contains("objecttypecode")
                ? form["objecttypecode"]
                : null;

            if (
                string.Equals(
                    Convert.ToString(objectTypeCode),
                    context.TableLogicalName,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return true;
            }

            return context.TableObjectTypeCode.HasValue
                && int.TryParse(Convert.ToString(objectTypeCode), out var numericObjectTypeCode)
                && numericObjectTypeCode == context.TableObjectTypeCode.Value;
        }

        private static string GetFormName(Entity form)
        {
            var name = form.GetAttributeValue<string>("name");
            return string.IsNullOrWhiteSpace(name) ? "Unnamed form" : name;
        }

        private static QueryExpression CreateQuery()
        {
            var query = new QueryExpression("webresource")
            {
                ColumnSet = new ColumnSet(
                    "name",
                    "displayname",
                    "content",
                    "webresourcetype",
                    "modifiedon",
                    "ismanaged"
                ),
            };
            query.Criteria.AddCondition(
                "webresourcetype",
                ConditionOperator.In,
                HtmlWebResourceType,
                JavaScriptWebResourceType
            );
            query.Criteria.AddCondition("content", ConditionOperator.NotNull);
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));

            return query;
        }

        private static string GetSearchIdentifier(UsageSearchContext context)
        {
            return context.SearchType == UsageSearchType.Column
                ? context.ColumnLogicalName
                : context.TableLogicalName;
        }

        private static UsageReference CreateReference(
            Entity webResource,
            UsageSearchContext context,
            string snippet,
            IReadOnlyCollection<FormLink> linkedForms
        )
        {
            var type = webResource.GetAttributeValue<OptionSetValue>("webresourcetype");
            var technicalName = webResource.GetAttributeValue<string>("name");
            var displayName = webResource.GetAttributeValue<string>("displayname");
            var isManaged = webResource.GetAttributeValue<bool?>("ismanaged");

            return new UsageReference
            {
                ComponentType = "Web Resource",
                ComponentEntityName = "webresource",
                ComponentId = webResource.Id,
                Name = string.IsNullOrWhiteSpace(displayName) ? technicalName : displayName,
                TableLogicalName = context.TableLogicalName,
                ColumnLogicalName = context.ColumnLogicalName,
                Status = isManaged == true ? "Managed" : "Unmanaged",
                ReferenceType = GetReferenceType(type),
                FoundIn = CreateFormSource(linkedForms),
                RawReference = snippet,
                ModifiedOn = webResource.GetAttributeValue<DateTime?>("modifiedon"),
                IsManaged = isManaged,
                Confidence = ReferenceConfidence.TextMatch,
                Details = CreateDetails(technicalName, context.TableLogicalName, linkedForms),
            };
        }

        private static string GetReferenceType(OptionSetValue type)
        {
            var resourceType =
                type != null && type.Value == JavaScriptWebResourceType
                    ? "JavaScript Reference"
                    : "HTML Reference";
            return "Form-linked " + resourceType;
        }

        private static string CreateFormSource(IEnumerable<FormLink> linkedForms)
        {
            return "Form: "
                + string.Join(
                    ", ",
                    linkedForms.Select(form => form.Name).Distinct().OrderBy(name => name)
                );
        }

        private static string CreateDetails(
            string technicalName,
            string tableLogicalName,
            IReadOnlyCollection<FormLink> linkedForms
        )
        {
            var details = "Web resource name: " + technicalName;
            var forms = linkedForms
                .GroupBy(form => form.Id)
                .Select(group => group.First())
                .OrderBy(form => form.Name)
                .Select(form => form.Name + " (" + form.Id.ToString("D") + ")");
            return details
                + Environment.NewLine
                + "Linked table: "
                + tableLogicalName
                + Environment.NewLine
                + "Linked forms: "
                + string.Join(", ", forms);
        }

        private sealed class FormLink
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }
}
