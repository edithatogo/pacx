# Specification: Microsoft Forms CLI

## Overview
Provide CLI access to the undocumented Microsoft Forms API for listing forms, exporting responses, and monitoring response counts. This enables compliance reporting, data archival, and automated form analytics.

## Scope
- **Forms List:** List all forms with metadata (ID, title, status, response count, owner, creation/modification dates).
- **Responses Export:** Export responses to CSV, JSON, or SQL with paged retrieval (`$skip`/`$top` for incremental sync).
- **Response Count:** Quick count of responses for monitoring and alerting.

## Constraints
- **Undocumented API** — `forms.office.com/formapi/api/` is internal; may change without notice.
- **User forms:** Work with Application permissions (Client Credentials flow) — clean for automation.
- **Group forms:** Require ROPC flow (username/password, no MFA) — needs dedicated service account.
- **Read-only** — create/update/delete not available yet.

## Dependencies
- MSAL (`Microsoft.Identity.Client`) for OAuth2 token management.
- Token caching and auto-refresh (401 retry logic).

## Success Criteria
- A compliance officer can run `pacx forms list --tenant contoso.onmicrosoft.com` and see all forms.
- An admin can run `pacx forms responses export --form-id abc123 --output responses.csv` to get all responses.
- A monitoring script can run `pacx forms response count --form-id abc123` and alert if responses exceed a threshold.

## API Readiness
- **List Forms:** `GET /formapi/api/{tenantId}/users/{userId}/light/forms`
- **Get Responses:** `GET /formapi/api/{tenantId}/users/{userId}/light/forms('{formId}')/responses?$expand=comments&$top=100&$skip=0`
- **Response Count:** `GET /formapi/api/{tenantId}/users/{userId}/light/forms('{formId}')?$select=rowCount`
- **Auth:** OAuth2 v2.0 token endpoint with Client Credentials (user forms) or ROPC (group forms)
