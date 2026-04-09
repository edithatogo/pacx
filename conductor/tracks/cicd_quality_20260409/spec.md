# Specification: CI/CD Quality & Solution Management

## Overview
Enhance CI/CD pipeline reliability with quality gates and granular solution management. These commands make pacx a first-class citizen in automated deployment pipelines.

## Scope
- **Quality Gate:** Parse `pac solution check` results and return non-zero exit code on "High" severity issues — replacing the spreadsheet output with CI/CD-friendly exit codes.
- **Solution Diff:** Compare two solutions or environments and report component differences (added, removed, modified).
- **Solution Component Move:** Move individual components between solutions without whole-solution import/export.

## Constraints
- Quality gate must integrate seamlessly with GitHub Actions and Azure DevOps pipelines.
- Solution diff must handle large solution files efficiently.
- Component move must respect dependency ordering.

## Dependencies
- Existing `pac solution check` command output format.
- Existing solution commands from the codebase.

## Success Criteria
- A CI/CD pipeline fails with a clear error message when `pacx quality gate` finds "High" severity issues.
- A developer can run `pacx solution diff --source dev --target prod` and see exactly what's different.
- Individual components can be moved between solutions without full import/export cycles.

## API Readiness
- **Quality Gate:** Parse `pac solution check` output (ZIP with Excel/report) — local processing
- **Solution Diff:** Dataverse Web API (solution, solutioncomponent) — query and compare
- **Component Move:** Dataverse Web API (solutioncomponent add/remove)
