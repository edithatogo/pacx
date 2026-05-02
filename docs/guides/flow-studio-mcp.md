# Flow Studio MCP Integration

The Flow Studio MCP surface exposes Power Automate flow management through the Model Context Protocol (MCP), enabling AI agents to browse, debug, govern, inspect, monitor, and run flows.

## Prerequisites

- PACX CLI installed and authenticated
- Access to a Power Platform environment with flows

## Available Commands

All flow MCP commands use the verb structure `pacx mcp flow <action>`:

### Browse

| Command | Description |
|---------|-------------|
| `pacx mcp flow browse` | Browse available flow MCP operations |
| `pacx mcp flow browse --category browse` | Filter browse-category operations |
| `pacx mcp flow browse --query "trigger"` | Search for trigger-related operations |

### Inspect

| Command | Description |
|---------|-------------|
| `pacx mcp flow inspect` | Show flow definition and action details |

### Debug

| Command | Description |
|---------|-------------|
| `pacx mcp flow debug` | Get error details and troubleshooting info |

### Govern

| Command | Description |
|---------|-------------|
| `pacx mcp flow govern` | Run policy and compliance checks |

### Monitor

| Command | Description |
|---------|-------------|
| `pacx mcp flow monitor` | View run history, status, and metrics |

### Run

| Command | Description |
|---------|-------------|
| `pacx mcp flow run` | Trigger, cancel, or resubmit flows |

### Start MCP Server

```powershell
pacx mcp start
```

Starts the MCP server for Flow Studio integration.

## Example Workflows

### Troubleshoot a failed flow

```powershell
pacx mcp flow browse --query "failed"
pacx mcp flow debug --name "My Flow"
pacx mcp flow govern --name "My Flow"
```

### Monitor and manage production flows

```powershell
pacx mcp flow monitor --name "My Flow"
pacx mcp flow run --name "My Flow" --action resubmit
```

## Catalog

The flow MCP catalog at `conductor/flow-mcp-catalog/flows.json` contains all discoverable operations. You can browse it with:

```powershell
pacx mcp flow browse --catalog conductor/flow-mcp-catalog/flows.json
```
