# Specification: Implement an MCP Server

## Overview
This track introduces a Model Context Protocol (MCP) server integration for PACX. This allows AI agents (like Claude or Gemini) to use PACX commands directly as tools.

## Scope
- **Architecture:** Implement a new project `Greg.Xrm.Command.Mcp` or integrate within the existing structure.
- **Protocol:** Support basic MCP functionality:
    - `listTools`: Expose PACX commands as tools.
    - `callTool`: Execute a PACX command and return the result.
- **Discovery:** Automatically map PACX `[Command]` definitions to MCP tool definitions.
- **Execution:** Leverage the existing `ICommandExecutorFactory` to run commands.

## Constraints
- Target .NET 8.
- Use a lightweight MCP library for .NET if available, or implement the JSON-RPC spec.
- Ensure authentication is handled securely (leveraging existing PACX auth profiles).
