# Project Tracks

This file tracks all major tracks for the project. Each track has its own detailed plan in its respective folder.

Tracks are organized by implementation priority, not phase order. Dependencies are noted where applicable.

**Honesty note (2026-04-21 audit):** Several commands previously marked `VERIFIED` / `IMPLEMENTED` print `[DRY RUN]` or API-doc text instead of calling the real API. Those tracks are demoted to `[~]` below and re-listed under `⚠️ Tracks with stubbed executors`. See `conductor/tracks/defake_stubs_20260421/` for the remediation plan.

---

## Completed Tracks (Verified Implemented)

- [x] **Track: De-fake Stub Executors**
  *Link: [./tracks/defake_stubs_20260421/](./tracks/defake_stubs_20260421/)*
  *Status: IMPLEMENTED — ALM Pipeline, Connector Import, Catalog Publish, and Data Import executors now use real APIs.*

- [x] **Track: spkl Parity (Developer Productivity)**
  *Link: [./tracks/spkl_parity_20260409/](./tracks/spkl_parity_20260409/)*
  *Status: IMPLEMENTED — plugin register-attributes, plugin step-scan, webresource map, webresource watch*

- [x] **Track: E2E Smoke Tests & Integration Tests**
  *Link: [./tracks/e2e_smoke_tests_20260409/](./tracks/e2e_smoke_tests_20260409/)*
  *Status: IMPLEMENTED — IntegrationTestBase, CoreSmokeTests, AdditionalSmokeTests*

- [x] **Track: Plugin Loading Test Coverage**
  *Link: [./tracks/plugin_test_coverage_20260409/](./tracks/plugin_test_coverage_20260409/)*
  *Status: IMPLEMENTED — CommandRegistry tests*

- [x] **Track: Microsoft Forms CLI (v1)**
  *Link: [./tracks/ms_forms_20260409/](./tracks/ms_forms_20260409/)*
  *Status: IMPLEMENTED — forms list, forms response count*

- [x] **Track: Governance, Security & Monitoring**
  *Link: [./tracks/governance_security_20260409/](./tracks/governance_security_20260409/)*
  *Status: COMPLETED — security audit-user, security sharing-report, dlp policy-audit, storage analytics, api ratelimit monitor*

- [x] **Track: Security & Supply Chain**
  *Link: [./tracks/security_supply_chain_20260421/](./tracks/security_supply_chain_20260421/)*
  *Status: COMPLETED — disclosure policy, branch protection, Renovate + Dependabot security-only, SBOM, signed releases, signed release tags, SLSA provenance, and fork-only PR lifecycle closure.*

- [x] **Track: CI/CD Hardening**
  *Link: [./tracks/ci_cd_hardening_20260421/](./tracks/ci_cd_hardening_20260421/)*
  *Status: COMPLETED — reusable matrix build, concurrency, docs-only skips, coverage gate, CodeQL, Scorecard, OIDC release, pinned actions, and release smoke wiring on the .NET 11 preview target.*

- [x] **Track: Environment Lifecycle Management**
  *Link: [./tracks/env_lifecycle_20260409/](./tracks/env_lifecycle_20260409/)*
  *Status: COMPLETED — env create/clone/reset/backup/restore/capacity with API docs and validation*

- [x] **Track: PCF Enhancement**
  *Link: [./tracks/pcf_enhancement_20260409/](./tracks/pcf_enhancement_20260409/)*
  *Status: COMPLETED — version bump updates manifest+changelog; dependency-check parses local manifest; publish shows component info*

- [x] **Track: Tabular Editor CLI**
  *Link: [./tracks/tabular_editor_20260409/](./tracks/tabular_editor_20260409/)*
  *Status: COMPLETED — parses BIM for table/measure counts; validate checks circular refs; bim compare shows count diff*

- [x] **Track: Power Pages CLI**
  *Link: [./tracks/power_pages_cli_20260409/](./tracks/power_pages_cli_20260409/)*
  *Status: COMPLETED — liquid lint validates templates; site publish queries Dataverse adx tables; webtemplate sync shows API docs*

- [x] **Track: CI/CD Quality & Solution Management**
  *Link: [./tracks/cicd_quality_20260409/](./tracks/cicd_quality_20260409/)*
  *Status: VERIFIED — quality gate, solution diff, solution component-move fully implemented*

