# Implementation Plan: Dataverse Gaps — Phase 2

## Context
`dataverse_gaps_20260409` shipped CustomAPI, Catalog, ElasticTable, VirtualTable, ConnectionRef. Coverage audit identified additional platform features still missing. This track picks them up.

## Phase 1: Business Rules
- [ ] Task: `business-rule list --table`.
- [ ] Task: `business-rule export --id --file <xml>` — exports rule XAML / JSON logic.
- [ ] Task: `business-rule import --file --table`.
- [ ] Task: `business-rule activate|deactivate --id`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Business Process Flows (BPF)
- [ ] Task: `bpf list`.
- [ ] Task: `bpf export --id` — stages, steps, transitions.
- [ ] Task: `bpf import --file`.
- [ ] Task: `bpf activate --id`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Duplicate Detection
- [ ] Task: `ddr list` — duplicate detection rules.
- [ ] Task: `ddr run --rule-id`.
- [ ] Task: `ddr enable|disable --rule-id`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: Audit
- [ ] Task: `audit status --env`.
- [ ] Task: `audit enable-table --name`.
- [ ] Task: `audit export --table --since <date> --format jsonl` — streams `audit` entity records with userland-friendly field resolution.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Field Security Profiles
- [ ] Task: `fsp list`.
- [ ] Task: `fsp apply --profile-id --user-or-team-id`.
- [ ] Task: `fsp bulk-assign --profile-id --file <csv>`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Service Endpoints (Webhooks)
- [ ] Task: `endpoint list`.
- [ ] Task: `endpoint register --url --auth <type>`.
- [ ] Task: `endpoint delete --id`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 7: Alternate Keys / File-Image / Rollup
- [ ] Task: `key create --table --columns` (alternate keys).
- [ ] Task: `column file-upload --table --column --file` (File column).
- [ ] Task: `column rollup-recalculate --table --column`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 8: PR Lifecycle
- [ ] Task: Phased PRs; `/ralph-loop`; merge.
