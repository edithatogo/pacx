# Implementation Plan: Power FX Validation

## Context
pac has `pac power-fx` (validate/format/repl). pacx has no counterpart. Calculated columns, Dataverse Business Rules, and canvas apps all use Power FX; a CLI validator unblocks CI for citizen-developer artifacts.

## Phase 1: Parser Integration
- [ ] Task: Reference `Microsoft.PowerFx.Core` (official NuGet).
- [ ] Task: `power-fx validate --expression <str>` — returns parse + semantic errors.
- [ ] Task: `power-fx validate --file <yml>` — validates a YAML/JSON file of named expressions.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Format
- [ ] Task: `power-fx format --expression <str>` — canonicalizes whitespace, identifier casing.
- [ ] Task: `power-fx format --file <yml> --in-place`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Schema-aware validation
- [ ] Task: `power-fx validate --expression <str> --table <name>` — binds to a Dataverse table's columns for completion-aware validation.
- [ ] Task: Useful for validating calculated columns and business rules before import.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: REPL (stretch)
- [ ] Task: `power-fx repl` — interactive eval against a Dataverse context.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.
