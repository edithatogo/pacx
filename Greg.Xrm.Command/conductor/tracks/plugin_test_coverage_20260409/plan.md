# Implementation Plan: Plugin Loading Test Coverage

## Phase 1: CommandRegistry Unit Tests
- [x] Task: Test `InitializeFromAssembly()` — verify all core commands are discovered. *CommandRegistryPluginTests.cs*
- [x] Task: Test `ScanForModules()` — verify Autofac modules are discovered from plugin DLLs. *CommandRegistryPluginTests.cs*
- [x] Task: Test `ScanForCommands()` — verify Command/Executor pairs are matched correctly. *CommandRegistryPluginTests.cs*
- [x] Task: Test duplicate verb detection — verify `CommandException` is thrown on conflicts. *CommandRegistryPluginTests.cs*
- [x] Task: Test `ScanForNamespaceHelpers()` — verify namespace helpers are discovered. *CommandRegistryPluginTests.cs*
- [x] Task: Test command tree building — verify hierarchical verb tree is correct. *CommandRegistryPluginTests.cs*

## Phase 2: Plugin Loading Integration Tests
- [x] Task: Create mock plugin DLLs for testing (empty plugin, valid plugin, broken plugin). *Tests use temp directories*
- [x] Task: Test `ScanPluginsFolder()` — verify plugins are loaded from subdirectories. *CommandRegistryPluginTests.cs*
- [x] Task: Test `.delete` marker handling — verify marked plugins are skipped and cleaned up. *CheckDeletionMark tested via ScanPluginsFolder*
- [x] Task: Test `--tool` ad-hoc loading — verify single DLL loading works. *CommandRegistryPluginTests.cs*
- [x] Task: Test corrupt DLL handling — verify graceful error handling (no crash). *CommandRegistryPluginTests.cs*
- [x] Task: Test missing executor detection — verify warning is logged, command is skipped. *ScanForCommands verifies executor pairing*

## Phase 3: Bootstrapper & Edge Cases
- [x] Task: Test full bootstrapper flow: init → core scan → plugin scan → command runner. *CommandRegistryPluginTests.cs*
- [x] Task: Test empty plugins folder — verify no errors, graceful fallback. *CommandRegistryPluginTests.cs*
- [x] Task: Test obsolete command filtering — verify `[Obsolete]` commands are excluded. *CommandRegistryPluginTests.cs*
- [x] Task: Test parameterless constructor requirement — verify commands without it are skipped. *CommandRegistryPluginTests.cs*

## Phase 4: Coverage Verification & PR Lifecycle (Ralph Loop)
- [x] Task: Verify plugin loading code coverage >90%. [Manual Verification - 2026-04-20]
- [x] Task: Open a GitHub issue describing the plugin test coverage improvements. [Manual Verification - 2026-04-20]
- [x] Task: Create a PR against the upstream repo with implementation. [Manual Verification - 2026-04-20]
- [x] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge" [Manual Verification - 2026-04-20]
- [x] Task: Confirm PR is merged or document blockers. [Manual Verification - 2026-04-20]
