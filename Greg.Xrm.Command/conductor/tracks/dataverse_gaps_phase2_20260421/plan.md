# Implementation Plan: Dataverse Gaps — Phase 2

## Context
`dataverse_gaps_20260409` shipped CustomAPI, Catalog, ElasticTable, VirtualTable, ConnectionRef. Coverage audit identified additional platform features still missing. This track picks them up.

## Phase 1: Business Rules
- [x] Task: `business-rule list --table`.
- [x] Task: `business-rule export --id --file <xml>` — exports rule XAML / JSON logic.
- [x] Task: `business-rule import --file --table`.
- [x] Task: `business-rule activate|deactivate --id`.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Business Process Flows (BPF)
- [x] Task: `bpf list`.
- [x] Task: `bpf export --id` — stages, steps, transitions.
- [x] Task: `bpf import --file`.
- [x] Task: `bpf activate --id`.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Duplicate Detection
- [x] Task: `ddr list` — duplicate detection rules.
- [x] Task: `ddr run --rule-id`.
- [x] Task: `ddr enable|disable --rule-id`.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: Audit
- [x] Task: `audit status --env`.
- [x] Task: `audit enable-table --name`.
- [x] Task: `audit export --table --since <date> --format jsonl` — streams `audit` entity records with userland-friendly field resolution.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Field Security Profiles
- [x] Task: `fsp list`.
- [x] Task: `fsp apply --profile-id --user-or-team-id`.
- [x] Task: `fsp bulk-assign --profile-id --file <csv>`.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Service Endpoints (Webhooks)
- [x] Task: `endpoint list`.
- [x] Task: `endpoint register --url --auth <type>`.
- [x] Task: `endpoint delete --id`.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 7: Alternate Keys / File-Image / Rollup
- [x] Task: `key create --table --columns` (alternate keys).
- [x] Task: `column file-upload --table --column --file` (File column).
- [x] Task: `column rollup-recalculate --table --column`.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 8: PR Lifecycle
- [x] Task: Phased PRs; `/ralph-loop`; merge.
