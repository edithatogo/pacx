# solution component list

Returns the list of components in a given solution.

## Usage

```powershell
pacx solution component list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--solution` | s | string? | False | The name of the solution. If not provided, the default solution will be used. |
| `--format` | f | OutputFormat | False | Chooses how to generate the output. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Solution/ComponentListCommand.cs`