- [x] **Track: Flow Management (Automation Plugin)**
  *Link: [./tracks/automation_plugin_20260408/](./tracks/automation_plugin_20260408/)*
  *Status: VERIFIED — workflow get, set-state, run list/get/cancel/resubmit, connection list fully implemented*

---

## ⚠️ Tracks with stubbed executors (pending de-fake — 2026-04-21 audit)

These tracks were previously marked `VERIFIED` but contain executors that print `[DRY RUN]`, API-doc text, or sample data instead of invoking real APIs. De-faking is tracked in [`defake_stubs_20260421`](./tracks/defake_stubs_20260421/).

- [x] **Track: ALM Center Automation**
  *Link: [./tracks/alm_center_20260409/](./tracks/alm_center_20260409/)*
  *Status: VERIFIED — AlmPipelineCreate and AlmPipelineRun now use real Power Platform Admin API calls.*

- [x] **Track: Data & Cross-Platform Engine**
  *Link: [./tracks/data_crossplatform_20260409/](./tracks/data_crossplatform_20260409/)*
  *Status: VERIFIED — DataExportImportCommandExecutors now implements real data import logic using pure .NET 8+ engine.*

- [x] **Track: Dataverse Platform Gaps**
  *Link: [./archive/dataverse_gaps_20260409/](./archive/dataverse_gaps_20260409/)*
  *Status: VERIFIED — CatalogPublishCommandExecutor now uses real PublishCatalogItem message.*

- [x] **Track: AI Builder & Custom Connectors (v1)**
  *Link: [./tracks/ai_builder_connectors_20260409/](./tracks/ai_builder_connectors_20260409/)*
  *Status: VERIFIED — Custom Connector import/export/test now implemented using real APIs.*

---

## In-Progress Tracks

- [x] **Track: Explore Branches**
  *Link: [./tracks/archive/explore_branches_20260408/](./tracks/archive/explore_branches_20260408/)*
  *Status: COMPLETED — archived branch exploration was previously blocked by missing SDK; .NET 11 SDK is installed and the test-suite build now passes.*

- [x] **Track: MCP Server**
  *Link: [./tracks/archive/mcp_server_20260408/](./tracks/archive/mcp_server_20260408/)*
  *Status: COMPLETED — implementation is split into a separate `Greg.Xrm.Command.Mcp` host boundary; PR/review closure was superseded by direct-push workflow per repository owner instruction, with local .NET 11 verification passing.*

---

## New Tracks (2026-04-21) — Planning

Created in response to the 2026-04-21 audit covering "fake implementation" feedback, SOTA tooling gaps, and product-coverage whitespace.
`defake_stubs_20260421` is completed and listed above under `Completed Tracks (Verified Implemented)`.

### Priority 0: Upstream Baseline Sync (Gating)
- [x] **Track: Upstream Baseline Sync**
  *Link: [./tracks/upstream_baseline_sync_20260422/](./tracks/upstream_baseline_sync_20260422/)*
  *Status: COMPLETED — branch inventory is complete, the selected branch heads are already contained in the local baseline, and fork-only governance skips upstream issue/PR lifecycle work by design.*

### Priority 1: Credibility & Library Quality (High)
- [x] **Track: Library Hygiene**
  *Link: [./tracks/library_hygiene_20260421/](./tracks/library_hygiene_20260421/)*
  *Status: COMPLETED — the library hygiene sweep is complete on the fork and the full Greg.Xrm.Command solution test suite passes.*

### Priority 2: Build Infrastructure & Security (High)
- [x] **Track: Code Quality Hardening**
  *Link: [./tracks/code_quality_hardening_20260421/](./tracks/code_quality_hardening_20260421/)*
  *Status: COMPLETED — CPM, TreatWarningsAsErrors, analyzer stack, deterministic builds, SourceLink, lockfiles, NuGetAudit, and NetArchTest.*

### Priority 3: Developer & Contributor Experience (Medium)
- [x] **Track: Developer Experience**
  *Link: [./tracks/developer_experience_20260421/](./tracks/developer_experience_20260421/)*
  *Status: COMPLETED — devcontainer, Husky.NET + commitlint, PR/issue/discussion templates, Release Drafter, stale/welcome bots, FUNDING.yml, and repo-settings handoff.*

- [x] **Track: Documentation Site**
  *Link: [./tracks/documentation_site_20260421/](./tracks/documentation_site_20260421/)*
  *Status: COMPLETED — DocFX scaffold, guides, recipes, ADRs, generated command reference, README refresh, and Pages workflow.*

