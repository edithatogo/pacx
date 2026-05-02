# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Self-update command (`pacx self-update`)
- Diagnostics command (`pacx diag`)
- Silent/quiet output modes
- Progress bars for long operations
- YAML output format via YamlDotNet
- Config auto-discovery (`pacx.config.json` directory walk)
- Multi-target .NET 10.0 + 11.0 support
- Docker image with GitHub Container Registry publish
- PowerShell module (5 cmdlets)
- VS Code extension (4 commands)
- GitHub Action (Docker-based)
- Azure DevOps task extension
- IaC integration guide (Bicep + Terraform)

### Changed
- All Microsoft Forms executors de-faked with real API calls
- Environment lifecycle (backup/restore/clone) now uses BAP API with async polling
- Tabular BIM operations now use Power BI REST API
- PCF test/publish/dependency-check now use pac CLI and Dataverse SDK
- Quality gate now supports `--run-check` to invoke pac solution check
- CI/CD: added dependency review, auto-merge for non-major deps, benchmark regression gates
- Code quality: SonarCloud workflow, API compatibility checks, enhanced Husky hooks
- Dockerfile: non-root user, layer caching, distroless runtime-deps image

### Security
- Removed NU1900 suppression — NuGet vulnerability warnings now fail build
- Warnings-as-errors for all vulnerability audit levels

### Fixed
- Dead code in CommandRunnerBase (unreachable IsFaulted/IsCanceled checks)
- CI matrix bug causing 12 builds instead of 6
- Sync-over-async patterns in 70+ test files
