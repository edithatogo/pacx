# column add integer

Creates an integer column.

## Usage

```powershell
pacx column add integer
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--min` | min | double? | False | For number type columns indicates the minimum value for the column. |
| `--max` | max | double? | False | For number type columns indicates the maximum value for the column. |
| `--intFormat` | if | IntegerFormat | False | For whole number type columns indicates the integer format for the column.(default: None) |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/Create/CreateIntegerCommand.cs`

