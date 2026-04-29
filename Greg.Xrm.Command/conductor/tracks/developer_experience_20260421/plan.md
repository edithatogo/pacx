# Implementation Plan: Developer Experience

## Context
Local onboarding friction: no devcontainer, no pre-commit hooks, no PR/issue templates, no conventional-commit enforcement. Contributors don't know what to install or how to format code before committing.

## Phase 1: Devcontainer & Containerized Build
- [x] Task: Add `.devcontainer/devcontainer.json` on `mcr.microsoft.com/devcontainers/dotnet:1-10.0-jammy`; features: `powershell`, `azure-cli`, `github-cli`, `docker-in-docker`.
- [x] Task: Add `postCreateCommand` to restore local tools, install Husky hooks, and restore the solution.
- [x] Task: Add `Dockerfile` at repo root that builds the tool for Linux/Windows containers.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Conventional Commits + Husky.NET
- [x] Task: Add Husky.NET (`dotnet husky install`) with hooks: `pre-commit` → `dotnet format --verify-no-changes`; `commit-msg` → `commitlint` (npm or the `conventional-commits` .NET port).
- [x] Task: Add `.commitlintrc.json` enforcing Conventional Commits with scopes matching our noun namespaces (`feat(alm)`, `fix(dataverse)`, `chore(ci)`, etc.).
- [x] Task: CI gate: Lint PR title + every commit message.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: PR & Issue Templates
- [x] Task: `.github/PULL_REQUEST_TEMPLATE.md` — sections: Summary, Linked Issue, Test Plan, Upstream impact, Breaking changes, Docs updated.
- [x] Task: `.github/ISSUE_TEMPLATE/bug_report.yml`, `feature_request.yml`, `security_disclosure.yml` (redirects to private reporting).
- [x] Task: `.github/ISSUE_TEMPLATE/config.yml` disables blank issues, pins Discussions link.
- [x] Task: `.github/DISCUSSION_TEMPLATE/ideas.yml` for feature requests preferred over issues.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: EditorConfig Expansion
- [x] Task: Expand `.editorconfig` with C# naming conventions (IDE1006), using-directive placement (IDE0065), expression-bodied preferences, file-scoped namespaces (IDE0161).
- [x] Task: Add `dotnet_diagnostic.<id>.severity = error` for the handful of rules we never want to regress.
- [x] Task: Add `editorconfig-checker` to CI.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Bots & Labels
- [x] Task: `.github/labeler.yml` — auto-label PRs by touched path (e.g., `area/alm`, `area/dataverse`, `area/ci`).
- [x] Task: Release Drafter config (`.github/release-drafter.yml`) — auto-generate release notes categorized by label.
- [x] Task: `actions/stale` workflow — label issues stale after 60 days, close after 90 (exclude `pinned`, `security`, `good-first-issue`).
- [x] Task: Welcome-bot workflow for first-time contributors.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Funding & Discoverability
- [x] Task: `.github/FUNDING.yml` (GitHub Sponsors / OpenCollective — opt-in).
- [x] Task: Set repository topics: `dotnet-tool`, `dataverse`, `power-platform`, `power-bi`, `powerapps-cli`, `fabric`. Tracked in `repo-settings-note.md` because this is repository-side configuration.
- [x] Task: Enable Discussions; seed categories: Q&A, Ideas, Show and Tell, Announcements. Tracked in `repo-settings-note.md` because this is repository-side configuration.
- [x] Task: Run local file/artifact verification.

## Phase 7: PR Lifecycle
- [ ] Task: One PR per phase; `/ralph-loop`; merge.

---

## Validation Snapshot (2026-04-28)

Local file-based developer experience work is present:

- `.devcontainer/devcontainer.json`
- `Dockerfile`
- `.husky/pre-commit`, `.husky/commit-msg`, `.husky/task-runner.json`
- `.commitlintrc.json`
- `.github/PULL_REQUEST_TEMPLATE.md`
- `.github/ISSUE_TEMPLATE/*`
- `.github/DISCUSSION_TEMPLATE/ideas.yml`
- `.github/labeler.yml`
- `.github/release-drafter.yml`
- `.github/workflows/editorconfig.yml`
- `.github/workflows/labeler.yml`
- `.github/workflows/release-drafter.yml`
- `.github/workflows/stale.yml`
- `.github/workflows/welcome.yml`
- `.github/FUNDING.yml`
- `repo-settings-note.md` for GitHub-only topics/discussions settings

Remaining non-local work:
- Apply the repo settings in GitHub when repository admin access is available.
