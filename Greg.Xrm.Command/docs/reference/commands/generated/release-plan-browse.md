# release-plan browse

Browse Microsoft release-plan families for Power Platform, Dynamics 365, and related products.

## Usage

```powershell
pacx release-plan browse
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--catalog` | c | string | False | Path to the release-plan catalog JSON file. |
| `--category` |  | string? | False | Filter by category (e.g. 'Power Platform', 'Dynamics 365', 'Power BI', 'AI'). |
| `--query` | q | string? | False | Filter by name, category, or summary. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/ReleasePlan/ReleasePlanBrowseCommand.cs`

