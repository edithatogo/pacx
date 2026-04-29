# solution component-move

Move individual components between solutions.

## Usage

```powershell
pacx solution component-move
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--component` | c | string | True | Component unique name or ID to move. |
| `--type` | t | string | True | Component type: entity, attribute, relationship, plugin, workflow, webresource, optionset, etc. |
| `--from` |  | string | True | Source solution unique name. |
| `--to` |  | string | True | Target solution unique name. |
| `--include-dependencies` | d | bool | False | Automatically include dependent components. |
| `--dry-run` |  | bool | False | Show what would be moved without actually moving. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Solution/SolutionComponentMoveCommand.cs`

