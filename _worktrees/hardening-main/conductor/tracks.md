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

- [~] **Track: Explore Branches**
  *Link: [./tracks/archive/explore_branches_20260408/](./tracks/archive/explore_branches_20260408/)*
  *Status: BLOCKED — final `/conductor:review` follow-up is still pending.*

- [~] **Track: MCP Server**
  *Link: [./tracks/archive/mcp_server_20260408/](./tracks/archive/mcp_server_20260408/)*
  *Status: BLOCKED — implementation is split into a separate `Greg.Xrm.Command.Mcp` host boundary, but PR/review closure work is blocked by missing `CreatePullRequest` permission on the moved `edithatogo/pacx` repository.*

---

## New Tracks (2026-04-21) — Planning

Created in response to the 2026-04-21 audit covering "fake implementation" feedback, SOTA tooling gaps, and product-coverage whitespace.
`defake_stubs_20260421` has moved into active execution and is listed above under `In-Progress Tracks`.

### Priority 0: Upstream Baseline Sync (Gating)
- [x] **Track: Upstream Baseline Sync**
  *Link: [./tracks/upstream_baseline_sync_20260422/](./tracks/upstream_baseline_sync_20260422/)*
  *Status: COMPLETED — branch inventory is complete, the selected branch heads are already contained in the local baseline, and fork-only governance skips upstream issue/PR lifecycle work by design.*

### Priority 1: Credibility & Library Quality (High)
- [x] **Track: Library Hygiene**
  *Link: [./tracks/library_hygiene_20260421/](./tracks/library_hygiene_20260421/)*
  *Status: COMPLETED — the library hygiene sweep is complete on the fork and the full Greg.Xrm.Command solution test suite passes.*

### Priority 2: Build Infrastructure & Security (High)
- [ ] **Track: CI/CD Hardening**
  *Link: [./tracks/ci_cd_hardening_20260421/](./tracks/ci_cd_hardening_20260421/)*
  *Matrix testing (OS × .NET), CodeQL, OpenSSF Scorecard, reusable workflows, concurrency cancel, SHA-pinned actions, OIDC trusted publishing to NuGet.org, coverage gate.*

- [ ] **Track: Code Quality Hardening**
  *Link: [./tracks/code_quality_hardening_20260421/](./tracks/code_quality_hardening_20260421/)*
  *Central Package Management, TreatWarningsAsErrors=true, Meziantou/Roslynator/Threading/BannedApi analyzers, deterministic builds, SourceLink, lockfiles, NuGetAudit, NetArchTest.*

- [ ] **Track: Security & Supply Chain**
  *Link: [./tracks/security_supply_chain_20260421/](./tracks/security_supply_chain_20260421/)*
  *SECURITY.md, CODEOWNERS, branch protection, Renovate (primary updater) + Dependabot security-only (no email noise), CycloneDX SBOM, signed releases (Sigstore), SLSA provenance.*

### Priority 3: Developer & Contributor Experience (Medium)
- [ ] **Track: Developer Experience**
  *Link: [./tracks/developer_experience_20260421/](./tracks/developer_experience_20260421/)*
  *.devcontainer, husky.net + commitlint, PR & issue templates, Release Drafter, stale/welcome bots, FUNDING.yml, expanded .editorconfig.*

- [ ] **Track: Documentation Site**
  *Link: [./tracks/documentation_site_20260421/](./tracks/documentation_site_20260421/)*
  *DocFX site, auto-generated command reference, recipes, ADRs, migration guide from `pac`, GitHub Pages.*

- [ ] **Track: CLI UX**
  *Link: [./tracks/cli_ux_20260421/](./tracks/cli_ux_20260421/)*
  *Shell completions (pwsh/bash/zsh/fish), exit code taxonomy, uniform --format/--verbose/--no-color, first-run wizard, opt-in OTLP telemetry, rich Spectre.Console output.*

- [ ] **Track: Testing Maturity**
  *Link: [./tracks/testing_maturity_20260421/](./tracks/testing_maturity_20260421/)*
  *Verify snapshot tests, Stryker.NET mutation, NetArchTest architecture rules, Pact/schema contract tests vs Dataverse, integration-env gating.*

