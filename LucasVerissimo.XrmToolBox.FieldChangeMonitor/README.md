# Field Change Monitor

Field Change Monitor is an XrmToolBox tool for monitoring Dataverse table field changes with visual and
FetchXML filters, persistent history, and Windows alerts.

## Features

- Monitor one or more Dataverse tables at the same time.
- Select one or more fields to watch.
- Build FetchXML filter conditions with a visual condition builder.
- Edit and validate FetchXML and synchronize supported conditions with the visual builder.
- Pick valid lookup and choice values when building conditions.
- Create and edit monitors through a guided four-step wizard.
- Name, edit, pause, resume, and remove individual monitors.
- Manage monitors from a scrollable grid with individual and bulk actions.
- Persist monitor definitions and the latest collected snapshot by Dataverse environment.
- Restore saved monitors safely as paused when the tool opens.
- Track record counts and the latest query time for every monitor.
- Keep the 100 most recent changes per environment in a structured history grid.
- Export and import versioned `.fcm.json` monitor definitions.
- Open changed Dataverse records in the default browser after confirmation.
- Enable or disable Windows notifications without interrupting change collection.

## Requirements

- XrmToolBox 1.2025.7.71 or later.
- A Dataverse environment and an account that can read the monitored tables and columns.
- .NET Framework 4.8, supplied by the supported XrmToolBox installation.

## Getting started

1. Open **Field Change Monitor** in XrmToolBox and select a Dataverse connection.
2. Create a monitor and choose the target table.
3. Select the fields to watch.
4. Configure optional visual or FetchXML conditions.
5. Review the definition and start the monitor.
6. Use the monitoring and history pages to inspect changes.

Monitors are restored as paused. Resume them deliberately after reviewing the selected environment and
definition.

## Stored data and privacy

Definitions, preferences, snapshots, and recent history are stored locally by the tool. Dataverse data is
queried directly through the connection selected in XrmToolBox and is not sent to an external service.

Exported monitor definitions contain the configuration needed to recreate a monitor and do not include local
snapshots.

## Version 1.2.0.0

This release introduced redesigned navigation, a guided monitor wizard, a scrollable management grid,
individual and bulk actions, dynamic record counts, last-query timestamps, popup preferences, table search,
improved `ModifiedBy` name resolution, and dedicated history and configuration pages.

## Source and issues

Source code and issue tracking are available in the
[XrmToolBox tools repository](https://github.com/lucassverissimo-org/XrmToolBox.FieldChangeMonitor).

## License

MIT
