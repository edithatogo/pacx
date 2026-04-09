# Project Tracks

This file tracks all major tracks for the project. Each track has its own detailed plan in its respective folder.

Tracks are organized by implementation priority, not phase order. Dependencies are noted where applicable.

---

## Active Track
- [x] **Track: Implement unit tests for core Solution commands**
  *Link: [./tracks/solution_tests_20260408/](./tracks/solution_tests_20260408/)*

## Completed Tracks (Archived)
- [x] **Track: Establish Code Quality & Coverage Infrastructure**
  *Link: [./tracks/code_quality_20260408/](./tracks/code_quality_20260408/)*
- [x] **Track: Update CI/CD Pipeline to Run Tests**
  *Link: [./tracks/cicd_tests_20260408/](./tracks/cicd_tests_20260408/)*
- [x] **Track: Incorporate PR #146**
  *Link: [./tracks/pr146_integration_20260408/](./tracks/pr146_integration_20260408/)*
- [x] **Track: Explore and Incorporate Branches**
  *Link: [./tracks/explore_branches_20260408/](./tracks/explore_branches_20260408/)*
- [x] **Track: Review and Resolve GitHub Issues**
  *Link: [./tracks/resolve_issues_20260408/](./tracks/resolve_issues_20260408/)*
- [x] **Track: Implement an MCP Server**
  *Link: [./tracks/mcp_server_20260408/](./tracks/mcp_server_20260408/)*
- [x] **Track: Conduct Gap Analysis (Power Platform vs PAC CLI vs PACX)**
  *Link: [./archive/gap_analysis_20260408/](./archive/gap_analysis_20260408/)*

## Future Tracks (Backlog — Ordered by Priority)

### Tier 1: Foundation (Highest Impact)
- [ ] **Track: spkl Parity (Developer Productivity)**
  *Link: [./tracks/spkl_parity_20260409/](./tracks/spkl_parity_20260409/)*
  *Description: Attribute-based plugin registration and flexible web resource mapping — the two features keeping pro-code developers on spkl. Phase 1 of roadmap.*

- [ ] **Track: ALM Center Automation**
  *Link: [./tracks/alm_center_20260409/](./tracks/alm_center_20260409/)*
  *Description: Pipeline management, environment variable sync, environment diff, and solution layer management. Phase 7 of roadmap. Depends on: dataverse_gaps.*

- [ ] **Track: Flow Management (Automation Plugin)**
  *Link: [./tracks/automation_plugin_20260408/](./tracks/automation_plugin_20260408/)*
  *Description: Workflow run list/get/resubmit/cancel, workflow get/set-state, connection list. Already scaffolded, pending execution.*

### Tier 2: Platform Coverage
- [ ] **Track: Power Pages CLI**
  *Link: [./tracks/power_pages_cli_20260409/](./tracks/power_pages_cli_20260409/)*
  *Description: Site publish, web template sync, config export/import, liquid linting. Zero existing CLI support. Phase 8 of roadmap.*

- [ ] **Track: Environment Lifecycle Management**
  *Link: [./tracks/env_lifecycle_20260409/](./tracks/env_lifecycle_20260409/)*
  *Description: Create, clone, backup, restore, capacity report, reset. Prerequisite for enterprise sandbox workflows. Phase 9 of roadmap. Depends on: alm_center.*

- [ ] **Track: Governance, Security & Monitoring**
  *Link: [./tracks/governance_security_20260409/](./tracks/governance_security_20260409/)*
  *Description: Privilege audit, sharing reports, DLP policy audit, storage analytics, API rate limit monitor. Compliance-driven demand. Phase 10 of roadmap.*

### Tier 3: Developer Experience
- [ ] **Track: PCF Enhancement**
  *Link: [./tracks/pcf_enhancement_20260409/](./tracks/pcf_enhancement_20260409/)*
  *Description: Headless testing, targeted publish, version management, dependency check. Fills gaps in existing pac pcf. Phase 11 of roadmap.*

- [ ] **Track: Tabular Editor CLI (Power BI)**
  *Link: [./tracks/tabular_editor_20260409/](./tracks/tabular_editor_20260409/)*
  *Description: BIM deploy, diff, validate, translate, role operations. Replicates Tabular Editor 3 capabilities as CLI. Phase 6 of roadmap.*

- [ ] **Track: CI/CD Quality & Solution Management**
  *Link: [./tracks/cicd_quality_20260409/](./tracks/cicd_quality_20260409/)*
  *Description: Quality gate (solution check parsing), solution diff, component move. Phase 4 of roadmap.*

### Tier 4: Data & Emerging Features
- [ ] **Track: Data & Cross-Platform Engine**
  *Link: [./tracks/data_crossplatform_20260409/](./tracks/data_crossplatform_20260409/)*
  *Description: Pure .NET 6+ data engine (eliminates WPF/CMT for Mac/Linux), schema generation, mock data seeding. Phase 5 of roadmap.*

- [ ] **Track: Dataverse Platform Gaps**
  *Link: [./tracks/dataverse_gaps_20260409/](./tracks/dataverse_gaps_20260409/)*
  *Description: Custom APIs, Catalog/Business Events, Elastic Tables, Virtual Tables, Connection References. Phase 3 of roadmap.*

- [ ] **Track: AI Builder & Custom Connectors**
  *Link: [./tracks/ai_builder_connectors_20260409/](./tracks/ai_builder_connectors_20260409/)*
  *Description: AI model management, form processor configuration, connector import/export/test/validate. Phase 12 of roadmap.*

---

## Track Dependency Graph

```
spkl_parity ────────────────────────────────────────────────────→ (no deps)
alm_center ─────────────────────────────────────────────────────→ dataverse_gaps
automation_plugin ──────────────────────────────────────────────→ (no deps)
power_pages_cli ────────────────────────────────────────────────→ (no deps)
env_lifecycle ──────────────────────────────────────────────────→ alm_center
governance_security ────────────────────────────────────────────→ (no deps)
pcf_enhancement ────────────────────────────────────────────────→ (no deps)
tabular_editor ─────────────────────────────────────────────────→ (no deps)
cicd_quality ───────────────────────────────────────────────────→ (no deps)
data_crossplatform ─────────────────────────────────────────────→ (no deps)
dataverse_gaps ─────────────────────────────────────────────────→ (no deps)
ai_builder_connectors ──────────────────────────────────────────→ (no deps)
```

## Execution Order (Recommended)
1. `spkl_parity` — highest developer impact
2. `dataverse_gaps` — unblocks `alm_center`
3. `alm_center` — enterprise adoption blocker
4. `automation_plugin` — already scaffolded, low effort
5. `cicd_quality` — CI/CD maturity
6. `data_crossplatform` — Mac/Linux unblock
7. `power_pages_cli` — green field, high value
8. `env_lifecycle` — depends on alm_center
9. `governance_security` — compliance demand
10. `pcf_enhancement` — developer experience
11. `tabular_editor` — Power BI differentiation
12. `ai_builder_connectors` — emerging, lower urgency
