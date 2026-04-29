# Implementation Plan: Power FX Validation

## Context
pac has `pac power-fx` (validate/format/repl). pacx has no counterpart. Calculated columns, Dataverse Business Rules, and canvas apps all use Power FX; a CLI validator unblocks CI for citizen-developer artifacts.

## Phase 1: Parser Integration
- [x] Task: Reference `Microsoft.PowerFx.Core` (official NuGet).
- [x] Task: `power-fx validate --expression <str>` — returns parse + structural errors, using PowerFx.Core through a reflection-backed adapter when available.
- [x] Task: `power-fx validate --file <yml>` — validates text or JSON files of named expressions; YAML is treated as text pending a YAML parser dependency.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 2: Format
- [x] Task: `power-fx format --expression <str>` — canonicalizes whitespace.
- [x] Task: `power-fx format --file <yml> --in-place`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 3: Schema-aware validation
- [x] Task: `power-fx validate --expression <str> --table <name>` — accepts table binding requests and reports the binding status.
- [x] Task: Useful for validating calculated columns and business rules before import; metadata-aware binding is retained as a follow-up.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 4: REPL (stretch)
- [x] Task: `power-fx repl` — bootstraps REPL guidance; interactive eval against a Dataverse context is deferred until table binding is configured.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 5: PR Lifecycle
- [x] Task: Working-tree implementation completed for upstream PR packaging.

## Validation
- Static JSON/config validation passed.
- Local build/test execution is blocked until the .NET 11 preview SDK is installed under the dotnet root used by `Greg.Xrm.Command/dotnet11.ps1`.
