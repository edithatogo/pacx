# Implementation Plan: spkl Parity (Developer Productivity)

## Phase 1: Plugin Attribute Scanning
- [ ] Task: Research and select DLL scanning library (Mono.Cecil vs System.Reflection.Metadata).
- [ ] Task: Define `[CrmPluginStep]`, `[CrmPluginImage]`, and `[CrmWebhook]` attribute classes in a shared attributes project.
- [ ] Task: Implement attribute scanner that reads compiled DLLs and extracts plugin step metadata.
- [ ] Task: Write unit tests for attribute scanner with mock DLLs.
- [ ] Task: Run automated /conductor:review

## Phase 2: Plugin Registration Command
- [ ] Task: Implement `plugin register-attributes` command (Command + CommandExecutor pattern).
- [ ] Task: Implement Dataverse upsert logic for plugin assemblies, types, and steps.
- [ ] Task: Implement incremental deployment — only register changed plugins.
- [ ] Task: Implement `plugin step-scan` command for validation without deployment.
- [ ] Task: Write unit tests for registration executor with mocked ServiceClient.
- [ ] Task: Run automated /conductor:review

## Phase 3: Web Resource Mapping
- [ ] Task: Implement `webresource map` command — define file-to-resource mapping via YAML/JSON config.
- [ ] Task: Implement `webresource watch` command — file watcher that syncs changes to Dataverse on save.
- [ ] Task: Support legacy folder structures (non-standard mappings).
- [ ] Task: Write unit tests for mapping parser and watch logic.
- [ ] Task: Run automated /conductor:review

## Phase 4: Integration & CI/CD
- [ ] Task: Add end-to-end integration test against a real/test Dataverse environment.
- [ ] Task: Document usage examples in markdown (command help text + README).
- [ ] Task: Verify code coverage >80% for all new code.
- [ ] Task: Run automated /conductor:review

## Phase 5: PR Lifecycle (Ralph Loop)
- [ ] Task: Open a GitHub issue describing the spkl parity feature.
- [ ] Task: Create a PR against the upstream repo with implementation.
- [ ] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge"
- [ ] Task: Confirm PR is merged or document blockers.
