# optionset delete

Removes a value from an option set (global or local) or from a StatusCode field.

## Usage

```powershell
pacx optionset delete
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string | False | To be specified only if you want to update a global option set. It's the schema name of the global option set. |
| `--table` | t | string | False | To be specified only if you want to update a local option set. It's the schema name of the table that contains the option set. |
| `--column` | c | string | False | To be specified only if you want to update a local option set. It's the schema name of the column that contains the option set. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/OptionSet/DeleteCommand.cs`
