# Implementation Plan: Tabular Power BI Integration

## Overview
Implement real Power BI Tabular operations. Currently all tabular deploy/compare/translate/roles/perspectives commands print "Note:" documentation instead of calling real APIs.

## Scope
- Deploy BIM models to Power BI via XMLA endpoints or REST API
- Compare local BIM against deployed model
- Full BIM structural comparison using TOM library
- Translation export/diff/deploy
- Role management (add measures to roles)
- Perspective management (list/create/delete/add-tables/remove-tables)

## Dependencies
- `powerbi_workspace_mgmt_20260421` — Existing Power BI client for auth and workspace operations
- Power BI Premium/Embedded capacity for XMLA endpoint access

## Libraries
- `Microsoft.AnalysisServices.Tabular` (TOM) — BIM model manipulation (note: .NET Framework only, may need compat shim)
- Power BI REST API — for dataset definition updates (`PUT /groups/{workspaceId}/datasets/{datasetId}/Default.UpdateDefinition`)

## Success Criteria
- `pacx tabular deploy` deploys BIM to a Power BI workspace via XMLA or REST
- `pacx tabular compare` shows diff between local BIM and deployed model
- `pacx tabular validate` catches structural issues (already works — local-only)
- `pacx tabular translate` exports/diffs/deploys translations
- `pacx tabular role add-measures` adds measures to roles
- `pacx tabular perspective *` manages perspectives
- All with proper auth via existing ITokenProvider chain

## Phases

### Phase 1: Deploy & Compare
- [x] Task: Add `ITabularClient` service wrapping Power BI REST API + XMLA
- [x] Task: Register in IoCModule
- [x] Task: De-fake `TabularDeployCommandExecutor` — deploy BIM via REST API
- [x] Task: De-fake `TabularDiffCommandExecutor` — fetch deployed model, diff locally
- [x] Task: Improve `BimCompareCommandExecutor` — show table name diffs (added/removed/changed)
- [ ] Task: Tests with mock HTTP/XMLA responses

### Phase 2: Translation Management
- [x] Task: Add translation CRUD methods to `ITabularClient`
- [x] Task: De-fake `TabularTranslateCommandExecutor` — deploy translations via REST API
- [ ] Task: Tests

### Phase 3: Roles & Perspectives
- [x] Task: De-fake `TabularRoleAddMeasuresCommandExecutor` — modify roles via BIM deployment
- [x] Task: De-fake `TabularPerspectiveManageCommandExecutor` — perspective CRUD via BIM deployment
- [ ] Task: Tests

### Phase 4: Documentation & CI
- [ ] Task: Document Premium/Embedded requirements
- [ ] Task: Update Known Stubs in tracks.md
