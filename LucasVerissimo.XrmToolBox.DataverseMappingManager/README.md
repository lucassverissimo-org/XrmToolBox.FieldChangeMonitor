# Dataverse Mapping Manager

XrmToolBox plugin for reviewing and maintaining native Dataverse column mappings between related tables.
Mappings provide initial values when creating related records; they do not synchronize existing records.

## Workflow

The interface follows **Tabelas → Mapear campos → Revisar e salvar**, with compact rows and standard system colors. Returning to Tabelas preserves pending work until another pair is loaded. Solution inclusion, publication, and on-demand help are grouped under **Mais ações**. The table catalog is loaded from step 1; each workspace list has its own refresh button.

1. Connect to Dataverse and select **Carregar tabelas**.
2. Choose the **source** table, then a supported **destination** table. The selectors show only directed pairs
   with an existing `entitymap`. A relationship alone does not guarantee mapping support.
3. Select **Continuar para mapeamentos** to load the three searchable lists: source columns, destination columns, mappings.
4. Check a column's checkbox to emphasize compatible columns and existing links in the other list. Each list
   permits one checked item. In the source and destination lists, checked items appear first, followed by
   unchecked items sorted by logical name. Mappings always sort by source logical name, then target,
   independently of their checkbox state. Column grids display logical name before display name.
   Clicking a row only changes keyboard focus. Check a mapping to check/reveal its
   source and destination. The selection summary retains checked identities even when a filter hides a row.
   Checking an origin/destination pair that already has an active mapping automatically checks that mapping
   in the third list and reveals it if its search was hiding it. An unmatched pair clears an automatic mapping
   check; a mapping explicitly checked for editing stays selected while replacement columns are chosen.
   **Limpar seleções** clears all three selections. **Somente compatíveis** filters the list opposite the selected column.
5. Check source and destination columns and choose **Criar mapeamento**. To edit, first check a mapping,
   select **Alterar**, choose the replacement source/destination in the modal, and select **Aplicar ajuste**. The editor starts with the current pair and validates compatibility and occupied destinations; cancelling leaves it unchanged. **Remover** marks an
   existing mapping for deletion; removing an unsaved addition cancels it. Nothing is written yet.
6. **Desfazer** restores the original mapping; **Descartar ajustes** discards all local changes.
7. **Revisar e salvar** opens a grid with action, original/current columns, state, progress, and details, plus
   counts of removed, changed, and added mappings. **Salvar** keeps the modal open while applying changes and
   attempting publication. Every executing mapping has its own animated progress bar. Dependent changes run
   together and their bars reflect the shared transaction; no invented percentage is displayed. The modal
   cannot be closed during writes. Both the review/result and solution modals use the application icon.
8. Read the per-operation results in the same grid, then choose **Fechar**. Confirmed failures remain pending for correction and retry. Successfully
   saved changes are not submitted again. **Publicar tabelas** retries publication without repeating writes.
9. In the save modal, **Adicionar à solution** becomes enabled after at least one mapping is saved and
   publication is confirmed. It opens solution selection without closing the save results or repeating writes.
   If publication fails or every mapping fails, the button stays disabled. The main screen's **Mais ações → Adicionar à solution** remains available for existing mappings and retries. Closing the save modal no longer opens an
   automatic solution prompt.

The workspace title/status identifies the loaded table pair even if a different pair is chosen in the selectors.
Source/destination refresh reads only that table's metadata and preserves staged work. Mapping refresh reads only server mappings. When pending changes exist and the baseline is unchanged, it preserves those changes; concurrent differences or an uncertain previous save block replacement and explain how to reload safely. Refresh never publishes or writes to Dataverse. Publication state and session solution selections are preserved.

The loaded workspace changes only after **Continuar para mapeamentos** succeeds.

## Validation and safeguards

- Compatibility checks include column type, text format/length, date behavior, integer format, lookup targets,
  choice integer values, derived columns, and whether the target accepts input during creation.
- A destination may have only one active mapping in the table map. A source may have several destinations.
- System and managed mappings are visible but protected from modification/removal.
- Colors have text equivalents in the **Situação**, **Ajuste**, and **Restrição** columns.
- Form placement is not inferred from metadata. Verify that mapped columns are present on the appropriate forms;
  choices should preferably share a global choice. The server is the final authority for mapping support.
- Server mappings and column metadata are checked again before saving. Detected concurrent changes require reload.
  This is a preflight check, not a lock against changes made during the request itself.
- Edits delete/recreate the attribute map inside a transaction. Changes sharing original/new destinations are
  committed as one group, supporting replacements/swaps without leaving half-applied edits. Independent groups
  can succeed or fail separately. Replacement mappings receive new IDs.
- A confirmed Dataverse fault rolls back its transaction. If a response is lost, the tool cannot know whether the
  server committed; it stops further writes and automatic publication, and requires reloading before another attempt.
  After reloading, use **Publicar tabelas** if publication is needed; this action appears under **Mais ações** when publication is pending.
- Publication is reported separately from saving. Publishing the two tables also publishes other pending
  customizations on those tables; this is disclosed in the confirmation dialog.
