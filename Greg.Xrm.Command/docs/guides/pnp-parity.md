# pnp/cli-microsoft365 Parity

This guide documents the parity between `pnp/cli-microsoft365` Power Platform commands and their `pacx` equivalents.

> **Reference:** pnp/cli-microsoft365 is a TypeScript/Node.js CLI that served as the API pattern reference for implementing these commands. Each `.ts` file was used to identify the exact REST endpoints, payloads, and response handling patterns.

## Flow (Power Automate) Commands

| pnp command | pacx equivalent | Status |
|---|---|---|
| `flow list` | `pacx flow list` | ✅ Implemented |
| `flow get` | `pacx flow get` | ✅ Implemented |
| `flow export` | `pacx flow export` | ✅ Implemented |
| `flow enable` | `pacx flow enable` | ✅ Implemented |
| `flow disable` | `pacx flow disable` | ✅ Implemented |
| `flow remove` | `pacx flow remove` | ✅ Implemented |
| `flow owner list` | `pacx flow owner list` | ✅ Implemented |
| `flow owner ensure` | `pacx flow owner ensure` | ✅ Implemented |
| `flow owner remove` | `pacx flow owner remove` | ✅ Implemented |
| `flow environment list` | `pacx flow environment list` | ✅ Implemented |
| `flow environment get` | `pacx flow environment get` | ✅ Implemented |
| `flow recyclebinitem list` | `pacx flow recyclebin list` | ✅ Implemented |
| `flow recyclebinitem restore` | `pacx flow recyclebin restore` | ✅ Implemented |

## Power Apps Commands

| pnp command | pacx equivalent | Status |
|---|---|---|
| `pa app list` | `pacx pa app list` | ✅ Implemented |
| `pa app get` | `pacx pa app get` | ✅ Implemented |
| `pa app remove` | `pacx pa app remove` | ✅ Implemented |
| `pa app export` | `pacx pa app export` | ✅ Implemented |
| `pa app consent set` | `pacx pa app consent set` | ✅ Implemented |
| `pa app owner set` | `pacx pa app owner set` | ✅ Implemented |
| `pa app permission list` | `pacx pa app permission list` | ✅ Implemented |
| `pa app permission ensure` | `pacx pa app permission add` | ✅ Implemented |
| `pa app permission remove` | `pacx pa app permission remove` | ✅ Implemented |

## Power Platform Admin Commands

| pnp command | pacx equivalent | Status |
|---|---|---|
| `pp gateway list` | `pacx gateway list` | ✅ Implemented |
| `pp gateway get` | `pacx gateway get` | ✅ Implemented |
| `pp managementapp list` | `pacx managementapp list` | ✅ Implemented |
| `pp tenant settings list` | `pacx tenant settings list` | ✅ Implemented |
| `pp tenant settings set` | `pacx tenant settings set` | ⚠️ Placeholder |
| `pp solution publisher *` | `pacx solution getPublisherList` | ✅ Covered by existing |
| `pp aibuildermodel remove` | — | ⏸️ Deferred |
| `pp copilot remove` | — | ⏸️ Deferred |

> **Legend:** ✅ Implemented, ⚠️ Placeholder / partial, ⏸️ Deferred

## API Resources Used

| API | Base URL | Used By |
|---|---|---|
| Power Automate (Microsoft.ProcessSimple) | `management.azure.com` | `flow *` |
| Power Apps (Microsoft.PowerApps) | `management.azure.com` | `pa app *` |
| Power BI | `api.powerbi.com` | `gateway *` |
| Business App Platform | `management.azure.com` | `managementapp *`, `tenant settings *` |
| Power Apps specific | `api.powerapps.com` | `consent`, `owner`, `permissions` |

## Implementation Notes

- All commands use the standard `[Command]` and `[Option]` attribute pattern for CLI binding.
- Executors follow the `ICommandExecutor<T>` interface with primary constructor DI.
- Service clients use `ITokenProvider` for auth and `IHttpClientFactory` for HTTP.
- All REST API patterns were extracted from `pnp/cli-microsoft365` TypeScript source files.
- The `pa app consent set`, `pa app owner set`, and `pa app permission *` commands use the Power Apps-specific API (`api.powerapps.com`) under the admin scope.
