# view clone

Creates a copy of a given view.

## Usage

```powershell
pacx view clone
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--new` | n | string | False | The new name of the view. If not provided, a suffix will be set by default. |
| `--table` | t | string? | False | The name of the table that contains the view. Required only if the view name is not unique in the system. |
| `--type` | q | QueryType1 | False | The type of query. |
| `--clean` | c | bool | False | Indicates that during the clone operation, all the filters applied on the previous view must be removed from the new view. |
| `--solution` | s | string? | False | Specifies the name of the solution that will contain the view after the creation. If not specified, the default solution for the current environment is used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Views/CloneCommand.cs`
