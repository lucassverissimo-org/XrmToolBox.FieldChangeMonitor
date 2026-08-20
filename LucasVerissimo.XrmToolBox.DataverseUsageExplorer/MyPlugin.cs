using System.ComponentModel.Composition;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer
{
    [
        Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Dataverse Usage Explorer"),
        ExportMetadata("Description", "Find where Dataverse tables and columns are used"),
        ExportMetadata("SmallImageBase64", PluginImages.SmallImageBase64),
        ExportMetadata("BigImageBase64", PluginImages.BigImageBase64),
        ExportMetadata("BackgroundColor", "White"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "DarkSlateGray")
    ]
    public sealed class MyPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new UsageExplorerControl();
        }
    }
}