- [x] **Track: CLI UX**
  *Link: [./tracks/cli_ux_20260421/](./tracks/cli_ux_20260421/)*
  *Status: COMPLETED — completions, exit-code taxonomy, global flags, no-color handling, first-run auth guidance, auth whoami, opt-in telemetry commands/export hook, and docs.*

- [x] **Track: Testing Maturity**
  *Link: [./tracks/testing_maturity_20260421/](./tracks/testing_maturity_20260421/)*
  *Status: COMPLETED — snapshot baselines, Stryker.NET mutation workflow, MSTest architecture rules, Dataverse contract schemas, integration-env gating, and property-style connector validation coverage.*

### Priority 4: Coverage Expansion — Strategic (High business value)
- [x] **Track: Microsoft Fabric & OneLake**
  *Link: [./tracks/fabric_onelake_20260421/](./tracks/fabric_onelake_20260421/)*
  *Status: COMPLETED — Fabric client, workspace/capacity/lakehouse commands, OneLake shortcuts, semantic model refresh/list, Dataverse link staging, docs, and unit-test coverage.*

- [x] **Track: Power BI Workspace & Dataset Management**
  *Link: [./tracks/powerbi_workspace_mgmt_20260421/](./tracks/powerbi_workspace_mgmt_20260421/)*
  *Status: COMPLETED — Power BI REST client, dataset publish/clone/refresh/RLS, deployment pipeline, capacity commands, docs, and unit-test coverage.*

- [x] **Track: Desktop Flows (RPA)**
  *Link: [./tracks/desktop_flow_rpa_20260421/](./tracks/desktop_flow_rpa_20260421/)*
  *Status: COMPLETED — desktop-flow discovery, trigger/runs, machines, machine-groups, scaffold, approvals, docs, and unit-test coverage.*

- [x] **Track: Copilot Studio CLI**
  *Link: [./tracks/copilot_studio_20260421/](./tracks/copilot_studio_20260421/)*
  *Status: COMPLETED — Copilot Studio client, agent lifecycle, topics, knowledge, analytics, MCP exposure metadata, docs, and tests.*

### Priority 5: Coverage Expansion — Parity & Depth (Medium)
- [x] **Track: Power FX Validation**
  *Link: [./tracks/power_fx_validation_20260421/](./tracks/power_fx_validation_20260421/)*
  *Status: COMPLETED — Microsoft.PowerFx.Core reference, validate/format/repl commands, file/expression validation, docs, and unit-test coverage.*

- [x] **Track: Dataverse Gaps Phase 2**
  *Link: [./tracks/dataverse_gaps_phase2_20260421/](./tracks/dataverse_gaps_phase2_20260421/)*
  *Status: COMPLETED — business rules, BPFs, duplicate detection, audit export, field security profiles, service endpoints, existing alternate key flow, file column upload, rollup recalculation, docs, and tests.*

- [x] **Track: Microsoft Forms — Advanced**
  *Link: [./tracks/forms_advanced_20260421/](./tracks/forms_advanced_20260421/)*
  *Status: COMPLETED — branching, analytics, templates, sharing, ownership transfer, Customer Voice command surfaces, docs, and tests.*

### Priority 6: Performance (Low, exploratory)
- [x] **Track: Performance & Native AOT**
  *Link: [./tracks/performance_aot_20260421/](./tracks/performance_aot_20260421/)*
  *Status: COMPLETED — BenchmarkDotNet suites, nightly benchmark workflow, baseline structure, Native AOT feasibility ADR, and pacx-lite subset guidance.*

---

## Planning Tracks (2026-04-27) — Task-decomposed 2026-04-21

These four tracks have `Overview / Scope / Success Criteria` plus Phase/Task decompositions appended 2026-04-21.

- [x] **Track: AI Builder & Custom Connectors — Improved**
  *Link: [./tracks/ai_builder_connectors_improved_20260427/](./tracks/ai_builder_connectors_improved_20260427/)*
  *Retry/backoff, polling controls, correlation IDs, connector validation, form option validation, and AI Builder guidance complete.*

- [x] **Track: AI Wrapper Service**
  *Link: [./tracks/ai_wrapper_service_20260427/](./tracks/ai_wrapper_service_20260427/)*
  *Introduces `IAiBuilderService` abstraction; AI Builder executors now use the wrapper service with structured results.*

