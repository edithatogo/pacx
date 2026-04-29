# key create

Creates an alternative key on a given table.

## Usage

```powershell
pacx key create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string | False | The display name of the key. If not provided, a name will be generated based on the table name. |
| `--schemaName` | sn | string | False | The schema name of the key. If not provided, a name will be generated based on the display name (Format: publisher prefix, underscore, display name all lowercase without spaces and special characters). |
| `--solution` | s | string? | False | The name of the solution where the key will be created. If not provided, the default solution will be used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Key/CreateCommand.cs`

