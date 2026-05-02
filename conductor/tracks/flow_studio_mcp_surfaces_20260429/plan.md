# Implementation Plan: Flow Studio MCP Surfaces

## Overview
MCP server surfaces for Flow Studio integration — browse, debug, govern, inspect, monitor, run flows.

## Scope
- 12 MCP flow command files covering:
  - Browse: discover flows, list, search
  - Debug: get error details, troubleshoot runs
  - Govern: policy checks, compliance
  - Inspect: flow definition, action details
  - Monitor: run history, status, metrics
  - Run: trigger, cancel, resubmit
- Flow catalog for MCP tool discovery.
- User guide for Flow Studio MCP integration.

## Improvements
- Rich MCP surface for Power Automate management.
- Enables AI agent integration via MCP protocol.

## Success Criteria
- All 12 MCP commands implemented and tested.
- Flow catalog JSON generated and discoverable.
- `docs/guides/flow-studio-mcp.md` with setup and usage.

## Phases

### Phase 1: Browse & Inspect (DONE)
- [x] Task: MCP tools for flow listing and search.
- [x] Task: MCP tool for flow definition inspection.
- [x] Task: MCP tool for action/trigger detail.
- [x] Task: Tests.

### Phase 2: Run & Monitor (DONE)
- [x] Task: MCP tools for run history and status.
- [x] Task: MCP tools for triggering, cancelling, resubmitting.
- [x] Task: MCP tool for run metrics.
- [x] Task: Tests.

### Phase 3: Debug & Govern (DONE)
- [x] Task: MCP tool for error details and troubleshooting.
- [x] Task: MCP tools for policy/compliance checks.
- [x] Task: Tests.

### Phase 4: Catalog & Guide (DONE)
- [x] Task: Flow catalog generation and output.
- [x] Task: `docs/guides/flow-studio-mcp.md` with setup instructions.
- [x] Task: Review pass.

