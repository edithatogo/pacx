# Specification: Governance, Security & Monitoring

## Overview
Provide CLI tools for security auditing, DLP policy review, storage analysis, and API monitoring. These are increasingly mandatory for enterprise compliance and governance requirements.

## Scope
- **User Privilege Audit:** Full privilege analysis — what can a specific user actually do? (field-level, record-level, hierarchy-based access).
- **Record Sharing Report:** Who has access to a specific record, and why? (direct share, team, business unit, role inheritance).
- **DLP Policy Audit:** Review and report on Data Loss Prevention policy coverage across connectors and environments.
- **Storage Analytics:** Table-by-table storage analysis with cleanup recommendations (duplicates, orphaned records, audit log bloat).
- **API Rate Limit Monitor:** Monitor and alert on API rate limit proximity.

## Constraints
- Security audit must be read-only — no modifications to security configuration.
- Storage analysis must handle large tables efficiently (batched queries).
- DLP policy audit requires Power Platform Admin permissions.

## Dependencies
- None — operates on existing Dataverse and Admin APIs.

## Success Criteria
- A security admin can run `pacx security audit-user john.doe@contoso.com` and get a full privilege report.
- A developer can run `pacx security sharing-report account:abc-123` and see who has access and why.
- Storage analytics produces actionable cleanup recommendations.

## API Readiness
- **User Privileges:** Dataverse Web API (role, systemuserroles, privilege, roleprivileges) + RetrievePrincipalAccess
- **Record Sharing:** PrincipalObjectAccess table + RetrieveSharedPrincipalsAndAccess action
- **DLP Policies:** Power Platform Admin API (DlpPolicy)
- **Storage:** Dataverse Web API (organization stats) + Admin API
- **Rate Limits:** HTTP response headers (x-ratelimit) + organization settings
