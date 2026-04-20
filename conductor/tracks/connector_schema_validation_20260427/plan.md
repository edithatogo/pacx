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