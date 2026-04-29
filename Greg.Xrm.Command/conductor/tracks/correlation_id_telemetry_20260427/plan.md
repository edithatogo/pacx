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
- [x] Task: `ICorrelationIdProvider { string Current { get; } }` in `Greg.Xrm.Command.Core.Diagnostics`.
- [x] Task: `AmbientCorrelationIdProvider` — AsyncLocal<string> storage; generates UUIDv7 per CLI invocation (v7 is time-ordered, easier to eyeball in logs).
- [x] Task: Register in DI at command-runner entry.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 2: HTTP propagation
- [x] Task: `CorrelationIdHandler : DelegatingHandler` adds `x-ms-client-request-id`, `x-correlation-id` headers to every outgoing request.
- [x] Task: Register via `HttpClientFactory` for every named client (Dataverse, AI Builder, Fabric, Power BI).
- [x] Task: Tests with a fake handler confirming headers present.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 3: Output integration
- [x] Task: `IOutput.WriteCorrelationHeader()` called at command start; printed at end in `--verbose` mode.
- [x] Task: Included in error summaries regardless of verbosity.
- [x] Task: Override via `--correlation-id <uuid>` for external orchestrators (GitHub Actions, Azure DevOps) that want to bring their own ID.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 4: OpenTelemetry bridge (stretch)
- [ ] Task: When `cli_ux_20260421` telemetry phase has shipped, emit correlation ID as `trace.id` on OTLP spans. Deferred until that telemetry phase exists.
- [x] Task: Run local focused verification for completed correlation ID implementation.

### Phase 5: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.

---

## Validation Snapshot (2026-04-28)

Audited implementation now present in the working tree:
- `Greg.Xrm.Command.Core/Diagnostics/ICorrelationIdProvider.cs`
- `Greg.Xrm.Command.Core/Diagnostics/AmbientCorrelationIdProvider.cs`
- `Greg.Xrm.Command.Core/Services/CorrelationIdHandler.cs`
- `Greg.Xrm.Command/Program.cs` DI registration and `--correlation-id` override
- `Greg.Xrm.Command/Bootstrapper.cs` startup correlation header
- `Greg.Xrm.Command/CommandRunnerBase.cs` error/failure correlation header
- `Greg.Xrm.Command.Interfaces/Services/Output/IOutput.cs`
- `Greg.Xrm.Command.Core/Services/Output/OutputToConsole.cs`
- `Greg.Xrm.Command.Core/Services/Output/OutputToMemory.cs`
- `Greg.Xrm.Command.Core.TestSuite/Diagnostics/CorrelationIdTelemetryTests.cs`
- `Greg.Xrm.Command.Core.TestSuite/Services/Telemetry/CorrelationIdTests.cs`

Validation commands:
- `dotnet10.ps1 build .\Greg.Xrm.Command.Core.TestSuite\Greg.Xrm.Command.Core.TestSuite.csproj --no-restore --verbosity minimal -p:UseSharedCompilation=false -p:BuildInParallel=false` passed with 0 warnings and 0 errors.
- `dotnet10.ps1 test .\Greg.Xrm.Command.Core.TestSuite\Greg.Xrm.Command.Core.TestSuite.csproj --no-build --filter "FullyQualifiedName~CorrelationId" --verbosity minimal` passed - 7 passed.

Remaining blockers:
- Phase 4 OpenTelemetry bridge remains future work pending the `cli_ux_20260421` telemetry phase; it is not required for the dependent 2026-04-27 tracks.
