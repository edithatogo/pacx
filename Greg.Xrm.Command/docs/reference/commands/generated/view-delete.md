# view delete

Deletes a given view

## Usage

```powershell
pacx view delete
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--table` | t | string? | False | The name of the table that contains the view. Required only if the view name is not unique in the system. |
| `--type` | q | QueryType1 | False | The type of query. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Views/DeleteCommand.cs`
