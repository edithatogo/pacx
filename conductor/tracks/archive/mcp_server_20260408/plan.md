# Implementation Plan: Implement an MCP Server

Historical archive note:
- This plan describes the original in-process MCP work.
- The active MCP architecture now lives in a separate host boundary under `Greg.Xrm.Command.Mcp`.

## Phase 1: Research and Scaffolding
- [x] Task: Identify a suitable .NET MCP library or define the JSON-RPC models. *Initial custom JSON-RPC approach replaced by official MCP SDK host boundary.*
- [x] Task: Create the scaffolding for the MCP server (e.g., a new command `pacx mcp start`). *McpStartCommand + Executor (NEW), now delegating to the separate MCP host.*
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase

## Phase 2: Tool Mapping
- [x] Task: Implement logic to discover all registered PACX commands. *Uses IReadOnlyCommandRegistry.Commands; the new host reads from the shared registry.*
- [x] Task: Implement logic to convert `[Command]` and `[Option]` metadata to MCP Tool definitions. *The new MCP host converts command metadata to SDK tool definitions.*
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase

## Phase 3: Command Execution
- [x] Task: Implement the `callTool` handler to parse arguments and execute commands via `CommandRunner`. *The new MCP host resolves PACX executors directly and returns captured output.*
- [x] Task: Capture `IOutput` stream and return it as the tool's result. *OutputToMemory is used in the new host for captured output.*
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase

## Phase 4: Finalization
- [x] Task: Verify the MCP server with an actual AI agent or MCP inspector. *Historical verification completed against the original handler; current host supersedes it.*
- [ ] Task: Open a PR against issue #162. *Blocked: the repository moved to `edithatogo/pacx`, and this account lacks `CreatePullRequest` permission there.*
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase. *Blocked until the PR can be created or an account with write/PR permission handles closure.*
