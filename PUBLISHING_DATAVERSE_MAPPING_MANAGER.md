# Publishing Dataverse Mapping Manager

Package ID: `LucasVerissimo.XrmToolBox.DataverseMappingManager`.
The current package version is `1.0.1`; the plugin assembly version is `1.0.1.0`.

## Prepare and validate

Run from the repository root, with the local XrmToolBox instance closed if it is using the build output:

```powershell
$notes = 'Fix Tool Library logo metadata: use only an explicit public PNG iconUrl, matching the registration-compatible packaging pattern. Add release validation to prevent embedded icon metadata from returning.'
.\scripts\publish-tool.ps1 `
    -ManifestPath .\LucasVerissimo.XrmToolBox.DataverseMappingManager\tool-release.json `
    -Version 1.0.1.0 `
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

## Logo metadata: required compatibility rule

Use only the public PNG URL in NuGet metadata:

```xml
<iconUrl>https://raw.githubusercontent.com/lucassverissimo-org/XrmToolBox.FieldChangeMonitor/main/LucasVerissimo.XrmToolBox.DataverseMappingManager/Assets/dataverse-mapping-manager-128.png</iconUrl>
```

Do not add an `<icon>` element, even alongside `iconUrl`. The historical fixes `03327c0` and
`008251f` removed embedded icon declarations after "Logo Url is not valid" registration failures.
Version 1.0.0 of Mapping Manager accidentally restored both declarations; 1.0.1 restores the URL-only pattern.
This repository intentionally favors Tool Library compatibility over the generic NuGet embedded-icon recommendation.
NU5048 may be emitted for this intentional compatibility choice. Do not address it by adding icon/PackageIcon; other warnings must still be investigated.

The public PNG and the plugin's small/large MEF icons remain in source. Validate the source AND the generated
package, not just a hard-coded URL string or a HEAD response:

```powershell
.\tests\tool-library-metadata-tests.ps1
.\scripts\test-tool-library-metadata.ps1 `
    -NuspecPath .\LucasVerissimo.XrmToolBox.DataverseMappingManager\LucasVerissimo.XrmToolBox.DataverseMappingManager.nuspec `
    -PackagePath .\bin\Release\LucasVerissimo.XrmToolBox.DataverseMappingManager.1.0.1.nupkg
```

The shared publication scripts run this check automatically. It rejects embedded declarations, checks source/package
metadata consistency, and validates the public image GET, PNG signature, content type, dimensions, and documentation URL.
These checks prevent the known metadata regression; only a successful portal registration confirms portal acceptance.

## Publish

Ensure the source, documentation, and icon URL are public on the repository's `main` branch. Set
`NUGET_API_KEY` securely in the publishing environment; never put its value in source control or release notes.
Run the same preparation command with `-Publish` to upload the validated package to NuGet.org.
A published version cannot be replaced; corrections need a new version.

After indexing, verify the package page:

https://www.nuget.org/packages/LucasVerissimo.XrmToolBox.DataverseMappingManager/1.0.1

## Register in Tool Library

Once NuGet indexing is complete, sign in to the XrmToolBox portal and register the package ID above.
Tool Library visibility depends on approval by the XrmToolBox administrators, separately from NuGet upload.

Official instructions:

- https://www.xrmtoolbox.com/documentation/for-developers/deploy-your-plugin-in-plugins-store/
- https://www.xrmtoolbox.com/documentation/for-developers/deploy-your-plugin-in-plugins-store/plugin-validation-check-list/
