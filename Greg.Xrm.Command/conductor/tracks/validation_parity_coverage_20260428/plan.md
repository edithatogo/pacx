# Implementation Plan: Validation, Parity & Coverage

## Overview
Expand validation so the repo checks real integrations, detects command/doc drift, and applies schema and contract validation to more of the command surface.

## Scope
- Integration coverage against live services for the highest-risk commands.
- Command parity validation for generated docs content and the live command registry, with help text, usage, options, and defaults still queued.
- Broader schema and contract validation beyond connector definitions.
- Regression coverage for the new validation surfaces.
- A `validate all` orchestration entry point that runs the core checks in one pass.

## Improvements
- Catch command drift before it reaches users.
- Reduce false confidence from unit-only coverage.
- Make failures easier to localize.
- Provide a single command for CI and local preflight validation.

## Success Criteria
- The important commands are covered by live-service integration tests.
- Parity checks compare command definitions against generated docs content, help text, and usage output.
- Schema and contract validation cover the major JSON-shaped payloads in the repo.
- `validate all` runs the core checks and returns a clear failure summary.

## Phases

### Phase 1: Integration coverage
- [ ] Task: Identify the highest-risk command surfaces for live-service tests.
- [ ] Task: Add integration tests for those commands.
- [ ] Task: Wire test gating so the suite can opt into live environments safely.

### Phase 2: Parity validation
- [ ] Task: Add a command metadata snapshot/parity check.
- [ ] Task: Compare defaults against command metadata and generated docs.
- [x] Task: Compare help text, usage, options, and generated docs against command metadata.
- [x] Task: Add regressions for detected drift.

### Phase 3: Schema and contract expansion
- [x] Task: Extend schema validation to other JSON-shaped payloads and manifests.
- [x] Task: Add contract validation for command payloads that currently fail late.
- [x] Task: Add tests for invalid and borderline shapes.

### Phase 4: Orchestration
- [x] Task: Add a `validate all` entry point for the core checks.
- [x] Task: Define output and exit-code behavior for aggregated validation failures.
- [x] Task: Add docs and examples for CI and local use.
- [x] Task: Add a `validate all` entry point for generated command reference parity.
