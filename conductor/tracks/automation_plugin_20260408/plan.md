# Implementation Plan: Extend Automation Capabilities (as Plugin)

## Phase 1: Plugin Scaffolding
- [ ] Task: Create the `Greg.Xrm.Command.Plugin.Automation` project.
- [ ] Task: Implement the `IoCModule` for Autofac registration.
- [ ] Task: Verify the plugin is correctly discovered by the PACX core.
- [ ] Task: Run automated /conductor:review

## Phase 2: Flow Run Commands
- [ ] Task: Implement `workflow run list` command.
- [ ] Task: Implement `workflow run get` command with action output inspection.
- [ ] Task: Implement `workflow run resubmit` and `workflow run cancel` commands.
- [ ] Task: Run automated /conductor:review

## Phase 3: Flow Definition & State Commands
- [ ] Task: Implement `workflow get` command.
- [ ] Task: Implement `workflow set-state` command.
- [ ] Task: Run automated /conductor:review

## Phase 4: Connection Commands
- [ ] Task: Implement `connection list` command.
- [ ] Task: Run automated /conductor:review

## Phase 5: Verification and PR
- [ ] Task: Verify all new commands work as expected.
- [ ] Task: Open a PR against issue #166.
- [ ] Task: Run automated /conductor:review
