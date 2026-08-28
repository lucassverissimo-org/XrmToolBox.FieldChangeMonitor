# Architecture

This repository contains three independent XrmToolBox tools, one shared library, one central test project, and one
development application:

- `LucasVerissimo.XrmToolBox.FieldChangeMonitor`: monitoring UI and monitoring rules.
- `LucasVerissimo.XrmToolBox.DataverseUsageExplorer`: usage-search UI, scanners, and parsers.
- `LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer`: multi-environment solution composition, layer analysis,
  backup, and explicitly confirmed Active Layer removal.
- `LucasVerissimo.XrmToolBox.Shared`: a Shared Project containing reusable Dataverse and WinForms source.
- `LucasVerissimo.XrmToolBox.Tests`: centralized unit tests organized by owning tool, plus reusable test doubles.
- `LucasVerissimo.XrmToolBox.ToolManager`: an internal WinForms application that discovers tool release manifests,
  validates packages, and coordinates NuGet publishing. It is not an XrmToolBox plugin and is never packaged with a tool.

## Dependency direction

All three tools import `Shared.projitems`. Shared code must never depend on a tool project.

```text
FieldChangeMonitor ───────┐
DataverseUsageExplorer ───┼──> Shared ──> Microsoft.Xrm.Sdk
SolutionLayerAnalyzer ────┘
```

## What belongs in Shared

Move code to `Shared` when it:

1. has the same purpose and behavior in more than one tool;
2. does not depend on a tool's controls, settings, or business rules; and
3. has a stable API that can remain backward compatible.

Examples currently centralized in `Shared.BusinessLogic`:

- retrieving table and column metadata;
- retrieving every page of a `QueryExpression`;
- resolving localized metadata labels.

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

## Central automated tests

`LucasVerissimo.XrmToolBox.Tests` targets .NET Framework 4.8 so it can reference every plugin assembly without
changing the runtime target of the tools. Tests are grouped by tool folder, and reusable Dataverse fakes belong in
`TestDoubles`.

The test project may reference multiple tools, but tools must never reference the test project or another tool.
Unit tests must be deterministic and must not connect to live environments. Read-only Dataverse integration tests
use the explicit `Integration` category and load connection strings from an ignored local settings file. They must
receive the guarded read-only organization-service adapter; destructive Dataverse requests are not permitted in
automated tests.

This packaging boundary prevents independently versioned tools from overwriting a common Shared DLL in the
XrmToolBox plugin directory. It also ensures that the XrmToolBox portal validates only the assembly whose
version matches the package. `Shared` is never published independently.

Changes to shared source must be evaluated by rebuilding every consuming tool. Release validation must confirm
that the plugin assembly has no external Shared assembly reference and contains the required Shared Project
types.
