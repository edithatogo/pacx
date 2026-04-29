# custom-api create

Create a Custom API (Custom Action) in Dataverse.

## Usage

```powershell
pacx custom-api create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--display-name` |  | string? | False | Display name. Defaults to the unique name. |
| `--description` |  | string? | False | Description of the Custom API. |
| `--input` |  | string[]? | False | Input parameter in format 'Type:Name' (e.g., 'String:Target'). Repeatable. |
| `--output` |  | string[]? | False | Output parameter in format 'Type:Name' (e.g., 'Entity:Result'). Repeatable. |
| `--binding-type` | Global | string | False | Binding type: Global, Entity, EntityCollection. |
| `--entity` | e | string? | False | Entity logical name for Entity/EntityCollection binding. |
| `--solution` | s | string? | False | Solution unique name to add the Custom API to. |
| `--execute-plugin` |  | string? | False | Plugin type name to execute this Custom API. |
| `--is-function` |  | bool | False | Whether the Custom API is a function (read-only, no side effects). |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/CustomApi/CustomApiCreateCommand.cs`

