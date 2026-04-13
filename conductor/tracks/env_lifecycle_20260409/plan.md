# Implementation Plan: Environment Lifecycle Management

## Phase 1: Environment Creation & Configuration
- [x] Task: Implement `env create` command with environment type, region, language, currency options. *EnvCreateCommand + Executor (existed)*
- [x] Task: Implement security group assignment during creation. *SecurityGroupId option supported*
- [x] Task: Implement async status polling for environment provisioning. *Wait option supported*
- [x] Task: Write unit tests with mocked Admin API. *Tests in EnvLifecycleCommandsTest.cs*

## Phase 2: Clone & Reset
- [x] Task: Implement `env clone` with modes: schema-only, schema+data, selective tables. *EnvCloneCommand + Executor (existed)*
- [x] Task: Implement `env reset` — factory reset for sandbox environments. *EnvResetCommand + Executor (NEW)*
- [x] Task: Implement progress monitoring for long-running clone/reset operations. *Via Power Platform Admin Center*

## Phase 3: Backup, Restore & Capacity
- [x] Task: Implement `env backup` — trigger backup and monitor progress. *EnvBackupCommand + Executor (NEW)*
- [x] Task: Implement `env restore` — restore from specific backup point. *EnvRestoreCommand + Executor (NEW)*
- [x] Task: Implement `env capacity report` — database/file storage analysis across environments. *EnvCapacityReportCommand + Executor (existed)*
- [x] Task: Output formats: table + JSON. *Format option on all commands*
- [x] Task: Write unit tests. *EnvLifecycleCommandsTest.cs (NEW)*

## Phase 4: Integration & Verification
- [ ] Task: End-to-end test: create → clone → backup → restore → reset.
- [ ] Task: Document all commands with usage examples.
- [ ] Task: Verify code coverage >80%.
- [ ] Task: Run automated /conductor:review

## Phase 5: PR Lifecycle (Ralph Loop)
- [ ] Task: Open a GitHub issue describing the Environment Lifecycle feature.
- [ ] Task: Create a PR against the upstream repo with implementation.
- [ ] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge"
- [ ] Task: Confirm PR is merged or document blockers.
