# data init-schema-from-solution

Generate schema definition from an existing solution.

## Usage

```powershell
pacx data init-schema-from-solution
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--solution` | s | string | True | Solution unique name. |
| `--output` | o | string | True | Output file path for schema (YAML or JSON). |
| `--format` | f | string | False | Output format: yaml, json. |
| `--include` | i | string[]? | False | Comma-separated list of entity logical names to include. |
| `--exclude-relationships` |  | bool | False | Exclude relationship definitions from schema. |
| `--exclude-optionsets` |  | bool | False | Exclude option set definitions from schema. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Data/DataCommands.cs`

