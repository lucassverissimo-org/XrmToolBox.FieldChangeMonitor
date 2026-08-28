# Lucas Verissimo tools for XrmToolBox

This repository is the home of the XrmToolBox tools maintained by Lucas Verissimo. Each tool has its own
project, assembly, NuGet package, documentation, version, and release lifecycle.

## Tools

| Tool | Purpose | Package | Documentation |
| --- | --- | --- | --- |
| Field Change Monitor | Monitor Dataverse field changes with FetchXML filters, history, and Windows alerts. | [NuGet](https://www.nuget.org/packages/LucasVerissimo.XrmToolBox.FieldChangeMonitor) | [Documentation](LucasVerissimo.XrmToolBox.FieldChangeMonitor/README.md) |
| Dataverse Usage Explorer | Find where Dataverse tables and columns are referenced by solution components. | [NuGet](https://www.nuget.org/packages/LucasVerissimo.XrmToolBox.DataverseUsageExplorer) | [Documentation](LucasVerissimo.XrmToolBox.DataverseUsageExplorer/README.md) |
| Solution Layer Analyzer | Compare solution composition and inspect or remove selected Active Layers across environments. | [NuGet](https://www.nuget.org/packages/LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer) | [Documentation](LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer/README.md) |

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
dotnet build .\LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer\LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.csproj
```

Launch the Solution Layer Analyzer in an isolated local XrmToolBox profile with:

```powershell
.\run-solution-layer-analyzer.ps1
```

## Testing

Tests for every tool are centralized in `LucasVerissimo.XrmToolBox.Tests` and organized by tool folder. Run the
local, deterministic unit suite with:

```powershell
dotnet test .\LucasVerissimo.XrmToolBox.Tests\LucasVerissimo.XrmToolBox.Tests.csproj --filter "TestCategory=Unit"
```

Read-only real-environment tests use the ignored `LucasVerissimo.XrmToolBox.Tests/local.settings.json` file and
the explicit `Integration` category. Their organization-service wrapper blocks all mutations and every `Execute`
request, including `RemoveActiveCustomization`. See the [test project instructions](LucasVerissimo.XrmToolBox.Tests/README.md).

## License

MIT
