# solution component moveAll

Moves a solution component from a solution to another (unmanaged) solution.

## Usage

```powershell
pacx solution component moveAll
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--fromSolution` | from | string? | False | The unique name of the source solution. If not provided, the default solution will be used. |
| `--toSolution` | to | string? | False | The unique name of the target solution. If not provided, the default solution will be used. |
| `--addRequiredComponents` | r | bool | False | To be specified only if `componentType` is `Entity`. Indicates whether other solution components that are required by the solution component should also be added to the unmanaged solution. |
| `--includeSubcomponents` | is | bool | False | To be specified only if `componentType` is `Entity`. Indicates whether the subcomponents should be included. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Solution/ComponentMoveAllCommand.cs`
