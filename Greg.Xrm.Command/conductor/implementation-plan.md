# Plan: PACX Full Roadmap Implementation

**Generated**: 2026-04-09

## Overview
Implement all 16 tracks for PACX (Greg.Xrm.Command) — a .NET 8 CLI tool for Dataverse. This includes infrastructure improvements (CI/CD, testing, code quality) and ~80+ new CLI commands across 12 feature domains. Each command follows the established pattern: `[Command]` POCO class + `ICommandExecutor<T>` implementation + parsing test + executor test.

> Note: `upstream_baseline_sync_20260422` is a gating track that must complete before the downstream planning waves begin. Treat it as a prerequisite sync step, not part of the feature waves below.

## Prerequisites
- .NET 8 SDK installed
- Git remote configured (currently blocked by firewall — PR creation will wait)
- 97 existing command executors as reference patterns
- MSTest + Moq test framework established
- Autofac DI container with primary constructors

## Dependency Graph

```
T0 (Upstream baseline sync gate) ─────────────────────────────────────────→ [T2-T58]
T1 (Directory.Build.props fix) ────────────────────────────────────────────→ []
T2 (.editorconfig + mass-format) ──────────────────────────────────────────→ [T1]
T3 (.runsettings) ─────────────────────────────────────────────────────────→ [T1]
T4 (CI pipeline: test + coverage + format) ────────────────────────────────→ [T2, T3]
T5 (Test utilities enhancement) ───────────────────────────────────────────→ []
T57 (Octokit + GitHub client) ─────────────────────────────────────────────→ []

── All feature tracks depend on T4 (CI test execution) AND T5 (test utils) ──
T6-T8 (Plugin test coverage) ──────────────────────────────────────────────→ [T4, T5]
T9-T12 (E2E smoke tests) ──────────────────────────────────────────────────→ [T4, T1]
T13-T17 (spkl parity) ─────────────────────────────────────────────────────→ [T4, T5]
T18-T22 (dataverse gaps) ──────────────────────────────────────────────────→ [T4, T5]
T24, T25 (alm: env-var sync, env diff) ────────────────────────────────────→ [T4, T5]
T26 (alm: solution layer) ─────────────────────────────────────────────────→ [T18, T4, T5]
T27, T29 (flow: run commands, connection list) ────────────────────────────→ [T4, T5]
T28 (flow: get/set-state) ─────────────────────────────────────────────────→ [T27, T4, T5]
T23 (alm: pipeline) ───────────────────────────────────────────────────────→ [T26, T4, T5]
T30 (quality gate) ────────────────────────────────────────────────────────→ [T4, T5]
T31, T32 (solution diff, component move) ──────────────────────────────────→ [T4, T5]
T33 (data engine) ─────────────────────────────────────────────────────────→ [T4, T5]
T34 (schema from solution) ────────────────────────────────────────────────→ [T33, T4, T5]
T35 (mock data seed) ──────────────────────────────────────────────────────→ [T34, T4, T5]
T36-T38 (power pages) ─────────────────────────────────────────────────────→ [T4, T5]
T39 (env create) ──────────────────────────────────────────────────────────→ [T4, T5]
T40, T41 (env clone/reset, backup/restore) ────────────────────────────────→ [T39, T4, T5]
T42, T44, T45 (security, dlp, storage) ────────────────────────────────────→ [T4, T5]
T43 (sharing-report) ──────────────────────────────────────────────────────→ [T42, T4, T5]
T46 (pcf: test/publish) ───────────────────────────────────────────────────→ [T4, T5]
T47 (pcf: version/dependency) ─────────────────────────────────────────────→ [T46, T4, T5]
T48 (tabular: deploy/diff) ────────────────────────────────────────────────→ [T4, T5]
T49 (tabular: validate) ───────────────────────────────────────────────────→ [T48, T4, T5]
T50 (tabular: translate/role) ─────────────────────────────────────────────→ [T48, T4, T5]
T51, T53 (ai builder, connectors) ─────────────────────────────────────────→ [T4, T5]
T52 (ai: form-processor) ──────────────────────────────────────────────────→ [T51, T4, T5]
T54 (ms_forms: auth + MSAL) ───────────────────────────────────────────────→ [T4, T5]
T55, T56 (ms_forms: list/count, export) ───────────────────────────────────→ [T54, T4, T5]
T58 (pr open/track/merge) ─────────────────────────────────────────────────→ [T57, T4, T5]
```

