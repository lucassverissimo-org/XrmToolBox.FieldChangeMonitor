# Lucas Verissimo tools for XrmToolBox

This repository is the home of the XrmToolBox tools maintained by Lucas Verissimo. Each tool has its own
project, assembly, NuGet package, documentation, version, and release lifecycle.

## Tools

| Tool | Purpose | Package | Documentation |
| --- | --- | --- | --- |
| Field Change Monitor | Monitor Dataverse field changes with FetchXML filters, history, and Windows alerts. | [NuGet](https://www.nuget.org/packages/LucasVerissimo.XrmToolBox.FieldChangeMonitor) | [Documentation](LucasVerissimo.XrmToolBox.FieldChangeMonitor/README.md) |
| Dataverse Usage Explorer | Find where Dataverse tables and columns are referenced by solution components. | [NuGet](https://www.nuget.org/packages/LucasVerissimo.XrmToolBox.DataverseUsageExplorer) | [Documentation](LucasVerissimo.XrmToolBox.DataverseUsageExplorer/README.md) |

## Shared library

[`LucasVerissimo.XrmToolBox.Shared`](LucasVerissimo.XrmToolBox.Shared/README.md) is a Shared Project containing
reusable Dataverse and WinForms source compiled into each consuming tool. It is not an XrmToolBox tool and is
not published or installed independently.

## Solution standards

- [Architecture](ARCHITECTURE.md)
- [Engineering rules for agents and contributors](AGENTS.md)
- [Dataverse Usage Explorer publishing guide](PUBLISHING_DATAVERSE_USAGE_EXPLORER.md)

## Building

Open `LucasVerissimo.XrmToolBox.slnx` in Visual Studio or build an individual project with the .NET CLI. Each
tool is independently buildable and must not reference another tool project.

```powershell
dotnet build .\LucasVerissimo.XrmToolBox.FieldChangeMonitor\LucasVerissimo.XrmToolBox.FieldChangeMonitor.csproj
dotnet build .\LucasVerissimo.XrmToolBox.DataverseUsageExplorer\LucasVerissimo.XrmToolBox.DataverseUsageExplorer.csproj
```

## License

MIT
