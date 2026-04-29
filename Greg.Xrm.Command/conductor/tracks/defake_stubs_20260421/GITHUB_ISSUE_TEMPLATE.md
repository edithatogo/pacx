## Overview

Several commands in Greg.Xrm.Command were previously marked as VERIFIED but contain executors that print [DRY RUN], API documentation text, or placeholder implementations instead of calling the real underlying APIs. This issue tracks the systematic de-faking of these executors to provide real, production-ready functionality.

## Background

Upstream PR review flagged these as "fake" implementations:
- ALM Pipeline (alm pipeline create/run)
- Connector Import/Test (connector import/test)
- Catalog Publish (catalog publish)
- Data Import/Export (data import/export)

## Remediation Strategy

This work is organized into 6 phases:

### Phase 1: Audit & Inventory ✅
- Scanned codebase for [DRY RUN], placeholder text patterns
- Produced canonical audit list with file:line references (conductor/tracks/defake_stubs_20260421/stubs.md)
- Categorized each: real (API available), partial (half-done), deferred (blocker)

### Phase 2: ALM Pipeline (Highest Visibility) ✅
- Implemented AlmPipelineCreateCommandExecutor with real BAP Admin API
- Implemented AlmPipelineRunCommandExecutor with real deployment
- Added mocked unit + integration test framework

### Phase 3: Connector Import/Export ✅
- Verified ConnectorImportCommandExecutor uses real Dataverse
- Verified ConnectorTestCommandExecutor POSTs to BAP Admin endpoint
- Added validation and error handling

### Phase 4: Catalog Publish & Data Import ✅
- Fixed CatalogPublishCommandExecutor to call PublishCatalogItem message
- Verified DataExportImportCommandExecutors implement real JSON import with upsert
- Added batch processing and EntityReference deserialization

### Phase 5: Honesty Sweep ✅
- Identified remaining stubs (API unreachable from CLI)
- Demoted affected tracks from [x] to [~] in conductor/tracks.md
- Documented intentionally deferred work

### Phase 6: PR Lifecycle (Current)
- [ ] Open GitHub issue (this issue)
- [ ] Create PRs per phase for upstream review
- [ ] Run automated review and fix issues
- [ ] Confirm merges

## Key Implementation Details

**APIs Used:**
- ALM: Power Platform Admin API (api.bap.microsoft.com) for pipeline operations
- Connectors: Dataverse connector.Create and BAP test endpoint
- Catalog: Dataverse PublishCatalogItem OrganizationRequest
- Data: Dataverse batch import with JSON parsing and upsert mode

**Files Modified:**
- Greg.Xrm.Command/Commands/Catalog/CatalogPublishCommandExecutor.cs
- Greg.Xrm.Command/Commands/Alm/AlmCommandExecutors.cs
- Greg.Xrm.Command/Commands/Connector/ConnectorCommandExecutors.cs
- Greg.Xrm.Command/Commands/Data/DataExportImportCommandExecutors.cs
- conductor/tracks.md
- conductor/tracks/defake_stubs_20260421/

## Status

- Phases 1-5: Complete
- Phase 6: In progress (awaiting upstream review)

See conductor/tracks/defake_stubs_20260421/ for complete track documentation and audit results.
