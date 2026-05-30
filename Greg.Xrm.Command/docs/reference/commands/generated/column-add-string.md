# column add string

Creates a string column.

## Usage

```powershell
pacx column add string
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--format` | f | StringFormat | False | The format of the string attribute (default: Text). |
| `--len` | l | int? | False | The maximum length for string attribute. |
| `--autoNumber` | an | string? | False | In case of autonumber field, the autonumber format to apply. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/Create/CreateStringCommand.cs`

