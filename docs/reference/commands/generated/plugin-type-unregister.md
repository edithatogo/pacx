# plugin type unregister

Removes a plugin type registration. If the type has registered steps, the command will list them and stop unless --force is specified.

## Usage

```powershell
pacx plugin type unregister
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--id` | id | Guid? | False | The unique identifier of the plugin type to be removed. |
| `--name` | n | string? | False | Name (or partial name) of the plugin type to be removed. Resolved via fuzzy search. |
| `--force` | f | bool | False | When specified, all registered steps (and their images) belonging to this plugin type are deleted before removing the type itself. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Plugin/Type/UnregisterCommand.cs`

