# plugin step list

Lists plugin steps associated with a plugin assembly, plugin type, table, or solution.

## Usage

```powershell
pacx plugin step list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--assembly` | a | string? | False | Name or GUID of the plugin assembly to filter steps by. |
| `--class` | c | string? | False | Name or GUID of the plugin type to filter steps by. Names support partial matching for fuzzy search. |
| `--table` | t | string? | False | Name of the table to filter steps by (e.g., account, contact). Shows all plugin steps registered for this table. |
| `--solution` | s | string? | False | Name of the solution to filter steps by. Shows all plugin steps from assemblies in the specified solution. If not specified, uses the current default solution. |
| `--showInternalPlugins` | all | bool | False | Include internal system plugin steps (all stages). By default, only user-manageable stages are shown (PreValidation, PreOperation, PostOperation). |
| `--format` | f | OutputFormat | False | Output format for the results. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Plugin/Step/ListCommand.cs`

