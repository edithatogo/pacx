# connector import

Import a custom connector from definition file.

## Usage

```powershell
pacx connector import
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the connector definition JSON file. |
| `--solution` | s | string? | False | Solution unique name to import into. |
| `--dry-run` |  | bool | False | Validate definition without importing. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Connector/ConnectorCommands.cs`

