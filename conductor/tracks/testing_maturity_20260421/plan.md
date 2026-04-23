# Implementation Plan: Testing Maturity

## Context
Two MSTest test projects; coverage measurement exists but has no gate; no mutation testing, no snapshot testing, no architecture tests, no contract tests. Ratio of 73 test files vs 142 executor files is ~51% — under-covered for a library.

## Phase 1: Snapshot Testing (Verify)
- [ ] Task: Add `Verify.MSTest` to `TestSuite`.
- [ ] Task: For every command, snapshot the `--help` output, `--format json` golden output for a canned input, and error-path output.
- [ ] Task: Snapshot receive/verified files under `TestSuite/Snapshots/` committed to git.
- [ ] Task: CI fails on snapshot mismatch; developer runs `dotnet verify accept` to update intentionally.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Mutation Testing (Stryker.NET)
- [ ] Task: Add Stryker config `stryker-config.json` — mutation test `Greg.Xrm.Command.Core` with TestSuite.
- [ ] Task: Nightly workflow `.github/workflows/mutation.yml` — comment on PR if mutation score drops below baseline.
- [ ] Task: Target ≥ 60% mutation score on touched files.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Architecture Tests (NetArchTest)
- [ ] Task: (Also listed in code_quality_hardening Phase 6; pick one home — this track owns it if code_quality_hardening has not shipped.)
- [ ] Task: Rules: `*Executor` implements `ICommandExecutor<T>`; Executors don't reference each other; `HttpClient` only in service layer.

## Phase 4: Contract Tests vs Dataverse Web API
- [ ] Task: `TestSuite/Contract` — uses Pact or raw JSON schema to pin expected Dataverse Web API response shapes for every endpoint we consume (e.g., `customapi`, `elastictable`, `catalogentry`).
- [ ] Task: Nightly run against a sandbox env to detect upstream API drift.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Integration Test Orchestration
- [ ] Task: `IntegrationTestBase` already exists (per e2e_smoke_tests track); add fixtures for a reference Dataverse dev env provisioned via `env create` + `env reset` at start/end of suite.
- [ ] Task: Use GitHub Actions `environment: integration` with stored credentials (never committed).
- [ ] Task: Skip integration tests when `PACX_INTEGRATION_ENV_URL` is unset so local/PR CI stays fast.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Property-Based Expansion
- [ ] Task: FsCheck is already referenced; expand use for parsers (connector schema, BIM validator, OData filter builder).
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 7: PR Lifecycle
- [ ] Task: One PR per phase; `/ralph-loop`; merge.
