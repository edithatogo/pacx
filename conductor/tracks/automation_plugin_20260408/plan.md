# Implementation Plan: Extend Automation Capabilities (as Plugin)

## Phase 1: Plugin Scaffolding
- [x] Task: Create the `Greg.Xrm.Command.Plugin.Automation` project. *Already existed*
- [x] Task: Implement the `IoCModule` for Autofac registration. *Already existed*
- [x] Task: Verify the plugin is correctly discovered by the PACX core. *Already working*

## Phase 2: Flow Run Commands
- [x] Task: Implement `workflow run list` command. *WorkflowRunListCommand + Executor (existed)*
- [x] Task: Implement `workflow run get` command with action output inspection. *WorkflowRunGetCommand + Executor (NEW)*
- [x] Task: Implement `workflow run resubmit` and `workflow run cancel` commands. *WorkflowRunResubmitCommand + Executor (NEW), WorkflowRunCancelCommand + Executor (NEW)*

## Phase 3: Flow Definition & State Commands
- [x] Task: Implement `workflow get` command. *WorkflowGetCommand + Executor (existed)*
- [x] Task: Implement `workflow set-state` command. *WorkflowSetStateCommand + Executor (NEW)*

## Phase 4: Connection Commands
- [x] Task: Implement `connection list` command. *ConnectionListCommand + Executor (existed)*

## Phase 5: Verification and PR
- [x] Task: Verify all new commands work as expected. *WorkflowAutomationCommandsTest.cs (NEW)*
- [ ] Task: Open a PR against issue #166.
- [ ] Task: Run automated /conductor:review
