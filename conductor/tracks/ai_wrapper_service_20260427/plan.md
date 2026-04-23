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

---

## Phases (task decomposition, added 2026-04-21)

### Phase 1: Interface
- [ ] Task: Define `IAiBuilderService` with methods: `TrainModelWithRetryAsync`, `PublishModelAsync`, `ConfigureFormProcessorAsync`, `ValidateConnectorSchemaAsync`, `TestConnectorAsync`. Each takes `CancellationToken`, `CorrelationId`, and returns a `Result<T, AiBuilderError>` discriminated union (use `OneOf` NuGet).
- [ ] Task: Put interface in `Greg.Xrm.Command.Core.Services.AiBuilder`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 2: Implementation
- [ ] Task: `AiBuilderService` implements the interface against existing `aiBuilderClient`.
- [ ] Task: Retry policy via `Microsoft.Extensions.Http.Resilience`.
- [ ] Task: Correlation ID attached to every outgoing HTTP header (via named `HttpClient` + `DelegatingHandler`).
- [ ] Task: Structured error mapping (Dataverse fault → `AiBuilderError.DataverseFailure`, HTTP 4xx → `AiBuilderError.ClientError`, etc.).
- [ ] Task: Unit tests with a fake HTTP handler.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 3: Migrate existing executors
- [ ] Task: Re-point `AiModelListCommandExecutor`, `AiModelTrainCommandExecutor`, `AiModelPublishCommandExecutor` at `IAiBuilderService`.
- [ ] Task: Keep old client call as a fallback behind `--no-wrapper` during migration; remove in a follow-up.
- [ ] Task: Tests.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 4: Configurability
- [ ] Task: `pacx.config.json` entries: `aiBuilder.retry.maxAttempts`, `aiBuilder.retry.initialBackoffMs`, `aiBuilder.http.timeoutMs`.
- [ ] Task: `--retry-max <n>`, `--retry-backoff-ms <n>` CLI overrides.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 5: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.