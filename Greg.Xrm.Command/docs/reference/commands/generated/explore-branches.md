# explore branches

List branches from a GitHub repository.

## Usage

```powershell
pacx explore branches
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--owner` | o | string? | False | Repository owner. Defaults to the current git remote if omitted. |
| `--repo` | r | string? | False | Repository name. Defaults to the current git remote if omitted. |
| `--format` | f | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Explore/ExploreCommands.cs`
