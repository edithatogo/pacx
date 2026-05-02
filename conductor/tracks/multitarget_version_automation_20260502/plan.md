# Implementation Plan: Multi-target .NET & Version Automation

## Anti-Stub Preamble
Every task in this track produces a **verifiable artifact** (a file change, a CI green build, a passing test). No task may be marked [x] without its verification step passing. At the end of every phase, the implementation MUST survive `/conductor-review` — if review finds any stub, incomplete implementation, or unhandled edge case, the phase is NOT complete and MUST be fixed before proceeding.

## Overview
Multi-target .NET 10 (stable) + .NET 11 (preview) so the project builds on any machine with either SDK. Automate SDK and package version tracking so the repo always builds against the latest stable toolchain without manual intervention.

## Phase Structure
Each phase follows this sequence:
1. **Analyze** — Read all files that need changes. Verify current state.
2. **Implement** — Make changes. No stubs, no TODOs, no "future work" comments.
3. **Verify** — Check every changed file compiles conceptually (pattern consistency, namespace resolution, type availability).
4. **Review** — `/conductor-review` auto-triggers. If any issue found, fix and re-review.
5. **Proceed** — Mark phase complete, advance to next phase.

## Phases

### Phase 1: Multi-target .NET 10 + 11

**Step 1: Analyze**
- Read `global.json` to understand current SDK pin
- List all `.csproj` files in the solution
- Check each csproj's `<TargetFramework>` element
- Read `_build.yml` for the CI build matrix
- Read `ci.yml` for the CI pipeline
- Read `release.yml` for the release pipeline

**Step 2: Implement**

Files to modify:

**`global.json`:**
```json
{
  "sdk": {
    "version": "11.0.100-preview.3.26207.106",
    "rollForward": "latestFeature",
    "allowPrerelease": true
  }
}
```
- `rollForward: "latestFeature"` allows falling back to .NET 10 SDK if the exact preview isn't installed
- `allowPrerelease: true` allows preview SDKs to satisfy the requirement

**Every `.csproj` in `Greg.Xrm.Command/`:**
Change `<TargetFramework>net11.0</TargetFramework>` to `<TargetFrameworks>net10.0;net11.0</TargetFrameworks>`

Do NOT change `Greg.Xrm.Command.Mcp/Greg.Xrm.Command.Mcp.csproj` — it already targets a different framework (net10.0) and is intentionally separate.

List of csproj files to update:
- `Greg.Xrm.Command/Greg.Xrm.Command.csproj`
- `Greg.Xrm.Command/Greg.Xrm.Command.Core/Greg.Xrm.Command.Core.csproj`
- `Greg.Xrm.Command/Greg.Xrm.Command.Core.TestSuite/Greg.Xrm.Command.Core.TestSuite.csproj`
- `Greg.Xrm.Command/Greg.Xrm.Command.Interfaces/Greg.Xrm.Command.Interfaces.csproj`
- `Greg.Xrm.Command/Greg.Xrm.Command.Core.IntegrationTests/Greg.Xrm.Command.Core.IntegrationTests.csproj`
- `Greg.Xrm.Command/Greg.Xrm.Command.Plugin.Automation/Greg.Xrm.Command.Plugin.Automation.csproj`

**`.github/workflows/_build.yml`:**
Add `dotnet_version: ['10.0.x', '11.0.100-preview.3.26207.106']` to the strategy matrix.

**`.github/workflows/ci.yml`:**
Update the matrix to include both versions:
```yaml
matrix:
  os: [windows-latest, ubuntu-latest, macos-latest]
  dotnet_version: ['10.0.x', '11.0.100-preview.3.26207.106']
```

**`.github/workflows/release.yml`:**
Use .NET 10 for the release build (stable). Add a note that release artifacts should target net10.0 for maximum compatibility.

**Step 3: Anti-Stub Verification**
- [ ] Every csproj was changed — not a single one missed
- [ ] `global.json` rollForward is set — verified by reading the file
- [ ] CI matrix includes both .NET 10 and 11 — verified by reading ci.yml
- [ ] `_build.yml` passes both TFM build targets

**Step 4: Auto-Review**
Run `/conductor-review`. If review flags:
- Missing csproj changes → go back to Step 2
- CI config errors → fix immediately
- Package incompatibility → add conditional PackageReference per TFM

**Step 5: Proceed**
All checks pass → mark Phase 1 complete, advance to Phase 2.

### Phase 2: SDK Roll Forward & Auto-Update

**Step 1: Analyze**
- Read `global.json` again
- Check `actions/setup-dotnet` usage across all workflow files
- Read Dependabot config (`.github/dependabot.yml`)
- Check for Renovate config

**Step 2: Implement**

**All workflows using `actions/setup-dotnet`:**
Ensure they use `dotnet-version: '11.0.x'` (latest 11.x patch) rather than a pinned preview version.

Create or update **`.github/dependabot.yml`:**
```yaml
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/Greg.Xrm.Command/"
    schedule:
      interval: "weekly"
    open-pull-requests-limit: 10
    labels:
      - "dependencies"
      - "nuget"
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
    open-pull-requests-limit: 10
```

**Step 3: Anti-Stub Verification**
- [ ] `dotnet-version` uses wildcard (`11.0.x`) not pinned version — verified
- [ ] Dependabot config covers both NuGet and GitHub Actions — verified
- [ ] Labels are applied correctly — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Advance to Phase 3.

### Phase 3: Compatibility Validation

**Step 1: Analyze**
- Check for APIs that differ between .NET 10 and .NET 11
- Search for any `#if` or conditional compilation that may be needed

**Step 2: Implement**
Add `Directory.Build.props` at repo root for shared configuration:
```xml
<Project>
  <PropertyGroup>
    <TargetFrameworks>net10.0;net11.0</TargetFrameworks>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

Check each project for framework-specific API usage. The .NET 10 → 11 transition is minor (it's a preview) so no conditional compilation should be needed. If any is found, add:
```xml
<ItemGroup>
  <PackageReference Include="System.Text.Json" Version="9.0.0" Condition="'$(TargetFramework)' == 'net10.0'" />
</ItemGroup>
```

**Step 3: Anti-Stub Verification**
- [ ] `Directory.Build.props` created with shared settings
- [ ] No conditional `#if` needed unless API mismatch confirmed
- [ ] Build passes on both TFMs conceptually

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Track complete.

## Rollback Plan
If multi-targeting causes build failures:
1. Revert `global.json` — return to exact version pin
2. Revert csproj changes — return to single TFM
3. Document the blocker in an ADR
4. Re-attempt after the blocker is resolved
