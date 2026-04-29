# optionset update

Updates an existing value on an option set (global or local).

## Usage

```powershell
pacx optionset update
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string | False | To be specified only if you want to update a global option set. It's the schema name of the global option set. |
| `--table` | t | string | False | To be specified only if you want to update a local option set. It's the schema name of the table that contains the option set. |
| `--column` | c | string | False | To be specified only if you want to update a local option set. It's the schema name of the column that contains the option set. |
| `--label` | l | string? | False | The new label to set on the option. |
| `--color` | col | string? | False | The exadecimal color code (e.g. #FF5733) of the new color to set on the option. The leading # is not mandatory. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/OptionSet/UpdateCommand.cs`
