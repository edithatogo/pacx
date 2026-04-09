# Implementation Plan: Plugin Loading Test Coverage

## Phase 1: CommandRegistry Unit Tests
- [ ] Task: Test `InitializeFromAssembly()` — verify all core commands are discovered.
- [ ] Task: Test `ScanForModules()` — verify Autofac modules are discovered from plugin DLLs.
- [ ] Task: Test `ScanForCommands()` — verify Command/Executor pairs are matched correctly.
- [ ] Task: Test duplicate verb detection — verify `CommandException` is thrown on conflicts.
- [ ] Task: Test `ScanForNamespaceHelpers()` — verify namespace helpers are discovered.
- [ ] Task: Test command tree building — verify hierarchical verb tree is correct.
- [ ] Task: Run automated /conductor:review

## Phase 2: Plugin Loading Integration Tests
- [ ] Task: Create mock plugin DLLs for testing (empty plugin, valid plugin, broken plugin).
- [ ] Task: Test `ScanPluginsFolder()` — verify plugins are loaded from subdirectories.
- [ ] Task: Test `.delete` marker handling — verify marked plugins are skipped and cleaned up.
- [ ] Task: Test `--tool` ad-hoc loading — verify single DLL loading works.
- [ ] Task: Test corrupt DLL handling — verify graceful error handling (no crash).
- [ ] Task: Test missing executor detection — verify warning is logged, command is skipped.
- [ ] Task: Run automated /conductor:review

## Phase 3: Bootstrapper & Edge Cases
- [ ] Task: Test full bootstrapper flow: init → core scan → plugin scan → command runner.
- [ ] Task: Test empty plugins folder — verify no errors, graceful fallback.
- [ ] Task: Test obsolete command filtering — verify `[Obsolete]` commands are excluded.
- [ ] Task: Test parameterless constructor requirement — verify commands without it are skipped.
- [ ] Task: Run automated /conductor:review

## Phase 4: Coverage Verification & PR Lifecycle (Ralph Loop)
- [ ] Task: Verify plugin loading code coverage >90%.
- [ ] Task: Open a GitHub issue describing the plugin test coverage improvements.
- [ ] Task: Create a PR against the upstream repo with implementation.
- [ ] Task: Run `/ralph-loop` on the PR with completion promise: "All Critical and High review issues resolved, PR ready for merge"
- [ ] Task: Confirm PR is merged or document blockers.