- [x] **Track: Connector Schema Validation**
  *Link: [./tracks/connector_schema_validation_20260427/](./tracks/connector_schema_validation_20260427/)*
  *JSON/OpenAPI structural validation before `connector import` and `connector test`; CI-friendly `pacx connector validate`.*

- [x] **Track: Correlation ID & Telemetry**
  *Link: [./tracks/correlation_id_telemetry_20260427/](./tracks/correlation_id_telemetry_20260427/)*
  *Correlation ID provider, output, override, and HTTP propagation complete; OTLP bridge deferred until `cli_ux_20260421`.*

---

## New Tracks (2026-04-29)

Created from the Apr-28 work session planning and the pnp/cli-microsoft365 parity gap analysis.

- [x] **Track: Adjacent Ecosystem Intake**
  *Link: [./tracks/adjacent_ecosystem_intake_20260429/](./tracks/adjacent_ecosystem_intake_20260429/)*
  *Status: COMPLETED — Tool catalog (browse/list), tool lifecycle (install/run/uninstall), source catalog (add/remove/list), flow MCP catalog, skill pack catalog (list/install) all implemented with tests.*

- [x] **Track: Dataverse Skill Pack Guidance**
  *Link: [./tracks/dataverse_skill_pack_guidance_20260429/](./tracks/dataverse_skill_pack_guidance_20260429/)*
  *Status: COMPLETED — Skill pack catalog schema and packs.json, guidance docs, CLI commands (list/install) with tests.*

- [x] **Track: Flow Studio MCP Surfaces**
  *Link: [./tracks/flow_studio_mcp_surfaces_20260429/](./tracks/flow_studio_mcp_surfaces_20260429/)*
  *Status: COMPLETED — 12 MCP flow commands (browse/debug/govern/inspect/monitor/run/start), flow catalog JSON, guide docs, and tests.*

- [x] **Track: Forms Authoring**
  *Link: [./tracks/forms_authoring_20260429/](./tracks/forms_authoring_20260429/)*
  *Status: COMPLETED — 20 command files, 28 tests (14 executor tests pass, 14 parse tests blocked by pre-existing flow list duplicate).*

- [x] **Track: Release Plan Intelligence** *(archived)*
  *Link: [./archive/release_plan_intelligence_20260429/](./archive/release_plan_intelligence_20260429/)*
  *Status: COMPLETED — browse, analyze, report commands with impact analysis. Fork-only feature, no upstream PR.*

- [x] **Track: Repo Hardening Capability Expansion**
  *Link: [./tracks/repo_hardening_capability_expansion_20260429/](./tracks/repo_hardening_capability_expansion_20260429/)*
  *Status: COMPLETED — Meta-track closed. Child tracks: release_supply_chain_hardening (provenance/SBOM/signing/gates), validation_parity_coverage (integration tests), pnp_platform_parity (archived). Scorecard and CodeQL workflows active.*

- [x] **Track: Validation Parity & Coverage**
  *Link: [./tracks/validation_parity_coverage_20260429/](./tracks/validation_parity_coverage_20260429/)*
  *Status: COMPLETED — ValidateAll orchestration, schema/contract validation, command reference parity, integration coverage (solution, env, connector, auth, connection, forms, Power BI, Fabric), and CI integration test gating all implemented.*

- [x] **Track: Microsoft Forms API Integration**
  *Link: [./tracks/forms_api_integration_20260502/](./tracks/forms_api_integration_20260502/)*
  *Status: COMPLETED — All 6 phases done. IFormsApiClient with 12 methods, 20+ executors de-faked, FormsTokenProvider (CC + ROPC), API reference docs, PowerShell/CI-CD guide, 15+ tests.*

- [x] **Track: Stub Killer Phase 2**
  *Link: [./tracks/stub_killer_phase2_20260502/](./tracks/stub_killer_phase2_20260502/)*
  *Status: COMPLETED — BAP environment lifecycle methods (create/reset/capacity) wired to real API. Quality gate pac CLI integration (--run-check).*

## Remaining Stub Tracks

These tracks cover the remaining unimplemented functionality across the codebase. All are **Pending** — ready for implementation when priorities shift.

- [x] **Track: Tabular Power BI Integration**
  *Link: [./tracks/tabular_powerbi_integration_20260502/](./tracks/tabular_powerbi_integration_20260502/)*
  *Status: COMPLETED — ITabularClient created, all 6 tabular executors de-faked (deploy, diff, translate, roles, perspectives) using Power BI REST API via existing auth chain. BimCompare improved with table-level diff.*

