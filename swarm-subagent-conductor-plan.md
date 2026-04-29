# Plan: Conductor Backlog Subagent Execution

**Generated**: 2026-04-27

## Overview

Set up the full conductor track set to run through subagents in dependency-aware waves until every non-complete track is complete, blocked with accepted rationale, or archived. This plan defines assignment, validation, integration, and closure gates so multiple agents can run safely without editing the same files at the same time.

The executable queue is now `swarm-track-completion-manifest.json`. The previous `pacx-conductor-backlog` setup was partial: it covered the newer backlog tracks but missed older pending/in-progress tracks such as `solution_tests_20260408`, `pr_lifecycle_20260409`, `pr146_integration_20260408`, and `resolve_issues_20260408`. The active mission is now `pacx-all-tracks-completion`.

## Prerequisites

- Use a clean mission branch or isolated worktrees before launching implementation agents.
- Each worker must own a disjoint file/module scope for the duration of a task.
- Each worker must update only its assigned conductor track plan after validation.
- A validator must review every worker output before integration.
- External dependency/API docs must be fetched by each worker immediately before implementing API-specific work. Context7 is preferred; if unavailable, use official vendor docs.
- `Integration_Lead` must resolve or record the local .NET SDK/workload resolver blocker before any implementation track is finally marked complete.
- `Mission_Lead` must keep `swarm-track-completion-manifest.json` aligned with queue state.

## All-Track Completion Queue

The active queue is the `active_track_queue` array in `swarm-track-completion-manifest.json`. It includes:

- New 2026-04-27 tracks: correlation ID, connector schema validation, AI Builder improved flow, AI wrapper service.
- New 2026-04-21 tracks: CI/CD hardening, developer experience, documentation site, CLI UX, testing maturity, Fabric, Power BI, desktop flows, Copilot Studio, Power FX, Dataverse gaps phase 2, Forms advanced, performance/AOT.
- Older unresolved tracks: solution tests, CI/CD tests, code quality, PR lifecycle, PR146 integration, issue resolution, archived explore branches, archived MCP server.

Already completed tracks remain in `already_complete_but_reconcile` and should only be touched if metadata contradicts `conductor/tracks.md`.

## Dependency Graph

```text
T0 ──┬── T1 ──┬── T2 ── T3 ──┬── T4
     │        │              └── T5
     │        ├── T6
     │        ├── T7 ── T8
     │        ├── T9
     │        ├── T10
     │        ├── T11
     │        ├── T12
     │        ├── T13
     │        ├── T14
     │        ├── T15
     │        ├── T16
     │        └── T17
     │        └── T18
     └── T19 ── T20
```

## Tasks

