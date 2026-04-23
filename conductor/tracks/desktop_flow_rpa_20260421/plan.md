# Implementation Plan: Desktop Flows (RPA)

## Context
`workflow` commands cover cloud flows. Desktop flows (Power Automate Desktop, RPA) have zero coverage today. Enterprise automation teams need CLI entry points.

## Phase 1: Discovery
- [ ] Task: `desktop-flow list --env <id>` — Dataverse query over `workflow` entity filtered by `category == 6` (desktop flow).
- [ ] Task: `desktop-flow get --id` — include script preview, machine-group binding.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Execution
- [ ] Task: `desktop-flow trigger --id --machine-group <name> [--input key=value ...]` — uses Power Automate REST `runWorkflow` action targeting desktop flow.
- [ ] Task: `desktop-flow run list --id`.
- [ ] Task: `desktop-flow run get --run-id` — surfaces logs + artifacts.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Machines & Groups
- [ ] Task: `desktop-flow machine list`.
- [ ] Task: `desktop-flow machine-group list`.
- [ ] Task: `desktop-flow machine-group assign --machine-id --group-id`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: Scaffolding
- [ ] Task: `desktop-flow scaffold --name <flow>` — writes a skeleton `.txt` (Robin script) with common building blocks.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Approvals (bonus — closes the Automation coverage gap)
- [ ] Task: `approval list --env`.
- [ ] Task: `approval respond --id --decision approve|reject --comment <text>`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.
