# LucasVerissimo.XrmToolBox.Shared

This Shared Project contains reusable Dataverse and WinForms source imported by tools in this solution. It does
not produce an assembly and is not an independently installed XrmToolBox tool.

## Current responsibilities

### Business logic

- Retrieve Dataverse table and column metadata.
- Retrieve every page produced by a `QueryExpression`.
- Resolve localized metadata labels consistently.

### Shared WinForms controls

- Select Dataverse lookup values.
- Select Dataverse choice values.

## Dependency rules

- Shared code must be genuinely useful to more than one tool.
- Shared code must not depend on a tool's controls, settings, models, or business rules.
- `Shared` must never reference a tool project.
- Tool-specific scanning, monitoring, navigation, and UI behavior stays in the owning tool.

## Packaging and compatibility

Tools import `LucasVerissimo.XrmToolBox.Shared.projitems`, so this source is compiled directly into each primary
plugin assembly. There is no `LucasVerissimo.XrmToolBox.Shared.dll` to distribute.

This keeps independently versioned tools from overwriting a common DLL in the XrmToolBox plugin directory.
Each NuGet package contains exactly one versioned assembly under `lib/net48/Plugins`.

Do not remove or change an existing shared member without rebuilding and validating every consuming tool.

See the solution [architecture](../ARCHITECTURE.md) and [engineering rules](../AGENTS.md) for the complete
boundary and maintenance standards.
