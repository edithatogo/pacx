# mcp start

Start the MCP server to expose PACX commands to AI agents.

## Usage

```powershell
pacx mcp start
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--port` | p | int | False | Port to listen on. Default is 3000. |
| `--transport` | t | string | False | Transport type: stdio, http. Default is stdio. |
| `--host` | localhost | string | False | Host to bind to (for HTTP transport). |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Mcp/McpStartCommand.cs`

