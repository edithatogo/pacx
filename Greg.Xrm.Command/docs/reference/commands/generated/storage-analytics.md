# storage analytics

Table-by-table storage analysis with cleanup recommendations.

## Usage

```powershell
pacx storage analytics
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--top` |  | int | False | Show top N tables by storage usage. Default is 20. |
| `--format` | f | string | False | Output format: table, json. |
| `--recommendations` | r | bool | False | Include cleanup recommendations. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Storage/StorageAnalyticsCommand.cs`
