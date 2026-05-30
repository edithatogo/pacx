# tool source add

Registers a new tool source feed.

## Usage

```powershell
pacx tool source add
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--type` | t | string | False | Source type (nuget, mcp, npm). |
| `--pat` |  | string? | False | Personal Access Token for private feeds. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Tool/SourceAddCommand.cs`

