# LucasVerissimo.XrmToolBox.Tests

Central test project for all tools in this repository. Tests are organized by owning tool and share only
general-purpose test doubles, integration infrastructure, and helpers.

## Unit tests

Unit tests are local, deterministic, and do not connect to Dataverse:

```powershell
dotnet test .\LucasVerissimo.XrmToolBox.Tests.csproj --filter "TestCategory=Unit"
```

## Real Dataverse integration tests

1. Fill the ignored `local.settings.json` file using `local.settings.example.json` as a reference.
2. Configure `SourceConnectionString` for QA - Serv, `TargetConnectionString` for Edp preprod, and
   `SolutionUniqueName` as `CRMRelease63`.
3. Run only the integration category:

```powershell
dotnet test .\LucasVerissimo.XrmToolBox.Tests.csproj --filter "TestCategory=Integration"
```

The `XRMTOOLBOX_TEST_SETTINGS` environment variable can point to another settings file when needed. Connection
strings are never written to test output. If the local file is absent or incomplete, integration tests are skipped.

Real-environment tests receive an `IOrganizationService` wrapper that permits only `Retrieve` and
`RetrieveMultiple`. It rejects `Create`, `Update`, `Delete`, `Associate`, `Disassociate`, and every `Execute`
request, including `RemoveActiveCustomization`.

## Organization rules

- Tool-specific fixtures stay under the matching tool folder.
- Reusable `IOrganizationService` fakes stay under `TestDoubles/Dataverse`.
- Real-connection infrastructure stays under `Integration`.
- Automated integration tests must remain read-only.
