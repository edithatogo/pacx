# PACX maximal harness engineering verification protocol

## Automated gate

Run the following from the repository root:

```powershell
./scripts/Test-Harness.ps1
./scripts/Test-UpstreamSync.ps1
git diff --check
zizmor --pedantic --min-severity high --no-exit-codes .github/workflows
```

The upstream command is expected to report `structurally-divergent` while the two repositories have no common ancestor. That result is evidence, not a failure requiring a merge.

## Manual GitHub verification

Complete the checklist in `docs/verification/pacx-harness-manual-verification.md` against the pushed branch. Record workflow run URLs and observed outcomes in the Conductor checkpoint before marking this track complete.

## Stop conditions

- Do not approve or merge a workflow that reintroduces `pull_request_target` with untrusted code execution.
- Do not force-merge or force-rebase the unrelated upstream history.
- Do not run release, rollback, tag-signing, or package-publishing workflows against production without the required environment approval.
