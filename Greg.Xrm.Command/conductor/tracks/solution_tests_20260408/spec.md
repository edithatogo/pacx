# Specification: Unit tests for core Solution commands

## Overview
This track aims to provide robust unit test coverage for the following commands in the `Greg.Xrm.Command.Commands.Solution` namespace:
- `ListCommand`
- `CreateCommand`
- `DeleteCommand`

## Scope
- **Parser Tests:** Verify that CLI arguments map correctly to the properties of the command definitions using `Utility.TestParseCommand<T>`.
- **Executor Tests:**
  - Verify filtering logic in `ListCommandExecutor` (Managed vs Unmanaged vs Both).
  - Verify sorting logic in `ListCommandExecutor` (Name, CreatedOn, ModifiedOn, Type).
  - Verify command parameter mapping in `CreateCommandExecutor`.
  - Verify error handling (e.g., `FaultException<OrganizationServiceFault>`) and Dataverse interaction (using mocks for `IOrganizationService`).
- Implement tests in `Greg.Xrm.Command.Core.TestSuite/Commands/Solution`.

## Constraints
- Use MSTest and Moq (the project's existing testing frameworks).
- Adhere to the project's existing coding style for tests.
- Target >80% code coverage for the logic within the executors.
