# Implementation Plan: Stub Killer Phase 2

## Overview
De-fake remaining architecture-level stubs that require external API integration.

## Stub Inventory

### High Impact (Implementable)
| Area | Files | Issue |
|------|-------|-------|
| **Env lifecycle** | `EnvCommandExecutors.cs`, `EnvLifecycleCommandExecutors.cs` | 6 commands stub: `env create`, `clone`, `reset`, `backup`, `restore`, `capacity` |
| **Quality gate** | `QualityGateCommandExecutor.cs` | Note: "This requires pac CLI to be installed" — needs Process invocation |

### Documented as Premium-only (Deferred)
| Area | Files | Issue |
|------|-------|-------|
| **Tabular deploy** | `TabularCommandExecutors.cs` | Requires Power BI XMLA endpoint (Premium/Embedded only) |
| **Tabular translate** | `TabularAdvancedCommandExecutors.cs` | Requires Power BI REST API for translation CRUD |
| **Tabular roles/perspectives** | `TabularAdvancedCommandExecutors.cs` | Requires Power BI REST API or XMLA |
| **PCF test/publish** | `PcfCommandExecutors.cs` | Requires PCF test harness / pac CLI |
| **Power Pages** | `PagesCommandExecutors.cs` | Dry-run flags on real implementations |

## Success Criteria
- `pacx env create` calls Power Platform Admin API
- `pacx env reset` calls Power Platform Admin API
- `pacx env capacity` calls Power Platform Admin API
- `pacx quality-gate --run-check` invokes `pac solution check` via Process
- Tabular/Power BI stubs documented with Premium requirements

## Phases

### Phase 1: BAP API Env Lifecycle Methods (DONE)
- [x] Task: Add environment CRUD methods to `IPowerPlatformAdminClient`
- [x] Task: Implement in `PowerPlatformAdminClient` (create, get, list, reset, copy, backup, restore, capacity)
- [x] Task: Wire `EnvCreateCommandExecutor` to call BAP API
- [x] Task: Wire `EnvResetCommandExecutor` to call BAP API
- [x] Task: Wire `EnvCapacityReportCommandExecutor` to call BAP API
- [x] Task: Keep backup/restore/clone as documented stubs (requires async job polling)

### Phase 2: Quality Gate pac CLI Integration (DONE)
- [x] Task: Add `--run-check` flag to run `pac solution check` via Process
- [x] Task: Parse `pac solution check` output directly
- [x] Task: Wire into existing quality gate pipeline

### Phase 3: Documentation (DONE)
- [x] Task: Document all remaining stubs with their requirements
- [x] Task: Tabular/Power BI stubs marked as Premium-only
- [x] Task: Update Known Stubs in tracks.md
