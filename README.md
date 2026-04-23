# PacPacx — AI Builder & Custom Connectors

Repository for Power Platform automation tracks and workflows.

## 📁 Contents

- `README.md` — This file
- `CONTRIBUTING.md` — Development setup and contribution guidelines
- `PAC_PACX_INVENTORY.md` — Complete list of all PAC & PACX commands and tools
- `POWER_PLATFORM_MCP_SETUP.md` — MCP server configuration guide
- `.mcp.json` — MCP server configuration template
- `tracks/` — Declarative track specifications and plans

## 🚀 Quick Start

### PAC CLI
```bash
pac auth list          # Check authentication
pac env list           # List environments
pac copilot mcp --run  # Start MCP server
```

### PACX CLI
```bash
pacx auth              # Setup authentication
pacx table list        # List Dataverse tables
pacx --interactive     # Interactive mode
```

### Build & Test (repo-level)
```bash
dotnet restore
dotnet build
dotnet test
```

If you do not have admin permissions, install the .NET SDK into your user profile and run it via `%USERPROFILE%\\.dotnet\\dotnet.exe` or by setting `DOTNET_ROOT=%USERPROFILE%\\.dotnet`.
If `dotnet` still resolves the wrong SDK, clear any stale `MSBuildSDKsPath` override in your shell before building.
For local commit hygiene, run `dotnet husky install` once after cloning, then keep `trufflehog` and `commitlint` available on your PATH.
The CI workflow now enforces the same Conventional Commits policy by linting PR titles and commit messages.
You can also open the repo in the devcontainer defined by `.devcontainer/devcontainer.json`; it restores tools, installs the hooks, and restores the solution on first start.
Pull requests and issues have templates under `.github/` so summaries, test plans, and security disclosures are captured consistently.

## 📋 Documentation

See `PAC_PACX_INVENTORY.md` for the complete list of all available commands and installed tools.

See `POWER_PLATFORM_MCP_SETUP.md` for MCP server configuration and setup instructions.

## 🏗️ Tracks

Tracks are stored under `tracks/`. Each track has a `metadata.json` and a `plan.md`. The preferred, improved tracks to use are:
- `tracks/upstream_baseline_sync_20260422/` - gates downstream planning work on upstream branch sync
- `tracks/ai_builder_connectors_improved_20260427/`
- `tracks/ai_wrapper_service_20260427/`
- `tracks/connector_schema_validation_20260427/`
- `tracks/correlation_id_telemetry_20260427/`
- `tracks/dry_run_and_checkpoint_20260427/`
- `tracks/rate_limit_handling_20260427/`
- `tracks/secrets_vault_integration_20260427/`
- `tracks/schema_versioning_20260427/`
- `tracks/help_command_20260427/`

## 🛡️ Quality & CI

- CI: `.github/workflows/ci.yml` runs build → test → lint on PRs and validates Conventional Commit titles/messages; deploy on tags.
- Formatting: `dotnet format` (enforced via `.editorconfig`), plus `editorconfig-checker` in CI for repo-wide file hygiene.
- SDK: pinned via `global.json` (v10.0.x); use the user-profile install path if you do not have admin rights.
- Tests: add unit tests under each track’s test project as needed.
- Local hooks: `.husky/pre-commit` runs TruffleHog and `.husky/commit-msg` runs commitlint.
- Containerized build: `Dockerfile` restores, builds, and publishes the CLI from the repo root using the pinned .NET 10 SDK line.

## 🔐 Security

- Do not commit secrets.
- Use vault integration for production credentials.
- Follow least-privilege for Azure RBAC.

## 📄 License

TBD — add LICENSE file.
