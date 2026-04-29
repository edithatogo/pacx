# quality gate

Parse pac solution check results and fail CI on high severity issues.

## Usage

```powershell
pacx quality gate
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--input` | i | string? | False | Path to the solution check result ZIP or directory. Auto-detects if not provided. |
| `--fail-on` | High | string | False | Minimum severity to fail: Error, High, Medium, Low. |
| `--format` | f | string | False | Output format: table, json. |
| `--solution` | s | string? | False | Run solution check on this solution before gating. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/QualityGate/QualityGateCommand.cs`