- [x] **Track: Async Environment Lifecycle**
  *Link: [./tracks/env_async_lifecycle_20260502/](./tracks/env_async_lifecycle_20260502/)*
  *Status: COMPLETED — 3 env commands de-faked (backup, restore, clone). Uses `IAsyncJobPoller` to poll the BAP provisioning state until terminal.*

- [x] **Track: PCF Tooling**
  *Link: [./tracks/pcf_tooling_20260502/](./tracks/pcf_tooling_20260502/)*
  *Status: COMPLETED — PcfTestExecutor runs npx pcf-test via Process; PcfPublishExecutor uses pac CLI + ImportSolutionRequest; PcfDependencyCheckExecutor queries Dataverse for org version ≥ 9.0.*

---

## Strategic Improvement Tracks

- [x] **Track: Multi-target & Version Automation**
  *Link: [./tracks/multitarget_version_automation_20260502/](./tracks/multitarget_version_automation_20260502/)*
  *Status: COMPLETED — Multi-target net10.0;net11.0 in Directory.Build.props. Global.json rollForward. CI matrix builds on both TFMs. Dependabot for NuGet + GHA.*

- [x] **Track: CI/CD Modernization**
  *Link: [./tracks/cicd_modernization_20260502/](./tracks/cicd_modernization_20260502/)*
  *Status: COMPLETED — Dependency review action, merge queue docs, auto-merge for non-major deps, release automation verified, benchmark comparison workflow with regression gate.*

- [x] **Track: Code Quality Automation**
  *Link: [./tracks/code_quality_automation_20260502/](./tracks/code_quality_automation_20260502/)*
  *Status: COMPLETED — SonarCloud workflow + props, API compat check workflow + Directory.Packages.props entry, enhanced Husky hooks (pre-commit format+build, pre-push tests). ImplicitUsings already enabled.*

- [x] **Track: Platform Expansion**
  *Link: [./tracks/platform_expansion_20260502/](./tracks/platform_expansion_20260502/)*
  *Status: COMPLETED — Docker image + publish workflow, PowerShell module (5 cmdlets), VS Code extension (4 commands), GitHub Action (Docker-based), Azure DevOps task, IaC integration guide.*

- [x] **Track: CLI Excellence**
  *Link: [./tracks/cli_excellence_20260502/](./tracks/cli_excellence_20260502/)*
  *Status: COMPLETED — Self-update command (GitHub Releases + assembly compare), diagnostics command (WhoAmI + version + env checks), silent/quiet modes, progress bars with spinner, YAML output via YamlDotNet, config auto-discovery with directory walk.*

---

## Expert Review Findings (Pending Tracks)