- Closing the plugin and replacing the loaded workspace warn about pending changes/publication. A connection
  change clears environment-specific state and warns when changes/publication were pending.

## Solutions

Only visible unmanaged solutions other than system Active/Default solutions are offered. The modal has three tabs:

- **Mapeamentos a transportar** lists all loaded active mappings, regardless of filters or checkbox state in the
  main screen. Successful additions and edits in the current workspace session start checked; other saved
  mappings start unchecked but can be selected, including mappings from previous sessions or other users.
  Unsaved/failed additions and edits are unavailable with **salve primeiro**. Search preserves hidden checks.
  **Marcar todos disponíveis** selects saved active mappings; **Limpar seleções** clears every tab.
- **Componentes necessários** is calculated from the checked mappings: both table shells, existing relationships
  for the directed pair, and only the columns involved in selected links/checks. Shared columns are deduplicated.
  Unchecking a mapping removes dependencies that no longer belong to another selection. Tables are added without
  all subcomponents; automatic required-component expansion is disabled. `EntityMap` (46) and `AttributeMap` (47)
  are not submitted individually to `AddSolutionComponent`.
- **Remoções** separates locally pending removals from saved removals, including the old side of a saved edit.
  Pending removals are unavailable. Saved-removal absence checks are optional and start unchecked. These checks
  do not execute or prove any deletion in another environment.

After adding supported components, the tool exports the selected solution as unmanaged, reads `customizations.xml`
in memory, and checks `EntityMaps/EntityMap/AttributeMaps` for each selected exact table/column pair. Only a match
receives **Verificado no pacote**. Missing mappings and denied/failed exports are **Não confirmado**, independently
of successful table/column inclusion. Export privileges are required. No archive is rewritten, saved or imported.
When an omitted mapping carries the Dataverse `issystem` or `ismanaged` flag, it receives a specific
**Nativo não exportado** or **Gerenciado não exportado** warning, not a component-addition failure. Its absence
never confirms availability in another environment. System/managed origin is retained separately from edit
protection and displayed explicitly; it is not inferred from a column name. An actual XML match still verifies
a system/managed mapping when one is exported. Warnings and component failures have separate totals.

An unmanaged export check does not validate managed export or successful import elsewhere. The selected mappings
are the requested/verified links, not an exclusive export filter; Dataverse may export other links as well.

Load a table pair and open **Adicionar à solution** to select existing mappings without resaving them. Availability
and defaults use committed snapshot records and successful session export expectations, never just a checked row
or an unsaved addition. Reloading clears session history, so existing mappings then start unchecked.

For a managed removal, remove the mapping in the development environment of the solution that delivered it and
apply a new managed version as an upgrade, after reviewing dependencies/layers. A normal update does not delete
components. An unmanaged removal needs an explicit, reviewed deletion in the destination. System-generated
mappings cannot be deleted individually. An absent XML element is only a warning, not deletion transport proof.
This tool does not apply solution upgrades or perform downstream deletions.

## Development and validation

Target: .NET Framework 4.8. Open `LucasVerissimo.XrmToolBox.slnx` in Visual Studio. All visual classes have normal
`.cs`, `.Designer.cs`, and `.resx` files. The standalone plugin imports Shared source and references no other tool.

```powershell
dotnet tool restore
dotnet csharpier check .
dotnet build .\LucasVerissimo.XrmToolBox.slnx -c Debug
dotnet build .\LucasVerissimo.XrmToolBox.slnx -c Release
.\bin\Debug\DataverseMappingManager.Tests.exe
.\bin\Release\DataverseMappingManager.Tests.exe
.\build-dataverse-mapping-manager-package.ps1
.\tests\dataverse-mapping-manager-release-tests.ps1 -SkipBuild
```

The executable regression suite uses a fake organization service to exercise compatibility, rollback, partial
success, retry, conflicts, uncertain responses, scoped solution requests, UI selection/editing, and WinForms
DesignSurface loading. It writes `bin/<configuration>/mapping-manager-preview.png` for visual inspection.
These checks do not replace a connected Dataverse smoke test with real privileges, relationships, solution
layers, and export/import. No environment credentials or live Dataverse writes are used by the tests.

To test in the application, run `run-dataverse-mapping-manager.ps1` (Release by default), or pass
`-Configuration Debug`. The script builds the plugin, copies it into the selected output's `Plugins` folder,
and launches XrmToolBox with an absolute `/overridepath:"<output folder>"` and `/plugin:"Dataverse Mapping Manager"`. Tools, connections,
logs and settings stay in that output folder, separate from the normal roaming profile. Pass `-PrepareOnly`
to prepare the folder without launching; `-SkipBuild` reuses an existing build.

After preparation, close the existing local XrmToolBox instance and open
`bin/Release/Start-DataverseMappingManager.cmd` (or the Debug equivalent). This launcher prepares the plugin
on every run and uses an absolute storage path; a relative override can resolve differently during startup. Opening `XrmToolBox.exe` directly
uses its default storage location and may not show the locally built tool.

Packaging creates exactly one
plugin DLL under `lib/net48/Plugins`; its version matches the NuGet version. Tool Manager discovers the independent
release through `tool-release.json`. The packaging script does not publish to NuGet.
