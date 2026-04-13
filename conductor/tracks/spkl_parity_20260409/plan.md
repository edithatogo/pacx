# Implementation Plan: spkl Parity (Developer Productivity)

## Phase 1: Plugin Attribute Scanning
- [x] Task: Research and select DLL scanning library (Mono.Cecil vs System.Reflection.Metadata). *Selected MetadataLoadContext — already referenced in project*
- [x] Task: Define `[CrmPluginStep]`, `[CrmPluginImage]`, and `[CrmWebhook]` attribute classes in a shared attributes project. *Already in Greg.Xrm.Command.Interfaces/PluginAttributes.cs*
- [x] Task: Implement attribute scanner that reads compiled DLLs and extracts plugin step metadata. *PluginScanner.cs using MetadataLoadContext*
- [x] Task: Write unit tests for attribute scanner with mock DLLs. *PluginScannerTest.cs already exists*

## Phase 2: Plugin Registration Command
- [x] Task: Implement `plugin register-attributes` command (Command + CommandExecutor pattern). *PluginRegisterAttributesCommand.cs + Executor*
- [x] Task: Implement Dataverse upsert logic for plugin assemblies, types, and steps. *Full CRUD in PluginRegisterAttributesCommandExecutor.cs*
- [x] Task: Implement incremental deployment — only register changed plugins. *Checks existing records by name, updates vs creates*
- [x] Task: Implement `plugin step-scan` command for validation without deployment. *PluginStepScanCommand.cs + Executor with validation rules*
- [x] Task: Write unit tests for registration executor with mocked ServiceClient. *PluginStepScanCommandTest.cs*

## Phase 3: Web Resource Mapping
- [x] Task: Implement `webresource map` command — define file-to-resource mapping via YAML/JSON config. *WebResourceMapCommand.cs + Executor (JSON config)*
- [x] Task: Implement `webresource watch` command — file watcher that syncs changes to Dataverse on save. *WebResourceWatchCommand.cs + Executor with FileSystemWatcher + polling fallback*
- [x] Task: Support legacy folder structures (non-standard mappings). *Config-based mapping supports any path → uniqueName mapping*
- [x] Task: Write unit tests for mapping parser and watch logic. *WebResourceMapAndWatchCommandTest.cs*

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
