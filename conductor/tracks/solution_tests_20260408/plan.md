# Implementation Plan: Unit tests for core Solution commands

## Phase 1: Preparation
- [x] Task: Review existing tests in `Greg.Xrm.Command.Core.TestSuite/Commands/Solution` for `GetPublisherListCommand` as a template for MSTest/Moq structure. [05994e8]
- [~] Task: Set up shared test utilities or base classes for mocking `IOrganizationServiceRepository` and `IOutput`.

## Phase 2: Implementation - ListCommand
- [ ] Task: Create `ListCommandTest.cs` to test the parser via `Utility.TestParseCommand<ListCommand>`.
- [ ] Task: Create `ListCommandExecutorTest.cs`.
- [ ] Task: Implement test for `ListCommandExecutor.ExecuteAsync` filtering logic.
- [ ] Task: Implement test for `ListCommandExecutor.ExecuteAsync` sorting logic.
- [ ] Task: Implement test for `ListCommandExecutor.ExecuteAsync` output formatting (Table vs TableCompact vs JSON) and error handling.

## Phase 3: Implementation - CreateCommand
- [ ] Task: Create `CreateCommandTest.cs` to test the parser via `Utility.TestParseCommand<CreateCommand>`.
- [ ] Task: Create `CreateCommandExecutorTest.cs`.
- [ ] Task: Implement test for `CreateCommandExecutor.ExecuteAsync` parameter mapping to `Entity`.
- [ ] Task: Implement test for `CreateCommandExecutor.ExecuteAsync` success and failure scenarios.

## Phase 4: Implementation - DeleteCommand
- [ ] Task: Create `DeleteCommandTest.cs` to test the parser via `Utility.TestParseCommand<DeleteCommand>`.
- [ ] Task: Create `DeleteCommandExecutorTest.cs`.
- [ ] Task: Implement test for `DeleteCommandExecutor.ExecuteAsync` solution identification and deletion logic.

## Phase 5: Verification
- [ ] Task: Run all new tests via `dotnet test` and verify they pass.
- [ ] Task: Verify overall code coverage for the `Solution` command executors.
- [ ] Task: Conductor - User Manual Verification 'Phase 5: Verification' (Protocol in workflow.md)
