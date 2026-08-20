# Architecture

This repository contains two independent XrmToolBox tools and one shared library:

- `LucasVerissimo.XrmToolBox.FieldChangeMonitor`: monitoring UI and monitoring rules.
- `LucasVerissimo.XrmToolBox.DataverseUsageExplorer`: usage-search UI, scanners, and parsers.
- `LucasVerissimo.XrmToolBox.Shared`: reusable Dataverse and WinForms building blocks.

## Dependency direction

Both tools may reference `Shared`. `Shared` must never reference either tool.

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

UI models, scanner rules, monitoring rules, and component-opening behavior stay in their owning tool.

## Readability rules

- Use one statement per line.
- Use descriptive names; avoid one-letter names outside very small loops.
- Validate public method arguments.
- Keep Dataverse communication out of forms and controls when the operation is reusable.
- Keep each method focused on one responsibility.
- Prefer explicit control flow over compressed expressions when debugging would otherwise be harder.

## Independent releases

Each tool keeps its own assembly, version, NuGet specification, and release process. A package that consumes
`Shared` must include the exact compatible `LucasVerissimo.XrmToolBox.Shared.dll` used during its build.

Because XrmToolBox loads plugins from a common directory, changes to the public API of `Shared` must be
backward compatible. Breaking changes require a new shared assembly identity instead of replacing the existing
assembly used by an already-published tool.
