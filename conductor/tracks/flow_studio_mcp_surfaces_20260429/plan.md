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

### Phase 1: Browse & Inspect
- [ ] Task: MCP tools for flow listing and search.
- [ ] Task: MCP tool for flow definition inspection.
- [ ] Task: MCP tool for action/trigger detail.
- [ ] Task: Tests.

### Phase 2: Run & Monitor
- [ ] Task: MCP tools for run history and status.
- [ ] Task: MCP tools for triggering, cancelling, resubmitting.
- [ ] Task: MCP tool for run metrics.
- [ ] Task: Tests.

### Phase 3: Debug & Govern
- [ ] Task: MCP tool for error details and troubleshooting.
- [ ] Task: MCP tools for policy/compliance checks.
- [ ] Task: Tests.

### Phase 4: Catalog & Guide
- [ ] Task: Flow catalog generation and output.
- [ ] Task: `docs/guides/flow-studio-mcp.md` with setup instructions.
- [ ] Task: Review pass.

### Phase 5: PR Lifecycle
- [ ] Task: Upstream PR; merge.
