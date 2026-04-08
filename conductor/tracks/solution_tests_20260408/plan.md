# Implementation Plan: Unit tests for core Solution commands

## Phase 1: Preparation [checkpoint: edc07fd]
- [x] Task: Review existing tests in `Greg.Xrm.Command.Core.TestSuite/Commands/Solution` for `GetPublisherListCommand` as a template for MSTest/Moq structure. [05994e8]
- [x] Task: Set up shared test utilities or base classes for mocking `IOrganizationServiceRepository` and `IOutput`. [e4ea582]

## Phase 2: Implementation - ListCommand [checkpoint: 958bc29]
- [x] Task: Create `ListCommandTest.cs` to test the parser via `Utility.TestParseCommand<ListCommand>`. [7a8dfd9]
- [x] Task: Create `ListCommandExecutorTest.cs`. [24ecfd8]
- [x] Task: Implement test for `ListCommandExecutor.ExecuteAsync` filtering logic. [24ecfd8]
- [x] Task: Implement test for `ListCommandExecutor.ExecuteAsync` sorting logic. [24ecfd8]
- [x] Task: Implement test for `ListCommandExecutor.ExecuteAsync` output formatting (Table vs TableCompact vs JSON) and error handling. [24ecfd8]

## Phase 3: Implementation - CreateCommand [checkpoint: 63e344e]
- [x] Task: Create `CreateCommandTest.cs` to test the parser via `Utility.TestParseCommand<CreateCommand>`. [3cd347b]
- [x] Task: Create `CreateCommandExecutorTest.cs`. [3cd347b]
- [x] Task: Implement test for `CreateCommandExecutor.ExecuteAsync` parameter mapping to `Entity`. [3cd347b]
- [x] Task: Implement test for `CreateCommandExecutor.ExecuteAsync` success and failure scenarios. [3cd347b]

## Phase 4: Implementation - DeleteCommand [checkpoint: d0aa8f7]
- [x] Task: Create `DeleteCommandTest.cs` to test the parser via `Utility.TestParseCommand<DeleteCommand>`. [19a1273]
- [x] Task: Create `DeleteCommandExecutorTest.cs`. [19a1273]
- [x] Task: Implement test for `DeleteCommandExecutor.ExecuteAsync` solution identification and deletion logic. [19a1273]

## Phase 5: Verification
- [~] Task: Run all new tests via `dotnet test` and verify they pass.
- [~] Task: Verify overall code coverage for the `Solution` command executors.
- [~] Task: Conductor - User Manual Verification 'Phase 5: Verification' (Protocol in workflow.md)
