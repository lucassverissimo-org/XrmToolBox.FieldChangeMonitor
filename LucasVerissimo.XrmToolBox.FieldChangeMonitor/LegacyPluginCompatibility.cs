using System;

namespace XrmTool_bravo
{
    /// <summary>
    /// Preserves compatibility with XrmToolBox manifests created before the
    /// plugin namespace was renamed. This type is intentionally not exported
    /// through MEF; new manifests discover the canonical plugin type only.
    /// </summary>
    [Obsolete("Use LucasVerissimo.XrmToolBox.FieldChangeMonitor.MyPlugin.")]
    public sealed class MyPlugin : LucasVerissimo.XrmToolBox.FieldChangeMonitor.MyPlugin
    {
    }
}
