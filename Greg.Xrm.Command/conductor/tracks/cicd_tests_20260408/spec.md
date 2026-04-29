# Specification: Update CI/CD Pipeline to Run Tests

## Overview
This track updates the GitHub Actions workflow to ensure that all unit tests are executed automatically on every push and pull request.

## Scope
- **Workflow Modification:** Update `.github/workflows/build-pipeline-01.yml` to add a `dotnet test` step.
- **Coverage Integration:** Ensure the test step uses the newly created `CodeCoverage.runsettings` and generates a report.
- **Fail-Fast:** Ensure the workflow fails if any tests fail.

## Constraints
- Do not interfere with the existing build and publish steps.
- Use the same .NET SDK version as defined in the build step.
