using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Parsers;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Services;
using LucasVerissimo.XrmToolBox.Shared.BusinessLogic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Scanners
{
    internal interface IUsageScanner
    {
        string Name { get; }
        IReadOnlyCollection<UsageReference> Scan(
            UsageSearchContext context,
            CancellationToken token,
            Action<string, int, int> progress
        );
    }

    internal sealed class WorkflowUsageScanner : IUsageScanner
    {
        private readonly WorkflowRepository repository;
        private readonly int category;
        private readonly string componentType;

        public WorkflowUsageScanner(
            WorkflowRepository repository,
            int category,
            string componentType
        )
        {
            this.repository = repository;
            this.category = category;
            this.componentType = componentType;
        }

        public string Name
        {
            get { return componentType; }
        }

        public IReadOnlyCollection<UsageReference> Scan(
            UsageSearchContext context,
            CancellationToken token,
            Action<string, int, int> progress
        )
        {
            var rows = new List<UsageReference>();
            var workflows = repository
                .GetAll(token)
                .Where(x =>
                    x.GetAttributeValue<OptionSetValue>("category") != null
                    && x.GetAttributeValue<OptionSetValue>("category").Value == category
                )
                .ToList();
            var current = 0;
            foreach (var workflow in workflows)
            {
                token.ThrowIfCancellationRequested();
                progress(Name, ++current, workflows.Count);
                var hits = Find(workflow, context);
                foreach (var hit in hits)
                    rows.Add(Create(workflow, context, hit));
            }
            return rows;
        }

        private IEnumerable<LocatedReference> Find(Entity workflow, UsageSearchContext context)
        {
            var token =
                context.SearchType == UsageSearchType.Column
                    ? context.ColumnLogicalName
                    : context.TableLogicalName;
            var hits = new List<LocatedReference>();
            if (
                context.SearchType == UsageSearchType.Table
                && string.Equals(
                    workflow.GetAttributeValue<string>("primaryentity"),
                    token,
                    StringComparison.OrdinalIgnoreCase
                )
            )
                hits.Add(
                    new LocatedReference
                    {
                        ReferenceType = "Primary Table",
                        FoundIn = "primaryentity",
                        Snippet = token,
                    }
                );
            foreach (
                var pair in new[]
                {
                    new { Name = "triggeronupdateattributelist", Type = "Trigger Attribute" },
                    new { Name = "clientdata", Type = "Configuration Reference" },
                    new { Name = "xaml", Type = "XAML Reference" },
                    new { Name = "description", Type = "Description Text" },
                }
            )
            {
                LocatedReference hit;
                if (
                    pair.Name == "triggeronupdateattributelist"
                    && context.SearchType == UsageSearchType.Column
                )
                    hit = FilteringAttributesParser.Contains(
                        workflow.GetAttributeValue<string>(pair.Name),
                        token
                    )
                        ? new LocatedReference
                        {
                            ReferenceType = pair.Type,
                            FoundIn = pair.Name,
                            Snippet = workflow.GetAttributeValue<string>(pair.Name),
                        }
                        : null;
                else
                    hit = TextReferenceParser.Find(
                        workflow.GetAttributeValue<string>(pair.Name),
                        token,
                        pair.Name,
                        pair.Type
                    );
                if (hit != null)
                    hits.Add(hit);
            }
            return hits.GroupBy(x => x.FoundIn + "|" + x.ReferenceType).Select(x => x.First());
        }

        private UsageReference Create(Entity e, UsageSearchContext c, LocatedReference hit)
        {
            return new UsageReference
            {
                ComponentType = componentType,
                Name =
                    e.GetAttributeValue<string>("name")
                    ?? e.GetAttributeValue<string>("uniquename"),
                ComponentId = e.Id,
                TableLogicalName = c.TableLogicalName,
                ColumnLogicalName = c.ColumnLogicalName,
                Status =
                    e.GetAttributeValue<OptionSetValue>("statecode") != null
                    && e.GetAttributeValue<OptionSetValue>("statecode").Value == 1
                        ? "Active"
                        : "Draft/Inactive",
                ReferenceType = hit.ReferenceType,
                FoundIn = hit.FoundIn,
                RawReference = hit.Snippet,
                ModifiedOn = e.GetAttributeValue<DateTime?>("modifiedon"),
                IsManaged = e.GetAttributeValue<bool?>("ismanaged"),
                Confidence =
                    hit.FoundIn == "primaryentity" || hit.FoundIn == "triggeronupdateattributelist"
                        ? ReferenceConfidence.Confirmed
                        : ReferenceConfidence.TextMatch,
            };
        }
    }

    internal sealed class PowerAutomateUsageScanner : IUsageScanner
    {
        private readonly WorkflowRepository repository;

        public PowerAutomateUsageScanner(WorkflowRepository repository)
        {
            this.repository = repository;
        }

        public string Name
        {
            get { return "Power Automate"; }
        }

        public IReadOnlyCollection<UsageReference> Scan(
            UsageSearchContext context,
            CancellationToken token,
            Action<string, int, int> progress
        )
        {
            var result = new List<UsageReference>();
            var flows = repository
                .GetAll(token)
                .Where(x =>
                    x.GetAttributeValue<OptionSetValue>("category") != null
                    && x.GetAttributeValue<OptionSetValue>("category").Value == 5
                )
                .ToList();
            var current = 0;
            foreach (var e in flows)
            {
                token.ThrowIfCancellationRequested();
                progress(Name, ++current, flows.Count);
                foreach (
                    var hit in PowerAutomateParser.Find(
                        e.GetAttributeValue<string>("clientdata"),
                        context.TableLogicalName,
                        context.ColumnLogicalName,
                        context.SearchType == UsageSearchType.Column
                    )
                )
                    result.Add(
                        new UsageReference
                        {
                            ComponentType = Name,
                            Name = e.GetAttributeValue<string>("name"),
                            ComponentId = e.Id,
                            TableLogicalName = context.TableLogicalName,
                            ColumnLogicalName = context.ColumnLogicalName,
                            Status =
                                e.GetAttributeValue<OptionSetValue>("statecode") != null
                                && e.GetAttributeValue<OptionSetValue>("statecode").Value == 1
                                    ? "Active"
                                    : "Draft/Inactive",
                            ReferenceType = hit.ReferenceType,
                            FoundIn = hit.FoundIn,
                            RawReference = hit.Snippet,
                            ModifiedOn = e.GetAttributeValue<DateTime?>("modifiedon"),
                            IsManaged = e.GetAttributeValue<bool?>("ismanaged"),
                            Confidence =
                                hit.ReferenceType == "Dataverse Trigger Table"
                                    ? ReferenceConfidence.Confirmed
                                    : ReferenceConfidence.TextMatch,
                        }
                    );
            }
            return result;
        }
    }

    internal abstract class EntityScanner : IUsageScanner
    {
        public abstract string Name { get; }
        protected abstract QueryExpression Query(UsageSearchContext context);
        protected abstract IEnumerable<UsageReference> Parse(
            Entity entity,
            UsageSearchContext context
        );

        public virtual IReadOnlyCollection<UsageReference> Scan(
            UsageSearchContext context,
            CancellationToken token,
            Action<string, int, int> progress
        )
        {
            var queryService = new DataverseQueryService(context.Service);
            var entities = queryService.RetrieveAll(Query(context), token);
            var result = new List<UsageReference>();
            var current = 0;

            foreach (var entity in entities)
            {
                token.ThrowIfCancellationRequested();
                progress(Name, ++current, entities.Count);
                result.AddRange(Parse(entity, context));
            }

            return result;
        }

        protected virtual string Status(Entity e)
        {
            var state = e.GetAttributeValue<OptionSetValue>("statecode");
            return state == null ? "Unknown" : state.Value.ToString();
        }

        protected UsageReference Reference(
            Entity e,
            UsageSearchContext c,
            string type,
            string found,
            string raw,
            string table = null
        )
        {
            return new UsageReference
            {
                ComponentType = Name,
                Name = e.GetAttributeValue<string>("name"),
                ComponentId = e.Id,
                TableLogicalName = table ?? c.TableLogicalName,
                ColumnLogicalName = c.ColumnLogicalName,
                Status = Status(e),
                ReferenceType = type,
                FoundIn = found,
                RawReference = raw,
                ModifiedOn = e.GetAttributeValue<DateTime?>("modifiedon"),
                IsManaged = e.GetAttributeValue<bool?>("ismanaged"),
                Confidence = ReferenceConfidence.Confirmed,
            };
        }
    }

    internal sealed class FormUsageScanner : EntityScanner
    {
        public override string Name
        {
            get { return "Form"; }
        }

        protected override QueryExpression Query(UsageSearchContext c)
        {
            var q = FormQuery();
            q.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, c.TableLogicalName);
            return q;
        }

        public override IReadOnlyCollection<UsageReference> Scan(
            UsageSearchContext c,
            CancellationToken token,
            Action<string, int, int> progress
        )
        {
            var references = base.Scan(c, token, progress).ToList();
            if (c.SearchType != UsageSearchType.Column || references.Count > 0)
                return references;
            var fallback = FormQuery();
            fallback.Criteria.AddCondition(
                "formxml",
                ConditionOperator.Like,
                "%" + c.ColumnLogicalName + "%"
            );
            var queryService = new DataverseQueryService(c.Service);
            var forms = queryService
                .RetrieveAll(fallback, token)
                .Where(e => MatchesTable(e, c))
                .ToList();
            var current = 0;
            foreach (var form in forms)
            {
                token.ThrowIfCancellationRequested();
                progress(Name + " (formxml fallback)", ++current, forms.Count);
                references.AddRange(Parse(form, c));
            }
            return references;
        }

        private static QueryExpression FormQuery()
        {
            return new QueryExpression("systemform")
            {
                ColumnSet = new ColumnSet(
                    "name",
                    "formxml",
                    "type",
                    "objecttypecode",
                    "formactivationstate"
                ),
            };
        }

        private static bool MatchesTable(Entity form, UsageSearchContext c)
        {
            var value = form.Attributes.Contains("objecttypecode") ? form["objecttypecode"] : null;
            if (
                string.Equals(
                    Convert.ToString(value),
                    c.TableLogicalName,
                    StringComparison.OrdinalIgnoreCase
                )
            )
                return true;
            int code;
            return c.TableObjectTypeCode.HasValue
                && int.TryParse(Convert.ToString(value), out code)
                && code == c.TableObjectTypeCode.Value;
        }

        protected override string Status(Entity e)
        {
            var state = e.GetAttributeValue<OptionSetValue>("formactivationstate");
            return state != null && state.Value == 1 ? "Active" : "Inactive";
        }

        protected override IEnumerable<UsageReference> Parse(Entity e, UsageSearchContext c)
        {
            if (c.SearchType == UsageSearchType.Table)
                return new[]
                {
                    Reference(e, c, "Form Definition", "objecttypecode", c.TableLogicalName),
                };
            var hit = TextReferenceParser.Find(
                e.GetAttributeValue<string>("formxml"),
                c.ColumnLogicalName,
                "formxml",
                "Form Control"
            );
            return hit == null
                ? Enumerable.Empty<UsageReference>()
                : new[] { Reference(e, c, hit.ReferenceType, hit.FoundIn, hit.Snippet) };
        }
    }

    internal sealed class ViewUsageScanner : EntityScanner
    {
        public override string Name
        {
            get { return "View"; }
        }

        protected override QueryExpression Query(UsageSearchContext c)
        {
            var q = new QueryExpression("savedquery")
            {
                ColumnSet = new ColumnSet(
                    "name",
                    "fetchxml",
                    "layoutxml",
                    "returnedtypecode",
                    "statecode",
                    "modifiedon",
                    "ismanaged"
                ),
            };
            q.Criteria.AddCondition(
                "returnedtypecode",
                ConditionOperator.Equal,
                c.TableLogicalName
            );
            return q;
        }

        protected override string Status(Entity e)
        {
            var state = e.GetAttributeValue<OptionSetValue>("statecode");
            return state != null && state.Value == 1 ? "Active" : "Inactive";
        }

        protected override IEnumerable<UsageReference> Parse(Entity e, UsageSearchContext c)
        {
            if (c.SearchType == UsageSearchType.Table)
                return new[]
                {
                    Reference(e, c, "View Definition", "returnedtypecode", c.TableLogicalName),
                };
            return XmlReferenceParser
                .FindViewReferences(
                    e.GetAttributeValue<string>("fetchxml"),
                    c.ColumnLogicalName,
                    "fetchxml"
                )
                .Concat(
                    XmlReferenceParser.FindViewReferences(
                        e.GetAttributeValue<string>("layoutxml"),
                        c.ColumnLogicalName,
                        "layoutxml"
                    )
                )
                .Select(x => Reference(e, c, x.ReferenceType, x.FoundIn, x.Snippet));
        }
    }

    internal sealed class PluginStepUsageScanner : EntityScanner
    {
        public override string Name
        {
            get { return "Plugin Step"; }
        }

        protected override QueryExpression Query(UsageSearchContext c)
        {
            var q = new QueryExpression("sdkmessageprocessingstep")
            {
                ColumnSet = new ColumnSet(
                    "name",
                    "filteringattributes",
                    "stage",
                    "mode",
                    "statecode",
                    "statuscode",
                    "modifiedon"
                ),
            };
            var filter = q.AddLink("sdkmessagefilter", "sdkmessagefilterid", "sdkmessagefilterid");
            filter.EntityAlias = "filter";
            filter.Columns = new ColumnSet("primaryobjecttypecode");
            filter.LinkCriteria.AddCondition(
                "primaryobjecttypecode",
                ConditionOperator.Equal,
                c.TableLogicalName
            );
            var message = filter.AddLink("sdkmessage", "sdkmessageid", "sdkmessageid");
            message.EntityAlias = "message";
            message.Columns = new ColumnSet("name");
            var plugin = q.AddLink(
                "plugintype",
                "eventhandler",
                "plugintypeid",
                JoinOperator.LeftOuter
            );
            plugin.EntityAlias = "plugin";
            plugin.Columns = new ColumnSet("typename");
            return q;
        }

        protected override string Status(Entity e)
        {
            var state = e.GetAttributeValue<OptionSetValue>("statecode");
            return state != null && state.Value == 0 ? "Enabled" : "Disabled";
        }

        protected override IEnumerable<UsageReference> Parse(Entity e, UsageSearchContext c)
        {
            if (
                c.SearchType == UsageSearchType.Column
                && !FilteringAttributesParser.Contains(
                    e.GetAttributeValue<string>("filteringattributes"),
                    c.ColumnLogicalName
                )
            )
                return Enumerable.Empty<UsageReference>();
            var details =
                "Message: "
                + Alias(e, "message.name")
                + "; Stage: "
                + (
                    e.GetAttributeValue<OptionSetValue>("stage") == null
                        ? ""
                        : e.GetAttributeValue<OptionSetValue>("stage").Value.ToString()
                )
                + "; Mode: "
                + (
                    e.GetAttributeValue<OptionSetValue>("mode") == null
                        ? ""
                        : e.GetAttributeValue<OptionSetValue>("mode").Value.ToString()
                )
                + "; Plugin: "
                + Alias(e, "plugin.typename");
            var r = Reference(
                e,
                c,
                c.SearchType == UsageSearchType.Column ? "Filtering Attribute" : "Message Entity",
                c.SearchType == UsageSearchType.Column ? "filteringattributes" : "sdkmessagefilter",
                c.SearchType == UsageSearchType.Column
                    ? e.GetAttributeValue<string>("filteringattributes")
                    : c.TableLogicalName
            );
            r.Details = details;
            return new[] { r };
        }

        private static string Alias(Entity e, string name)
        {
            var a = e.GetAttributeValue<AliasedValue>(name);
            return a == null || a.Value == null ? "" : a.Value.ToString();
        }
    }
}
