# Implementation Plan: Flow Studio Capability Surfaces

## Overview
Translate the most useful Flow Studio patterns into PACX-native PACX command surfaces for build, debug, monitor, and governance workflows.

## Scope
- Packaged PACX commands for flow authoring and inspection.
- Flow debug and monitor operations exposed through PACX-friendly commands.
- Governance helpers for approvals, diagnostics, and run-history inspection.
- Tests for command registration and invocation behavior.

## Improvements
- Make flow operations more automatable for agents and scripts.
- Give PACX a stronger Power Automate workflow story around the same capability class Flow Studio exposes.
- Reduce the gap between the CLI and agent-driven flow operations.

## Success Criteria
- Flow-related PACX commands can be discovered and invoked consistently.
- The command surface has clear outputs and failure modes.
- Tool registration and invocation are covered by tests.

## Phases

### Phase 1: Command model
- [x] Task: Define the packaged command layout for flow operations.
- [x] Task: Add core authoring and inspection surfaces.
- [x] Task: Add tests for registration and invocation.

### Phase 2: Debug and governance
- [x] Task: Add run/debug/monitor helpers.
- [x] Task: Add governance and diagnostics helpers.
- [x] Task: Document the MCP flow surface.
