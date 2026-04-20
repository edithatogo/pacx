# Implementation Plan: AI Builder & Custom Connectors — Wrapper Service Track

## Overview
Introduce a shared wrapper service (e.g., `AiBuilderService`) that encapsulates resilient, observable, and consistent interactions with AI Builder and connector APIs.

## Scope
- Centralize retry/backoff, exponential backoff policies, and circuit-breaker patterns.
- Attach correlation IDs to every request for end-to-end tracing.
- Provide helper methods: `trainModelWithRetry`, `publishModelWithId`, `configureFormProcessorSafe`, `validateConnectorSchema`, `testConnectorWithTrace`.
- Standardize error mapping and user-friendly messages.

## Improvements
- Consistent retry/backoff across all AI/connector operations.
- Correlation ID propagation for logs and diagnostics.
- Schema validation before sending requests.
- Easier maintenance and testability.

## Constraints
- Must be backward compatible with existing command executors.
- Should not introduce runtime dependencies not already present.

## Success Criteria
- `AiBuilderService` methods accept correlation ID and return structured results/errors.
- Retry/backoff policies are configurable per-environment.
- All commands using the service include correlation ID in output.

## Next Steps
1. Create `AiBuilderService` class in `Greg.Xrm.Command`.
2. Wire existing executors to use the service.
3. Add config for retry/backoff (e.g., max attempts, initial backoff).
4. Add correlation ID generation at command entry point.
5. Add integration tests for failure scenarios.