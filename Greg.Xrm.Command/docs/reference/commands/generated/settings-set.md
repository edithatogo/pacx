# settings set

Sets the value of a setting in the current environment

## Usage

```powershell
pacx settings set
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--app` | a | string? | False | The unique name of the app to set the value for (if setting the value at app level). |
| `--solution` | s | string? | False | The solution where to save the created setting. If not specified, the default solution is considered. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Settings/SetValueCommand.cs`
