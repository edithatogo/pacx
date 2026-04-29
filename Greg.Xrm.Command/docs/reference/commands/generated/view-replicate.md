# view replicate

Replicates the structure (layout and sort order) of a given view.

## Usage

```powershell
pacx view replicate
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--table` | t | string? | False | The name of the table that contains the view. Required only if the view name is not unique in the system. |
| `--onto` | o | string? | False | The name of the views that should be updated with the new layout, separated by comma (,). If not specified, all saved queries except for lookup views will be updated. If * is provided as value, all views will be updated (lookup views included). |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Views/ReplicateCommand.cs`
