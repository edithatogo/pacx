# tabular deploy

Deploy a .bim file to a Power BI semantic model (idempotent).

## Usage

```powershell
pacx tabular deploy
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--bim` | b | string | True | Path to the .bim file. |
| `--workspace` | w | string | True | Power BI workspace name or ID. |
| `--dataset` | d | string? | False | Dataset name. Creates new if not exists. |
| `--mode` | m | string | False | Connection mode: auto, xmla, rest. Auto-detects based on workspace tier. |
| `--dry-run` |  | bool | False | Show what would be deployed without actually deploying. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Tabular/TabularCommands.cs`
