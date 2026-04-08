# Implementation Plan: Unit tests for core Solution commands

## Phase 1: Preparation
- [ ] Task: Review existing tests in `Greg.Xrm.Command.Core.TestSuite/Commands/Solution` for `GetPublisherListCommand`.
- [ ] Task: Set up shared test utilities or base classes for mocking `IOrganizationService`.

## Phase 2: Implementation - ListCommand
- [ ] Task: Create `ListCommandExecutorTest.cs` in `Greg.Xrm.Command.Core.TestSuite/Commands/Solution`.
- [ ] Task: Implement test for `ListCommandExecutor.ExecuteAsync` filtering logic.
- [ ] Task: Implement test for `ListCommandExecutor.ExecuteAsync` sorting logic.
- [ ] Task: Implement test for `ListCommandExecutor.ExecuteAsync` output formatting (Table vs TableCompact vs JSON).

## Phase 3: Implementation - CreateCommand
- [ ] Task: Create `CreateCommandExecutorTest.cs` in `Greg.Xrm.Command.Core.TestSuite/Commands/Solution`.
- [ ] Task: Implement test for `CreateCommandExecutor.ExecuteAsync` parameter mapping to `Entity`.
- [ ] Task: Implement test for `CreateCommandExecutor.ExecuteAsync` success and failure scenarios.

## Phase 4: Implementation - DeleteCommand
- [ ] Task: Create `DeleteCommandExecutorTest.cs` in `Greg.Xrm.Command.Core.TestSuite/Commands/Solution`.
- [ ] Task: Implement test for `DeleteCommandExecutor.ExecuteAsync` solution identification and deletion.

## Phase 5: Verification
- [ ] Task: Run all new tests and verify they pass.
- [ ] Task: Verify overall code coverage for the `Solution` command executors.
- [ ] Task: Conductor - User Manual Verification 'Phase 5: Verification' (Protocol in workflow.md)
