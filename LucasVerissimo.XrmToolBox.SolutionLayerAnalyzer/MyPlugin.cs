using System.ComponentModel.Composition;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer
{
    [
        Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Solution Layer Analyzer"),
        ExportMetadata(
            "Description",
            "Compare solution composition and safely inspect or remove Active Layers"
        ),
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
            return new SolutionLayerAnalyzerControl();
        }
    }
}
