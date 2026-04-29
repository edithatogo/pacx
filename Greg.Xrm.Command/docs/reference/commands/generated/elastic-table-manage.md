# elastic-table manage

Manage Elastic Table retention policies and scaling.

## Usage

```powershell
pacx elastic-table manage
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--retention` | r | string? | False | Retention period (e.g., '90d', '6m', '1y'). |
| `--scale` | s | string? | False | Scale capacity setting. |
| `--changelog` |  | bool? | False | Enable/disable change feed tracking. |
| `--show` |  | bool | False | Show current elastic table configuration. |
| `--format` | f | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/ElasticTable/ElasticTableManageCommand.cs`
