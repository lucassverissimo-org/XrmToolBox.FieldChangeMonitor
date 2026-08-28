# Solution Layer Analyzer

Solution Layer Analyzer is an XrmToolBox tool for comparing the composition of a Dataverse solution between a
Source and a Target environment and diagnosing active unmanaged customizations.

## Main workflow

1. Use the current XrmToolBox connection as Source.
2. Connect a second environment as Target.
3. Load and select a Source solution. The matching Target solution is resolved by `uniquename`.
4. Run the incremental analysis. The Target solution is optional; Target component existence and layers are
   still inspected when it is missing.
5. Filter or export the results, then explicitly select Active Layers for removal.
6. Optionally create a selected-component unmanaged solution backup.
7. Confirm the destructive action. If backup is not confirmed, two explicit risk confirmations are required.
8. Remove the selected active customizations and re-query their layers.

## Analysis strategy

- `solutioncomponent` supplies the Source and optional Target composition.
- Components are correlated by `componenttype + objectid`; the resolver boundary allows future type-specific
  identities.
- Target environment existence is queried independently from Target solution membership.
- `msdyn_componentlayer` requests are restricted by exact component ID and component type and grouped inside
  `ExecuteMultipleRequest` batches. The default batch size is 100 with maximum parallelism 2.
- Retries are limited, `Retry-After` is honored when exposed by the SDK fault, and timeout batches are split
  down to a conservative minimum size.

## Initially supported removal types

Entity, Attribute, Relationship, Option Set, Form, System Form, Saved Query, Workflow, Web Resource, Site Map,
and Canvas App. Unknown component types remain visible in analysis and export; removal is attempted only when
a valid component logical name can be resolved.

## Backup limitation

The backup creates a temporary unmanaged solution in Target, adds only selected components without required
components or subcomponents where supported, exports the ZIP, and removes the temporary solution. Dataverse
may reject some component types or dependency shapes. A partial or failed backup is clearly reported and does
not block an explicitly confirmed removal.

Removing active unmanaged customizations can permanently discard changes. Validate the generated backup and
test the tool in a non-production environment before production use.
