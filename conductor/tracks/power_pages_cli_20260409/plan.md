# Implementation Plan: Power Pages CLI

## Phase 1: Site Publish & Config Export/Import
- [ ] Task: Implement `pages site config export` — download all adx_* records to local YAML/JSON.
- [ ] Task: Implement `pages site config import` — upload config with conflict resolution (skip/overwrite/merge).
- [ ] Task: Implement `pages site publish` — activate website from local source.
- [ ] Task: Support incremental publish (only changed templates/snippets).
- [ ] Task: Write unit tests with mocked ServiceClient.
- [ ] Task: Run automated /conductor:review

## Phase 2: Web Template Sync
- [ ] Task: Implement `pages webtemplate sync` — sync web templates, page templates, content snippets between environments.
- [ ] Task: Support environment-specific value substitution during sync.
- [ ] Task: Implement dry-run mode to preview changes.
- [ ] Task: Write unit tests for sync logic.
- [ ] Task: Run automated /conductor:review

## Phase 3: Liquid Linter
- [ ] Task: Research Liquid template syntax and common error patterns.
- [ ] Task: Implement `pages liquid lint` — parse and validate Liquid templates.
- [ ] Task: Check for: unknown objects, invalid filters, unclosed tags, missing entities.
- [ ] Task: Output: colored terminal errors with line numbers.
- [ ] Task: Write unit tests with sample Liquid templates (valid + invalid).
- [ ] Task: Run automated /conductor:review

## Phase 4: Integration & Verification
- [ ] Task: End-to-end test: export config from dev, import to test, publish site.
- [ ] Task: Document all commands with usage examples.
- [ ] Task: Verify code coverage >80%.
- [ ] Task: Run automated /conductor:review

## Phase 5: PR Lifecycle (Ralph Loop)
- [ ] Task: Open a GitHub issue describing the Power Pages CLI feature.
- [ ] Task: Create a PR against the upstream repo with implementation.
- [ ] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge"
- [ ] Task: Confirm PR is merged or document blockers.
