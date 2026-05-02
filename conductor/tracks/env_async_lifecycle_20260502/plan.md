# Implementation Plan: Async Environment Lifecycle

## Overview
Environment backup, restore, and clone operations are stubbed — they validate the environment exists via Dataverse then print "Note:" documentation. These operations require async job submission and polling via the Power Platform Admin API.

## Scope
- Environment backup (with optional data inclusion)
- Environment restore from backup
- Environment clone/copy (full, minimal, or data-only)

## Dependencies
- BAP API methods already added to `IPowerPlatformAdminClient` in stub_killer_phase2

## Success Criteria
- `pacx env backup` submits backup job and polls until complete
- `pacx env restore` restores from backup with job tracking
- `pacx env clone` copies environment with progress polling
- All commands report job status and handle timeout

## Phases

### Phase 1: Async Job Polling Service
- [x] Task: `IAsyncJobPoller` interface — wraps BAP async operation pattern
- [x] Task: `AsyncJobPoller` — polls `Location` header or operation status endpoint
- [x] Task: Configurable timeout, polling interval, retry logic
- [x] Task: Register in IoCModule

### Phase 2: Backup & Restore
- [x] Task: De-fake `EnvBackupCommandExecutor` — submit backup, poll completion
- [x] Task: De-fake `EnvRestoreCommandExecutor` — submit restore, poll completion
- [ ] Task: Tests with mock BAP responses

### Phase 3: Clone/Copy
- [x] Task: De-fake `EnvCloneCommandExecutor` — submit copy, poll completion
- [x] Task: Support copy modes: MinimalCopy, FullCopy, DataOnly
- [ ] Task: Tests

### Phase 4: Cleanup & Docs
- [ ] Task: Remove old `IOrganizationServiceRepository` dependency from env executors
- [x] Task: Update Known Stubs in tracks.md
