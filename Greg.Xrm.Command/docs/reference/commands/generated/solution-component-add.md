# solution component add

Adds a solution component to an unmanaged solution.

## Usage

```powershell
pacx solution component add
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--solution` | s | string? | False | The unique name of the solution. If not provided, the default solution will be used. |
| `--addRequiredComponents` | r | bool | False | To be specified only if `componentType` is `Entity`. Indicates whether other solution components that are required by the solution component should also be added to the unmanaged solution. |
| `--includeSubcomponents` | is | bool | False | To be specified only if `componentType` is `Entity`. Indicates whether the subcomponents should be included. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Solution/ComponentAddCommand.cs`

