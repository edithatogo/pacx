# Implementation Plan: Security Deep Clean

## Anti-Stub Preamble
Every task removes a security vulnerability or information disclosure risk. No TODO comments. No deferred fixes. `/conductor-review` at every phase boundary.

## Overview
Fix medium-priority security issues identified by the security expert: SSRF via unvalidated Forms API inputs, error response body leakage, MCP exception information disclosure, XmlDocument DTD processing, AutoUpdater PowerShell injection pattern, OutputRedactor regex gaps.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: FormsApiClient Input Validation
- **Analyze:** Read `FormsApiClient.cs` — `tenantId`, `ownerId`, `formId`, `templateId` are interpolated into URL paths without validation.
- **Implement:** Validate `tenantId` matches GUID regex `^[0-9a-fA-F-]{36}$`. Validate `formId` is alphanumeric + hyphens. Use `Uri.EscapeDataString()` on all user-supplied URL segments. Reject invalid inputs with clear error messages.
- **Verify:** Tests with invalid tenant IDs, path traversal attempts, and malicious inputs are all rejected. Valid inputs work normally.
- **Auto-Review:** `/conductor-review`.

### Phase 2: Redact API Error Response Bodies
- **Analyze:** Read `FormsApiClient.cs`, `PowerAutomateClient.cs`, `PowerPlatformAdminClient.cs` — error messages include full API response bodies (`content`).
- **Implement:** Parse error responses and extract only the error message, not the full body. Use `try { JsonDocument.Parse(content); return error.message } catch { return "API error"; }`. For non-JSON responses, truncate to 200 characters.
- **Verify:** Stack traces, server paths, and tokens are not exposed in error messages. Useful error info is preserved.
- **Auto-Review:** `/conductor-review`.

### Phase 3: Sanitize MCP Exception Output
- **Analyze:** Read `McpServerLauncher.cs` — exception messages are returned verbatim as MCP tool error output.
- **Implement:** Sanitize exception messages before returning via MCP: strip connection strings, tokens, file paths. Return a generic "Command failed: {exception type}" message with details logged to stderr only.
- **Verify:** MCP tool errors don't leak sensitive information. Full details available in stderr logs.
- **Auto-Review:** `/conductor-review`.

### Phase 4: Harden XmlDocument Usage
- **Analyze:** Search for `new XmlDocument()` in production code — `PluginPackageReader.cs`, `Replicator.cs`.
- **Implement:** Set `XmlResolver = null` on all XmlDocument instances to prevent XXE attacks. Add `XmlReader.Create()` with `XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }` pattern.
- **Verify:** XML parsing rejects DTD declarations. Tests confirm XXE attempts fail.
- **Auto-Review:** `/conductor-review`.

### Phase 5: Harden AutoUpdater PowerShell Invocation
- **Analyze:** Read `AutoUpdater.cs` — uses `powershell.exe -ExecutionPolicy unrestricted` with string interpolation for command construction.
- **Implement:** Replace PowerShell invocation with direct .NET API calls (e.g., `Assembly.LoadFrom`, `Process.Start` with specific arguments). Remove the `-ExecutionPolicy unrestricted` bypass. If PowerShell is required, construct arguments via array, not string interpolation.
- **Verify:** Update process doesn't use unrestricted execution policy. No string interpolation for command construction.
- **Auto-Review:** `/conductor-review`.

## Rollback
Any security fix that breaks functionality: revert, fix with less invasive approach, re-verify. Security fixes must not break the user-facing command.
