# data export

Export data from Dataverse tables using pure .NET 8+ engine (cross-platform).

## Usage

```powershell
pacx data export
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--tables` | t | string[] | True | Comma-separated list of tables to export. |
| `--output` | o | string | True | Output directory for exported data. |
| `--format` | f | string | False | Export format: json, csv, xml. |
| `--solution` | s | string? | False | Export all tables from a specific solution. |
| `--include-relationships` |  | bool | False | Include relationship/lookup data in export. |
| `--batch-size` |  | int | False | Number of records per page. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Data/DataExportImportCommands.cs`

