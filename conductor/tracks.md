# Project Tracks

This file tracks all major tracks for the project. Each track has its own detailed plan in its respective folder.

---

## Active Track
- [ ] **Track: Implement unit tests for core Solution commands**
  *Link: [./tracks/solution_tests_20260408/](./tracks/solution_tests_20260408/)*

## Future Tracks (Backlog)
- [ ] **Track: Establish Code Quality & Coverage Infrastructure**
  *Description: Introduce an `.editorconfig` for consistent formatting and configure `coverlet.collector` in the test suite to automatically measure and report code coverage.*

- [ ] **Track: Update CI/CD Pipeline to Run Tests**
  *Description: Modify `.github/workflows/build-pipeline-01.yml` to execute `dotnet test` (with code coverage) so that tests are run automatically on PRs and commits.*

- [ ] **Track: Incorporate PR #146**
  *Description: Review and integrate the pull request from https://github.com/neronotte/Greg.Xrm.Command/pull/146.*

- [ ] **Track: Explore and Incorporate Branches**
  *Description: Review existing branches in the repository to evaluate their features/fixes and incorporate them.*

- [ ] **Track: Review and Resolve GitHub Issues**
  *Description: Go through the open issues at https://github.com/neronotte/Greg.Xrm.Command/issues one by one and implement fixes or enhancements.*

- [ ] **Track: Implement an MCP Server**
  *Description: Add support for an MCP (Model Context Protocol) server to expose the CLI's capabilities to AI agents. Must be .NET 8 compatible and integrate with the existing hybrid Autofac/Microsoft DI setup.*

- [ ] **Track: Extend Automation Capabilities (as Plugin)**
  *Description: Expand tool functions to cover advanced platform automation, implementing them as a first-class PACX Plugin utilizing `McMaster.NETCore.Plugins` and Autofac `Module` registrations.*
