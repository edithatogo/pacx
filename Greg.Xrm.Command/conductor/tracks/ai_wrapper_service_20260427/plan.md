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
- `AiBuilderService` methods return structured results/errors.
- Retry/backoff remains centralized in the existing AI Builder API client and is called through the wrapper service.
- All commands using the service include correlation ID through shared command output and HTTP propagation.

## Next Steps
1. Create `AiBuilderService` class in `Greg.Xrm.Command`.
2. Wire existing executors to use the service.
3. Add config for retry/backoff (e.g., max attempts, initial backoff).
4. Add correlation ID generation at command entry point.
5. Add integration tests for failure scenarios.

---

## Phases (task decomposition, added 2026-04-21)

### Phase 1: Interface
- [x] Task: Define `IAiBuilderService` with model list, train, publish, and form processor configuration methods returning structured operation results.
- [x] Task: Put interface in `Greg.Xrm.Command.Core.Services.AiBuilder`.
- [x] Task: Run local focused verification.

### Phase 2: Implementation
- [x] Task: `AiBuilderService` implements the interface against existing `IAiBuilderApiClient`.
- [x] Task: Retry/backoff remains in `AiBuilderApiClient.SendWithRetryAsync`, reused through the wrapper.
- [x] Task: Correlation ID attached to outgoing HTTP headers through the registered `CorrelationIdHandler`.
- [x] Task: Structured success/error mapping.
- [x] Task: Unit tests for wrapper success and failure mapping.
- [x] Task: Run local focused verification.

### Phase 3: Migrate existing executors
- [x] Task: Re-point `AiModelListCommandExecutor`, `AiModelTrainCommandExecutor`, `AiModelPublishCommandExecutor`, and `AiFormProcessorConfigureCommandExecutor` at `IAiBuilderService`.
- [x] Task: Keep existing factory-based constructors as compatibility shims during migration.
- [x] Task: Tests.
- [x] Task: Run local focused verification.

### Phase 4: Configurability
- [ ] Task: `pacx.config.json` entries: `aiBuilder.retry.maxAttempts`, `aiBuilder.retry.initialBackoffMs`, `aiBuilder.http.timeoutMs`. Deferred to config track/work.
- [ ] Task: `--retry-max <n>`, `--retry-backoff-ms <n>` CLI overrides. Deferred to CLI/config track/work.
- [x] Task: Run local focused verification for completed wrapper migration.

### Phase 5: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.

---

## Validation Snapshot (2026-04-28)

Validated under local .NET SDK 10.0.202 via `Greg.Xrm.Command/dotnet10.ps1`.

- Test-suite build passes with 0 errors using `--no-restore --verbosity minimal -p:UseSharedCompilation=false -p:BuildInParallel=false`.
- Combined focused tests pass: `dotnet10.ps1 test ... --no-build --filter "FullyQualifiedName~AiBuilder|FullyQualifiedName~AiModel|FullyQualifiedName~AiFormProcessor|FullyQualifiedName~Connector|FullyQualifiedName~CorrelationId"` - 53 passed.
- Added `IAiBuilderService`, `AiBuilderService`, and structured `AiBuilderOperationResult` types.
- Registered `IAiBuilderService` in DI.
- Repointed AI Builder executors to the wrapper service while preserving compatibility constructors.