## Parallel Execution Groups

| Wave | Tasks | Can Start When |
|------|-------|----------------|
| 1 | T1 | Immediately (foundational — all others depend) |
| 2 | T2, T3, T5, T57 | Wave 1 complete |
| 3 | T4, T9, T58 | T2+T3+T5 complete, T57 complete |
| 4 | ALL feature tracks (T6-T8, T13-T56) | Wave 3 complete |

**NOTE:** Wave 4 contains ~50 tasks. They should be sequenced by NuGet dependency to avoid .sln file conflicts:
- Group A (no new NuGets): T6-T8, T18-T22, T24-T26, T27, T29, T30, T36, T42, T44, T45, T46
- Group B (new NuGets, separate projects): T13-T15 (no new NuGet), T31, T32, T47, T43
- Group C (heavy new NuGets): T48 (PowerBI.Api), T54 (MSAL), T51 (AI Builder API), T53 (Octokit if not in T57)

## Tasks

### T1: Fix Directory.Build.props
- **depends_on**: []
- **location**: `Greg.Xrm.Command/Directory.Build.props`
- **description**: Verify the existing Directory.Build.props builds correctly. Fix: (a) logo/README paths use `$(MSBuildThisFileDirectory)` with `Condition="Exists(...)"`, (b) remove `Version` from ItemGroup package references (let individual projects override), (c) ensure Plugin.Automation project's `Autofac.Extensions.DependencyInjection` v11.0.0 is not overridden by the shared v10.0.0 (add `NoWarn` or version override in that project). Verify `dotnet build` succeeds for all 6 projects.
- **validation**: `dotnet build` succeeds. `dotnet pack` produces valid nupkg for Greg.Xrm.Command and Greg.Xrm.Command.Interfaces.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T2: Create .editorconfig + mass-format existing code
- **depends_on**: [T1]
- **location**: `Greg.Xrm.Command/.editorconfig` (repo root of src)
- **description**: Create .editorconfig at `Greg.Xrm.Command/` directory level. Rules: 4-space indent (matching existing C# convention, NOT Google's 2-space), 100-char line limit, K&R braces, using directives outside namespace, `var` for obvious types / explicit for primitives, explicit access modifiers, PascalCase public members, _camelCase private fields. After creating, run `dotnet format` on the entire solution to apply formatting to all 97 existing commands — this is a one-time mass-format commit.
- **validation**: `dotnet format --verify-no-changes` passes with zero diffs. No build errors after formatting.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T3: Create .runsettings
- **depends_on**: []
- **location**: `Greg.Xrm.Command/Greg.Xrm.Command.Core.TestSuite/CodeCoverage.runsettings`
- **description**: Configure coverlet.collector for cobertura output. Include Greg.Xrm.Command.Core assembly, exclude test classes, set merge true.
- **validation**: `dotnet test --collect:"XPlat Code Coverage" --settings CodeCoverage.runsettings` produces `coverage.cobertura.xml`.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T4: Add dotnet test to CI pipeline
- **depends_on**: [T2, T3]
- **location**: `.github/workflows/build-pipeline-01.yml`
- **description**: Add `dotnet test` step after build, before pack. Use `--collect:"XPlat Code Coverage"` with `.runsettings`. Upload coverage artifact. Add `dotnet format --verify-no-changes` step. Fail CI if tests fail or coverage <80%.
- **validation**: CI workflow runs tests on push to master. Coverage file is produced as artifact.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T5: Enhance test utilities
- **depends_on**: []
- **location**: `Greg.Xrm.Command.Core.TestSuite/Commands/Utility.cs`
- **description**: Ensure Utility.TestParseCommand<T> handles all option types correctly. Add helper methods for common test patterns. Verify existing 33 test files still pass.
- **validation**: `dotnet test` in TestSuite passes all existing tests.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T6-T8: Plugin Loading Test Coverage
- **depends_on**: [T5] (T6), [T5] (T7), [T5] (T8)
- **location**: `Greg.Xrm.Command.Core.TestSuite/Parsing/CommandRegistryTest.cs`, `PluginLoaderTest.cs`, `BootstrapperTest.cs`
- **description**:
  - T6: CommandRegistry tests — initialize from assembly, scan modules, scan commands, duplicate verb detection, namespace helpers, command tree building.
  - T7: PluginLoader tests — scan from folder, .delete marker, --tool loading, corrupt DLL, missing executor. Create mock plugin DLLs.
  - T8: Bootstrapper tests — full init flow, empty plugins folder, obsolete command filtering, parameterless constructor requirement.
- **validation**: `dotnet test` passes. Plugin loading code coverage >90%.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T9-T12: E2E Smoke Tests
- **depends_on**: [T4] (T9), [T9] (T10), [T10] (T11), [T11] (T12)
- **location**: `Greg.Xrm.Command/Greg.Xrm.Command.Core.IntegrationTests/`
- **description**:
  - T9: Create new IntegrationTests project. Add ServiceClient test fixture with credential loading from env vars. Base class with automatic cleanup.
  - T10: Auth smoke test (authenticate + list orgs). Solution smoke test (list + create + delete).
  - T11: Table/column/relationship/webresource smoke tests with cleanup.
  - T12: Create `.github/workflows/e2e-smoke-tests.yml` triggered on push to master. GitHub Secrets for credentials.
- **validation**: Integration tests pass against real environment. CI workflow runs on push.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T13-T17: spkl Parity
- **depends_on**: [T4] (T13), [T13] (T14), [T14] (T15), [T4] (T16), [T16] (T17)
- **location**: New directories under `Commands/Plugin/` and `Commands/WebResources/`
- **description**:
  - T13: Define `[CrmPluginStep]`, `[CrmPluginImage]`, `[CrmWebhook]` attribute classes in `Greg.Xrm.Command.Interfaces`.
  - T14: Implement DLL scanner using `System.Reflection.MetadataLoadContext` to extract attribute metadata from compiled assemblies.
  - T15: Implement `plugin register-attributes` command + executor. Upsert pluginassembly, plugintype, sdkmessageprocessingstep via ServiceClient. Support incremental deployment.
  - T16: Implement `webresource map` command — YAML/JSON config mapping local files to web resource unique names.
  - T17: Implement `webresource watch` command — FileSystemWatcher that syncs changes to Dataverse on file save.
- **validation**: Each command has parsing test + executor test. End-to-end: annotate plugin → compile → register-attributes → verify in Dataverse.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T18-T22: Dataverse Platform Gaps
- **depends_on**: [T4] (all)
- **location**: New directories: `Commands/CustomApi/`, `Commands/Catalog/`, `Commands/ElasticTable/`, `Commands/VirtualTable/`, `Commands/ConnectionRef/`
- **description**:
  - T18: `custom-api create` — create Custom API with input/output parameters. Support all parameter types. Also `custom-api list` and `custom-api delete`.
  - T19: `catalog publish-item` — manage Catalog & Business Events via catalog/catalogitem tables.
  - T20: `elastic-table manage` — configure retention policies and scaling via table metadata.
  - T21: `virtual-table scaffold` — scaffold virtual table definitions from external data source metadata.
  - T22: `connection-ref map-interactive` — interactive mapping of connection references across solutions.
- **validation**: Each command has parsing test + executor test. End-to-end against test environment.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T23-T26: ALM Center Automation
- **depends_on**: [T18] (T23), [T4] (T24, T25), [T18] (T26)
- **location**: New directory: `Commands/Alm/`
- **description**:
  - T23: `alm pipeline create` and `alm pipeline run` — create pipeline stage, trigger with async status polling. Use Power Platform Admin API.
  - T24: `alm env-var sync` — sync environment variables across environments with YAML mapping file for value overrides. Dry-run mode.
  - T25: `alm env diff` — compare two environments: tables, columns, solutions, env vars, connections. Table + JSON output.
  - T26: `solution layer` — version pinning and dependency resolution for solution layers.
- **validation**: Each command has parsing test + executor test. End-to-end: sync env vars between dev→test.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T27-T29: Automation Plugin (Flow Management)
- **depends_on**: [T4] (T27), [T27] (T28), [T4] (T29)
- **location**: `Greg.Xrm.Command.Plugin.Automation/Commands/Workflow/`
- **description**:
  - T27: `workflow run list` — list recent runs with status/timestamps. `workflow run get` — detailed run info with action outputs. `workflow run resubmit` and `workflow run cancel`.
  - T28: `workflow get` — download JSON definition. `workflow set-state` — start/stop flow.
  - T29: `connection list` — list all connections with status.
- **validation**: Each command has parsing test + executor test. Uses Power Automate Management REST API.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T30-T32: CI/CD Quality
- **depends_on**: [T4] (T30), [T4] (T31), [T31] (T32)
- **location**: New directory: `Commands/QualityGate/`, extend `Commands/Solution/`
- **description**:
  - T30: `quality gate` — parse `pac solution check` ZIP output (Excel report). Return non-zero exit code on configurable severity threshold. Table + JSON summary.
  - T31: `solution diff` — compare two solutions or environments. Report added/removed/modified components.
  - T32: `solution component-move` — move individual components between solutions with dependency resolution and dry-run mode.
- **validation**: Each command has parsing test + executor test. Quality gate correctly parses sample check reports.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T33-T35: Data & Cross-Platform
- **depends_on**: [T4] (T33), [T33] (T34), [T34] (T35)
- **location**: New directory: `Commands/Data/`
- **description**:
  - T33: Pure .NET 6+ data export/import using ServiceClient (no WPF/CMT). Support same ZIP/XML format as legacy CMT for compatibility.
  - T34: `data init-schema-from-solution` — generate YAML/JSON schema from existing solution (tables, columns, relationships, choice values).
  - T35: `data seed-mock` — generate mock data from schema. Support configurable record count, field value strategies, constraint respect.
- **validation**: Round-trip test: export → schema → mock seed → import. Cross-platform verification.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T36-T38: Power Pages CLI
- **depends_on**: [T4] (all)
- **location**: New directory: `Commands/Pages/`
- **description**:
  - T36: `pages site publish` — activate website from local source. `pages site config export/import` — portal configuration with conflict resolution.
  - T37: `pages webtemplate sync` — sync web templates, page templates, content snippets between environments with value substitution.
  - T38: `pages liquid lint` — parse and validate Liquid templates. Check unknown objects, invalid filters, unclosed tags, missing entities. Colored output with line numbers.
- **validation**: Each command has parsing test + executor test. Liquid lint validated against sample valid/invalid templates.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T39-T41: Environment Lifecycle
- **depends_on**: [T23] (T39), [T39] (T40, T41)
- **location**: New directory: `Commands/Env/`
- **description**:
  - T39: `env create` — create environment with type, region, language, currency, security groups. Async status polling.
  - T40: `env clone` — clone with modes (schema-only, schema+data, selective tables). `env reset` — factory reset for sandbox.
  - T41: `env backup` — trigger and monitor. `env restore` — from specific backup point. `env capacity report` — storage analysis.
- **validation**: Each command has parsing test + executor test. Uses Power Platform Admin API (BAP).
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T42-T45: Governance & Security
- **depends_on**: [T4] (T42, T44, T45), [T42] (T43)
- **location**: New directory: `Commands/Security/`, `Commands/Dlp/`, `Commands/Storage/`
- **description**:
  - T42: `security audit-user` — full privilege audit: query user roles, privileges, entity-level access, hierarchy traversal.
  - T43: `security sharing-report` — query PrincipalObjectAccess for record access. Trace paths: direct share → team → BU → role.
  - T44: `dlp policy audit` — list DLP policies, connector classifications, environment coverage, gaps.
  - T45: `storage analytics` — table-by-table storage analysis with cleanup recommendations. `api ratelimit monitor` — track x-ratelimit headers.
- **validation**: Each command has parsing test + executor test. Security commands verified against known privilege configurations.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T46-T47: PCF Enhancement
- **depends_on**: [T4] (T46), [T46] (T47)
- **location**: New directory: `Commands/Pcf/`
- **description**:
  - T46: `pcf test` — run PCF tests headless. `pcf publish` — publish single component without full solution import (incremental).
  - T47: `pcf version bump` — semantic versioning with ControlManifest.Input.xml update + changelog. `pcf dependency-check` — validate environment has required features.
- **validation**: Each command has parsing test + executor test.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T48-T50: Tabular Editor CLI
- **depends_on**: [T4] (T48), [T48] (T49, T50)
- **location**: New directory: `Commands/Tabular/`
- **description**:
  - T48: Add `Microsoft.PowerBI.Api` NuGet. `tabular deploy` — deploy .bim to Power BI dataset (idempotent, XMLA + REST API modes). `tabular diff` — compare local .bim vs deployed model. `bim compare` — compare two .bim files.
  - T49: `tabular validate` — check circular dependencies, invalid references, best practices. Output colored errors with object paths.
  - T50: `tabular translate` — manage/deploy multi-language translations. `tabular role-add-measures` — bulk-add measures to roles. `tabular perspective-manage`.
- **validation**: Each command has parsing test + executor test. Deploy validated against test Power BI workspace.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T51-T53: AI Builder & Custom Connectors
- **depends_on**: [T4] (T51, T53), [T51] (T52)
- **location**: New directories: `Commands/AiBuilder/`, `Commands/Connector/`
- **description**:
  - T51: `ai model list` — list models with training status/accuracy. `ai model train` — trigger training with async polling. `ai model publish` — publish trained model.
  - T52: `ai form-processor configure` — configure document type, fields, tables for form processing.
  - T53: `connector import/export` — import/export custom connectors from definition files. `connector test` — test with sample payloads. `connector validate` — validate against OpenAPI schema.
- **validation**: Each command has parsing test + executor test.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T54-T56: Microsoft Forms CLI
- **depends_on**: [T4] (T54), [T54] (T55, T56)
- **location**: New directory: `Commands/Forms/`
- **description**:
  - T54: Add `Microsoft.Identity.Client` (MSAL) NuGet. Implement token manager with auto-refresh, 401 retry, caching. Support Client Credentials (user forms) and ROPC (group forms).
  - T55: `forms list` — list forms via `GET /formapi/api/{tenantId}/users/{userId}/light/forms`. `forms response count` — `?$select=rowCount`. Table + JSON output.
  - T56: `forms responses export` — paged retrieval with `$skip`/`$top`. Export to CSV, JSON, SQL. Incremental sync support.
- **validation**: Each command has parsing test + executor test (mocked HTTP). End-to-end against real Forms tenant.
- **status**: Not Completed
- **log**:
- **files edited/created**:

### T57-T58: Fork PR Lifecycle Tooling
- **depends_on**: [T4] (T57), [T57] (T58)
- **location**: New directory: `Commands/Pr/`, new NuGet: Octokit.NET
- **description**:
  - T57: Add `Octokit` NuGet. Implement GitHub API client wrapper with token-based auth. Rate limit handling.
  - T58: `pr open` — create a PR from the local branch in this fork. `pr track` — monitor PR status, detect conflicts. `pr merge` — auto-merge when approved and CI passes.
- **validation**: Each command has parsing test + executor test (mocked Octokit client).
- **status**: Not Completed
- **log**:
- **files edited/created**:

## Testing Strategy

### Unit Tests (every command, mandatory)
- **Parsing tests:** `*CommandTest.cs` for every command using `Utility.TestParseCommand<T>`. Test: defaults, long names (`--flag value`), short names (`-f value`), required field rejection, enum validation, invalid input handling.
- **Executor tests:** `*CommandExecutorTest.cs` extending `CommandExecutorTestBase`. Mock `IOrganizationServiceAsync2` and `IOrganizationServiceRepository`. Test: success path, failure path (FaultException), empty result, filtering logic verification via `.Verify()`.
- **Both overloads** of async methods must be mocked: `RetrieveMultipleAsync(query)` and `RetrieveMultipleAsync(query, cancellationToken)`.

### Integration Tests (high-risk commands only)
- **Core CRUD:** T10 (auth + org + solution + table + column) — `Greg.Xrm.Command.Core.IntegrationTests`
- **Plugin registration:** T15 — register-attributes against test environment, verify plugin appears
- **Flow management:** T27 — list runs, get run details against test environment with active flows
- **Web resource sync:** T17 — push test web resource, verify upload, delete
- **Security audit:** T42 — audit known user, verify privilege report matches expected
- **Environment lifecycle:** T39 — create sandbox env (if permissions allow), verify, delete
- Each integration test must be **idempotent** — creates resources with unique test-prefixed names, deletes in teardown.

### Negative-Path Testing (API failure modes)
- For every command that calls external APIs, test: timeout handling, 429 rate limit (retry with backoff), 503 service unavailable (graceful error message), malformed response (catch + log), network partition (connection refused → user-friendly error).
- Implement via Moq's `.ThrowsAsync()` for `FaultException<OrganizationServiceFault>` with specific error codes.

### Coverage Enforcement
- **Overall target:** >80% for `Greg.Xrm.Command.Core` assembly.
- **Plugin loading target:** >90% (critical infrastructure, currently 0%).
- **CI enforcement:** `dotnet test --collect:"XPlat Code Coverage" --settings CodeCoverage.runsettings`. Use `--filter "Category!=Integration"` to exclude integration tests from coverage calculation. Coverage <80% fails CI.
- **Supplement with mutation testing** (Stryker.NET) in Phase 3 for critical paths — not in this plan's scope.

### E2E Smoke Tests
- T10-T12 cover the happy path for core operations. After each major feature track, add at least one smoke test for the new command domain.

## Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|-----------|
| Undocumented APIs (Forms, Catalog) may change | Medium | Isolate API calls behind interfaces with mockable contracts. Add version comments. Test against mock responses. |
| Group Forms ROPC requires MFA-excluded account | Medium | Use Client Credentials for user forms as primary path. Document ROPC requirement for group forms as known limitation. |
| Power BI XMLA endpoint requires Premium | Medium | Support both XMLA (Premium) and REST API (Pro) modes. Auto-detect workspace tier, fall back to REST API. |
| Wave 4: 50 tasks competing for .sln/.csproj edits | High | Sequence by NuGet group (A→B→C). Each group commits separately. Use `dotnet sln add` with explicit paths to minimize merge conflicts. |
| FileSystemWatcher unreliable on macOS/Linux | Medium | T17: Add debouncing (500ms), retry on upload failure (3x with exponential backoff), use polling fallback for network shares. |
| PrincipalObjectAccess query timeouts on large orgs | Medium | T43: Add pagination (`$top`/`$skip`), default scope to single record, add `--all` flag for full scan with warning. |
| Liquid lint lacks formal Power Pages grammar | Medium | T38: Start with Shopify Liquid grammar as baseline. Add Power Pages-specific objects (`portal`, `website`, `webpage`) as known entities. Mark unknown objects as warnings, not errors. |
| Mock data seed constraint satisfaction is combinatorially complex | High | T35: Start with basic constraint respect: required fields, max length, optionset valid values. Defer foreign key resolution and complex lookups to a future iteration. |
| `pac solution check` requires PAC CLI installed | Medium | T30: Quality gate accepts `--input <path>` to specify pre-existing check result ZIP. CI workflow installs PAC CLI before running quality gate tests. |
| Firewall blocks git remote | Low | All implementation proceeds locally. PR creation (T58) deferred until connectivity restored. Ralph loop protocol works with local review. |
| DLL scanner MetadataLoadContext needs runtime assembly paths | Medium | T14: Use `AppContext.BaseDirectory` + `RuntimeEnvironment.GetRuntimeDirectory()` for core assembly resolver. Test with a known-good plugin DLL. |
| ROPC flow being deprecated by Microsoft | Medium | T54: Document deprecation timeline. Plan migration path to certificate-based auth for group forms when Microsoft provides it. |
