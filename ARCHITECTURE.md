# Architecture

This repository contains three independent XrmToolBox tools, one shared library, and one development application:

- `LucasVerissimo.XrmToolBox.FieldChangeMonitor`: monitoring UI and monitoring rules.
- `LucasVerissimo.XrmToolBox.DataverseUsageExplorer`: usage-search UI, scanners, and parsers.
- `LucasVerissimo.XrmToolBox.DataverseMappingManager`: column mapping UI, compatibility rules, staged changes, publication, and solution inclusion.
- `LucasVerissimo.XrmToolBox.Shared`: a Shared Project containing reusable Dataverse and WinForms source.
- `LucasVerissimo.XrmToolBox.ToolManager`: an internal WinForms application that discovers tool release manifests,
  validates packages, and coordinates NuGet publishing. It is not an XrmToolBox plugin and is never packaged with a tool.

## Dependency direction

All three tools import `Shared.projitems`. Shared code must never depend on a tool.

```text
FieldChangeMonitor ──────┐
DataverseUsageExplorer ───┼──> Shared ──> Microsoft.Xrm.Sdk
DataverseMappingManager ──┘
```

## What belongs in Shared

Move code to `Shared` when it:

1. has the same purpose and behavior in both tools;
2. does not depend on a tool's controls, settings, or business rules; and
3. has a stable API that can remain backward compatible.

Examples currently centralized in `Shared.BusinessLogic`:

- retrieving table and column metadata;
- retrieving every page of a `QueryExpression`;
- resolving localized metadata labels.

Reusable WinForms controls are centralized in `Shared.Controls`. `GridPickerControl` is the standard picker
for lists that need a searchable and sortable grid. The consuming tool supplies the item identity, display
text, search rule, and column definitions through `GridPickerConfiguration`; the shared control must not
reference tool-specific models.

UI models, scanner rules, monitoring rules, and component-opening behavior stay in their owning tool.

## Readability rules

- Use one statement per line.
- Use descriptive names; avoid one-letter names outside very small loops.
- Validate public method arguments.
- Keep Dataverse communication out of forms and controls when the operation is reusable.
- Keep each method focused on one responsibility.
- Prefer explicit control flow over compressed expressions when debugging would otherwise be harder.

## Independent releases

Each tool keeps its own assembly, version, NuGet specification, and release process. The Shared Project source
is compiled directly into every consuming tool. It does not produce `LucasVerissimo.XrmToolBox.Shared.dll`.
Each NuGet package contains exactly one DLL under `lib/net48/Plugins`.

This packaging boundary prevents independently versioned tools from overwriting a common Shared DLL in the
XrmToolBox plugin directory. It also ensures that the XrmToolBox portal validates only the assembly whose
version matches the package. `Shared` is never published independently.

Changes to shared source must be evaluated by rebuilding every consuming tool. Release validation must confirm
that the plugin assembly has no external Shared assembly reference and contains the required Shared Project
types.

## Mapping manager

Mapping Manager edits `attributemap` records under an existing directed `entitymap`; it does not create
relationships or synchronize business data. `MappingWorkspace` keeps the original snapshot and staged changes.
`MappingRules` validates structural compatibility and unique destinations. `MappingService` rechecks server
state and metadata before applying changes. Edits replace mappings using delete/create within a transaction.
Changes sharing old/new destinations form an atomic group; independent groups may succeed separately.
Confirmed faults preserve pending changes; uncertain communication failures require a reload before retrying.
Publishing and adding selected components to an unmanaged solution are separate operations with separate results.
No tool-specific rules were added to Shared. The release manifest is discovered by Tool Manager automatically.

Solution inclusion adds supported table, relationship and column components. Map subcomponents are not added
individually. `SolutionMappingExportVerifier` reads an unmanaged solution export in memory and confirms exact
mapping pairs in `customizations.xml`, reporting missing mappings/export failures separately from successful
component additions. Removal absence is a warning, not confirmation of a downstream deletion. All loaded saved
mappings are selectable without resaving; only successful session additions/edits are preselected. The solution
modal derives component dependencies from its own checked mappings and keeps removals in a separate section.
No export package is modified or imported.

Mapping editing uses a dedicated modal with native constrained column selectors, compatibility validation,
and an explicit local Apply action. Independent source/target refreshes replace only metadata. Mapping
refreshes retain pending rows when the original server snapshot is unchanged and reject concurrent changes
without replacing local work. Refreshing never clears publication state or session export expectations.
