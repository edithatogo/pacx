# Implementation Plan: Microsoft Forms — Advanced

## Context
`ms_forms_20260409` shipped list + response count + response export. Missing: branching logic, analytics, template management, org-wide sharing, Forms Pro (Customer Voice).

## Phase 1: Branching Logic Export
- [ ] Task: `forms branching export --form-id` — extracts conditional-path configuration from the Forms API.
- [ ] Task: Human-readable summary + machine-readable JSON.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Analytics
- [ ] Task: `forms analytics summary --form-id` — submission count, completion rate, median time, dropoff by question.
- [ ] Task: `forms analytics timeseries --form-id --bucket day|week`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Template Management
- [ ] Task: `forms template list --org`.
- [ ] Task: `forms template create --from-form <id> --scope org|team`.
- [ ] Task: `forms template share --id --group <AAD group>`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: Org-wide Sharing & Permissions
- [ ] Task: `forms share --form-id --with-group <AAD group> --role respond|collaborate`.
- [ ] Task: `forms ownership transfer --form-id --to <upn>`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Customer Voice (Forms Pro) Bridge
- [ ] Task: `forms cv list` — Customer Voice surveys.
- [ ] Task: `forms cv send --survey-id --to <upn list>`.
- [ ] Task: `forms cv responses export --survey-id`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.
