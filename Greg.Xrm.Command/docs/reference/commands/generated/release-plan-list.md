# release-plan list

List available release plans / roadmap items. Filters by product, status, or category.

## Usage

```powershell
pacx release-plan list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--product` | p | string? | False | Filter by product name (e.g., Teams, SharePoint). |
| `--status` | s | string? | False | Filter by status (e.g., Launched, RollingOut, InDevelopment). |
| `--category` | c | string? | False | Filter by category (e.g., New, Updated, Deprecated). |
| `--max` | m | int | False | Maximum number of results to return (default: 50). |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/ReleasePlan/ReleasePlanCommands.cs`

