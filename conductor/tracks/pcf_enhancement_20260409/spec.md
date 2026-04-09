# Specification: PCF Enhancement

## Overview
Enhance the existing `pac pcf` commands with CI/CD-friendly features: headless testing, targeted publishing, version management, and dependency validation. The official `pac pcf` is limited to build/start.

## Scope
- **PCF Test:** Run PCF component tests in headless mode for CI/CD pipeline integration.
- **PCF Publish:** Publish a single PCF component to an environment without full solution import.
- **Version Bump:** Semantic version management for PCF components (major/minor/patch).
- **Dependency Check:** Validate that all PCF dependencies (datasets, APIs, features) are satisfied in the target environment.

## Constraints
- PCF test harness must work without a browser (headless).
- Publish must be incremental — only changed components.
- Version bump must update ControlManifest.Input.xml and produce a changelog entry.

## Dependencies
- Existing `pac pcf` commands as baseline.

## Success Criteria
- A CI/CD pipeline can run `pacx pcf test` and get a pass/fail result with no browser.
- A developer can run `pacx pcf publish --component MyComponent --env dev` without importing a full solution.
- `pacx pcf version bump --type patch` updates the manifest and commits the change.

## API Readiness
- **PCF Test:** PCF test harness (npm-based, run via Node interop or harness CLI)
- **PCF Publish:** Dataverse Web API (solutioncomponent, webresource for PCF)
- **Version Bump:** Local file manipulation (ControlManifest.Input.xml)
- **Dependency Check:** Dataverse Web API (environment feature check)
