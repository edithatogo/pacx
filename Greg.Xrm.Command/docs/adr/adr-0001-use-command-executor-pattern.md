# ADR 0001: Use Command Executor Pattern

## Status

Accepted

## Context

PACX has many commands across multiple Power Platform domains. Command parsing, validation, execution, and output need clear boundaries.

## Decision

Represent CLI inputs as command types and execute them through `ICommandExecutor<T>` implementations.

## Consequences

- Commands stay testable without invoking the full CLI.
- New command areas can be added with predictable structure.
- Shared behavior should live in services rather than in one executor calling another executor.
