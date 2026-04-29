# Implementation Plan: AI Builder & Custom Connectors — Improved Track

## Overview
Provide CLI management for AI Builder models and custom connectors with enhanced resilience, observability, and usability.

## Scope
- **AI Model Management:** List models with training status/accuracy, trigger training with retry/backoff, publish trained models, and poll with configurable intervals.
- **Form Processor Configuration:** Configure form processing models (document type, fields, tables) with schema validation.
- **Custom Connector Management:** Import/export custom connectors, test with sample payloads, validate against OpenAPI schema (with pre-validation), and include correlation IDs for tracing.

## Improvements Over Previous Track
- Added retry + exponential backoff for transient failures.
- Added `--poll-interval` and `--timeout` for training commands.
- Added operation correlation IDs (telemetry/tracing).
- Added schema pre-validation for connector definitions.
- Improved user guidance after operations (next steps).

## Constraints
- AI Builder training is asynchronous — must support status polling with configurable intervals.
- Custom connector testing requires actual HTTP calls to the connector's backend.
- Connector import/export uses solution component packaging.

## Dependencies
- `upstream_baseline_sync_20260422`
- `correlation_id_telemetry_20260427`
- `connector_schema_validation_20260427`

## Success Criteria
- `pacx ai model train` supports `--poll-interval` and `--timeout`.
- `pacx ai model publish` includes operation ID in output.
- `pacx connector validate` pre-validates schema and returns clear errors.
- All commands emit a correlation ID for log tracing.
- An AI admin can run `pacx ai model list` and see all models with training status.
- A developer can run `pacx connector validate --file ./connector-definition.json` to validate against OpenAPI.
- A form processing model can be fully configured via CLI without the maker portal.

## API Readiness
- **AI Models:** Dataverse Web API (aimodel) + AI Builder API
- **Form Processor:** Dataverse Web API (aibuilder_formprocessing)
- **Custom Connectors:** Dataverse Web API (connector) — import/export as solution components
- **Connector Test:** HTTP calls to connector backend using stored authentication
- **Telemetry:** Correlation IDs included in CLI output and logs

## Implementation Notes
- Use existing `Greg.Xrm.Command` project methods (`TrainModelAsync`, `PublishModelAsync`, `ConfigureFormProcessorAsync`, connector APIs).
- Add wrapper logic for retry/backoff, polling control, correlation IDs, and validation before calling into the core APIs.
- Add unit tests for command executors (mocking the API client) to ensure behavior and error handling.
- Provide clear user guidance after each operation (e.g., "Use `ai model list` to check status").

## Next Steps
1. Implement wrapper logic for `ai model train` with polling controls and retry.
2. Implement wrapper for `ai model publish` with correlation ID.
3. Add schema validation for connector definitions.
4. Add correlation ID handling across all commands.
5. Write unit tests for new/error paths.
6. Update documentation and help text.

## Validation Snapshot (2026-04-28)

Validated under local .NET SDK 10.0.202 via `Greg.Xrm.Command/dotnet10.ps1`.

- `Greg.Xrm.Command.Core` builds successfully with `--no-restore`.
- `Greg.Xrm.Command.Core.TestSuite` builds successfully when shared compilation and parallel project builds are disabled.
- AI Builder focused tests pass: `dotnet10.ps1 test ... --no-build --filter "FullyQualifiedName~AiBuilder|FullyQualifiedName~AiModel|FullyQualifiedName~AiFormProcessor"` — 26 passed.
- Connector focused tests pass: `dotnet10.ps1 test ... --no-build --filter "FullyQualifiedName~Connector"` — 14 passed.
- Correlation ID focused tests pass: `dotnet10.ps1 test ... --no-build --filter "FullyQualifiedName~CorrelationId"` — 7 passed.
- Combined dependency-surface tests pass after final guidance/validation changes: `dotnet10.ps1 test ... --no-build --filter "FullyQualifiedName~AiBuilder|FullyQualifiedName~AiModel|FullyQualifiedName~AiFormProcessor|FullyQualifiedName~Connector|FullyQualifiedName~CorrelationId"` - 51 passed.
- Test-suite build passes under .NET 10 with 0 errors.
- `ai model train`, `ai model publish`, and `ai form-processor configure` emit next-step guidance.
- `ai form-processor configure` validates blank field/table options before creating an API client.

Test harness fixes applied:
- Added `dotnet10.ps1` wrapper to clear stale `MSBuildSDKsPath` and pin the local .NET 10 SDK.
- Added project-reference metadata to avoid the .NET 10 target-framework aggregation failure.
- Copied the test assembly runtime config to `testhost.runtimeconfig.json` and removed the failing native `testhost.exe` apphost so VSTest launches `testhost.dll` through `dotnet`.

---

## Phases (task decomposition, added 2026-04-21)

### Phase 1: Shared resilience helper
- [x] Task: Depends on `correlation_id_telemetry_20260427` — use landed `ICorrelationIdProvider`/HTTP propagation.
- [x] Task: Add retry/backoff helper in the AI Builder API client for transient 408/429/5xx responses.
- [x] Task: Unit tests for retry behavior with simulated 429/503 responses.
- [x] Task: Run local focused verification.

### Phase 2: `ai model train` — polling & retry
- [x] Task: Add `--poll-interval <seconds>` (default 5) and `--timeout <seconds>` (default 600) flags.
- [x] Task: Wrap existing `TrainModelAsync` call in retry/backoff handling.
- [x] Task: Surface correlation ID through shared command output/HTTP propagation.
- [x] Task: Tests including timeout/options paths.
- [x] Task: Run local focused verification.

### Phase 3: `ai model publish`
- [x] Task: Add correlation ID plumbing + retry policy.
- [x] Task: Emit next-step guidance (`Use 'ai model list' to confirm publication status`).
- [x] Task: Tests.
- [x] Task: Run local focused verification.

### Phase 4: `connector validate` — schema pre-validation
- [x] Task: Depends on `connector_schema_validation_20260427` — use its `ConnectorSchemaValidator`.
- [ ] Task: Bind `--schema-file` flag. Deferred to the connector validation future scope.
- [x] Task: Tests for valid/invalid connector definitions.
- [x] Task: Run local focused verification.

### Phase 5: Form Processor configuration
- [x] Task: Extend `ConfigureFormProcessorAsync` with validated field + table options.
- [x] Task: Tests.
- [x] Task: Run local focused verification.

### Phase 6: PR Lifecycle
- [ ] Task: Open upstream issue; PR per 2-3 phases; `/ralph-loop`; merge.
