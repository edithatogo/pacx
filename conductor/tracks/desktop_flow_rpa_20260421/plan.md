# Implementation Plan: Desktop Flows (RPA)

## Context
`workflow` commands cover cloud flows. Desktop flows (Power Automate Desktop, RPA) have zero coverage today. Enterprise automation teams need CLI entry points.

## Phase 1: Discovery
- [x] Task: `desktop-flow list --env <id>` — Dataverse query over `workflow` entity filtered by `category == 6` (desktop flow).
- [x] Task: `desktop-flow get --id` — includes script preview from workflow client data.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 2: Execution
- [x] Task: `desktop-flow trigger --id --machine-group <name> [--input key=value ...]` — uses Power Automate REST-style run endpoint targeting desktop flow.
- [x] Task: `desktop-flow run list --id`.
- [x] Task: `desktop-flow run get --run-id` — surfaces run payloads, logs, and artifacts returned by the API.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 3: Machines & Groups
- [x] Task: `desktop-flow machine list`.
- [x] Task: `desktop-flow machine-group list`.
- [x] Task: `desktop-flow machine-group assign --machine-id --group-id`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 4: Scaffolding
- [x] Task: `desktop-flow scaffold --name <flow>` — writes a skeleton `.txt` (Robin script) with common building blocks.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 5: Approvals (bonus — closes the Automation coverage gap)
- [x] Task: `approval list --env`.
- [x] Task: `approval respond --id --decision approve|reject --comment <text>`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 6: PR Lifecycle
- [x] Task: Working-tree implementation completed for upstream PR packaging.

## Validation
- Static JSON/config validation passed.
- Local build/test execution is blocked until the .NET 11 preview SDK is installed under the dotnet root used by `Greg.Xrm.Command/dotnet11.ps1`.
