# pcf publish

Publish a PCF component without full solution import.

## Usage

```powershell
pacx pcf publish
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--path` | p | string? | False | Path to the PCF project directory. |
| `--solution` | s | string? | False | Solution unique name to publish into. |
| `--dry-run` |  | bool | False | Show what would be published without actually publishing. |
| `--pac-path` | pac | string | False | Path to the pac CLI executable. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Pcf/PcfCommands.cs`