### T0: Prepare Swarm Mission Baseline
- **depends_on**: []
- **location**: `subagents.yaml`, `.swarm/config.json`, `.swarm/missions/`, `task_plan.md`, `conductor/tracks.md`
- **description**: Create a new mission definition for the conductor backlog. Record current dirty worktree state, active track statuses, and ownership rules. Explicitly close, archive, or supersede the stale/narrow `.swarm` mission records before launching the backlog mission so agent tooling does not confuse old `Junior`/`Quality_Validator` state with the new run.
- **validation**: Mission file lists all agents, track ownership, validation gates, communication rules, stale mission handling, and no unrelated repo changes are reverted.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T1: Create Shared Agent Prompts And Rules
- **depends_on**: [T0]
- **location**: `subagents.yaml`, `task_plan.md`
- **description**: Define reusable prompts for `Track_Worker`, `Track_Validator`, `Integration_Lead`, and optional domain workers. Prompts must state that agents are not alone in the codebase, must not revert others' edits, and must keep changes inside assigned files. Record prerequisite gates that are already satisfied, including `code_quality_hardening_20260421` for `testing_maturity_20260421`, and record unresolved gates such as `mcp_server_20260408` for `copilot_studio_20260421`.
- **validation**: Prompts include ownership boundaries, validation expectations, conductor plan update rules, satisfied prerequisite gates, unresolved prerequisite gates, and a handoff format with files changed and tests run.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T2: Correlation ID And Telemetry Foundation
- **depends_on**: [T1]
- **location**: `conductor/tracks/correlation_id_telemetry_20260427/`, `Greg.Xrm.Command/**`
- **description**: Assign a single worker to land `ICorrelationIdProvider` and shared telemetry plumbing. This is the first implementation wave for the 2026-04-27 AI/connector tracks.
- **validation**: Track tests pass; public interfaces are documented; downstream workers can consume the provider without duplicating correlation logic.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T3: Connector Schema Validation
- **depends_on**: [T2]
- **location**: `conductor/tracks/connector_schema_validation_20260427/`, `Greg.Xrm.Command/**`
- **description**: Assign a worker to implement connector schema parsing/validation after correlation support exists. Worker must fetch current official docs for OpenAPI parsing and selected NuGet packages before coding.
- **validation**: Valid and invalid connector definitions are covered by tests; command output is deterministic for CI use.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T4: AI Builder Improved Flow
- **depends_on**: [T2, T3]
- **location**: `conductor/tracks/ai_builder_connectors_improved_20260427/`, `Greg.Xrm.Command/**`
- **description**: Assign a worker to add retry, polling, schema validation, and correlation ID support to AI Builder and connector commands.
- **validation**: Retry/timeout tests pass; connector validation uses T3 output; no duplicate schema validator or correlation provider is introduced.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T5: AI Wrapper Service
- **depends_on**: [T2, T3]
- **location**: `conductor/tracks/ai_wrapper_service_20260427/`, `Greg.Xrm.Command/**`
- **description**: Assign a worker to introduce `IAiBuilderService`, result/error types, and wrapper migration behind controlled fallback behavior. This waits for T3 because the wrapper interface includes `ValidateConnectorSchemaAsync` and must reuse the connector schema abstractions instead of inventing a parallel validator.
- **validation**: Existing AI commands still pass; wrapper service has fake HTTP-handler tests; service does not conflict with T4 changes.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T6: CI/CD Hardening Remaining Work
- **depends_on**: [T1]
- **location**: `conductor/tracks/ci_cd_hardening_20260421/`, `.github/workflows/**`, `coverage-threshold.yml`
- **description**: Assign a workflow-focused worker to finish remaining CI matrix, concurrency, path filters, NuGet cache, release smoke, and release verification tasks.
- **validation**: Workflow YAML parses; local static checks pass where available; validator confirms no duplicate or conflicting workflow triggers.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### Workflow Ownership Matrix

Workers may only edit workflow files in their assigned row. Any workflow change outside this matrix must be routed through `Integration_Lead`.

| Task | Allowed Workflow Ownership |
|------|----------------------------|
| T6 | `.github/workflows/ci.yml`, `.github/workflows/_build.yml`, `.github/workflows/release.yml`, release smoke workflow files owned by `ci_cd_hardening_20260421` |
| T7 | none by default; repository template files only unless `Integration_Lead` grants a workflow file |
| T8 | documentation/Pages workflow only, if created for the docs site |
| T10 | mutation, contract, integration-test, or scheduled test workflows only |
| T17 | benchmark/performance workflow only, and only if opt-in or scheduled |

