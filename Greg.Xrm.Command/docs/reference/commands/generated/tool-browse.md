# tool browse

Browse the PACX tool catalog.

## Usage

```powershell
pacx tool browse
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--catalog` | c | string | False | Path to the tool catalog JSON file. |
| `--category` |  | string? | False | Filter by category. |
| `--query` | q | string? | False | Filter by name, provider, kind, or summary. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Tool/BrowseCommand.cs`

