# Implementation Plan: Dataverse Platform Gaps

## Phase 1: Custom API Commands
- [ ] Task: Implement `custom-api create` — create Custom API with input/output parameters.
- [ ] Task: Support all parameter types: String, Entity, Boolean, Number, DateTime, OptionSet.
- [ ] Task: Implement `custom-api list` and `custom-api delete` for lifecycle management.
- [ ] Task: Write unit tests with mocked ServiceClient.
- [ ] Task: Run automated /conductor:review

## Phase 2: Catalog & Elastic Tables
- [ ] Task: Implement `catalog publish-item` — manage Catalog & Business Events.
- [ ] Task: Implement `elastic-table manage` — configure retention policies and scaling.
- [ ] Task: Research and document Catalog API (less documented feature).
- [ ] Task: Write unit tests.
- [ ] Task: Run automated /conductor:review

## Phase 3: Virtual Tables & Connection References
- [ ] Task: Implement `virtual-table scaffold` — scaffold virtual table from external data source.
- [ ] Task: Implement `connection-ref map-interactive` — map connection references across solutions.
- [ ] Task: Write unit tests.
- [ ] Task: Run automated /conductor:review

## Phase 4: Integration & Verification
- [ ] Task: End-to-end test: create custom API → use in plugin → catalog publish.
- [ ] Task: Document all commands with usage examples.
- [ ] Task: Verify code coverage >80%.
- [ ] Task: Run automated /conductor:review

## Phase 5: PR Lifecycle (Ralph Loop)
- [ ] Task: Open a GitHub issue describing the Dataverse Platform Gaps feature.
- [ ] Task: Create a PR against the upstream repo with implementation.
- [ ] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge"
- [ ] Task: Confirm PR is merged or document blockers.
