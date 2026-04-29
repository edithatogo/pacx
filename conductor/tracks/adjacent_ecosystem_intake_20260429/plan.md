# Implementation Plan: Adjacent Ecosystem Intake

## Overview
Build surfaces for discovering and consuming tools/flows from adjacent ecosystems (MCP, Flow Studio, skill packs).

## Scope
- Tool catalog for browsing available capabilities.
- Browse/install/run surfaces for tools and flows.
- Source catalog (MCP, Flow Studio, NuGet, npm, etc.).
- Flow MCP catalog for discovering MCP-enabled flows.
- Skill pack catalog for reusable capability bundles.

## Improvements
- Unified discovery across heterogeneous tool sources.
- Reduced friction for adopting community/shared tooling.

## Success Criteria
- `pacx tool browse` lists available tools from registered sources.
- `pacx tool install <name>` downloads and registers a tool.
- `pacx tool run <name> [args]` executes a tool.
- `pacx tool source add/remove/list` manages source catalogs.
- `pacx flow mcp list` discovers MCP flows.
- `pacx skill pack list/install` manages skill packs.

## Phases

### Phase 1: Tool catalog
- [ ] Task: Define `ToolCatalog` data model and storage.
- [ ] Task: `pacx tool browse` — discover tools from registered sources.
- [ ] Task: `pacx tool list` — show installed tools.
- [ ] Task: Tests and documentation.

### Phase 2: Tool lifecycle
- [ ] Task: `pacx tool install <name>` — download and register.
- [ ] Task: `pacx tool run <name> [args]` — execute with proper isolation.
- [ ] Task: `pacx tool remove <name>` — unregister.
- [ ] Task: Tests and documentation.

### Phase 3: Source catalog
- [ ] Task: `pacx tool source add <url>` — register a source.
- [ ] Task: `pacx tool source remove <name>` — unregister.
- [ ] Task: `pacx tool source list` — enumerate registered sources.
- [ ] Task: Tests and documentation.

### Phase 4: Flow MCP catalog
- [ ] Task: `pacx flow mcp list` — discover MCP-enabled flows.
- [ ] Task: `pacx flow mcp info <id>` — show flow metadata.
- [ ] Task: Integration with `flow_studio_mcp_surfaces`.
- [ ] Task: Tests and documentation.

### Phase 5: Skill pack catalog
- [ ] Task: `pacx skill pack list` — available skill packs.
- [ ] Task: `pacx skill pack install <pack>` — apply skill pack.
- [ ] Task: Integration with `dataverse_skill_pack_guidance`.
- [ ] Task: Tests and documentation.

### Phase 6: PR Lifecycle
- [ ] Task: Upstream PR; merge.
