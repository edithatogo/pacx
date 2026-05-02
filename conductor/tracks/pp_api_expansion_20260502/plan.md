# Implementation Plan: Power Platform API Expansion

## Anti-Stub Preamble
Every task produces a typed service method or a new command. No generic `JsonDocument` returns for new methods. No stubs. No "TODO" comments. `/conductor-review` at every phase boundary.

## Overview
Fill critical Power Platform API gaps: solution import/export, DLP policy management, Environment Groups, Managed Environments, API version currency, typed service clients.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: Solution Import/Export/Clone/Patch/Upgrade
- **Analyze:** Read existing `Commands/Solution/` — command set. Read `ServiceClient.ImportSolutionAsync()` and `ExportSolutionAsync()` SDK methods. Read `CloneAsSolutionRequest`, `CreateSolutionPatchRequest`, `StageAndUpgradeRequest`.
- **Implement:** Add `solution import` — wraps `ImportSolutionAsync` with overwrite/publish options. Add `solution export` — wraps `ExportSolutionAsync` with managed/unmanaged options. Add `solution clone`, `solution patch`, `solution upgrade`. Add `solution pack` and `solution unpack` by wrapping `pac solution pack/unpack` via Process.
- **Verify:** Each command has executor tests with mocked SDK calls. Parse tests. Integration tests with real solution file.
- **Auto-Review:** `/conductor-review`.

### Phase 2: Add Missing Admin APIs
- **Analyze:** Read current `IPowerPlatformAdminClient` — 11 methods. Research DLP Policy API, Environment Groups API, Managed Environments API, Power Platform Analytics API, Power Platform Inventory API.
- **Implement:** Add DLP policy CRUD (`ListPolicies`, `CreatePolicy`, `UpdatePolicy`, `DeletePolicy`). Add Environment Groups (`ListGroups`, `CreateGroup`, `AssignEnvironment`, `RemoveEnvironment`). Add Managed Environments settings (`GetSettings`, `UpdateSettings`, `EnableManagedEnvironment`, `DisableManagedEnvironment`). Add Analytics (`GetUsageAnalytics`, `GetStorageAnalytics`). Add Inventory (`GetEnvironmentInventory`, `GetConnectorInventory`).
- **Verify:** Each method has typed return types. Tests with mocked responses.
- **Auto-Review:** `/conductor-review`.

### Phase 3: Bump API Versions
- **Analyze:** Read all service clients — BAP uses `2020-10-01` (latest: `2023-06-01`), ProcessSimple uses `2016-11-01` (latest: `2022-09-01`), PowerApps mixed.
- **Implement:** Create `ApiVersions.cs` static class with all version constants. Update BAP to `2023-06-01`. Update ProcessSimple to `2022-09-01`. Update PowerApps to `2023-08-01`. Verify backward compatibility.
- **Verify:** All existing commands work with new API versions. Version constants centralized in one file.
- **Auto-Review:** `/conductor-review`.

### Phase 4: Add Typed Service Clients
- **Analyze:** Read `IPowerBiClient`, `IFabricClient`, `ICopilotStudioClient` — all are generic HTTP wrappers.
- **Implement:** Add typed methods to Power BI (workspace CRUD, dataset operations, report operations). Add typed methods to Fabric (workspace, lakehouse, semantic model). Add typed methods to Copilot Studio (bot CRUD, topic operations, analytics).
- **Verify:** Each new method has a test. Generic HTTP methods are replaced where typed alternatives exist.
- **Auto-Review:** `/conductor-review`.

### Phase 5: PAC CLI Integration
- **Analyze:** Read `PacCliAdapter` concept. Understand where PACX should wrap vs complement pac CLI.
- **Implement:** Create `Services/PacCli/PacCliAdapter.cs` — wraps `pac solution pack`, `pac solution unpack`, `pac solution check`. Add `--pac-path` option to relevant commands. Add `pacx pac` pass-through command.
- **Verify:** Shells out to pac CLI correctly. Error handling for missing pac installation.
- **Auto-Review:** `/conductor-review`.

## Rollback
Any API version change that breaks commands: revert version, document incompatibility, re-attempt with migration path.