### T7: Developer Experience Remaining Work
- **depends_on**: [T1]
- **location**: `conductor/tracks/developer_experience_20260421/`, `.devcontainer/`, `.husky/`, `.github/`, `.editorconfig`
- **description**: Assign a DX worker to complete remaining contributor tooling and repository hygiene tasks.
- **validation**: Hooks/templates/config files are present, minimal, and documented; no CI workflow ownership conflicts with T6.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T8: Documentation Site
- **depends_on**: [T7]
- **location**: `conductor/tracks/documentation_site_20260421/`, `docs/`, `.github/workflows/**`
- **description**: Assign a docs worker after DX structure is settled. Build DocFX site, generated command reference, recipes, ADRs, migration notes, and Pages workflow if needed.
- **validation**: Docs build locally or has a documented blocked reason; generated command reference matches current command registry.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T9: CLI UX
- **depends_on**: [T1]
- **location**: `conductor/tracks/cli_ux_20260421/`, `Greg.Xrm.Command/**`
- **description**: Assign a CLI worker for completions, exit code taxonomy, output format consistency, first-run flow, telemetry opt-in, and Spectre.Console polish.
- **validation**: Parser tests cover new CLI surfaces; output snapshots or golden tests are added for stable help/format behavior.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T10: Testing Maturity
- **depends_on**: [T1]
- **location**: `conductor/tracks/testing_maturity_20260421/`, `Greg.Xrm.Command.*Test*/`, `.github/workflows/**`
- **description**: Assign a testing worker to add Verify snapshots, Stryker.NET config, architecture rules, contract tests, integration gating, and FsCheck expansion. T1 must explicitly record that `code_quality_hardening_20260421` is complete before this worker starts; otherwise this task remains blocked.
- **validation**: Fast test suite remains runnable locally; long-running mutation/integration checks are gated behind explicit workflows or environment variables.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T11: Fabric And OneLake
- **depends_on**: [T1]
- **location**: `conductor/tracks/fabric_onelake_20260421/`, `Greg.Xrm.Command/**`
- **description**: Assign a Fabric worker. Worker must fetch current official Microsoft Fabric REST/API docs before implementation.
- **validation**: Token scopes are isolated; commands have unit tests with fake clients; no live Fabric dependency is required for PR CI.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T12: Power BI Workspace Management
- **depends_on**: [T1]
- **location**: `conductor/tracks/powerbi_workspace_mgmt_20260421/`, `Greg.Xrm.Command/**`
- **description**: Assign a Power BI worker. Worker must fetch current official Power BI REST API docs before implementation.
- **validation**: Client wrapper tests cover list/publish/clone/refresh flows using fakes; command names do not conflict with existing tabular commands.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T13: Desktop Flow RPA
- **depends_on**: [T1]
- **location**: `conductor/tracks/desktop_flow_rpa_20260421/`, `Greg.Xrm.Command/**`
- **description**: Assign a Dataverse/RPA worker for desktop-flow list/trigger/runs, machines, groups, scaffold, and approvals.
- **validation**: Dataverse queries are unit-tested; commands skip or fake live calls in CI; no assumptions about tenant-specific machine groups are hard-coded.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T14: Dataverse Gaps Phase 2
- **depends_on**: [T1]
- **location**: `conductor/tracks/dataverse_gaps_phase2_20260421/`, `Greg.Xrm.Command/**`
- **description**: Assign a Dataverse worker for business rules, BPFs, duplicate detection, audit export, field security, service endpoints, alternate keys, file columns, and rollup recalculation.
- **validation**: Each command has parser and executor tests; features are split into smaller PR-sized batches inside the track.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T15: Forms Advanced
- **depends_on**: [T1]
- **location**: `conductor/tracks/forms_advanced_20260421/`, `Greg.Xrm.Command/**`
- **description**: Assign a Forms worker for branching export, analytics, template management, org sharing, and Customer Voice bridge.
- **validation**: Existing `ms_forms_20260409` behavior remains unchanged; API calls are isolated behind testable clients.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T16: Power FX Validation
- **depends_on**: [T1]
- **location**: `conductor/tracks/power_fx_validation_20260421/`, `Greg.Xrm.Command/**`, `Directory.Packages.props`
- **description**: Assign a Power FX worker. Worker must fetch current official `Microsoft.PowerFx.Core` docs/package guidance before adding dependencies.
- **validation**: Package version is centralized; parser/format/repl commands have deterministic tests.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T17: Performance And Native AOT
- **depends_on**: [T1]
- **location**: `conductor/tracks/performance_aot_20260421/`, `Greg.Xrm.Command.Benchmarks/`, `.github/workflows/**`
- **description**: Assign a performance worker for BenchmarkDotNet, regression gates, Native AOT feasibility, trim safety, and hot-path source generator exploration.
- **validation**: Benchmarks are opt-in and do not slow default CI; AOT limitations are documented with concrete evidence.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T18: Copilot Studio CLI Gate And Worker
- **depends_on**: [T1]
- **location**: `conductor/tracks/copilot_studio_20260421/`, `conductor/tracks/archive/mcp_server_20260408/`, `Greg.Xrm.Command/**`
- **description**: Do not launch implementation until T1 records the `mcp_server_20260408` dependency as closed, archived, or intentionally bypassed. Once unblocked, assign a Copilot worker for agent lifecycle, topic import/export, knowledge sources, analytics, and MCP bridge work.
- **validation**: If blocked, mission state records the blocker and no worker edits code. If unblocked, Copilot API/client work has fake-client tests and does not reopen the archived MCP host boundary accidentally.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T19: Validator Pool
- **depends_on**: [T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18]
- **location**: All assigned worker outputs
- **description**: Create validator assignments that run in parallel with completed worker handoffs. Validators inspect diffs, run relevant tests, and verify conductor plan checkboxes.
- **validation**: Every implementation task has a validator note with commands run, findings, and acceptance decision.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T20: Integration Lead And Conflict Resolution
- **depends_on**: [T19]
- **location**: Repository root, `conductor/tracks.md`, `logs/`, `.swarm/`
- **description**: Assign one integration lead to merge validated worker outputs, resolve cross-track conflicts, refresh status reports, and preserve unrelated dirty work.
- **validation**: Final repo status is summarized; conductor status reflects completed tasks; failed validations are routed back to the owning worker.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T21: Legacy Track Closure
- **depends_on**: [T1]
- **location**: `conductor/tracks/solution_tests_20260408/`, `conductor/tracks/cicd_tests_20260408/`, `conductor/tracks/code_quality_20260408/`, `conductor/tracks/pr_lifecycle_20260409/`, `conductor/tracks/pr146_integration_20260408/`, `conductor/tracks/resolve_issues_20260408/`, `conductor/tracks/archive/explore_branches_20260408/`, `conductor/tracks/archive/mcp_server_20260408/`
- **description**: Assign `Legacy_Backlog_Worker` to close, complete, block, or archive older non-complete tracks that were not part of the newer backlog roster.
- **validation**: Each legacy track has an accepted closure note, metadata status update, and clear rationale if archived or blocked.
- **status**: Not Completed
- **log**:
- **files edited/created**:

