# Implementation Plan: CI/CD Hardening

## Context
Current CI is a single Linux × .NET 8.0.x workflow with a placeholder deploy step. To meet 2026 SOTA for a .NET dotnet-tool OSS project, expand to matrix testing, SAST, coverage gates, reusable workflows, concurrency control, and OIDC trusted publishing to NuGet.

## Phase 1: Matrix & Concurrency
- [ ] Task: Expand `.github/workflows/ci.yml` matrix to `{os: [windows-latest, ubuntu-latest, macos-latest], dotnet: ['8.0.x', '9.0.x']}`.
- [ ] Task: Add `concurrency: { group: ci-${{ github.ref }}, cancel-in-progress: true }` to cancel superseded runs.
- [ ] Task: Add `paths-ignore: ['docs/**', '**/*.md']` path filters so doc-only PRs skip CI.
- [ ] Task: Cache `.nuget/packages` keyed on `packages.lock.json` hash.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Current Status
Phase 1 is underway: the CI workflow now cancels superseded runs, skips docs-only changes, caches the .NET toolchain, and fans out over the available OS matrix with the repository's pinned 10.0.202 SDK. The remaining phase 1 work is to reconcile the desired wider SDK matrix with the repo pin before the tracker can move on.

## Phase 2: Security & Dependency Scanning
- [x] Task: Add `.github/workflows/codeql.yml` — `github/codeql-action@v3` for C# on PR + weekly schedule.
- [x] Task: Add `.github/workflows/scorecard.yml` — OpenSSF Scorecard action, publishes SARIF to Security tab + README badge.
- [x] Task: Enable GitHub-native secret scanning + push protection (repo settings note in track log).
- [x] Task: Enable GitHub-native dependency graph + Dependabot **security-only** alerts (no version update PRs/emails).
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Reusable Workflows
- [x] Task: Extract build+test into `.github/workflows/_build.yml` callable via `workflow_call`; consumed by ci.yml and release.yml.
- [x] Task: Pin every GitHub Action to a commit SHA (Renovate will refresh). Example: `actions/checkout@<sha> # v5.0.0`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: Coverage Gate
- [x] Task: Coverlet emits Cobertura; `.github/workflows/ci.yml` publishes summary via `irongut/CodeCoverageSummary`.
- [x] Task: Fail CI if line coverage < 70% on touched assemblies (ratcheting threshold stored in `coverage-threshold.yml`).
- [x] Task: Upload coverage to Codecov via OIDC (no secret).
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Release Pipeline (OIDC → NuGet.org)
- [x] Task: Add `.github/workflows/release.yml` triggered on tag `v*`.
- [x] Task: Use NuGet.org **trusted publishing** (OIDC) — no `NUGET_API_KEY` secret. Register GitHub publisher on nuget.org.
- [x] Task: Deterministic build: set `ContinuousIntegrationBuild=true`, enable SourceLink.
- [x] Task: Publish to GitHub Releases with auto-generated notes via Release Drafter.
- [x] Task: Environment `production-nuget` with required reviewer before push.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Verification
- [ ] Task: Trigger a prerelease tag `v0.0.0-ci-smoke` or a workflow_dispatch dry run and confirm the release path end-to-end.
- [ ] Task: Verify the fork-owned release workflow publishes NuGet packages, GitHub Release assets, and PACX release artifacts end-to-end.
