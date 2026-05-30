# settings export

List settings defined for the current environment

## Usage

```powershell
pacx settings export
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--origin` | o | Origin | False | Indicates if the list of settings to retrieve is the whole list of settings, or just the settings in the specified solution. |
| `--filter` | f | Which | False | Indicates if the list of settings to retrieve should include all settings, or only visible settings. |
| `--format` | fmt | Format | False | The format of the output. Default is Text. Use Json to get the output in JSON format. |
| `--output` | out | string? | False | If the format specified is Json or Excel, this is the name of the file where the output will be saved. For Excel files is mandatory. For JSON, if not specified, the output will be written only to the console. |
| `--run` | r | bool | False | Allows to specify whether the output file should be automatically opened or not. |
| `--solution` | s | string? | False | The solution to get the settings from. If not specified, the default solution is considered. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Settings/ExportCommand.cs`

