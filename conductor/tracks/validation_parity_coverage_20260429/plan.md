# Implementation Plan: Validation Parity & Coverage

## Overview
Achieve validation parity and expand test coverage — ValidateAll, schema/contract validation, integration coverage.

## Scope
- ValidateAll orchestration for running all validations.
- Schema validation for Dataverse contracts.
- Contract validation for command inputs/outputs.
- Command reference parity — every command has at least one test.
- Integration test coverage for high-risk commands.

## Dependencies
- `testing_maturity_20260421` — testing infrastructure and patterns.

## Success Criteria
- `pacx validate all` runs all validations and produces a report.
- Schema validation catches contract violations.
- Every command has at least one unit test (command reference parity).
- Integration tests cover all CRUD command groups.
- CI fails on validation errors.

## Phases

### Phase 1: ValidateAll orchestration (DONE)
- [x] Task: `pacx validate all` command that orchestrates available validators.
- [x] Task: `ValidateAllExecutor` — runs schema, contract, and reference validators.
- [x] Task: ValidateAll report output (pass/fail per validator).
- [x] Task: Tests.

### Phase 2: Schema/contract validation (DONE)
- [x] Task: Dataverse contract schema definitions.
- [x] Task: Schema validation service for command inputs.
- [x] Task: Contract validation for command outputs.
- [x] Task: Tests.

### Phase 3: Command reference parity (DONE)
- [x] Task: Inventory all commands without tests.
- [x] Task: Create boilerplate test templates.
- [x] Task: Fill test coverage to at least one test per command.
- [x] Task: Tests.

### Phase 4: Integration coverage (DONE)
- [x] Task: Identify high-risk command groups for integration tests.
- [x] Task: Extend IntegrationTestBase patterns to remaining groups.
- [x] Task: Integration tests for solution, env, and connector commands.
- [x] Task: Integration tests for auth and connection lifecycle.
- [x] Task: Integration tests for forms, Power BI, Fabric commands.
- [x] Task: CI integration test gating.

