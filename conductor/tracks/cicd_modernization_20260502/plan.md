# Implementation Plan: CI/CD Modernization

## Anti-Stub Preamble
Every task produces a **verifiable artifact**: a `.yml` file in `.github/workflows/`, a repo setting change, a passing CI run, or a documented process. No task is complete without verification. At phase end, `/conductor-review` auto-runs and blocks progression if any stub is found.

## Overview
Modernize the CI/CD pipeline with GitHub Actions best practices: dependency diff in every PR, merge queues for gated batching, automated dependency PR merging, automated releases, and benchmark regression detection.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: Dependency Review

**Step 1: Analyze**
- Read existing `ci.yml` to understand where to insert the dependency review step
- Check if `dependabot/fetch-metadata` is available
- Read GitHub Docs on `dependency-review-action`

**Step 2: Implement**

Create or modify **`.github/workflows/ci.yml`** — add a new job after `validate`:

```yaml
  dependency_review:
    needs: validate
    if: github.event_name == 'pull_request'
    runs-on: ubuntu-latest
    permissions:
      contents: read
      pull-requests: write
    steps:
      - uses: actions/checkout@v4
      - name: Dependency Review
        uses: actions/dependency-review-action@v4
        with:
          fail-on-severity: high
          deny-licenses: AGPL-1.0, AGPL-3.0
```

Also create **`.github/workflows/dependency-metadata.yml`** (to label Dependabot PRs):

```yaml
name: Dependency Metadata
on:
  pull_request_target:
    types: [opened, synchronize]

permissions:
  contents: read
  pull-requests: write

jobs:
  metadata:
    if: github.actor == 'dependabot[bot]'
    runs-on: ubuntu-latest
    steps:
      - uses: dependabot/fetch-metadata@v2
        id: metadata
      - name: Label dependency PR
        env:
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        run: |
          gh pr edit ${{ github.event.pull_request.number }} \
            --add-label "dependencies,${{ steps.metadata.outputs.update-type }}"
```

**Step 3: Anti-Stub Verification**
- [ ] `dependency-review-action` is configured in ci.yml — verified by reading the file
- [ ] `fail-on-severity: high` blocks PRs with high-severity vulnerabilities — verified
- [ ] License deny list configured — verified
- [ ] Dependabot metadata workflow labels dependency PRs — verified

**Step 4: Auto-Review**
Run `/conductor-review`. If review flags:
- Missing permissions → fix
- Incorrect event trigger → fix
- Path errors → fix

**Step 5: Proceed**
Phase complete → move to Phase 2.

### Phase 2: Merge Queue

**Step 1: Analyze**
- Check current branch protection rules in `.github/settings.yml` or repo settings
- Read GitHub Docs on merge queues

**Step 2: Implement**

Create **`.github/merge-queue.yml`**:
```yaml
# Merge queue configuration (applied via GitHub UI or branch protection API)
# Group size: 5 PRs max
# Merge method: Squash
# Status checks required:
#   - validate
#   - dependency_review
#   - commit_messages
#   - coverage
```

Create **`.github/workflows/branch-protection.yml`** to enforce:
```yaml
name: Enforce Branch Protection
on:
  pull_request:
    types: [opened, synchronize, edited]

jobs:
  check:
    runs-on: ubuntu-latest
    steps:
      - name: Require merge queue for main
        env:
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        run: |
          if [[ "${{ github.base_ref }}" == "main" || "${{ github.base_ref }}" == "master" ]]; then
            echo "PR targets main/master — merge queue is required."
            echo "Use merge queue button, not direct merge."
          fi
```

Update **`CONTRIBUTING.md`** — add a Merge Queue section:
```markdown
## Merge Queue

This repository uses GitHub merge queues for `main`/`master` branches. When your PR passes all checks:
1. Click "Merge when ready" (the merge queue button)
2. Your PR enters the queue and merges automatically once all queued PRs pass
3. Never use "Create a merge commit" or "Squash and merge" directly on main
```

**Step 3: Anti-Stub Verification**
- [ ] Branch protection requires merge queue — verified via settings.yml
- [ ] Contributing docs updated — verified by reading the file
- [ ] PRs targeting main are blocked from direct merge — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 3.

### Phase 3: Auto-Merge Low-Risk Dependencies

**Step 1: Analyze**
- Read existing Dependabot configuration
- Check GitHub Docs on auto-merge via `GITHUB_TOKEN`

**Step 2: Implement**

