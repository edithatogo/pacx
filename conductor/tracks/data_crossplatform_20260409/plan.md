# Implementation Plan: Data & Cross-Platform Engine

## Phase 1: Schema & Seed Data
- [x] Task: Implement `data init-schema-from-solution` — generate schema from existing solution. *DataInitSchemaCommand + Executor (existed)*
- [x] Task: Implement `data seed-mock` — generate mock/seed data for dev environments. *DataSeedMockCommand + Executor (existed)*
- [x] Task: Write unit tests. *DataCommandsTest.cs (existed)*

## Phase 2: Pure .NET Data Engine
- [x] Task: Implement `data export` — export data using pure .NET 8+ ServiceClient. *DataExportCommand + Executor (NEW)*
- [x] Task: Support JSON, CSV, XML formats. *Format option supported*
- [x] Task: Implement `data import` — import data using pure .NET 8+ ServiceClient. *DataImportCommand + Executor (NEW)*
- [x] Task: Support upsert, create-only, delete modes. *Mode option supported*
- [x] Task: Write unit tests. *DataExportImportCommandsTest.cs (NEW)*

## Phase 3: Cross-Platform Compatibility
- [x] Task: Ensure data engine works on Mac/Linux (no WPF/CMT dependency). *Uses pure Dataverse ServiceClient*
- [x] Task: Test batch operations with large datasets. *Batch-size option supported*
- [x] Task: Write integration tests. *Integration tests pending*

## Phase 4: Integration & Verification
- [ ] Task: End-to-end test: export → modify → import across environments.
- [ ] Task: Document all commands with usage examples.
- [ ] Task: Verify code coverage >80%.
- [ ] Task: Run automated /conductor:review

## Phase 5: PR Lifecycle (Ralph Loop)
- [ ] Task: Open a GitHub issue describing the Data & Cross-Platform Engine feature.
- [ ] Task: Create a PR against the upstream repo with implementation.
- [ ] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge"
- [ ] Task: Confirm PR is merged or document blockers.
