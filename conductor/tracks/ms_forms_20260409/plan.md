# Implementation Plan: Microsoft Forms CLI

## Phase 1: Authentication & Token Management
- [ ] Task: Research Forms API authentication patterns (Client Credentials vs ROPC).
- [ ] Task: Implement MSAL-based token manager with auto-refresh and 401 retry.
- [ ] Task: Implement token caching to avoid repeated token requests.
- [ ] Task: Support both user-owned and group-owned form authentication paths.
- [ ] Task: Write unit tests with mocked HTTP client.
- [ ] Task: Run automated /conductor:review

## Phase 2: Forms List & Response Count
- [ ] Task: Implement `forms list` command — list all forms with metadata.
- [ ] Task: Implement `forms response count` command — quick response count.
- [ ] Task: Support output formats: table + JSON.
- [ ] Task: Implement tenant/user/group scoping.
- [ ] Task: Write unit tests with mocked API responses.
- [ ] Task: Run automated /conductor:review

## Phase 3: Responses Export
- [ ] Task: Implement `forms responses export` command — paged response retrieval.
- [ ] Task: Support export formats: CSV, JSON, SQL INSERT statements.
- [ ] Task: Implement incremental sync (track last exported response via `$skip`).
- [ ] Task: Handle large response sets (thousands of responses) efficiently.
- [ ] Task: Write unit tests with sample response data.
- [ ] Task: Run automated /conductor:review

## Phase 4: Integration & Verification
- [ ] Task: End-to-end test against real Forms tenant: list → count → export.
- [ ] Task: Document all commands with usage examples.
- [ ] Task: Verify code coverage >80%.
- [ ] Task: Run automated /conductor:review

## Phase 5: PR Lifecycle (Ralph Loop)
- [ ] Task: Open a GitHub issue describing the Microsoft Forms CLI feature.
- [ ] Task: Create a PR against the upstream repo with implementation.
- [ ] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge"
- [ ] Task: Confirm PR is merged or document blockers.
