# script solution

Generates PACX scripts for all tables in a PowerApps solution.

## Usage

```powershell
pacx script solution
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--output` | o | string | False | Output directory for generated files. |
| `--scriptFileName` | script | string | False | Name for the generated PACX script file. |
| `--stateFileName` | state | string | False | Name of the CSV file that will contain the state fields. |
| `--includeStateFields` | i | bool | False | If true, exports the statecode and statuscode fields as CSV. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Script/ScriptSolutionCommand.cs`

