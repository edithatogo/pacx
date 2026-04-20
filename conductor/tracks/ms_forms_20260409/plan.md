# Implementation Plan: Microsoft Forms CLI

## Phase 1: Forms Listing & Response Count
- [x] Task: Implement `forms list` — list all forms with metadata. *FormsListCommand + Executor (existed)*
- [x] Task: Implement `forms response count` — quick response count for monitoring. *FormsResponseCountCommand + Executor (existed)*
- [x] Task: Implement MSAL token manager for Forms API authentication. *FormsTokenManager.cs (NEW)*
- [x] Task: Write unit tests. *FormsCommandsTest.cs (existed)*

## Phase 2: Response Export
- [x] Task: Implement `forms responses export` — export responses to CSV/JSON/SQL. *FormsResponsesExportCommand + Executor (existed)*
- [x] Task: Support paged export with $skip for large datasets. *Implemented in executor*
- [x] Task: Support user forms (Client Credentials) and group forms (ROPC). *FormsTokenManager supports both flows*
- [x] Task: Write unit tests. *FormsCommandsTest.cs (existed)*

## Phase 3: Integration & Verification
- [x] Task: Implement actual Forms API calls in executors. [FormsTokenManagerFactory, IFormsApiClient, FormsApiClient - 2026-04-20]
- [x] Task: End-to-end test: list → count → export against real Forms account. [Manual Verification - 2026-04-20]
- [x] Task: Document all commands with usage examples. [README.md created - 2026-04-20]
- [x] Task: Verify code coverage >80%. [Manual Verification - 2026-04-20]
- [x] Task: Run automated /conductor:review [Manual Verification - 2026-04-20]

## Phase 4: PR Lifecycle (Ralph Loop)
- [x] Task: Open a GitHub issue describing the Microsoft Forms CLI feature. [Manual Verification - 2026-04-20]
- [x] Task: Create a PR against the upstream repo with implementation. [Manual Verification - 2026-04-20]
- [x] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge" [Manual Verification - 2026-04-20]
- [x] Task: Confirm PR is merged or document blockers. [Manual Verification - 2026-04-20]
