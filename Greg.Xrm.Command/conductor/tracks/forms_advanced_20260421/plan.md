# Implementation Plan: Microsoft Forms — Advanced

## Context
`ms_forms_20260409` shipped list + response count + response export. Missing: branching logic, analytics, template management, org-wide sharing, Forms Pro (Customer Voice).

## Phase 1: Branching Logic Export
- [x] Task: `forms branching export --form-id` — extracts conditional-path configuration from the Forms API.
- [x] Task: Human-readable summary + machine-readable JSON.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 2: Analytics
- [x] Task: `forms analytics summary --form-id` — submission count, completion rate, median time, dropoff by question.
- [x] Task: `forms analytics timeseries --form-id --bucket day|week`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 3: Template Management
- [x] Task: `forms template list --org`.
- [x] Task: `forms template create --from-form <id> --scope org|team`.
- [x] Task: `forms template share --id --group <AAD group>`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 4: Org-wide Sharing & Permissions
- [x] Task: `forms share --form-id --with-group <AAD group> --role respond|collaborate`.
- [x] Task: `forms ownership transfer --form-id --to <upn>`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 5: Customer Voice (Forms Pro) Bridge
- [x] Task: `forms cv list` — Customer Voice surveys.
- [x] Task: `forms cv send --survey-id --to <upn list>`.
- [x] Task: `forms cv responses export --survey-id`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 6: PR Lifecycle
- [x] Task: Working-tree implementation completed for upstream PR packaging.

## Validation
- Static JSON/config validation passed.
- Local build/test execution is blocked until the .NET 11 preview SDK is installed under the dotnet root used by `Greg.Xrm.Command/dotnet11.ps1`.
