# Implementation Plan: Auth Modernization

## Anti-Stub Preamble
Every task produces a working auth flow with tests. No ROPC remains as the only interactive option. Tokens survive process restarts. No task is complete without verification.

## Overview
Add device code flow for interactive auth, managed identity for cloud scenarios, persistent token cache, and unify the three separate token providers into one.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: Add Device Code Flow
- **Analyze:** Read `TokenProvider.cs` — current OAuth flow silently fails with `MsalUiRequiredException`. Read MSAL `AcquireTokenWithDeviceCode` API docs. Read `Program.cs` for console access.
- **Implement:** Add `DeviceCodeFlowTokenProvider` strategy. On `MsalUiRequiredException`, fall back to device code flow: print the device code URL to `IOutput`, poll for authentication. Wire into the `ITokenProvider` chain.
- **Verify:** User sees device code URL on fresh auth. Token acquired successfully. Tests with mocked MSAL.
- **Auto-Review:** `/conductor-review`.

### Phase 2: Add Managed Identity Support
- **Analyze:** Read `DefaultAzureCredential` from `Azure.Identity`. Understand IMDS endpoint flow.
- **Implement:** Add `ManagedIdentityTokenProvider` that tries `DefaultAzureCredential` before client credentials. Add `--auth-type managed-identity` to `auth create`.
- **Verify:** Token acquired via managed identity in Azure environments. Graceful fallback when not in Azure.
- **Auto-Review:** `/conductor-review`.

### Phase 3: Implement Persistent Token Cache
- **Analyze:** Read `TokenCache.cs` — `SaveAsync` is a no-op. Read MSAL's `Microsoft.Identity.Client.Extensions.Msal` for cross-platform encrypted cache.
- **Implement:** Wire MSAL's built-in `StorageCreationProperties` with DPAPI encryption (Windows), Keychain (macOS), libsecret (Linux). Implement `SaveAsync` using `ProtectedData.Protect()`. Remove the no-op.
- **Verify:** Tokens survive process restart. Cache is encrypted on disk. Tests confirm rehydration.
- **Auto-Review:** `/conductor-review`.

### Phase 4: Unify FormsTokenProvider with ITokenProvider
- **Analyze:** Read `FormsTokenProvider.cs` (raw HttpClient OAuth), `FormsTokenManager.cs` (MSAL-based), and `TokenProvider.cs` (main MSAL). Understand the duplication.
- **Implement:** Replace `FormsTokenProvider` with the main `ITokenProvider`. Add `https://forms.office.com/.default` as a supported resource. Remove the raw HttpClient flow. Remove ROPC path.
- **Verify:** Forms API works through the unified `ITokenProvider`. No raw token endpoint calls outside MSAL.
- **Auto-Review:** `/conductor-review`.

## Rollback
Any phase that breaks auth: revert, fix, re-verify. Auth failures block all downstream functionality.
