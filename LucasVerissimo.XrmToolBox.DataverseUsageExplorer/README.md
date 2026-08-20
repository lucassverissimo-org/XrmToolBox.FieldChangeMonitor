# Dataverse Usage Explorer

Dataverse Usage Explorer is an XrmToolBox tool for finding where a Dataverse table or column is referenced.
It brings several component searches into one screen and lets makers and developers inspect the result before
changing or removing schema elements.

## Features

- Search tables and columns by display name or logical name.
- Scan business rules, classic workflows, business process flows, and Power Automate flows.
- Scan model-driven app forms and system views.
- Scan plugin steps and filtering attributes.
- Show the component name, status, reference type, source property, confidence, modification date, and ID.
- Open supported components directly in the connected environment.
- Report progressive per-record scan progress.
- Return one result per component ID to avoid duplicate rows.

## Requirements

- XrmToolBox 1.2025.7.71 or later.
- A Dataverse environment and an account with permission to read metadata and the component records being
  inspected.
- .NET Framework 4.8, supplied by the supported XrmToolBox installation.

## Getting started

1. Open **Dataverse Usage Explorer** in XrmToolBox.
2. Select a Dataverse connection.
3. Load and select a table.
4. Choose whether to search by table or by column.
5. When searching by column, select a column from the chosen table.
6. Select the component types to inspect and click **Scan**.
7. Select a result and use **Open Component** when direct navigation is supported.

## How results are detected

The tool combines structured Dataverse queries with XML, JSON, configuration, and filtering-attribute
inspection. The **Confidence** column distinguishes confirmed structured matches from text-based matches.

Text-based detection is intentionally broad and can produce false positives. Dataverse components or newer
component formats that are not included in the current scanners can also contain references that are not
reported. Review results before changing production customizations.

## Privacy

The tool communicates directly with the Dataverse environment selected in XrmToolBox. It does not send
environment data to an external service.

## Version 1.0.3

This release compiles the Shared Project source directly into the plugin assembly. The public NuGet package
contains one versioned plugin DLL without a runtime Shared dependency. Version 1.0.2 introduced the explicit
public PNG icon URL required by the XrmToolBox Tool Library registration validator.

## Source and issues

Source code and issue tracking are available in the
[XrmToolBox tools repository](https://github.com/lucassverissimo-org/XrmToolBox.FieldChangeMonitor). Use the
[issue tracker](https://github.com/lucassverissimo-org/XrmToolBox.FieldChangeMonitor/issues) to report a problem
or request an enhancement.

## License

MIT
