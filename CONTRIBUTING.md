# PacPacx Repository Standards

## Overview
This repo contains infrastructure and track definitions for the PacPacx Power Platform automation project.

## Structure
- conductor/tracks/* — Individual tracks with metadata.json and plan.md
- .github/workflows — CI/CD pipelines
- docs/ — High-level docs (optional)

## Getting Started
- Ensure the .NET 11 preview SDK is installed (see `global.json`).
- If you do not have admin rights, install the SDK under your user profile and set `DOTNET_ROOT=%USERPROFILE%\\.dotnet` before running `dotnet`.
- If restore or build resolves an older SDK, clear any `MSBuildSDKsPath` override in the shell first.
- Run `dotnet restore && dotnet build` to verify.
- Run unit tests with `dotnet test`.
- To use the containerized dev environment, open `.devcontainer/devcontainer.json` in VS Code or a compatible devcontainer client.
- `postCreateCommand` restores local tools, installs Husky hooks, and restores the solution inside the container.
- Run `dotnet husky install` once after cloning if you want local pre-commit and commit-msg hooks.
- The repo's Husky hooks run TruffleHog at pre-commit and commitlint at commit-msg.
- Use the PR template and issue templates under `.github/` so summaries, test plans, and security disclosures stay consistent.

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
