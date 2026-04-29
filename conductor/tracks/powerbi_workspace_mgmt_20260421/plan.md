# Implementation Plan: Power BI Workspace & Dataset Management

## Context
pacx has `tabular deploy/diff/validate` for BIM files (Tabular Editor parity) but no workspace-level management: datasets, RLS, refresh schedules, capacity, deployment pipelines. Power BI admins are the main audience asking for CLI tooling.

## Phase 1: Auth & Client
- [x] Task: `IPowerBiClient` wrapper around Power BI REST API v1 (`https://api.powerbi.com/v1.0/myorg/...`).
- [x] Task: Scope: `https://analysis.windows.net/powerbi/api/.default`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 2: Dataset CRUD
- [x] Task: `dataset list --workspace-id`.
- [x] Task: `dataset publish --pbix <file> --workspace-id`.
- [x] Task: `dataset clone --source-id --target-workspace-id`.
- [x] Task: `dataset delete --id`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 3: Row-Level Security
- [x] Task: `dataset rls list --dataset-id` — returns roles from `/datasets/{id}/users`.
- [x] Task: `dataset rls apply --dataset-id --role <name> --user <upn>` assigns users to a role; XMLA DAX filter authoring remains in the tabular/XMLA slice.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 4: Refresh Management
- [x] Task: `dataset refresh trigger --id`.
- [x] Task: `dataset refresh status --id`.
- [x] Task: `dataset refresh schedule --id --cron <expr>` — stores a schedule payload for API-side handling.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 5: Deployment Pipelines
- [x] Task: `pipeline list`.
- [x] Task: `pipeline stage-assign --pipeline-id --stage dev|test|prod --workspace-id`.
- [x] Task: `pipeline deploy --pipeline-id --source-stage dev --target-stage test`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 6: Capacity Ops (Premium)
- [x] Task: `capacity list`.
- [x] Task: `capacity workspace-assign --capacity-id --workspace-id`.
- [x] Task: `capacity metrics --capacity-id` — surfaces capacity workload/metric payloads from the REST API.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 7: PR Lifecycle
- [x] Task: Working-tree implementation completed for upstream PR packaging.

## Validation
- Static JSON/config validation passed.
- Local build/test execution is blocked until the .NET 11 preview SDK is installed under the dotnet root used by `Greg.Xrm.Command/dotnet11.ps1`.
