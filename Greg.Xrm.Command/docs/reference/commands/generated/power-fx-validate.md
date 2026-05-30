# power-fx validate

Validate a Power Fx expression or expression file.

## Usage

```powershell
pacx power-fx validate
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--expression` | e | string? | False | Power Fx expression to validate. |
| `--file` | f | string? | False | Text, JSON, or YAML file containing expressions. |
| `--table` | t | string? | False | Optional Dataverse table logical name for future binding-aware validation. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/PowerFx/PowerFxCommands.cs`

