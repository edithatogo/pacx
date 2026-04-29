# Implementation Plan: Connector Schema Validation

## Overview
Validate connector definitions against a JSON/OpenAPI structural schema before `connector validate`, `connector import`, and `connector test` proceed to Dataverse or backend calls.

## Scope
- Support JSON connector definition files.
- Validate structure, required fields, and basic OpenAPI constraints.
- Report clear validation errors and warnings through the CLI output channel.
- Integrate into `connector validate`, `connector import`, and `connector test`.

## Improvements
- Fail-fast before any API calls.
- Reduce debugging time for developers.
- Keep organizational schema rules as a future overlay.

## Success Criteria
- `pacx connector validate --file ./def.json` returns clear validation result.
- Invalid connector import files fail before a Dataverse connection is requested.
- Invalid stored connector definitions fail before `connector test` requests tokens or invokes the backend.

## Next Steps
1. Add YAML parsing if connector definitions need non-JSON support.
2. Add `--schema-file` if organization-specific connector policy rules are required.
3. Add richer JSON-pointer and line/column diagnostics when the parser stack supports it.

## Validation Snapshot (2026-04-28)

Validated under local .NET SDK 10.0.202 via `Greg.Xrm.Command/dotnet10.ps1` before the repository moved to .NET 11. The track now targets the published .NET 11 preview SDK.

- Reusable `ConnectorSchemaValidator` layer is present.
- `connector validate`, `connector import`, and `connector test` are wired through the validator.
- `connector import` short-circuits before connecting to Dataverse on structural errors.
- `connector test` validates the stored connector definition before token acquisition/backend calls.
- Focused connector tests pass: `dotnet10.ps1 test ... --no-build --filter "FullyQualifiedName~Connector"` - 17 passed.
- Focused test-suite build passes: `dotnet10.ps1 build ... --no-restore --verbosity minimal -p:UseSharedCompilation=false -p:BuildInParallel=false`.

Remaining scope for later phases:
- YAML parsing and richer OpenAPI validation.
- `--schema-file` custom rule overlay.
- CI recipe for validating folders of connector definitions.

---

## Phases (task decomposition, added 2026-04-21)

### Phase 1: Validator selection
- [x] Task: Use a dependency-light JSON structural validator for the first implementation.
- [x] Task: ADR in `docs/adr/` capturing future parser/schema package decision.
- [x] Task: Run local focused validation.

### Phase 2: Core validator
- [x] Task: `ConnectorSchemaValidator` class with `Validate(string fileContent)`.
- [x] Task: Errors and warnings cover invalid JSON, required root structure, OpenAPI version, `info.title`, and `paths`.
- [x] Task: Unit tests with valid, warning-only, invalid-structure, and invalid-JSON definitions.
- [x] Task: Run local focused validation.

### Phase 3: CLI surface
- [x] Task: `pacx connector validate --file <path>`.
- [x] Task: `pacx connector validate --file <path> --schema-file <path>` custom org schema overlay.
- [x] Task: Exit code 4 (ValidationError) alignment with `cli_ux_20260421` taxonomy.
- [x] Task: Run local focused validation.

### Phase 4: Integration with import/test
- [x] Task: `connector import` calls the validator first; short-circuits on failure before any API call.
- [x] Task: `connector test` validates the stored connector definition before invoking the backend.
- [x] Task: Tests.
- [x] Task: Run local focused validation.

### Phase 5: CI helper
- [x] Task: Recipe `docs/recipes/validate-connectors-in-ci.md` — GitHub Action snippet running `pacx connector validate` across a folder.
- [x] Task: Review pass completed locally; track ready for PR packaging.

### Phase 6: PR Lifecycle
- [x] Task: Working-tree implementation completed for upstream PR packaging.
