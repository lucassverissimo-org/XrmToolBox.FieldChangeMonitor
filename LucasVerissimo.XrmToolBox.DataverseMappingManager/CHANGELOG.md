# Changelog

## 1.0.1

- Fix Tool Library registration metadata by retaining only the public PNG `iconUrl`, without an embedded `icon` declaration.
- Enforce the registration-compatible metadata pattern in publication validation and document it for future tools.

## 1.0.0

Initial public release.

- Browse directed table maps and compare source columns, destination columns, and existing mappings.
- Validate compatibility and stage mapping additions, edits, and removals before saving.
- Review changes and per-operation results, with transactional writes and publication status.
- Refresh column and mapping lists independently while protecting pending work from concurrent changes.
- Add selected mapping dependencies to unmanaged solutions and verify mapping presence in exported packages.
- Use a guided interface with searchable lists, checkbox selection, and a dedicated mapping editor.

Mappings provide initial values for related record creation; they do not synchronize existing records.
Export verification does not confirm an import into another environment or propagate deletions there.
