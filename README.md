# Field Change Monitor for XrmToolBox

Field Change Monitor is an XrmToolBox tool for monitoring Dataverse table field changes with FetchXML filters and Windows alerts.

## Features

- Monitor one or more Dataverse tables at the same time.
- Select one or more fields to watch.
- Build FetchXML filter conditions with a visual condition builder.
- Edit and validate FetchXML and synchronize supported conditions back to the visual builder.
- Pick valid lookup and option set values when building conditions.
- Name, edit, pause, resume, and remove individual monitors.
- Persist monitor definitions per Dataverse environment and restore them safely as paused.
- Export and import versioned `.fcm.json` monitor definitions with Dataverse metadata validation.
- Review recent changes in a structured grid with ModifiedOn, ModifiedBy, record ID, field, and old/new values.
- Open changed Dataverse records in the default browser after confirmation.
- Receive Windows notifications when monitored values change.

## Version 1.1.0.1

This release introduces a redesigned monitoring workflow, persistent monitor definitions,
portable import/export, monitor editing and pause/resume controls, FetchXML validation,
and a structured recent-change history.

## Packaging

Build the release assembly before packing:

```powershell
dotnet build .\XrmTool-bravo.csproj -c Release
nuget pack .\LucasVerissimo.XrmToolBox.FieldChangeMonitor.nuspec
```

The NuGet package version and assembly version must match. Before publishing to nuget.org
and XrmToolBox Tool Library, confirm the package id, author, project URL, icon URL,
release notes, license metadata, and the `lib/net48/Plugins` package content.
