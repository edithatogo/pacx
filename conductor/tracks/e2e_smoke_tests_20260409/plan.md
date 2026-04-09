# Implementation Plan: E2E Smoke Tests & Integration Test Infrastructure

## Phase 1: Integration Test Project
- [ ] Task: Create `Greg.Xrm.Command.Core.IntegrationTests` project in the solution.
- [ ] Task: Add `ServiceClient` test fixture with secure credential loading (env vars → GitHub Secrets → local .env).
- [ ] Task: Implement test base class with automatic cleanup (create resources in setup, delete in teardown).
- [ ] Task: Add configuration for test environment URL, client ID, and secret.
- [ ] Task: Write unit tests for credential loading logic (no real connection needed).
- [ ] Task: Run automated /conductor:review

## Phase 2: Smoke Test Suite
- [ ] Task: Implement auth smoke test — authenticate and list organizations.
- [ ] Task: Implement solution smoke test — list solutions, create test solution, verify, delete.
- [ ] Task: Implement table smoke test — create test table, verify metadata, delete.
- [ ] Task: Implement column smoke test — create column on test table, verify, clean up.
- [ ] Task: Implement relationship smoke test — create test relationship, verify, clean up.
- [ ] Task: Implement web resource smoke test — push test web resource, verify, delete.
- [ ] Task: Add timeout configuration (default 60s per operation).
- [ ] Task: Run automated /conductor:review

## Phase 3: CI Integration
- [ ] Task: Create `.github/workflows/e2e-smoke-tests.yml` workflow (trigger: on push to master, on PR merge).
- [ ] Task: Configure GitHub Secrets for test environment credentials.
- [ ] Task: Add test result reporting (pass/fail with timing).
- [ ] Task: Add notification on failure (GitHub issue auto-created).
- [ ] Task: Verify workflow runs successfully against test environment.
- [ ] Task: Run automated /conductor:review

## Phase 4: PR Lifecycle (Ralph Loop)
- [ ] Task: Open a GitHub issue describing the E2E smoke test infrastructure.
- [ ] Task: Create a PR against the upstream repo with implementation.
- [ ] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge"
- [ ] Task: Confirm PR is merged or document blockers.
