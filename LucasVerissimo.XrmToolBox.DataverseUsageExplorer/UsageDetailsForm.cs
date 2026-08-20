using System.Text;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer
{
    internal partial class UsageDetailsForm : Form
    {
        public UsageDetailsForm(UsageReference item)
        {
            InitializeComponent();
            detailsTextBox.Text = BuildDetails(item);
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
