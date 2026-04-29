# ADR: Connector Schema Validation Baseline

## Status
Accepted

## Context
Connector import, validate, and test commands need fast feedback before they call Dataverse or backend APIs. The repository now targets .NET 11, so the first validation layer should avoid adding a large schema runtime until the command scenarios require full JSON Schema or OpenAPI semantic validation.

## Decision
Use a dependency-light structural validator for JSON connector definitions. It validates JSON syntax, root object shape, OpenAPI or Swagger version fields, `info.title`, and non-empty `paths`. The `connector validate` command also accepts `--schema-file` for organization-specific root-property requirements.

## Consequences
- Invalid connector definitions fail before Dataverse connection or backend token acquisition.
- Validation failures map to the CLI validation exit-code taxonomy.
- Full OpenAPI semantic validation, YAML parsing, and richer JSON pointer diagnostics remain future enhancements.
