using System;
using System.Diagnostics;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Services
{
    internal sealed class ComponentNavigationTarget
    {
        private ComponentNavigationTarget(string linkText, string url, string unavailableReason)
        {
            LinkText = linkText;
            Url = url;
            UnavailableReason = unavailableReason;
        }

        public bool CanOpen => !string.IsNullOrWhiteSpace(Url);

        public string LinkText { get; }

        public string UnavailableReason { get; }

        public string Url { get; }

        public static ComponentNavigationTarget Available(string linkText, string url)
        {
            return new ComponentNavigationTarget(linkText, url, null);
        }

        public static ComponentNavigationTarget Unavailable(string reason)
        {
            return new ComponentNavigationTarget(null, null, reason);
        }
    }

    internal static class ComponentNavigationService
    {
        public static ComponentNavigationTarget Resolve(
            UsageReference item,
            string environmentUrl,
            DefaultSolutionNavigationContext defaultSolution
        )
        {
            if (item == null)
            {
                return ComponentNavigationTarget.Unavailable("Select a component first.");
            }

            if (!item.ComponentId.HasValue)
            {
                return ComponentNavigationTarget.Unavailable(
                    "This reference does not contain a component identifier."
                );
            }

            if (string.IsNullOrWhiteSpace(environmentUrl))
            {
                return ComponentNavigationTarget.Unavailable(
                    "Connect to a Dataverse environment before opening the component."
                );
            }

            switch (item.ComponentType)
            {
                case "Business Rule":
                case "Business Process Flow":
                case "Classic Workflow":
                    return CreateClassicProcessTarget(item.ComponentId.Value, environmentUrl);
                case "Plugin Step":
                    return ComponentNavigationTarget.Unavailable(
                        "Plugin steps must be viewed or edited with the XrmToolBox Plugin Registration tool."
                    );
                case "Power Automate":
                    return CreatePowerAutomateTarget(item, defaultSolution);
                case "Form":
                    return CreateTableComponentTarget(item, defaultSolution, "form");
                case "View":
                    return CreateTableComponentTarget(item, defaultSolution, "view");
                case "Web Resource":
                    return CreateDefaultSolutionTarget(item, defaultSolution, "web resources");
                default:
                    return ComponentNavigationTarget.Unavailable(
                        "A direct viewing or editing route is not available for this component type."
                    );
            }
        }

        public static void Open(ComponentNavigationTarget target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!target.CanOpen)
            {
                throw new InvalidOperationException(target.UnavailableReason);
            }

            Process.Start(new ProcessStartInfo(target.Url) { UseShellExecute = true });
        }

        private static ComponentNavigationTarget CreateClassicProcessTarget(
            Guid componentId,
            string environmentUrl
        )
        {
            var encodedId = Uri.EscapeDataString("{" + componentId.ToString("D") + "}");
            var url = environmentUrl.TrimEnd('/') + "/sfa/workflow/edit.aspx?id=" + encodedId;

            return ComponentNavigationTarget.Available("Open process for viewing or editing", url);
        }

        private static ComponentNavigationTarget CreateDefaultSolutionTarget(
            UsageReference item,
            DefaultSolutionNavigationContext defaultSolution,
            string objectCategory
        )
        {
            if (defaultSolution == null)
            {
                return ComponentNavigationTarget.Unavailable(
                    "The Default solution could not be resolved for this environment."
                );
            }

            var url = CreateDefaultSolutionBaseUrl(defaultSolution);

            url += string.IsNullOrWhiteSpace(objectCategory)
                ? "/entities"
                : "/objects/" + Uri.EscapeDataString(objectCategory);

            return ComponentNavigationTarget.Available(
                "Open the Default solution to locate " + item.Name,
                url
            );
        }

        private static ComponentNavigationTarget CreatePowerAutomateTarget(
            UsageReference item,
            DefaultSolutionNavigationContext defaultSolution
        )
        {
            if (defaultSolution == null)
            {
                return CreateDefaultSolutionTarget(item, defaultSolution, "cloudflows");
            }

            var url =
                CreateDefaultSolutionBaseUrl(defaultSolution)
                + "/objects/cloudflows/"
                + item.ComponentId.Value.ToString("D")
                + "/view";

            return ComponentNavigationTarget.Available(
                "Open " + item.Name + " in Power Automate",
                url
            );
        }

        private static ComponentNavigationTarget CreateTableComponentTarget(
            UsageReference item,
            DefaultSolutionNavigationContext defaultSolution,
            string componentRoute
        )
        {
            if (defaultSolution == null || string.IsNullOrWhiteSpace(item.TableLogicalName))
            {
                return CreateDefaultSolutionTarget(item, defaultSolution, null);
            }

            var url =
                CreateTableDesignerBaseUrl(defaultSolution)
                + "/entity/"
                + Uri.EscapeDataString(item.TableLogicalName)
                + "/"
                + componentRoute;

            if (string.Equals(componentRoute, "form", StringComparison.OrdinalIgnoreCase))
            {
                url += "/edit";
            }

            url += "/" + item.ComponentId.Value.ToString("D");

            return ComponentNavigationTarget.Available(
                "Open " + item.Name + " for viewing or editing",
                url
            );
        }

        private static string CreateDefaultSolutionBaseUrl(
            DefaultSolutionNavigationContext defaultSolution
        )
        {
            return "https://make.powerapps.com/environments/"
                + Uri.EscapeDataString(defaultSolution.EnvironmentId)
                + "/solutions/"
                + defaultSolution.SolutionId.ToString("D");
        }

        private static string CreateTableDesignerBaseUrl(
            DefaultSolutionNavigationContext defaultSolution
        )
        {
            return "https://make.powerapps.com/e/"
                + Uri.EscapeDataString(defaultSolution.EnvironmentId)
                + "/s/"
                + defaultSolution.SolutionId.ToString("D");
        }
    }
}
