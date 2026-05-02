# Implementation Plan: Microsoft Forms API Integration

## Overview
De-fake all stubbed Microsoft Forms command executors and implement real Forms Office API integration. Add missing features discovered through ecosystem research (PowerShell cmdlets, Selenium bots, admin reporting patterns).

## Research Sources
- **jackparker.co.uk** — PowerShell TokenManager, Get-MSFormsByOwner/Response/Responses, ROPC flow for groups
- **michev.info** — Tenant-wide Forms reporting, Forms.Read.All application permissions, group enumeration
- **Adam-ZS/Microsoft-Forms-Fill-Bot** — Selenium-based form submission automation
- **glazedtorus/Selenium-for-MS-forms** — Selenium automated form completion
- **Microsoft Learn** — Admin settings (external sharing, phishing protection, record names), quiz/assessment features
- **dataMinerMsForms.api** — C# ASP.NET Core Forms API wrapper

## Scope
- Real Forms Office API calls in all stubbed executors
- `FormsApiClient` service for authenticated HTTP calls to `forms.office.com/formapi/api/`
- Token management (MSAL client credentials + ROPC for groups)
- Group-owned forms support
- Admin tenant-wide reporting
- Form close/reopen, duplication, settings management
- PowerShell module wrapper
- Tests for all real API interactions

## Success Criteria
- `pacx forms list` returns real form data from the Forms Office API
- `pacx forms response count` returns real response counts
- `pacx forms responses export` exports real responses to CSV/JSON/SQL
- Group-owned forms accessible via ROPC flow
- Admin commands for tenant-wide reporting
- All tests pass with mocked HTTP responses

## Phases

### Phase 1: Forms API Client Service (DONE)
- [x] Task: `IFormsApiClient` interface with methods for all Forms API endpoints
- [x] Task: `FormsApiClient` implementation using HttpClient + MSAL token acquisition
- [x] Task: Token caching and refresh (client credentials + ROPC flows)
- [x] Task: Register in DI via IoCModule

### Phase 2: Real Executors — List & Response Count (DONE)
- [x] Task: De-fake `FormsListCommandExecutor` — call Forms API, render table
- [x] Task: De-fake `FormsResponseCountCommandExecutor` — call $select=rowCount
- [x] Task: Support owner-type (User vs Group) for both commands
- [x] Task: Parse tests and executor tests

### Phase 3: Real Executors — Responses Export (DONE)
- [x] Task: De-fake `FormsResponsesExportCommandExecutor` — paged response retrieval
- [x] Task: CSV output with headers mapped from form questions
- [x] Task: JSON output with structured answer data
- [x] Task: SQL output with INSERT statements
- [x] Task: Incremental export (track last export position)
- [x] Task: Tests

### Phase 4: Real Executors — Advanced Commands (DONE)
- [x] Task: De-fake `FormsBranchingExportCommandExecutor`
- [x] Task: De-fake `FormsAnalyticsSummaryCommandExecutor`
- [x] Task: De-fake `FormsAnalyticsTimeseriesCommandExecutor`
- [x] Task: De-fake `FormsShareCommandExecutor`
- [x] Task: De-fake `FormsOwnershipTransferCommandExecutor`
- [x] Task: De-fake `FormsTemplateListCommandExecutor`
- [x] Task: De-fake `FormsTemplateCreateCommandExecutor`
- [x] Task: De-fake `FormsTemplateShareCommandExecutor`
- [x] Task: Tests

### Phase 5: Admin & Management Commands (DONE)
- [x] Task: `forms admin report` — tenant-wide form inventory across all users
- [x] Task: `forms close` / `forms reopen` — control response collection (documentation-only, API unsupported)
- [x] Task: `forms duplicate` — clone a form (documentation-only, API unsupported)
- [x] Task: `forms response get <id>` — get single response details
- [x] Task: Tests

### Phase 6: PowerShell Module & Documentation (DONE)
- [x] Task: `docs/guides/forms-powershell.md` — PowerShell usage guide with CI/CD examples
- [x] Task: `docs/references/forms-api.md` — comprehensive undocumented API reference
- [x] Task: Examples for CI/CD integration (GitHub Actions + Azure DevOps in guide)
- [x] Task: All old stubbed executor files removed (FormsCommandExecutors.cs, FormsAdvancedCommandExecutors.cs)
