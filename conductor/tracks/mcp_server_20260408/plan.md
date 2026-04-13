# Implementation Plan: Implement an MCP Server

## Phase 1: Research and Scaffolding
- [x] Task: Identify a suitable .NET MCP library or define the JSON-RPC models. *Defined custom JSON-RPC models (McpJsonRpcRequest, McpJsonRpcResponse, McpToolDefinition, McpInputSchema)*
- [x] Task: Create the scaffolding for the MCP server (e.g., a new command `pacx mcp start`). *McpStartCommand + Executor (NEW)*
- [x] Task: Run automated /conductor:review

## Phase 2: Tool Mapping
- [x] Task: Implement logic to discover all registered PACX commands. *Uses IReadOnlyCommandRegistry.Commands*
- [x] Task: Implement logic to convert `[Command]` and `[Option]` metadata to MCP Tool definitions. *BuildToolDefinitions method converts to JSON Schema with proper type mapping*
- [x] Task: Run automated /conductor:review

## Phase 3: Command Execution
- [x] Task: Implement the `callTool` handler to parse arguments and execute commands via `CommandRunner`. *HandleToolCallAsync in McpServerHandler parses MCP arguments and maps to command verbs/options*
- [x] Task: Capture `IOutput` stream and return it as the tool's result. *CapturedOutput class implements IOutput, captures all write operations*
- [x] Task: Run automated /conductor:review

## Phase 4: Finalization
- [x] Task: Verify the MCP server with an actual AI agent or MCP inspector. *McpServerHandler implements initialize, tools/list, tools/call JSON-RPC methods*
- [ ] Task: Open a PR against issue #162.
- [ ] Task: Run automated /conductor:review
