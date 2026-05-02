# Implementation Plan: Security Hotfix Sprint

## Anti-Stub Preamble
Every task produces a measurable security improvement: a removed suppression, a pinned action SHA, a replaced encryption key, or a corrected config. No task is complete without verification. `/conductor-review` auto-triggers at every phase boundary.

## Overview
Fix P0 security issues identified by the security, GitHub, and C# expert reviews. These are the most immediately exploitable vulnerabilities in the codebase.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: Remove Static AES Key/IV from Source Code
- **Analyze:** Read `Properties/Resources.resx` — find `AesKey` and `AesIV` entries. Read `ConnectionSetting.cs` (encrypt/decrypt methods). Read `AesEncryption.cs` interface.
- **Implement:** Generate a unique key per machine using `ProtectedData.Protect()` with `DataProtectionScope.CurrentUser`. Store the protected key as a base64 string in `%LOCALAPPDATA%/Greg.Xrm.Command/encryption.key`. Remove `AesKey` and `AesIV` from Resources.resx. Generate a random IV per encryption operation and prepend it to the ciphertext.
- **Verify:** Old connection strings can be decrypted (migration path). New connection strings use DPAPI. No static key exists in source code.
- **Auto-Review:** `/conductor-review`. If any static key/IV remains in Resources.resx, reject.

### Phase 2: Remove NU1900 Suppression & Fix Vulnerable Packages
- **Analyze:** Read `Directory.Build.props` line 35 — `NU1900` in NoWarn. Read `Directory.Packages.props` — list all packages. Run `dotnet list package --vulnerable` to find actual CVEs.
- **Implement:** Remove `NU1900` from NoWarn. Add `<WarningsAsErrors>$(WarningsAsErrors);NU1901;NU1902;NU1903;NU1904</WarningsAsErrors>`. Update any packages with known vulnerabilities.
- **Verify:** `dotnet build` passes without NU1900 suppression. No vulnerable packages remain. Run `dotnet list package --vulnerable` confirms clean.
- **Auto-Review:** `/conductor-review`.

### Phase 3: Pin All GitHub Actions to Commit SHAs
- **Analyze:** Read ALL 24 workflow files. List every third-party action using `@v` version tags. Cross-reference against the GitHub expert's list of 20+ unpinned actions.
- **Implement:** Replace each `@vN` or `@vX.Y.Z` tag with the corresponding commit SHA plus a version comment. For `dependabot/fetch-metadata`, `docker/*`, `SonarSource/sonarcloud-github-action`, `actions/dependency-review-action`, `actions/stale`, `actions/labeler`, `actions/first-interaction`.
- **Verify:** No `@v` tags remain in any workflow file (except git tags). All references use `@<sha>` format.
- **Auto-Review:** `/conductor-review`.

### Phase 4: Resolve Dependabot/Renovate Contradiction
- **Analyze:** Read `SECURITY.md` lines 27-29 — claims no Dependabot. Read `.github/dependabot.yml` — exists. Read `dependency-metadata.yml` and `auto-merge.yml` — both target Dependabot only.
- **Implement:** Delete `.github/dependabot.yml`. Update `dependency-metadata.yml` and `auto-merge.yml` to also handle `renovate[bot]`. Update `SECURITY.md` to accurately reflect Renovate-only strategy.
- **Verify:** `SECURITY.md` and actual config are consistent. Only one dependency manager is configured.
- **Auto-Review:** `/conductor-review`.

### Phase 5: Fix SonarCloud Placeholder Values
- **Analyze:** Read `sonar-project.properties` and `.github/workflows/sonarcloud.yml` — find `your-org` placeholders.
- **Implement:** Replace with the actual SonarCloud organization key and project key.
- **Verify:** SonarCloud analysis can connect and report results.
- **Auto-Review:** `/conductor-review`.

### Phase 6: Add Missing Permissions Blocks
- **Analyze:** Read each workflow without top-level `permissions:` (api-compat.yml, branch-protection.yml, _build.yml, ci.yml, release.yml, package-release.yml).
- **Implement:** Add `permissions: contents: read` at the top level of each. For `_build.yml` as a reusable workflow, add `permissions: contents: read` explicitly.
- **Verify:** All 24 workflows have explicit `permissions:` blocks. No workflow relies on the `write-all` default.
- **Auto-Review:** `/conductor-review`.

## Rollback
Any phase that fails review: revert the change, fix the issue, re-verify, re-run review. Only proceed on pass.
