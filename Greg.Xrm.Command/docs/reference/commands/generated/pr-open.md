# pr open

Create a pull request from the current branch.

## Usage

```powershell
pacx pr open
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--title` | t | string? | False | PR title. Defaults to current branch name. |
| `--body` | b | string? | False | PR body/description. Defaults to auto-generated from commits. |
| `--repo` | r | string? | False | Repository in owner/repo format. Auto-detected from git remote if not provided. |
| `--base` |  | string? | False | Base branch to merge into. Defaults to master. |
| `--token` |  | string? | False | GitHub personal access token. Reads from GITHUB_TOKEN env var if not provided. |
| `--dry-run` |  | bool | False | Show what would be created without actually creating it. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Pr/PrOpenCommand.cs`
