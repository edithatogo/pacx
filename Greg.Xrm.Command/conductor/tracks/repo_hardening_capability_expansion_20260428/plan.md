# Implementation Plan: Repo Hardening & Capability Expansion

## Overview
Consolidate the remaining repo-level improvements into one umbrella track covering release hardening, stronger validation, repo hygiene, diagnostics, and the next capability slices that were recommended after the current implementation pass.

## Scope
- Release path hardening: signed releases, provenance, SBOMs, and a documented promotion flow.
- Validation depth: real integration coverage, command parity checks, and broader schema/contract validation.
- Repo hygiene: reduce workspace drift, define a canonical checkout shape, and keep generated output bounded.
- Observability: structured diagnostics, correlation-aware traces, and an explicit "validate all" command.
- Capability expansion: environment compare/drift reporting, dependency visualization, bulk import/export helpers, and richer MCP tooling.

## Reference Signals
These adjacent projects point at capability shapes worth absorbing rather than copying wholesale:
- XrmToolBox: plugin-first tool discovery, a tool library, and a large catalog of discrete Dataverse utilities.
- Flow Studio capability surfaces and related skills: PACX-native flow build, debug, monitoring, governance, and environment discovery.
- Dataverse MCP / Dataverse skills: Dataverse-specific MCP and agent skill packaging for discovery, operations, and guided workflows.
- Power Platform CLI docs: command-group parity, launcher tooling, and discoverability across the PAC surface area.
- Dataverse / Power BI / PowerShell package ecosystems: SDK-backed operations and package/source catalog browsing as first-class commands.

## Improvements
- Lower release risk and clearer artifact provenance.
- Catch drift between commands, docs, and generated metadata earlier.
- Improve supportability when long-running commands fail.
- Make the repo easier to navigate and maintain across nested worktrees and generated artifacts.
- Add a few high-value workflow commands without scattering them across unrelated tracks.

## Success Criteria
- A release workflow exists for signed artifacts with provenance and SBOM output.
- Real integration coverage exercises the important commands against live services.
- A parity check can compare command definitions, help text, and generated docs.
- Schema and contract validation covers the major JSON-shaped payloads beyond connector definitions.
- The repo has one documented authoritative workspace shape and a clean generated-artifact policy.
- Commands emit structured diagnostics that carry correlation IDs through failures and retries.
- A `validate all` style entry point runs the core validation checks in one pass.
- Environment compare/drift reporting is available for the supported platform surfaces.
- Dependency visualization is available for solution and package relationships.
- Bulk import/export helpers exist for the requested Power BI and Fabric flows.
- MCP tooling exposes richer discovery and execution helpers for command surfaces.

## Phases

### Phase 1: Release and supply chain hardening
- [ ] Task: Define the signed release pipeline for binaries and packages.
- [ ] Task: Add provenance and SBOM generation to the release path.
- [ ] Task: Document the promotion path from `master` to release artifacts.
- [ ] Task: Add release verification checks for signatures and provenance metadata.

### Phase 2: Validation coverage and parity
- [ ] Task: Add integration coverage against real services for the commands with the most risk.
- [ ] Task: Add command parity validation for help text, options, defaults, and generated docs.
- [ ] Task: Extend schema and contract validation to other JSON-shaped payloads and manifests.
- [ ] Task: Add regression tests for the new validation surfaces.

### Phase 3: Diagnostics and orchestration
- [ ] Task: Add structured diagnostics for long-running commands and failures.
- [ ] Task: Ensure correlation IDs are surfaced in logs, summaries, and downstream calls.
- [ ] Task: Add a `validate all` style command that runs the core checks in one pass.
- [ ] Task: Add tests for diagnostics output and orchestration behavior.

### Phase 4: Repo hygiene and canonical workspace
- [ ] Task: Define the authoritative checkout/worktree shape in repo docs.
- [ ] Task: Tighten generated-output boundaries so local diagnostics and build artifacts stay out of source control.
- [ ] Task: Add cleanup guidance or automation for nested workspace scratch files.
- [ ] Task: Add tests or documentation that point contributors at the canonical workspace.

### Phase 5: Capability expansion
- [ ] Task: Add environment compare and drift reporting for supported surfaces.
- [ ] Task: Add dependency visualization for solution and package relationships.
- [ ] Task: Add bulk import/export helpers for Power BI and Fabric where the APIs support it.
- [ ] Task: Expand MCP tooling for richer discovery, execution, and validation workflows.
- [ ] Task: Add tests and docs for the new commands.

### Phase 6: Adjacent ecosystem intake
- [ ] Task: Add a plugin-style tool library model inspired by XrmToolBox for browsing, installing, and running discrete utilities.
- [ ] Task: Add MCP-packaged flow operations inspired by Flow Studio for build, debug, monitor, governance, and environment discovery.
- [ ] Task: Add Dataverse skill-pack and guidance surfaces inspired by Dataverse MCP / Dataverse-skills for common operational workflows.
- [ ] Task: Add PAC CLI surface mapping and launcher/discovery parity against the Microsoft Learn command-group model.
- [ ] Task: Add package/source browsing for the Dataverse, Power BI, and PowerShell ecosystems so external capabilities are discoverable in one place.

### Phase 7: Track decomposition
- [x] Task: Split the highest-confidence work into child tracks: `release_supply_chain_hardening_20260428` and `validation_parity_coverage_20260428`.
- [x] Task: Split the remaining ecosystem intake into `adjacent_ecosystem_intake_20260428`.
- [ ] Task: Split the remaining repo-hygiene and diagnostics work into narrower child tracks when it is ready to implement independently.
