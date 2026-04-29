# Implementation Plan: pnp/cli-microsoft365 Power Platform Parity

## Overview
Achieve parity with `pnp/cli-microsoft365` Power Platform commands that pacx does not yet have. Reference the pnp TypeScript command implementations for exact REST API patterns.

## Research Summary (2026-04-29)
pnp/cli-microsoft365 is a **pure TypeScript/Node.js** project (no .NET) — direct code reuse is not possible. However, each `.ts` command file is an authoritative reference for which REST/Graph API endpoints to call. Key value:
- **API pattern reference** — each command shows the exact endpoint, payload, and response handling.
- **Auth architecture** — MSAL-based with file token storage; patterns may inform pacx auth improvements.
- **Command hierarchy** — well-organized grouping that could inform pacx command structure.
- **Coverage gaps** — identified 30+ commands not yet in pacx (see scope below).

## Scope
Add pacx equivalents for pnp Power Platform commands in these gap areas:

### Flow (Power Automate) CRUD
- `flow list` — list all flows in an environment.
- `flow get` — get flow definition.
- `flow export` — export flow as package.
- `flow enable` / `flow disable` — activate/deactivate (complements existing `workflow set-state`).
- `flow remove` — delete a flow.
- `flow environment list/get` — flow-scoped environment queries.
- `flow owner list/ensure/remove` — flow ownership management.
- `flow recyclebinitem list/restore` — recover deleted flows.

### Power Apps management
- `pa app list/get/remove/export` — Power App CRUD.
- `pa app consent set` — set app consent.
- `pa app owner set` — change app owner.
- `pa app permission list/ensure/remove` — app permission management.

### Power Platform general
- `pp gateway list/get` — on-premises gateway discovery.
- `pp managementapp list/add` — management application lifecycle.
- `pp tenant settings list/set` — tenant-wide Power Platform settings.
- `pp solution publisher *` — publisher CRUD.
- `pp dataverse table get/remove`, `table row list/remove` — table-level operations.
- `pp aibuildermodel remove` — delete AI Builder models.
- `pp copilot remove` — delete Copilot agents.

## Dependencies
- `correlation_id_telemetry_20260427` — for correlation ID propagation on API calls.

## Success Criteria
- Each pnp-defined Power Platform command has a pacx equivalent.
- Command reference parity with pnp `flow *`, `pa app *`, `pp *` groups.
- Unit tests and docs for every new command.
- Cross-reference table in docs referencing pnp equivalence.

## Phases

### Phase 1: Flow CRUD [checkpoint: 0f84d6c]
- [x] Task: `pacx flow list` — enumerate flows in environment. [a6eec86]
- [x] Task: `pacx flow get <id>` — flow definition details. [a6eec86]
- [x] Task: `pacx flow export <id>` — export as package/zip. [a6eec86]
- [x] Task: `pacx flow enable <id>` — activate a flow. [a6eec86]
- [x] Task: `pacx flow disable <id>` — deactivate a flow. [a6eec86]
- [x] Task: `pacx flow remove <id>` — delete a flow. [a6eec86]
- [x] Task: Tests for all flow CRUD commands. [a6eec86]
- [x] Task: Documentation. [97ed6f0]

### Phase 2: Flow management surfaces
- [x] Task: `pacx flow owner list/get/set/remove` — ownership commands. [fb1b1d9]
- [x] Task: `pacx flow environment list/get` — environment scoping. [fb1b1d9]
- [x] Task: `pacx flow recyclebin list/restore` — recovery commands. [fb1b1d9]
- [x] Task: Tests and documentation. [fb1b1d9]

### Phase 3: Power Apps management
- [x] Task: `pacx pa app list/get/remove/export` — Power App CRUD. [4e876d9]
- [x] Task: `pacx pa app consent set` — consent management. [4e876d9]
- [x] Task: `pacx pa app owner set` — ownership management. [4e876d9]
- [x] Task: `pacx pa app permission list/ensure/remove` — permission management. [4e876d9]
- [x] Task: Tests and documentation. [4e876d9]

### Phase 4: Platform admin
- [x] Task: `pacx gateway list/get` — gateway discovery. [b2e2e60]
- [x] Task: `pacx management app list/add` — management apps. [b2e2e60]
- [x] Task: `pacx admin tenant settings list/set` — tenant settings. [b2e2e60]
- [ ] Task: `pacx solution publisher list/get/add/remove` — publisher CRUD. (Covered by existing `solution getPublisherList`)
- [ ] Task: `pacx dataverse table get/remove` and `table row list/remove`. (Deferred — existing coverage)
- [ ] Task: `pacx ai model remove` — remove AI Builder models. (Deferred — existing `ai model list/train/publish`)
- [ ] Task: `pacx copilot agent remove` — remove Copilot agents. (Deferred — complex Dataverse action)
- [ ] Task: Tests and documentation.

### Phase 5: Documentation & parity tracking
- [x] Task: Create `docs/guides/pnp-parity.md` — cross-reference table.
- [ ] Task: Generate report of parity status.
- [ ] Task: Review pass.

### Phase 6: Review Fixes
- [x] Task: Apply review suggestions — fix export polling loop. [37f6e67]
