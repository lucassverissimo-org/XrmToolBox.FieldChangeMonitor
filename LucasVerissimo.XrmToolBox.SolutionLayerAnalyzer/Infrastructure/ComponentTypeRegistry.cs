using System.Collections.Generic;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure
{
    internal sealed class ComponentTypeDefinition
    {
        public int Value { get; set; }

        public string DisplayName { get; set; }

        public string LayerComponentName { get; set; }

        public string RemovalLogicalName { get; set; }

        public string BackingEntityName { get; set; }

        public string BackingEntityPrimaryId { get; set; }

        public string BackingEntityPrimaryName { get; set; }
    }

    internal static class ComponentTypeRegistry
    {
        private static readonly Dictionary<int, ComponentTypeDefinition> Definitions =
            new Dictionary<int, ComponentTypeDefinition>
            {
                { 1, Create(1, "Entity", "Entity", "entity") },
                { 2, Create(2, "Attribute", "Attribute", "attribute") },
                { 3, Create(3, "Relationship", "Relationship", "relationship") },
                { 9, Create(9, "Option Set", "OptionSet", "optionset") },
                {
                    24,
                    CreateRecordType(24, "Form", "Form", "form", "systemform", "formid", "name")
                },
                {
                    26,
                    CreateRecordType(
                        26,
                        "Saved Query",
                        "SavedQuery",
                        "savedquery",
                        "savedquery",
                        "savedqueryid",
                        "name"
                    )
                },
                {
                    29,
                    CreateRecordType(
                        29,
                        "Workflow",
                        "Workflow",
                        "workflow",
                        "workflow",
                        "workflowid",
                        "name"
                    )
                },
                {
                    60,
                    CreateRecordType(
                        60,
                        "System Form",
                        "SystemForm",
                        "systemform",
                        "systemform",
                        "formid",
                        "name"
                    )
                },
                {
                    61,
                    CreateRecordType(
                        61,
                        "Web Resource",
                        "WebResource",
                        "webresource",
                        "webresource",
                        "webresourceid",
                        "name"
                    )
                },
                {
                    62,
                    CreateRecordType(
                        62,
                        "Site Map",
                        "SiteMap",
                        "sitemap",
                        "sitemap",
                        "sitemapid",
                        "sitemapname"
                    )
                },
                {
                    91,
                    CreateRecordType(
                        91,
                        "Plugin Assembly",
                        "PluginAssembly",
                        null,
                        "pluginassembly",
                        "pluginassemblyid",
                        "name"
                    )
                },
                {
                    300,
                    CreateRecordType(
                        300,
                        "Canvas App",
                        "CanvasApp",
                        "canvasapp",
                        "canvasapp",
                        "canvasappid",
                        "name"
                    )
                },
            };

        public static ComponentTypeDefinition Get(int componentType)
        {
            ComponentTypeDefinition definition;
            if (Definitions.TryGetValue(componentType, out definition))
            {
                return definition;
            }

            return Create(componentType, "Unknown (" + componentType + ")", "Unknown", null);
        }

        private static ComponentTypeDefinition Create(
            int value,
            string displayName,
            string layerName,
            string removalName
        )
        {
            return new ComponentTypeDefinition
            {
                Value = value,
                DisplayName = displayName,
                LayerComponentName = layerName,
                RemovalLogicalName = removalName,
            };
        }

        private static ComponentTypeDefinition CreateRecordType(
            int value,
            string displayName,
            string layerName,
            string removalName,
            string entityName,
            string primaryId,
            string primaryName
        )
        {
            var definition = Create(value, displayName, layerName, removalName);
            definition.BackingEntityName = entityName;
            definition.BackingEntityPrimaryId = primaryId;
            definition.BackingEntityPrimaryName = primaryName;
            return definition;
        }
    }
}
