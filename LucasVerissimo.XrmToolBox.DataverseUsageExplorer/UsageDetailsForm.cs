using System;
using System.Text;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Services;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer
{
    internal partial class UsageDetailsForm : Form
    {
        private readonly ComponentNavigationTarget navigationTarget;

        public UsageDetailsForm(
            UsageReference item,
            string environmentUrl,
            DefaultSolutionNavigationContext defaultSolution
        )
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            InitializeComponent();
            detailsTextBox.Text = BuildDetails(item);
            navigationTarget = ComponentNavigationService.Resolve(
                item,
                environmentUrl,
                defaultSolution
            );
            ConfigureComponentLink();
        }

        private void ConfigureComponentLink()
        {
            componentLinkLabel.Visible = navigationTarget.CanOpen;
            componentLinkLabel.Text = navigationTarget.LinkText ?? string.Empty;
            navigationMessageLabel.Visible = !navigationTarget.CanOpen;
            navigationMessageLabel.Text = navigationTarget.UnavailableReason ?? string.Empty;
        }

        private void ComponentLinkLabelLinkClicked(
            object sender,
            LinkLabelLinkClickedEventArgs eventArguments
        )
        {
            try
            {
                ComponentNavigationService.Open(navigationTarget);
                componentLinkLabel.LinkVisited = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    this,
                    exception.Message,
                    "Unable to open component",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static string BuildDetails(UsageReference item)
        {
            var details = new StringBuilder();

            details
                .AppendLine("Component Type: " + item.ComponentType)
                .AppendLine("Component Name: " + item.Name)
                .AppendLine("Component Id: " + item.ComponentId)
                .AppendLine("Table: " + item.TableLogicalName)
                .AppendLine("Column: " + item.ColumnLogicalName)
                .AppendLine("Status: " + item.Status)
                .AppendLine("Reference Type: " + item.ReferenceType)
                .AppendLine("Found In: " + item.FoundIn)
                .AppendLine("Confidence: " + item.Confidence)
                .AppendLine("Managed: " + item.IsManaged)
                .AppendLine("Modified On: " + item.ModifiedOn)
                .AppendLine("Details: " + item.Details)
                .AppendLine()
                .AppendLine("Raw Reference")
                .AppendLine(item.RawReference);

            return details.ToString();
        }
    }
}
