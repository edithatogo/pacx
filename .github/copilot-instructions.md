# Copilot Instructions for pac_pacx

This repository is a workspace for Power Platform / Dataverse tooling docs and environment setup. The nested `Greg.Xrm.Command/` directory contains the PACX CLI source and has its own Copilot instructions file.

## Working in this repo

- Use `README.md`, `POWER_PLATFORM_MCP_SETUP.md`, and `PAC_PACX_INVENTORY.md` as the source of truth for CLI setup details.
- Do not rely on any repo-local MCP servers in the root `.mcp.json`.
- Do not treat PACX as an MCP server; it is a CLI tool only and should be used via `pacx ...` commands.
- Avoid changing generated conductor artifacts unless the task explicitly targets them.

## Editing guidance

- Keep documentation consistent with the actual installed tool state.
- Prefer small, surgical changes over broad rewrites.
- If you need the PACX project-specific rules, read `Greg.Xrm.Command/.github/copilot-instructions.md`.
