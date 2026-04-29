# Implementation Plan: Adjacent Ecosystem Intake

## Overview
Translate useful patterns from adjacent ecosystems into PACX capabilities without copying product surfaces wholesale.

## Scope
- Plugin-style tool discovery and execution inspired by XrmToolBox.
- MCP-packaged flow operations inspired by Flow Studio.
- Dataverse skill-pack and guided workflow surfaces inspired by Dataverse MCP / Dataverse-skills.
- PAC CLI surface mapping and launcher/discovery parity against the Microsoft Learn command-group model.
- Package/source browsing for Dataverse, Power BI, and PowerShell ecosystems.

## Improvements
- Make the repo feel like a tool ecosystem, not just a flat CLI.
- Improve discoverability for discrete utilities and workflows.
- Let users browse, install, and run capabilities from one place.
- Surface external ecosystem knowledge in a way that can be extended over time.

## Success Criteria
- Users can discover utilities in a tool-library style view.
- Flow-related MCP workflows have a clear packaged surface.
- Dataverse operational guidance is available as reusable skill packs.
- PAC CLI command-group discovery is mirrored in the repo's docs and metadata.
- Package/source browsing surfaces at least the main Dataverse, Power BI, and PowerShell providers.

## Phases

### Phase 1: Tool-library model
- [x] Task: Define the plugin/tool library data model.
- [x] Task: Add browse/install/run surfaces for discrete utilities.
- [x] Task: Add tests for discovery and launch behavior.

### Phase 2: Flow and Dataverse capability surfaces
- [x] Task: Define packaged Flow Studio-style capability surfaces.
- [x] Task: Define reusable Dataverse skill-pack guidance surfaces.
- [x] Task: Add tests for command registration and invocation.

### Phase 3: CLI and package discovery
- [x] Task: Add PAC CLI surface mapping in docs and metadata.
- [x] Task: Add package/source browsing for NuGet and PowerShell Gallery entries.
- [x] Task: Add docs for Dataverse, Power BI, and related providers.

### Phase 4: Track decomposition
- [x] Task: Split large subareas into narrower child tracks once the data model settles.
