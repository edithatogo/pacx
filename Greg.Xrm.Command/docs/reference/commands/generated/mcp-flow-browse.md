# mcp flow browse

Browse Flow Studio-style flow operations.

## Usage

```powershell
pacx mcp flow browse
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--catalog` | c | string | False | Path to the flow MCP catalog JSON file. |
| `--category` |  | string? | False | Filter by category. |
| `--query` | q | string? | False | Filter by name, provider, kind, summary, or operation. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Mcp/FlowBrowseCommand.cs`

