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
- None — operates on existing Dataverse and Power Platform APIs.

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

---

## Phases (task decomposition, added 2026-04-21)

### Phase 1: Shared resilience helper
- [ ] Task: Depends on `correlation_id_telemetry_20260427` — wait for `ICorrelationIdProvider` to land, OR land a minimal inline `CorrelationIdSource` here and migrate later.
- [ ] Task: Add `ResiliencePolicy` helper using `Microsoft.Extensions.Http.Resilience` (or Polly v8) — exponential backoff with jitter, max attempts, per-status-code handling.
- [ ] Task: Unit tests for retry behavior with simulated 429/503 responses.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 2: `ai model train` — polling & retry
- [ ] Task: Add `--poll-interval <seconds>` (default 5) and `--timeout <seconds>` (default 600) flags.
- [ ] Task: Wrap existing `TrainModelAsync` call in resilience policy.
- [ ] Task: Surface correlation ID on both stdout preamble and final summary.
- [ ] Task: Tests including timeout path.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 3: `ai model publish`
- [ ] Task: Add correlation ID plumbing + retry policy.
- [ ] Task: Emit next-step guidance (`Use 'ai model list' to confirm publication status`).
- [ ] Task: Tests.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 4: `connector validate` — schema pre-validation
- [ ] Task: Depends on `connector_schema_validation_20260427` — use its `ConnectorSchemaValidator`.
- [ ] Task: Bind `--schema-file` flag.
- [ ] Task: Tests for valid/invalid connector definitions.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 5: Form Processor configuration
- [ ] Task: Extend `ConfigureFormProcessorAsync` with schema-validated field + table options.
- [ ] Task: Tests.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 6: PR Lifecycle
- [ ] Task: Open upstream issue; PR per 2-3 phases; `/ralph-loop`; merge.