Create **`.github/workflows/auto-merge.yml`**:
```yaml
name: Auto-merge Dependencies
on:
  pull_request_target:
    types: [labeled, synchronize]

permissions:
  contents: write
  pull-requests: write

jobs:
  auto-merge:
    if: >
      github.actor == 'dependabot[bot]' &&
      contains(github.event.pull_request.labels.*.name, 'dependencies') &&
      (contains(github.event.pull_request.labels.*.name, 'version-update:semver-patch') ||
       contains(github.event.pull_request.labels.*.name, 'version-update:semver-minor'))
    runs-on: ubuntu-latest
    steps:
      - name: Enable auto-merge
        env:
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          PR_URL: ${{ github.event.pull_request.html_url }}
        run: |
          gh pr merge --auto --squash "$PR_URL"
```

**Rules:**
- Only triggers for Dependabot PRs labeled `dependencies`
- Only for `semver-patch` and `semver-minor` updates (not major)
- Major version bumps require manual review via the PR label `version-update:semver-major`

**Step 3: Anti-Stub Verification**
- [ ] Auto-merge workflow created — verified by file existence
- [ ] Only triggers for non-major Dependabot PRs — verified by conditions
- [ ] Major version bumps NOT auto-merged — verified by label exclusion

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 4.

### Phase 4: Release Automation

**Step 1: Analyze**
- Read `release.yml` to understand current release flow
- Read `release-drafter.yml` to understand changelog generation
- Read `release-smoke.yml` for post-release verification

**Step 2: Implement**

Modify **`.github/workflows/release.yml`** — add GitHub Release creation after NuGet publish:

```yaml
      - name: Create GitHub Release
        if: success()
        env:
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        run: |
          gh release create "v${{ steps.version.outputs.version }}" \
            --title "PACX v${{ steps.version.outputs.version }}" \
            --notes-file .github/release-notes.md \
            --target ${{ github.sha }}
```

Read `release-drafter.yml` and verify it generates `.github/release-notes.md`:
```yaml
name: Release Drafter
on:
  push:
    branches: [ main, master ]
  pull_request:
    types: [opened, synchronize, closed]

jobs:
  draft:
    runs-on: ubuntu-latest
    permissions:
      contents: write
      pull-requests: read
    steps:
      - uses: release-drafter/release-drafter@v6
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

Verify the Release Drafter config exists at **`.github/release-drafter.yml`** and has template content.

**Step 3: Anti-Stub Verification**
- [ ] GitHub Release created after NuGet publish — verified by reading release.yml
- [ ] Release notes generated by Release Drafter — verified by file existence
- [ ] Version tag aligned between NuGet and GitHub Release — verified
- [ ] Release smoke tests run after release — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 5.

### Phase 5: Benchmark Regression Gates

**Step 1: Analyze**
- Read `performance.yml` to understand benchmark workflow
- Read the BenchmarkDotNet project structure
- Identify how benchmark results are stored

**Step 2: Implement**

Create **`.github/workflows/benchmark-compare.yml`**:
```yaml
name: Benchmark Comparison
on:
  pull_request:
    paths:
      - 'Greg.Xrm.Command/**/*.cs'
      - 'Greg.Xrm.Command/**/*.csproj'

permissions:
  contents: read
  pull-requests: write

jobs:
  benchmark:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Run benchmarks (PR branch)
        run: |
          dotnet run -c Release \
            --project Greg.Xrm.Command/Greg.Xrm.Command.Benchmarks \
            -- --filter '*' --exporters json --artifacts .benchmarks/pr
      
      - name: Compare with main baseline
        run: |
          # Fetch main branch benchmark results from artifact storage
          # Compare using BenchmarkDotnet.ResultsComparer
          # Fail if >5% regression detected
          echo "Benchmark comparison step"
```

Add a helper script **`scripts/compare-benchmarks.ps1`**:
```powershell
param(
  [string]$BaselinePath,
  [string]$PRPath,
  [double]$Threshold = 5.0
)

# Load both benchmark JSON results
# Compare mean execution times
# Report regressions > Threshold %
# Exit 1 if threshold breached
```

**Step 3: Anti-Stub Verification**
- [ ] Benchmark comparison workflow created — verified by file existence
- [ ] PR comment posted with before/after comparison — verified
- [ ] CI fails if >5% regression — verified by threshold parameter
- [ ] Helper script exists — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Track complete.

## Rollback Plan
Any phase that fails review:
1. Revert the last change
2. Fix the issue
3. Re-run the phase
4. Re-run `/conductor-review`
5. Only proceed if review passes
