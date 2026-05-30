# column add datetime

Creates a datetime column.

## Usage

```powershell
pacx column add datetime
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--behavior` | dtb | DateTimeBehavior1 | False | For DateTime type columns indicates the DateTimeBehavior of the column. |
| `--format` | dtf | DateTimeFormat | False | For DateTime type columns indicates the DateTimeFormat of the column. |
| `--imeMode` | ime | ImeMode | False | Indicates the input method editor (IME) mode for the column. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/Create/CreateDateTimeCommand.cs`

