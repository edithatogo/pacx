# column delete

Deletes a column from a given Dataverse table.

## Usage

```powershell
pacx column delete
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--force` | f | bool | False | (preview) If specified, tries to force the deletion removing the column dependencies (thanks @daryllabar) |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/DeleteCommand.cs`

