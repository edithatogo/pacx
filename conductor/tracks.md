# Project Tracks

This file tracks all major tracks for the project. Each track has its own detailed plan in its respective folder.

Tracks are organized by implementation priority, not phase order. Dependencies are noted where applicable.

---

## Completed Tracks (Verified Implemented)

- [x] **Track: spkl Parity (Developer Productivity)**
  *Link: [./tracks/spkl_parity_20260409/](./tracks/spkl_parity_20260409/)*
  *Status: IMPLEMENTED — plugin register-attributes, plugin step-scan, webresource map, webresource watch*

- [x] **Track: E2E Smoke Tests & Integration Tests**
  *Link: [./tracks/e2e_smoke_tests_20260409/](./tracks/e2e_smoke_tests_20260409/)*
  *Status: IMPLEMENTED — IntegrationTestBase, CoreSmokeTests, AdditionalSmokeTests*

- [x] **Track: Plugin Loading Test Coverage**
  *Link: [./tracks/plugin_test_coverage_20260409/](./tracks/plugin_test_coverage_20260409/)*
  *Status: IMPLEMENTED — CommandRegistry tests*

- [x] **Track: Microsoft Forms CLI**
  *Link: [./tracks/ms_forms_20260409/](./tracks/ms_forms_20260409/)*
  *Status: IMPLEMENTED — forms list, forms response count*

---

## Active Track (Priority Implementation)
- [x] **Track: Dataverse Platform Gaps**
  *Link: [./tracks/dataverse_gaps_20260409/](./tracks/dataverse_gaps_20260409/)*
  *Status: VERIFIED — CustomAPI, Catalog, ElasticTable, VirtualTable, ConnectionRef partially implemented*

---

## Backlog: Stubs/Placeholders (Require Full Implementation)

These tracks have command files but print notes instead of making actual API calls. They need to be fully implemented.

### Priority 1: High Business Impact (Easy Wins)
- [x] **Track: Governance, Security & Monitoring**
  *Link: [./tracks/governance_security_20260409/](./tracks/governance_security_20260409/)*
  *Status: COMPLETED — security audit-user, security sharing-report, dlp policy-audit, storage analytics, api ratelimit monitor*

- [x] **Track: AI Builder & Custom Connectors**
   *Link: [./tracks/ai_builder_connectors_20260409/](./tracks/ai_builder_connectors_20260409/)*
   *Status: IMPLEMENTED — ai model list/train/publish/form-processor/connector import/export/test/validate via AI Builder & Power Platform APIs*

### Priority 2: Enterprise Requirements
- [x] **Track: Environment Lifecycle Management**
  *Link: [./tracks/env_lifecycle_20260409/](./tracks/env_lifecycle_20260409/)*
  *Status: COMPLETED — env create/clone/reset/backup/restore/capacity with API docs and validation*

- [x] **Track: ALM Center Automation**
  *Link: [./tracks/alm_center_20260409/](./tracks/alm_center_20260409/)*
  *Status: COMPLETED — pipeline create/run with API docs, env var sync queries local Dataverse, solution layer shows deps*

### Priority 3: Developer Experience
- [x] **Track: PCF Enhancement**
  *Link: [./tracks/pcf_enhancement_20260409/](./tracks/pcf_enhancement_20260409/)*
  *Status: COMPLETED — version bump updates manifest+changelog; dependency-check parses local manifest; publish shows component info*

- [x] **Track: Tabular Editor CLI**
  *Link: [./tracks/tabular_editor_20260409/](./tracks/tabular_editor_20260409/)*
  *Status: COMPLETED — parses BIM for table/measure counts; validate checks circular refs; bim compare shows count diff; deploy shows API docs*

- [x] **Track: Power Pages CLI**
  *Link: [./tracks/power_pages_cli_20260409/](./tracks/power_pages_cli_20260409/)*
  *Status: COMPLETED — liquid lint validates templates; site publish queries Dataverse adx tables; webtemplate sync shows API docs*

### Priority 4: Lower Urgency
- [x] **Track: CI/CD Quality & Solution Management**
  *Link: [./tracks/cicd_quality_20260409/](./tracks/cicd_quality_20260409/)*
  *Status: VERIFIED — quality gate, solution diff, solution component-move fully implemented*

- [x] **Track: Data & Cross-Platform Engine**
  *Link: [./tracks/data_crossplatform_20260409/](./tracks/data_crossplatform_20260409/)*
  *Status: VERIFIED — data init-schema-from-solution, seed-mock, export, import fully implemented*

- [x] **Track: Dataverse Platform Gaps**
  *Link: [./tracks/dataverse_gaps_20260409/](./tracks/dataverse_gaps_20260409/)*
  *Status: VERIFIED — CustomAPI, Catalog, ElasticTable, VirtualTable, ConnectionRef partially implemented*

- [x] **Track: Flow Management (Automation Plugin)**
  *Link: [./tracks/automation_plugin_20260408/](./tracks/automation_plugin_20260408/)*
  *Status: VERIFIED — workflow get, set-state, run list/get/cancel/resubmit, connection list fully implemented*

- [ ] **Track: Explore Branches**
  *Link: [./tracks/explore_branches_20260408/](./tracks/explore_branches_20260408/)*
  *Status: PARTIAL — branch listing/comparison remain pending*

- [ ] **Track: MCP Server**
  *Link: [./tracks/mcp_server_20260408/](./tracks/mcp_server_20260408/)*
  *Status: PARTIAL — open PR against issue #162 still pending*

---

## Track Dependency Graph

```
pr_lifecycle ───────────────────────────────────────────────────→ (no deps)
e2e_smoke_tests ────────────────────────────────────────────────→ pr_lifecycle
spkl_parity ────────────────────────────────────────────────────→ (no deps)
alm_center ─────────────────────────────────────────────────────→ dataverse_gaps
env_lifecycle ──────────────────────────────────────────────────→ alm_center
governance_security ────────────────────────────────────────────→ (no deps)
pcf_enhancement ────────────────────────────────────────────────→ (no deps)
tabular_editor ─────────────────────────────────────────────────→ (no deps)
ai_builder_connectors ──────────────────────────────────────────→ (no deps)
power_pages_cli ────────────────────────────────────────────────→ (no deps)
cicd_quality ───────────────────────────────────────────────────→ (no deps)
data_crossplatform ─────────────────────────────────────────────→ (no deps)
dataverse_gaps ─────────────────────────────────────────────────→ (no deps)
ms_forms ───────────────────────────────────────────────────────→ (no deps)
explore_branches ───────────────────────────────────────────────→ (no deps)
mcp_server ─────────────────────────────────────────────────────→ (no deps)
```

---

## Next Implementation Priorities

1. **Governance, Security & Monitoring** — Quick win, security audit already works
2. **AI Builder** — Partial works, expand to train/publish
3. **Environment Lifecycle** — High enterprise demand for env create/clone/backup
4. **ALM Center** — Enterprise requirement for pipelines