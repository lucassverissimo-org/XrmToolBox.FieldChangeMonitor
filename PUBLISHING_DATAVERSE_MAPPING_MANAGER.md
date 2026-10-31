# Publishing Dataverse Mapping Manager

Package ID: `LucasVerissimo.XrmToolBox.DataverseMappingManager`.
The initial package version is `1.0.0`; the plugin assembly version is `1.0.0.0`.

## Prepare and validate

Run from the repository root, with the local XrmToolBox instance closed if it is using the build output:

```powershell
$notes = 'Initial release: guided mapping review and editing, compatibility validation, staged transactional changes, publication results, independent list refresh, and selected solution components with export verification.'
.\scripts\publish-tool.ps1 `
    -ManifestPath .\LucasVerissimo.XrmToolBox.DataverseMappingManager\tool-release.json `
    -Version 1.0.0.0 `
    -ReleaseNotes $notes

dotnet build .\tests\DataverseMappingManager.Tests\DataverseMappingManager.Tests.csproj -c Debug
.\bin\Debug\DataverseMappingManager.Tests.exe
dotnet build .\tests\DataverseMappingManager.Tests\DataverseMappingManager.Tests.csproj -c Release
.\bin\Release\DataverseMappingManager.Tests.exe
```

The preparation script checks formatting, builds Debug and Release, creates the package, validates its
contents, and prints its SHA256 hash. The package contains exactly one plugin DLL under `lib/net48/Plugins`,
with Shared Project code compiled into that DLL. The tests also check WinForms designer loading.

Before publication, verify the connected workflow in a test environment: table selection, creation/editing/
removal, publication, solution selection and export verification. Automated tests use a fake Dataverse
service and do not replace connected testing or an export/import test in a separate environment.

## Publish

Ensure the source, documentation, and icon URL are public on the repository's `main` branch. Set
`NUGET_API_KEY` securely in the publishing environment; never put its value in source control or release notes.
Run the same preparation command with `-Publish` to upload the validated package to NuGet.org.
A published version cannot be replaced; corrections need a new version.

After indexing, verify the package page:

https://www.nuget.org/packages/LucasVerissimo.XrmToolBox.DataverseMappingManager/1.0.0

## Register in Tool Library

Once NuGet indexing is complete, sign in to the XrmToolBox portal and register the package ID above.
Tool Library visibility depends on approval by the XrmToolBox administrators, separately from NuGet upload.

Official instructions:

- https://www.xrmtoolbox.com/documentation/for-developers/deploy-your-plugin-in-plugins-store/
- https://www.xrmtoolbox.com/documentation/for-developers/deploy-your-plugin-in-plugins-store/plugin-validation-check-list/
