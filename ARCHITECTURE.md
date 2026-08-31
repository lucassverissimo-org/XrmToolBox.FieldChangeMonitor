# Architecture

This repository contains two independent XrmToolBox tools, one shared library, and one development application:

- `LucasVerissimo.XrmToolBox.FieldChangeMonitor`: monitoring UI and monitoring rules.
- `LucasVerissimo.XrmToolBox.DataverseUsageExplorer`: usage-search UI, scanners, and parsers.
- `LucasVerissimo.XrmToolBox.Shared`: a Shared Project containing reusable Dataverse and WinForms source.
- `LucasVerissimo.XrmToolBox.ToolManager`: an internal WinForms application that discovers tool release manifests,
  validates packages, and coordinates NuGet publishing. It is not an XrmToolBox plugin and is never packaged with a tool.

## Dependency direction

Both tools import `Shared.projitems`. Shared code must never depend on either tool.

```text
FieldChangeMonitor ───────┐
                          ├──> Shared ──> Microsoft.Xrm.Sdk
DataverseUsageExplorer ──┘
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
