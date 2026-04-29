# pr merge

Merge a GitHub PR when CI passes and reviews are approved.

## Usage

```powershell
pacx pr merge
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--number` | n | int | True | PR number to merge. |
| `--repo` | r | string? | False | Repository in owner/repo format. Auto-detected from git remote if not provided. |
| `--token` |  | string? | False | GitHub personal access token. Reads from GITHUB_TOKEN env var if not provided. |
| `--method` | m | MergeMethod | False | Merge method: squash, merge, rebase. |
| `--wait` |  | bool | False | Wait for CI checks to pass before merging. Polls every 30s, times out after 30min. |
| `--delete-branch` | d | bool | False | Delete the source branch after merging. |
| `--dry-run` |  | bool | False | Check mergeability without actually merging. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Pr/PrMergeCommand.cs`
