# LucasVerissimo.XrmToolBox.Shared

Reusable components shared by the XrmToolBox plugins in this solution.

Code belongs in this project only when it has no dependency on a specific plugin,
its settings, or its business rules. The project currently provides generic
WinForms pickers for Dataverse lookup and choice values.

Every plugin that references this project must include
`LucasVerissimo.XrmToolBox.Shared.dll` in its own NuGet package.
