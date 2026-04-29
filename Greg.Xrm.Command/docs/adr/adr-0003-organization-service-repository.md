# ADR 0003: Organization Service Repository

## Status

Accepted

## Context

Commands need Dataverse connections, but direct connection creation in executors would make command code harder to test and would spread authentication behavior across the codebase.

## Decision

Use `IOrganizationServiceRepository` as the connection boundary for command and service code that needs Dataverse access.

## Consequences

- Tests can provide mocked `IOrganizationServiceAsync2` instances.
- Authentication and connection lookup stay centralized.
- Commands should depend on the repository or higher-level services rather than constructing Dataverse clients directly.
