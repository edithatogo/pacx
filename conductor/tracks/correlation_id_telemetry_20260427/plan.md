# Implementation Plan: Correlation ID & Telemetry

## Overview
Add correlation ID generation at the command-entry layer and propagate it through all Azure/Power Platform calls and logs to enable end-to-end tracing.

## Scope
- Generate a correlation ID (UUIDv4) for each CLI invocation.
- Include it in:
  - Console output (e.g., `CorrelationID: <id>`)
  - HTTP headers to Dataverse/AI Builder where supported (`Ocp-Apim-Subscription-Key`, `x-correlation-id`, or custom header)
  - Structured logs (if logging framework is used)
- Ensure retries and downstream calls retain the same correlation ID.

## Improvements
- Consistent tracing across microservice boundaries.
- Faster support diagnosis and SLA tracking.
- Minimal code change if added at the pipeline level.

## Success Criteria
- Every command prints a Correlation ID in the first/last line.
- All HTTP client calls include the correlation ID in headers (if API supports it).
- Correlation ID is included in any error/execution summary.

## Next Steps
1. Add a `CorrelationIdMiddleware` or CLI hook to generate and store the ID.
2. Update HTTP client creation to attach the header.
3. Update command executors to output the ID.
4. Add tests verifying ID presence in mocks and traces.

---

## Phases (task decomposition, added 2026-04-21)

**Ordering note:** This track must land BEFORE `ai_builder_connectors_improved_20260427` and `ai_wrapper_service_20260427` — they both depend on `ICorrelationIdProvider`.

### Phase 1: Abstraction
- [ ] Task: `ICorrelationIdProvider { string Current { get; } }` in `Greg.Xrm.Command.Core.Diagnostics`.
- [ ] Task: `AmbientCorrelationIdProvider` — AsyncLocal<string> storage; generates UUIDv7 per CLI invocation (v7 is time-ordered, easier to eyeball in logs).
- [ ] Task: Register in DI at command-runner entry.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 2: HTTP propagation
- [ ] Task: `CorrelationIdHandler : DelegatingHandler` adds `x-ms-client-request-id`, `x-correlation-id` headers to every outgoing request.
- [ ] Task: Register via `HttpClientFactory` for every named client (Dataverse, AI Builder, Fabric, Power BI).
- [ ] Task: Tests with a fake handler confirming headers present.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 3: Output integration
- [ ] Task: `IOutput.WriteCorrelationHeader()` called at command start; printed at end in `--verbose` mode.
- [ ] Task: Included in error summaries regardless of verbosity.
- [ ] Task: Override via `--correlation-id <uuid>` for external orchestrators (GitHub Actions, Azure DevOps) that want to bring their own ID.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 4: OpenTelemetry bridge (stretch)
- [ ] Task: When `cli_ux_20260421` telemetry phase has shipped, emit correlation ID as `trace.id` on OTLP spans.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 5: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.