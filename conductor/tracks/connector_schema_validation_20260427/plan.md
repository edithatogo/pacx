# Implementation Plan: Connector Schema Validation

## Overview
Validate connector definition files against OpenAPI schema before import/test/export to provide fast, clear feedback.

## Scope
- Support JSON and YAML connector definition files.
- Validate structure, required fields, and basic OpenAPI constraints.
- Report precise errors with line/column and suggestions.
- Integrate into `connector import`, `connector validate`, and CI scenarios.

## Improvements
- Fail-fast before any API calls.
- Reduce debugging time for developers.
- Enforce organizational schema rules optionally.

## Success Criteria
- `pacx connector validate --file ./def.json` returns clear validation result.
- Validation errors include path and suggestion.
- Optionally support an organization-specific schema version.

## Next Steps
1. Choose a validator (e.g., ajv for OpenAPI 3.x).
2. Define the minimal schema subset required by our connector model.
3. Add CLI flag `--schema-file` for custom rules.
4. Add unit tests for valid/invalid definitions.

---

## Phases (task decomposition, added 2026-04-21)

### Phase 1: Validator selection
- [ ] Task: Evaluate NuGet options: `Microsoft.OpenApi.Readers` (parse), `JsonSchema.Net` (validate), `YamlDotNet` (YAML parse → JSON).
- [ ] Task: ADR in `docs/adr/` capturing decision.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 2: Core validator
- [ ] Task: `ConnectorSchemaValidator` class: `ValidationResult Validate(string fileContent, ConnectorFormat format, string? customSchemaPath = null)`.
- [ ] Task: Errors include file path, line/column (from YAML parser), JSON pointer, suggestion.
- [ ] Task: Unit tests with a matrix of valid + invalid fixtures in `TestSuite/Fixtures/Connectors/`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 3: CLI surface
- [ ] Task: `pacx connector validate --file <path>` — format auto-detected from extension.
- [ ] Task: `pacx connector validate --file <path> --schema-file <path>` — custom org schema overlay.
- [ ] Task: Exit code 4 (ValidationError) on failure — aligns with `cli_ux_20260421` taxonomy.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 4: Integration with import/test
- [ ] Task: `connector import` calls the validator first; short-circuits on failure before any API call.
- [ ] Task: `connector test` validates the definition file before invoking the backend.
- [ ] Task: Tests.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 5: CI helper
- [ ] Task: Recipe `docs/recipes/validate-connectors-in-ci.md` — GitHub Action snippet running `pacx connector validate` across a folder.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 6: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.