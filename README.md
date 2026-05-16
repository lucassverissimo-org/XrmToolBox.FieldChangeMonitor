# Field Change Monitor for XrmToolBox

Field Change Monitor is an XrmToolBox tool for monitoring Dataverse table field changes with FetchXML filters and Windows alerts.

## Features

- Monitor one or more Dataverse tables at the same time.
- Select one or more fields to watch.
- Build FetchXML filter conditions with a visual condition builder.
- Pick valid lookup and option set values when building conditions.
- Receive Windows notifications when monitored values change.

## Packaging

Build the release assembly before packing:

```powershell
dotnet build .\XrmTool-bravo.csproj -c Release
nuget pack .\LucasVerissimo.XrmToolBox.FieldChangeMonitor.nuspec
```

Before publishing to nuget.org and XrmToolBox Tool Library, confirm the NuGet package id, author, project URL, icon URL, and license metadata.