## Parallel Execution Groups

| Wave | Tasks | Can Start When |
|------|-------|----------------|
| 0 | T0 | Immediately |
| 1 | T1, T18 | T0 complete |
| 2 | T2, T6, T7, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18 | T1 complete; T10 also requires T1 to verify `code_quality_hardening_20260421`; T18 stays blocked until `mcp_server_20260408` is resolved |
| 3 | T3, T8 | T2 complete for T3; T7 complete for T8 |
| 4 | T4, T5 | T2 and T3 complete |
| 5 | T19, T21 | Worker handoffs complete; legacy track closure can run once T1 ownership rules are recorded |
| 6 | T20 | Validator acceptance complete |

## Recommended Subagent Roster

| Agent | Mode | Primary Ownership |
|-------|------|-------------------|
| `Mission_Lead` | coordinator | T0, T1, T19 |
| `Telemetry_Worker` | parallel | T2 |
| `Connector_Worker` | parallel | T3 |
| `Ai_Worker` | parallel | T4, T5 after dependencies are satisfied |
| `Workflow_Worker` | parallel | T6 |
| `DX_Worker` | parallel | T7, then T8 |
| `CLI_Worker` | parallel | T9 |
| `Testing_Worker` | parallel | T10 |
| `Fabric_Worker` | parallel | T11 |
| `PowerBI_Worker` | parallel | T12 |
| `DesktopFlow_Worker` | parallel | T13 |
| `Dataverse_Gaps_Worker` | parallel | T14 |
| `Forms_Worker` | parallel | T15 |
| `PowerFx_Worker` | parallel | T16 |
| `Performance_Worker` | parallel | T17 |
| `Copilot_Worker` | parallel | T18, only after MCP gate is resolved |
| `Quality_Validator` | validator | T19 for all worker handoffs |

## Testing Strategy

- Workers run the narrowest relevant test project first, then broader solution tests only when shared code changes.
- Workflow changes get YAML/static validation plus a dry review of triggers, permissions, and matrix expansion.
- API/client work uses fake clients or fake HTTP handlers in PR CI; live integration remains opt-in through environment variables.
- Validators record exact commands run and any skipped checks.

## Risks & Mitigations

- **Dirty worktree conflicts**: Start from T0 inventory and require file ownership before edits.
- **Shared core collisions**: Run telemetry, connector validation, and AI wrapper tracks in dependency order.
- **Long-running agents diverge**: Require validators to reject work that does not rebase or integrate cleanly.
- **External API drift**: Each API-specific worker fetches current official docs before coding and records doc links in the handoff.
- **CI cost explosion**: Keep mutation, benchmark, integration, and live API checks opt-in or scheduled until proven stable.
