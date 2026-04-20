# PacPacx Repository Standards

## Overview
This repo contains infrastructure and track definitions for the PacPacx Power Platform automation project.

## Structure
- conductor/tracks/* — Individual tracks with metadata.json and plan.md
- .github/workflows — CI/CD pipelines
- docs/ — High-level docs (optional)

## Getting Started
- Ensure .NET 8 SDK is installed (see global.json).
- Run `dotnet restore && dotnet build` to verify.
- Run unit tests with `dotnet test`.

## Contributing
See CONTRIBUTING.md for setup and PR guidelines.

## Quality
- Code must pass `dotnet format --verify-no-changes`.
- All non-trivial logic should have unit tests.
- Use the defined SDK version (global.json).

## CI/CD
- Pushes to master/main trigger build + test.
- Tag pushes produce build artifacts.
- PRs must pass checks before merge.

## Licensing
See LICENSE (add if missing).