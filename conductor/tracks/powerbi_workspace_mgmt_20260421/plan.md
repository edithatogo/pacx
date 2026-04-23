# Implementation Plan: Power BI Workspace & Dataset Management

## Context
pacx has `tabular deploy/diff/validate` for BIM files (Tabular Editor parity) but no workspace-level management: datasets, RLS, refresh schedules, capacity, deployment pipelines. Power BI admins are the main audience asking for CLI tooling.

## Phase 1: Auth & Client
- [ ] Task: `IPowerBiClient` wrapper around Power BI REST API v1 (`https://api.powerbi.com/v1.0/myorg/...`).
- [ ] Task: Scope: `https://analysis.windows.net/powerbi/api/.default`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Dataset CRUD
- [ ] Task: `dataset list --workspace-id`.
- [ ] Task: `dataset publish --pbix <file> --workspace-id`.
- [ ] Task: `dataset clone --source-id --target-workspace-id`.
- [ ] Task: `dataset delete --id`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Row-Level Security
- [ ] Task: `dataset rls list --dataset-id` — returns roles from `/datasets/{id}/users` + DAX filters.
- [ ] Task: `dataset rls apply --dataset-id --role <name> --filter-dax <expr>` — writes DAX via XMLA, assigns users.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: Refresh Management
- [ ] Task: `dataset refresh trigger --id`.
- [ ] Task: `dataset refresh status --id`.
- [ ] Task: `dataset refresh schedule --id --cron <expr>` — converts to Power BI scheduled-refresh format.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Deployment Pipelines
- [ ] Task: `pipeline list`.
- [ ] Task: `pipeline stage-assign --pipeline-id --stage dev|test|prod --workspace-id`.
- [ ] Task: `pipeline deploy --pipeline-id --source-stage dev --target-stage test`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Capacity Ops (Premium)
- [ ] Task: `capacity list`.
- [ ] Task: `capacity workspace-assign --capacity-id --workspace-id`.
- [ ] Task: `capacity metrics --capacity-id` — CPU, memory, throttling events.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 7: PR Lifecycle
- [ ] Task: Upstream PR per 2 phases; `/ralph-loop`; merge.
