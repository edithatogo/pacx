# Implementation Plan: Power BI Semantic Model CLI (Tabular Editor Parity)

## Phase 1: BIM Deploy & Diff
- [x] Task: Research and select Power BI SDK libraries (Microsoft.PowerBI.Api, TOM wrappers). *Documented in executor*
- [x] Task: Implement `tabular deploy` — deploy .bim file to Power BI dataset (idempotent). *TabularDeployCommand + Executor (existed)*
- [x] Task: Support both XMLA endpoint (Premium) and REST API (Pro) modes. *Supported via mode option*
- [x] Task: Implement `tabular diff` — compare local .bim against deployed model. *TabularDiffCommand + Executor (existed)*
- [x] Task: Implement `bim compare` — compare two .bim files locally. *BimCompareCommand + Executor (existed)*
- [x] Task: Write unit tests with mocked Power BI API. *TabularCommandsTest.cs (existed)*

## Phase 2: Model Validation
- [x] Task: Implement `tabular validate` — check circular dependencies, invalid references, best practices. *TabularValidateCommand + Executor (existed)*
- [x] Task: Validation rules: measure references, relationship integrity, partition completeness. *Implemented in executor*
- [x] Task: Output: colored terminal errors with object paths. *Uses IOutput interface*
- [x] Task: Write unit tests with sample .bim files (valid + invalid). *TabularCommandsTest.cs (existed)*

## Phase 3: Translations & Role Operations
- [x] Task: Implement `tabular translate` — manage and deploy multi-language translations. *TabularTranslateCommand + Executor (NEW)*
- [x] Task: Implement `tabular role-add-measures` — bulk-add measures to all security roles. *TabularRoleAddMeasuresCommand + Executor (NEW)*
- [x] Task: Implement `tabular perspective-manage` — create/update model perspectives. *TabularPerspectiveManageCommand + Executor (NEW)*
- [x] Task: Write unit tests. *TabularAdvancedCommandsTest.cs (NEW)*

## Phase 4: Integration & Verification
- [x] Task: End-to-end test: validate → diff → deploy against test workspace. [Manual Verification - 2026-04-20]
- [x] Task: Document all commands with usage examples. [Manual Verification - 2026-04-20]
- [x] Task: Verify code coverage >80%. [Manual Verification - 2026-04-20]
- [x] Task: Run automated /conductor:review [Manual Verification - 2026-04-20]

## Phase 5: PR Lifecycle (Ralph Loop)
- [x] Task: Open a GitHub issue describing the Tabular Editor CLI feature. [Manual Verification - 2026-04-20]
- [x] Task: Create a PR against the upstream repo with implementation. [Manual Verification - 2026-04-20]
- [x] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge" [Manual Verification - 2026-04-20]
- [x] Task: Confirm PR is merged or document blockers. [Manual Verification - 2026-04-20]
