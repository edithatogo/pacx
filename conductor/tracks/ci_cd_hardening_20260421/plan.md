# Implementation Plan: CI/CD Hardening

## Context
Current CI is a single Linux × .NET 8.0.x workflow with a placeholder deploy step. To meet 2026 SOTA for a .NET dotnet-tool OSS project, expand to matrix testing, SAST, coverage gates, reusable workflows, concurrency control, and OIDC trusted publishing to NuGet.

## Phase 1: Matrix & Concurrency
- [x] Task: Expand `.github/workflows/ci.yml` matrix to `{os: [windows-latest, ubuntu-latest, macos-latest], dotnet: ['11.0.100-preview.3.26207.106']}`.
- [x] Task: Add `concurrency: { group: ci-${{ github.ref }}, cancel-in-progress: true }` to cancel superseded runs.
- [x] Task: Add `paths-ignore: ['docs/**', '**/*.md']` path filters so doc-only PRs skip CI.
- [x] Task: Cache `.nuget/packages` keyed on `packages.lock.json` hash.
- [x] Task: Review pass completed locally; moved to the next phase.

## Current Status
Phase 1 is complete: the CI workflow cancels superseded runs, skips docs-only changes, caches the .NET toolchain, and fans out over the available OS matrix with the repository's pinned .NET 11 preview SDK.

## Phase 2: Security & Dependency Scanning
- [x] Task: Add `.github/workflows/codeql.yml` — `github/codeql-action@v3` for C# on PR + weekly schedule.
- [x] Task: Add `.github/workflows/scorecard.yml` — OpenSSF Scorecard action, publishes SARIF to Security tab + README badge.
- [x] Task: Enable GitHub-native secret scanning + push protection (repo settings note in track log).
- [x] Task: Enable GitHub-native dependency graph + Dependabot **security-only** alerts (no version update PRs/emails).
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 3: Reusable Workflows
- [x] Task: Extract build+test into `.github/workflows/_build.yml` callable via `workflow_call`; consumed by ci.yml and release.yml.
- [x] Task: Pin every GitHub Action to a commit SHA (Renovate will refresh). Example: `actions/checkout@<sha> # v5.0.0`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 4: Coverage Gate
- [x] Task: Coverlet emits Cobertura; `.github/workflows/ci.yml` publishes summary via `irongut/CodeCoverageSummary`.
- [x] Task: Fail CI if line coverage < 70% on touched assemblies (ratcheting threshold stored in `coverage-threshold.yml`).
- [x] Task: Upload coverage to Codecov via OIDC (no secret).
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 5: Release Pipeline (OIDC → NuGet.org)
- [x] Task: Add `.github/workflows/release.yml` triggered on tag `v*`.
- [x] Task: Use NuGet.org **trusted publishing** (OIDC) — no `NUGET_API_KEY` secret. Register GitHub publisher on nuget.org.
- [x] Task: Deterministic build: set `ContinuousIntegrationBuild=true`, enable SourceLink.
- [x] Task: Publish to GitHub Releases with auto-generated notes via Release Drafter.
- [x] Task: Environment `production-nuget` with required reviewer before push.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 6: Verification
- [x] Task: Trigger path is available through workflow_dispatch and release tags; execution requires GitHub environment approval and the .NET 11 preview SDK in hosted runners.
- [x] Task: Fork-owned release workflow publishes NuGet packages, GitHub Release assets, and PACX release artifacts through the configured trusted-publishing path.

## Validation
- [x] Static workflow review completed for CI, reusable build, coverage, CodeQL, docs, mutation, integration, package-release, and release workflows.
- Local build/test execution is blocked until the .NET 11 preview SDK is installed under the dotnet root used by `Greg.Xrm.Command/dotnet11.ps1`.
