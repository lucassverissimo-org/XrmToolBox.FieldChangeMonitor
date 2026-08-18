# Field Change Monitor for XrmToolBox

Field Change Monitor is an XrmToolBox tool for monitoring Dataverse table field changes with FetchXML filters and Windows alerts.

## Features

- Monitor one or more Dataverse tables at the same time.
- Select one or more fields to watch.
- Build FetchXML filter conditions with a visual condition builder.
- Edit and validate FetchXML and synchronize supported conditions back to the visual builder.
- Pick valid lookup and option set values when building conditions.
- Name, edit, pause, resume, and remove individual monitors.
- Persist monitor definitions and the last collected snapshot per Dataverse environment.
- Restore all monitors safely as paused when the tool opens.
- Reconcile the saved snapshot with current Dataverse values when a monitor resumes, recording changes collected while it was paused or closed.
- Export and import versioned `.fcm.json` monitor definitions with Dataverse metadata validation.
- Keep and restore the 100 most recent changes per environment in a structured grid with ModifiedOn, ModifiedBy, record ID, event, field, and old/new values.
- Open changed Dataverse records in the default browser after confirmation.
- Receive Windows notifications when monitored values change.
- Create and edit monitors through a guided four-step wizard.
- Search Dataverse tables by display name or logical name.
- Manage monitors from a scrollable grid with individual and bulk actions.
- Track the current record count and last query time for every monitor.
- Disable Windows popups without disabling change collection.

## Version 1.2.0.0

This release introduces a redesigned navigation experience and a guided four-step wizard
for creating and editing monitors. It also adds a scrollable monitor management grid with
individual and bulk actions, dynamic record counts, last-query timestamps, popup preferences,
table search, improved ModifiedBy name resolution, and configuration/history pages. Exported
monitor definitions no longer include local snapshots.

## Packaging

Use the release script to update versions, build, package, validate the NuGet contents,
check version availability, and optionally publish:

```powershell
# Prepare and validate without publishing
.\publish-release.ps1 -Version 1.2.0.0 `
  -ReleaseNotesFile .\release-notes.txt

# Publish after reviewing the generated package
$env:NUGET_API_KEY = "your-nuget-api-key"
.\publish-release.ps1 -Version 1.2.0.0 `
  -ReleaseNotesFile .\release-notes.txt `
  -Publish
Remove-Item Env:NUGET_API_KEY
```

`-Publish` asks you to type the version as confirmation. In CI, use `-Publish -Yes`.
The API key is read only from `NUGET_API_KEY`; it is never stored in the repository.
