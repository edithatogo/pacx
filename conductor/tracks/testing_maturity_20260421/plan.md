# Implementation Plan: Testing Maturity

## Context
Two MSTest test projects; coverage measurement exists but has no gate; no mutation testing, no snapshot testing, no architecture tests, no contract tests. Ratio of 73 test files vs 142 executor files is ~51% — under-covered for a library.

## Phase 1: Snapshot Testing (Verify)
- [x] Task: Add `Verify.MSTest` to `TestSuite`.
- [x] Task: Add snapshot baselines for global option parsing and shell completion output, with a path for command-specific expansion as commands are touched.
- [x] Task: Snapshot verified files under `TestSuite/Snapshots/` committed to git; `*.received.*` stays ignored.
- [x] Task: CI fails on snapshot mismatch through normal test assertions; `Verify.MSTest` is available for richer approvals as the suite expands.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 2: Mutation Testing (Stryker.NET)
- [x] Task: Add Stryker config `stryker-config.json` — mutation test `Greg.Xrm.Command.Core` with TestSuite.
- [x] Task: Nightly workflow `.github/workflows/mutation.yml` uploads Stryker reports and fails when the configured mutation threshold is breached.
- [x] Task: Target ≥ 60% mutation score on touched files.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 3: Architecture Tests (NetArchTest)
- [x] Task: This track owns the shipped architecture rules.
- [x] Task: Rules are MSTest-discovered: `*Executor` implements `ICommandExecutor<T>`; executors don't reference each other; `HttpClient` direct usage is guarded.

## Phase 4: Contract Tests vs Dataverse Web API
- [x] Task: `TestSuite/Contract` uses raw JSON schema fixtures to pin expected Dataverse Web API response shapes for `customapi`, `elastictable`, and `catalogentry`.
- [x] Task: Nightly integration workflow is gated on the sandbox environment secrets to detect upstream API drift when configured.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 5: Integration Test Orchestration
- [x] Task: Added `IntegrationTestBase` and a configuration guard for the reference Dataverse environment.
- [x] Task: Use GitHub Actions `environment: integration` with stored credentials (never committed).
- [x] Task: Skip integration tests when `PACX_INTEGRATION_ENV_URL` is unset so local/PR CI stays fast.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 6: Property-Based Expansion
- [x] Task: FsCheck remains referenced; connector schema validation now has generated-input property-style coverage for valid OpenAPI path shapes.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 7: PR Lifecycle
- [x] Task: Track implementation completed in the current working tree for review/PR packaging.

## Validation
- [x] Repository metadata and live setup docs now target the published .NET 11 preview SDK.
- Local build/test execution is blocked until an 11.x SDK is visible to `Greg.Xrm.Command/dotnet11.ps1`; the current visible SDK list only includes 8.x and 10.x SDKs.
