# Implementation Plan: Environment Lifecycle Management

## Phase 1: Environment Creation & Configuration
- [ ] Task: Implement `env create` command with environment type, region, language, currency options.
- [ ] Task: Implement security group assignment during creation.
- [ ] Task: Implement async status polling for environment provisioning.
- [ ] Task: Write unit tests with mocked Admin API.
- [ ] Task: Run automated /conductor:review

## Phase 2: Clone & Reset
- [ ] Task: Implement `env clone` with modes: schema-only, schema+data, selective tables.
- [ ] Task: Implement `env reset` — factory reset for sandbox environments.
- [ ] Task: Implement progress monitoring for long-running clone/reset operations.
- [ ] Task: Write unit tests.
- [ ] Task: Run automated /conductor:review

## Phase 3: Backup, Restore & Capacity
- [ ] Task: Implement `env backup` — trigger backup and monitor progress.
- [ ] Task: Implement `env restore` — restore from specific backup point.
- [ ] Task: Implement `env capacity report` — database/file storage analysis across environments.
- [ ] Task: Output formats: table + JSON.
- [ ] Task: Write unit tests.
- [ ] Task: Run automated /conductor:review

## Phase 4: Integration & Verification
- [ ] Task: End-to-end test: create → clone → backup → restore → reset.
- [ ] Task: Document all commands with usage examples.
- [ ] Task: Verify code coverage >80%.
- [ ] Task: Open PR against upstream repo.
- [ ] Task: Run automated /conductor:review
