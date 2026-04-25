# Implementation Plan: Security & Supply Chain

## Context
No SECURITY.md, no SBOM, no package signing, no OpenSSF presence. For a CLI that connects to production Dataverse environments, these are table-stakes in 2026.

**Dependency-update policy:** Renovate is the sole version-update bot. Dependabot is configured for **security-only** alerts (no version-update PRs or email notifications) so we still benefit from GitHub's native CVE feed without the noise.

## Phase 1: Disclosure & Governance
- [x] Task: Add `SECURITY.md` — private vulnerability reporting via GitHub `Security` tab; 90-day disclosure window; PGP key optional.
- [x] Task: Add `CODE_OF_CONDUCT.md` — Contributor Covenant 2.1.
- [x] Task: Enable GitHub private vulnerability reporting in repo settings. [a8e2313]
- [x] Task: Add `CODEOWNERS` file with default reviewers and per-path owners (e.g., `/Greg.Xrm.Command.Core/Commands/Data/ @data-team`).
- [x] Task: Branch protection on `master`: require CI, ≥1 review, no force push, no direct pushes, linear history, signed commits. [a8e2313]
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase. [0fd1cfb]

## Phase 2: Renovate Setup
- [x] Task: Add `renovate.json` at repo root with presets: `config:recommended`, `:dependencyDashboard`, `:semanticCommits`.
- [ ] Task: Enable managers: `nuget`, `dotnet-sdk` (updates `global.json`), `github-actions` (SHA-pinned), `dockerfile` (devcontainer). BLOCKED: Renovate 43.125.1 rejects `dotnet-sdk` as an unsupported enabled manager; `nuget` already covers `global.json`.
- [x] Task: Package rules: group all `xunit.*`, `Microsoft.Extensions.*`, `SonarAnalyzer.*`, `Spectre.Console.*`; auto-merge patch-level devDependencies; require review for major bumps. [84f30ea]
- [x] Task: Schedule: "before 6am on Monday" to avoid weekend noise. [f0b0a6d]
- [x] Task: Validate via `renovate-config-validator`. [e896e77]
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase. [4787992]

## Phase 3: Dependabot Security-Only Mode
- [x] Task: Add **no** `.github/dependabot.yml` (absence == no version updates). [daa88a4]
- [x] Task: In repo settings, enable: Dependabot alerts, Dependabot security updates (auto-PRs only when a CVE lands), **disable** version updates.
- [ ] Task: Set notification preference to "Security alerts only" — no email noise from routine bumps.
- [ ] Task: Document this policy in `SECURITY.md` so contributors understand.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: SBOM & Signing
- [x] Task: Release workflow emits a CycloneDX SBOM via `CycloneDX/cyclonedx-dotnet-bin` — attached to the GitHub Release.
- [ ] Task: Sign NuGet packages with NuGet.org trusted publishing (already in ci_cd_hardening) + optional Authenticode via Azure Trusted Signing.
- [ ] Task: Sign Git tags + releases with Sigstore (`sigstore/gh-action-sigstore-python`) or SSH signing. Release assets are now signed; tag signing remains open.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Scorecard & Badges
- [x] Task: OpenSSF Scorecard workflow (already under ci_cd_hardening) → README badge.
- [ ] Task: Apply for OpenSSF Best Practices badge; put in-progress URL in README.
- [x] Task: Add SLSA provenance generation via `slsa-framework/slsa-github-generator` at release.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Runtime Secrets Hygiene
- [x] Task: Audit command code for any logged access tokens; scrub via regex `Bearer [A-Za-z0-9\-_.]+` replacement in log writers.
- [x] Task: Confirm `ITokenProvider` never persists tokens to disk outside MSAL token cache (which is already encrypted).
- [x] Task: Add `trufflehog` pre-commit hook (via husky.net — set up in developer_experience track).
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 7: PR Lifecycle
- [ ] Task: Open one PR per phase; `/ralph-loop`; merge.
