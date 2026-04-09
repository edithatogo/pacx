# Specification: E2E Smoke Tests & Integration Test Infrastructure

## Overview
Create a dedicated integration test project and E2E smoke test suite that runs against a real Dataverse test environment. This is the safety net that catches issues unit tests can't find — real API behavior, authentication flows, network errors, and cross-command interactions.

## Scope
- **Integration Test Project:** New `Greg.Xrm.Command.Core.IntegrationTests` project with real `ServiceClient` connections (not mocks).
- **E2E Smoke Test Suite:** Minimal set of end-to-end tests that exercise the critical user paths: auth → org list → solution list → table create → table delete.
- **CI Integration:** Smoke tests run as a separate GitHub Actions workflow triggered on PR merge (not on every commit — too slow/expensive).
- **Test Environment Configuration:** Secure storage of test environment credentials via GitHub Secrets.

## Constraints
- Tests must be idempotent — they create and clean up their own data.
- Tests must not modify production environments.
- Credentials must never be logged or exposed in CI output.
- Tests should have configurable timeouts for slow Dataverse operations.

## Dependencies
- `pr_lifecycle` track — CI pipeline must be able to run tests with secrets.
- A dedicated test Dataverse environment (sandbox or developer tier).

## Success Criteria
- `dotnet test --filter "Category=Integration"` runs against a real environment and passes.
- CI workflow fails if smoke tests fail.
- Tests clean up all created resources (no orphaned test tables/solutions).
- Average smoke test suite completes in under 5 minutes.

## API Readiness
- **Dataverse Web API:** Real `ServiceClient` against test environment
- **GitHub Actions:** Secrets for credentials, separate workflow for E2E tests
- **Test Framework:** MSTest (consistent with existing test suite) + `ServiceClient` integration
