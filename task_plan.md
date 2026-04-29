# Task Plan

Mission: PACX all-track conductor completion.

Primary queue: [swarm-track-completion-manifest.json](./swarm-track-completion-manifest.json)

Detailed execution plan: [swarm-subagent-conductor-plan.md](./swarm-subagent-conductor-plan.md)

## Completion Contract

The mission is not done when agents are merely assigned. It is done only when every track in `active_track_queue` is one of:

- `complete`: implementation, tests, track plan, metadata, and validator acceptance are present.
- `blocked`: a concrete blocker is recorded with the command/API/permission failure and accepted by `Quality_Validator`.
- `archived`: the track is superseded or intentionally closed, with rationale recorded in the track plan and metadata.

## Execution Waves

- Wave 0: `Mission_Lead` and `Integration_Lead` reconcile stale mission state, dirty worktree state, duplicate track statuses, and the local .NET validation blocker.
- Wave 1: `Telemetry_Worker`, `Connector_Worker`, `Workflow_Worker`, `CLI_Worker`, and `Testing_Worker` start dependency-ready foundation work.
- Wave 2: `Ai_Worker` starts after telemetry/schema gates clear; `DX_Worker`, `Fabric_Worker`, `PowerBI_Worker`, `DesktopFlow_Worker`, `PowerFx_Worker`, `Dataverse_Gaps_Worker`, and `Forms_Worker` run in parallel where file ownership does not conflict.
- Wave 3: `Legacy_Backlog_Worker` closes or archives older tracks not owned by the domain workers: PR lifecycle, PR146 integration, issue resolution, archived explore branches, and archived MCP server.
- Wave 4: `Performance_Worker` and `DX_Worker` complete docs/performance work after code-facing command surfaces stabilize.
- Wave 5: `Copilot_Worker` starts only after `archive/mcp_server_20260408` is completed, archived, or explicitly bypassed.
- Wave 6: `Quality_Validator` and `Integration_Lead` reconcile all accepted handoffs, regenerate status reports, and update `conductor/tracks.md`.

## Agent Ownership

- `Mission_Lead`: queue control, dependency gating, worker launch order.
- `Integration_Lead`: cross-track merge/conflict resolution, status report regeneration, final metadata reconciliation.
- `Telemetry_Worker`: `correlation_id_telemetry_20260427`.
- `Connector_Worker`: `connector_schema_validation_20260427`.
- `Ai_Worker`: `ai_builder_connectors_improved_20260427`, `ai_wrapper_service_20260427`.
- `Workflow_Worker`: `ci_cd_hardening_20260421`.
- `DX_Worker`: `developer_experience_20260421`, `documentation_site_20260421`.
- `CLI_Worker`: `cli_ux_20260421`.
- `Testing_Worker`: `testing_maturity_20260421`, `solution_tests_20260408`, `cicd_tests_20260408`, `code_quality_20260408`.
- `Fabric_Worker`: `fabric_onelake_20260421`.
- `PowerBI_Worker`: `powerbi_workspace_mgmt_20260421`.
- `DesktopFlow_Worker`: `desktop_flow_rpa_20260421`.
- `Dataverse_Gaps_Worker`: `dataverse_gaps_phase2_20260421`.
- `Forms_Worker`: `forms_advanced_20260421`.
- `PowerFx_Worker`: `power_fx_validation_20260421`.
- `Performance_Worker`: `performance_aot_20260421`.
- `Copilot_Worker`: `copilot_studio_20260421`, gated by MCP closure.
- `Legacy_Backlog_Worker`: `pr_lifecycle_20260409`, `pr146_integration_20260408`, `resolve_issues_20260408`, `archive/explore_branches_20260408`, `archive/mcp_server_20260408`.
- `Quality_Validator`: every worker handoff before track completion.

## Required Handoff Format

Each worker must hand off:

- Track ID and target completion state.
- Files changed.
- Track plan checkboxes updated.
- Metadata updates made.
- Tests and validation commands run.
- Blockers, if any, with exact command output summary.
- Follow-up ownership if validation cannot complete locally.

## Known Global Blockers

- Local .NET validation is currently blocked by MSBuild/workload resolver failures. `Integration_Lead` must either repair this or record an accepted external validation path before final completion.
- `copilot_studio_20260421` remains gated by `archive/mcp_server_20260408`.

## Notes

- Do not reuse the old `Junior`/`Quality_Validator` CI-only mission state.
- File ownership collisions go through `Integration_Lead`.
- Keep each worker inside the file scope described in the manifest and conductor track plan.
