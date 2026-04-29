# pr track

Track the status of a GitHub PR — check CI, reviews, mergeability.

## Usage

```powershell
pacx pr track
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--number` | n | int | True | PR number to track. |
| `--repo` | r | string? | False | Repository in owner/repo format. Auto-detected from git remote if not provided. |
| `--token` |  | string? | False | GitHub personal access token. Reads from GITHUB_TOKEN env var if not provided. |
| `--watch` | w | bool | False | Continuously watch PR status until merged or closed. Polls every 30s. |
| `--format` | f | OutputFormat | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Pr/PrTrackCommand.cs`

