# Implementation Plan: Power BI Semantic Model CLI (Tabular Editor Parity)

## Phase 1: BIM Deploy & Diff
- [ ] Task: Research and select Power BI SDK libraries (Microsoft.PowerBI.Api, TOM wrappers).
- [ ] Task: Implement `tabular deploy` — deploy .bim file to Power BI dataset (idempotent).
- [ ] Task: Support both XMLA endpoint (Premium) and REST API (Pro) modes.
- [ ] Task: Implement `tabular diff` — compare local .bim against deployed model.
- [ ] Task: Implement `bim compare` — compare two .bim files locally.
- [ ] Task: Write unit tests with mocked Power BI API.
- [ ] Task: Run automated /conductor:review

## Phase 2: Model Validation
- [ ] Task: Implement `tabular validate` — check circular dependencies, invalid references, best practices.
- [ ] Task: Validation rules: measure references, relationship integrity, partition completeness.
- [ ] Task: Output: colored terminal errors with object paths.
- [ ] Task: Write unit tests with sample .bim files (valid + invalid).
- [ ] Task: Run automated /conductor:review

## Phase 3: Translations & Role Operations
- [ ] Task: Implement `tabular translate` — manage and deploy multi-language translations.
- [ ] Task: Implement `tabular role-add-measures` — bulk-add measures to all security roles.
- [ ] Task: Implement `tabular perspective-manage` — create/update model perspectives.
- [ ] Task: Write unit tests.
- [ ] Task: Run automated /conductor:review

## Phase 4: Integration & Verification
- [ ] Task: End-to-end test: validate → diff → deploy against test workspace.
- [ ] Task: Document all commands with usage examples.
- [ ] Task: Verify code coverage >80%.
- [ ] Task: Open PR against upstream repo.
- [ ] Task: Run automated /conductor:review
