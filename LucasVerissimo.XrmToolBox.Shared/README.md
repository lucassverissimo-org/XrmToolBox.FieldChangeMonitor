# LucasVerissimo.XrmToolBox.Shared

This project contains reusable Dataverse and WinForms building blocks shared by tools in this solution. It is
an internal solution library, not an independently installed XrmToolBox tool.

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

Every tool that references this project must include the exact compatible
`LucasVerissimo.XrmToolBox.Shared.dll` in its own NuGet package. XrmToolBox loads plugin assemblies from a
common directory, so the public Shared API must remain backward compatible across independently released
tools.

Do not remove or change an existing public member without evaluating every tool in the solution. A breaking
change requires an explicit assembly identity and migration strategy.

See the solution [architecture](../ARCHITECTURE.md) and [engineering rules](../AGENTS.md) for the complete
boundary and maintenance standards.
