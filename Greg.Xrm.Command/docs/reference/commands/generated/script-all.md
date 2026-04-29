# script all

Generates PACX scripts for all entities with specified custom prefixes.

## Usage

```powershell
pacx script all
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--output` | o | string | False | Output directory for generated files. |
| `--scriptFileName` | script | string | False | Name for the generated PACX script file. |
| `--stateFileName` | state | string | False | Name of the CSV file that will contain the state fields. |
| `--includeStateFields` | i | bool | False | If true, exports the statecode and statuscode fields as CSV. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Script/ScriptAllCommand.cs`
