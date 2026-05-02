# Implementation Plan: CI/CD & DevOps Overhaul

## Anti-Stub Preamble
Every task produces a verifiable workflow change, a new workflow file, or a config change. No task is complete without verification. `/conductor-review` auto-triggers.

## Overview
Fix the CI matrix bug that silently doubles build time. Add rollback capability for bad releases. Harden Docker for production use. Fix NuGet packaging metadata.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: Fix CI Matrix Bug
- **Analyze:** Read `_build.yml` lines 33-34 — inner matrix overrides caller's `dotnet_version` input. Read `ci.yml` matrix. Confirm 12 builds instead of 6.
- **Implement:** Remove the inner matrix from `_build.yml`. Use `${{ inputs.dotnet_version }}` directly. Full 3×2 matrix on main branch only; reduced 1×2 on PRs.
- **Verify:** CI runs 6 builds instead of 12. All required status checks still pass. PR jobs complete faster.
- **Auto-Review:** `/conductor-review`.

### Phase 2: Create Rollback Workflow
- **Analyze:** Read `release.yml` — understand publish flow. Read NuGet deprecate API docs. Read Docker manifest tool docs.
- **Implement:** Create `.github/workflows/rollback.yml` with `workflow_dispatch` inputs: version, type (nuget/docker/tag/all). Steps: NuGet package deprecation, Docker re-tag to previous, git tag revert.
- **Verify:** Workflow triggers with valid inputs. Dry-run mode works without making changes.
- **Auto-Review:** `/conductor-review`.

### Phase 3: Harden Docker Build
- **Analyze:** Read current `Dockerfile` — runs as root, no layer caching, no healthcheck, no labels.
- **Implement:** Restructure for layer caching (csproj first, restore, then source). Use `runtime-deps:11.0-jammy`. Add non-root user with `--chown`. Add `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`. Add `HEALTHCHECK`. Add OCI labels.
- **Verify:** Docker image builds. Runs as non-root. Layer cache works across builds.
- **Auto-Review:** `/conductor-review`.

### Phase 4: Fix NuGet Packaging
- **Analyze:** Read `Directory.Build.props` — missing `PackageLicenseExpression`, wrong URLs. Read `Greg.Xrm.Command.Interfaces.csproj` — hardcoded version `1.1.1`.
- **Implement:** Add `<PackageLicenseExpression>MIT</PackageLicenseExpression>`. Fix `<PackageProjectUrl>` and `<RepositoryUrl>` to point to the actual repo. Remove hardcoded version from Interfaces.csproj — let CI set it via `-p:VersionNumber=`.
- **Verify:** `dotnet pack` produces packages with correct metadata. NuGet.org warnings resolved.
- **Auto-Review:** `/conductor-review`.

### Phase 5: Align Release SDK Versions
- **Analyze:** Read `release.yml` — validate uses .NET 10, publish uses .NET 11. Dockerfile targets 11.0.
- **Implement:** Use `11.0.x` for both validate and publish in release.yml. Dockerfile publishes only `net11.0` TFM. Document dual-targeting strategy.
- **Verify:** Validate and publish use the same SDK. Docker image contains only relevant TFM.
- **Auto-Review:** `/conductor-review`.

## Rollback
Any phase that breaks CI: revert, fix, re-verify. CI must pass before phase is complete.