### Priority 4: Coverage Expansion — Strategic (High business value)
- [ ] **Track: Microsoft Fabric & OneLake**
  *Link: [./tracks/fabric_onelake_20260421/](./tracks/fabric_onelake_20260421/)*
  *`fabric workspace`, `fabric lakehouse`, `onelake shortcut`, `fabric semantic-model`, Dataverse Direct Lake link. Currently zero coverage.*

- [ ] **Track: Power BI Workspace & Dataset Management**
  *Link: [./tracks/powerbi_workspace_mgmt_20260421/](./tracks/powerbi_workspace_mgmt_20260421/)*
  *`dataset list/publish/clone/refresh`, RLS, deployment pipelines, capacity metrics. Complements existing `tabular` (BIM) commands.*

- [ ] **Track: Desktop Flows (RPA)**
  *Link: [./tracks/desktop_flow_rpa_20260421/](./tracks/desktop_flow_rpa_20260421/)*
  *`desktop-flow list/trigger/runs`, machines & machine-groups, scaffold, approvals. Currently zero coverage.*

- [ ] **Track: Copilot Studio CLI**
  *Link: [./tracks/copilot_studio_20260421/](./tracks/copilot_studio_20260421/)*
  *Agent lifecycle, topic export/import, knowledge sources, analytics, MCP bridge. Currently zero coverage.*

### Priority 5: Coverage Expansion — Parity & Depth (Medium)
- [ ] **Track: Power FX Validation**
  *Link: [./tracks/power_fx_validation_20260421/](./tracks/power_fx_validation_20260421/)*
  *`power-fx validate/format/repl` using Microsoft.PowerFx.Core; pac parity for calculated columns and business rules.*

- [ ] **Track: Dataverse Gaps Phase 2**
  *Link: [./tracks/dataverse_gaps_phase2_20260421/](./tracks/dataverse_gaps_phase2_20260421/)*
  *Business Rules, Business Process Flows, Duplicate Detection, Audit export, Field Security Profiles, Service Endpoints, alternate keys, file columns, rollup recalc.*

- [ ] **Track: Microsoft Forms — Advanced**
  *Link: [./tracks/forms_advanced_20260421/](./tracks/forms_advanced_20260421/)*
  *Branching logic export, analytics, template mgmt, org sharing, Customer Voice bridge.*

### Priority 6: Performance (Low, exploratory)
- [ ] **Track: Performance & Native AOT**
  *Link: [./tracks/performance_aot_20260421/](./tracks/performance_aot_20260421/)*
  *BenchmarkDotNet suite, regression gates, Native AOT feasibility, trim-safe pacx-lite, source-generator hot-path optimizations.*

---

## Planning Tracks (2026-04-27) — Task-decomposed 2026-04-21

These four tracks have `Overview / Scope / Success Criteria` plus Phase/Task decompositions appended 2026-04-21.

- [ ] **Track: AI Builder & Custom Connectors — Improved**
  *Link: [./tracks/ai_builder_connectors_improved_20260427/](./tracks/ai_builder_connectors_improved_20260427/)*
  *Depends on: `correlation_id_telemetry_20260427`, `connector_schema_validation_20260427`.*

- [ ] **Track: AI Wrapper Service**
  *Link: [./tracks/ai_wrapper_service_20260427/](./tracks/ai_wrapper_service_20260427/)*
  *Introduces `IAiBuilderService` abstraction; shared resilience + correlation ID plumbing for all AI/connector ops.*

- [ ] **Track: Connector Schema Validation**
  *Link: [./tracks/connector_schema_validation_20260427/](./tracks/connector_schema_validation_20260427/)*
  *OpenAPI schema validation before `connector import/test/export`; CI-friendly `pacx connector validate`.*

- [ ] **Track: Correlation ID & Telemetry**
  *Link: [./tracks/correlation_id_telemetry_20260427/](./tracks/correlation_id_telemetry_20260427/)*
  *Must land before the other three 2026-04-27 tracks — provides `ICorrelationIdProvider`.*

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
