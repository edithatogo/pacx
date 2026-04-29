# MCP Migration

This repo now has a separate MCP host boundary in `Greg.Xrm.Command.Mcp`.

Current state:
- `Greg.Xrm.Command.Core` still owns the command model, registry, storage, and executors.
- `Greg.Xrm.Command.Mcp` hosts the official MCP SDK-based server boundary.
- `mcp start` now delegates to the new MCP launcher rather than starting the legacy handler directly.

Next cut:
- Move MCP-only integration tests onto the new host.
- Keep the CLI and MCP host focused on protocol-neutral shared infrastructure only.

Recommended follow-up:
- Keep the CLI and MCP host in separate app projects.
- Share only protocol-neutral command infrastructure between them.
- Do not use a git submodule unless the MCP code must remain vendor-pinned.
