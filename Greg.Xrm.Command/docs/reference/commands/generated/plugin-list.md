# plugin list

Searches plugin registrations by name and displays results as a hierarchy tree (package → assembly → type → step → image).

## Usage

```powershell
pacx plugin list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--level` | l | SearchLevel? | False | Restrict the search to a single hierarchy level: Package, Assembly, Type, or Step. When omitted, all levels are searched. Images are always shown as children of matching steps. |
| `--solution` | s | string? | False | Unique name of a solution. When provided, only plugin registrations that belong to the specified solution are shown. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Plugin/ListCommand.cs`

