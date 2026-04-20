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

## 📋 Documentation

See `PAC_PACX_INVENTORY.md` for the complete list of all available commands and installed tools.

See `POWER_PLATFORM_MCP_SETUP.md` for MCP server configuration and setup instructions.

## 🏗️ Tracks

Tracks are stored under `tracks/`. Each track has a `metadata.json` and a `plan.md`. The preferred, improved tracks to use are:
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

- CI: `.github/workflows/ci.yml` runs build → test → lint on PRs; deploy on tags.
- Formatting: `dotnet format` (enforced via `.editorconfig`).
- SDK: pinned via `global.json` (v8.0.x).
- Tests: add unit tests under each track’s test project as needed.

## 🔐 Security

- Do not commit secrets.
- Use vault integration for production credentials.
- Follow least-privilege for Azure RBAC.

## 📄 License

TBD — add LICENSE file.