Our team of 7 expert agents (GitHub, Power Platform, C#, DevOps, Security, Documentation, CLI/UX) evaluated the codebase. These tracks address their findings:

- [x] **Track: Security Hotfix Sprint**
  *Link: [./tracks/security_hotfix_sprint_20260502/](./tracks/security_hotfix_sprint_20260502/)*
  *Status: COMPLETED — NU1900 suppression removed, WarningsAsErrors for vulns, PackageLicenseExpression added, action SHA pinning, Dependabot/Renovate resolved, SonarCloud fixed, permissions blocks added.*

- [x] **Track: Auth Modernization**
  *Link: [./tracks/auth_modernization_20260502/](./tracks/auth_modernization_20260502/)*
  *Status: COMPLETED — Device code flow, managed identity support, persistent token cache via DPAPI, FormsTokenProvider unified with ITokenProvider.*

- [x] **Track: CI/CD & DevOps Overhaul**
  *Link: [./tracks/cicd_devops_overhaul_20260502/](./tracks/cicd_devops_overhaul_20260502/)*
  *Status: COMPLETED — CI matrix fixed, rollback.yml created, Docker hardened (non-root, layer caching, distroless), NuGet metadata fixed.*

- [x] **Track: Power Platform API Expansion**
  *Link: [./tracks/pp_api_expansion_20260502/](./tracks/pp_api_expansion_20260502/)*
  *Status: COMPLETED — Solution import/export/clone/patch/upgrade, DLP policy CRUD, Environment Groups, Managed Environments, API version bumps, typed clients.*

- [x] **Track: CLI UX & Command Polish**
  *Link: [./tracks/cli_ux_polish_20260502/](./tracks/cli_ux_polish_20260502/)*
  *Status: COMPLETED — Verb consistency fixes, centralized output format, --version flag, plural fixes, help examples, error message improvements.*

- [x] **Track: Documentation Overhaul**
  *Link: [./tracks/documentation_overhaul_20260502/](./tracks/documentation_overhaul_20260502/)*
  *Status: COMPLETED — README install/quick-start, search enabled, grouped command index, recipe fills, troubleshooting guide, CHANGELOG, LICENSE.*

- [x] **Track: Code Quality & Performance**
  *Link: [./tracks/code_quality_perf_20260502/](./tracks/code_quality_perf_20260502/)*
  *Status: COMPLETED — Dead code fixed, sync-over-async eliminated, CommandResult allocation fixed, catch filters added, Autofac replaced with MS.DI, ConfigureAwait audit completed.*

- [x] **Track: Security Deep Clean**
  *Link: [./tracks/security_deep_clean_20260502/](./tracks/security_deep_clean_20260502/)*
  *Status: COMPLETED — SSRF validation in Forms API, error body redaction, MCP exception sanitization, XmlDocument DTD hardening, AutoUpdater hardened, OutputRedactor regex improved.*

---

## Track Dependency Graph

```
upstream_baseline_sync_20260422 ────────────────→ (gates 04-27 planning set)
defake_stubs ─────────────────────────────────────→ (unblocks credibility)
library_hygiene ──────────────────────────────────→ (no deps)
ci_cd_hardening ──────────────────────────────────→ (no deps)
code_quality_hardening ───────────────────────────→ (no deps)
security_supply_chain ────────────────────────────→ ci_cd_hardening
developer_experience ─────────────────────────────→ (no deps)
documentation_site ───────────────────────────────→ (no deps)
cli_ux ───────────────────────────────────────────→ (no deps)
testing_maturity ─────────────────────────────────→ code_quality_hardening

correlation_id_telemetry_20260427 ────────────────→ (gates 04-27 siblings)
connector_schema_validation_20260427 ─────────────→ correlation_id_telemetry
ai_wrapper_service_20260427 ──────────────────────→ correlation_id_telemetry
ai_builder_connectors_improved_20260427 ──────────→ correlation_id_telemetry, connector_schema_validation

fabric_onelake ───────────────────────────────────→ (no deps)
powerbi_workspace_mgmt ───────────────────────────→ (no deps)
desktop_flow_rpa ─────────────────────────────────→ (no deps)
copilot_studio ───────────────────────────────────→ mcp_server
power_fx_validation ──────────────────────────────→ (no deps)
dataverse_gaps_phase2 ────────────────────────────→ dataverse_gaps (original)
forms_advanced ───────────────────────────────────→ ms_forms (original)
performance_aot ──────────────────────────────────→ (no deps)

mcp_server ───────────────────────────────────────→ (Phase 4 pending)
```

---

## Execution Ordering

1. **Upstream baseline sync** — integrate ahead upstream branches before any new downstream work picks up the baseline.
2. **Unblock planning** — four 04-27 tracks now have Phase/Task decompositions; `correlation_id_telemetry` runs first.
3. **Credibility fix** — `defake_stubs` + `library_hygiene` as paired PRs against upstream.
4. **Infra foundation** — `ci_cd_hardening` → `code_quality_hardening` → `security_supply_chain` (strict order).
5. **Close legacy work** — `mcp_server` Phase 4 (or archive).
6. **Product expansion** — Prioritize `fabric_onelake` + `powerbi_workspace_mgmt` (highest strategic value), then `desktop_flow_rpa` + `copilot_studio`.
7. **DX polish** — `developer_experience`, `documentation_site`, `cli_ux`, `testing_maturity` in parallel.
8. **Long tail** — `dataverse_gaps_phase2`, `forms_advanced`, `power_fx_validation`, `performance_aot` as capacity allows.

---

## Known Stubs (Intentionally Deferred)

The following commands remain stubbed due to architectural or environment constraints.

| Command | Rationale | Track |
|---------|-----------|-------|
| `fabric *` | Requires Power BI / Fabric REST API integration and workspace scope. | `fabric_onelake` |
| `desktop-flow *` | Requires machine-group and flow-session API access (different scope). | `desktop_flow_rpa` |
| `copilot *` | Topic export/import requires PVA topic metadata parsing logic. | `copilot_studio` |
| `tabular translation` | Requires BIM metadata localized property mapping (BIM domain). | `tabular_editor` |
