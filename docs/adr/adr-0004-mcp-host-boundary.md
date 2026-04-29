# ADR 0004: MCP Host Boundary

## Status

Accepted

## Context

The MCP server has different runtime behavior from normal CLI command execution. It needs long-running process behavior, tool registration, and host-level logging without weakening the CLI command runner.

## Decision

Keep MCP hosting behind a dedicated host boundary and launcher service rather than mixing server lifetime logic into individual command executors.

## Consequences

- CLI command execution remains short-lived and testable.
- MCP server startup can own its host configuration.
- Shared business logic should stay in services that both CLI commands and MCP tools can call.
