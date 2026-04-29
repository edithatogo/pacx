# optionset add

Adds a value on an option set (global or local) or on a StatusCode field.

## Usage

```powershell
pacx optionset add
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string | False | To be specified only if you want to update a global option set. It's the schema name of the global option set. |
| `--table` | t | string | False | To be specified only if you want to update a local option set. It's the schema name of the table that contains the option set. |
| `--column` | c | string | False | To be specified only if you want to update a local option set. It's the schema name of the column that contains the option set. |
| `--value` | v | int? | False | The value of the option to add. If not provided, is generated automatically. |
| `--color` | col | string? | False | The exadecimal color code (e.g. #FF5733) of the color to set on the option. The leading # is not mandatory. |
| `--statecode` | sc | int | False | If the optionset you're trying to update with the new value is a statuscode field, you must provide also the statecode value. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/OptionSet/AddCommand.cs`

