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