# column add double

Creates a double column.

## Usage

```powershell
pacx column add double
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--min` | min | double? | False | For number type columns indicates the minimum value for the column. |
| `--max` | max | double? | False | For number type columns indicates the maximum value for the column. |
| `--precision` | p | int? | False | For money or decimal type columns indicates the precision for the column. |
| `--imeMode` | ime | ImeMode | False | Indicates the input method editor (IME) mode for the column. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/Create/CreateDoubleCommand.cs`
