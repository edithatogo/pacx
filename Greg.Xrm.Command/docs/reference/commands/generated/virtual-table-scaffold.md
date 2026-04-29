# virtual-table scaffold

Scaffold virtual table definitions from external data sources.

## Usage

```powershell
pacx virtual-table scaffold
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--tables` | t | string[]? | False | Comma-separated list of external table names to scaffold. |
| `--prefix` | p | string? | False | Logical name prefix for virtual tables. Defaults to data source type. |
| `--solution` | s | string? | False | Solution unique name to add virtual tables to. |
| `--dry-run` |  | bool | False | Show what would be created without actually creating. |
| `--format` | f | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/VirtualTable/VirtualTableScaffoldCommand.cs`
