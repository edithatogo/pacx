# solution diff

Compare two solutions or environments and report component differences.

## Usage

```powershell
pacx solution diff
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--source` | s | string | True | Source solution unique name or environment. |
| `--target` | t | string | True | Target solution unique name or environment. |
| `--type` | solution | string | False | Diff type: solution (compare two solutions) or environment (compare two environments). |
| `--format` | f | string | False | Output format: table, json. |
| `--component-type` |  | string? | False | Filter by component type: entity, attribute, relationship, plugin, workflow, webresource, etc. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Solution/SolutionDiffCommand.cs`

