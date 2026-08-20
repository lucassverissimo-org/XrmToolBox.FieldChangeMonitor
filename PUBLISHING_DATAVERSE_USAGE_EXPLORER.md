# Publishing Dataverse Usage Explorer

The Dataverse Usage Explorer is released independently from Field Change Monitor.

## Pre-release validation

From the repository root:

```powershell
dotnet tool restore
dotnet csharpier check .
.\tests\dataverse-usage-explorer-release-tests.ps1
```

The validation builds the Release package and confirms that:

- the NuGet and plugin assembly versions match;
- the package contains exactly one plugin assembly in `lib/net48/Plugins`;
- Shared Project types are compiled into the plugin and there is no external Shared assembly reference;
- the Field Change Monitor assembly is absent;
- the README and icon are present;
- debug symbols and known host dependencies are not bundled.

## Local installation test

1. Close XrmToolBox.
2. Back up `%AppData%\MscrmTools\XrmToolBox\Plugins` if it contains development builds.
3. Open the generated `.nupkg` from `bin\Release` as a ZIP archive.
4. Copy the contents of `lib\net48\Plugins` to the XrmToolBox `Plugins` directory.
5. Start XrmToolBox and verify the official validation checklist:
   - the tool appears with small and large icons;
   - it opens before selecting a connection;
   - it resizes correctly;
   - table and column selectors work;
   - every scanner completes against a test environment;
   - progress increases per record;
   - **Open Component** opens the selected component;
   - Field Change Monitor still opens and operates normally.

## Publish to NuGet.org

Package publication is permanent for a given version. Confirm the final version before pushing.

```powershell
nuget push .\bin\Release\LucasVerissimo.XrmToolBox.DataverseUsageExplorer.1.0.3.nupkg `
  -Source https://api.nuget.org/v3/index.json `
  -ApiKey $env:NUGET_API_KEY
```

After upload, wait until the package is indexed and visible on NuGet.org. Preview its README, icon, metadata,
and package contents.

## Register in XrmToolBox Tool Library

After NuGet indexing, sign in to the XrmToolBox portal and register this package ID:

```text
LucasVerissimo.XrmToolBox.DataverseUsageExplorer
```

The XrmToolBox team reviews new tools before they become visible in Tool Library.

Official references:

- https://www.xrmtoolbox.com/documentation/for-developers/deploy-your-plugin-in-plugins-store/
- https://www.xrmtoolbox.com/documentation/for-developers/deploy-your-plugin-in-plugins-store/plugin-validation-check-list/
- https://learn.microsoft.com/nuget/nuget-org/publish-a-package
