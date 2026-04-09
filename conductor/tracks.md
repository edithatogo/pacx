# Project Tracks

This file tracks all major tracks for the project. Each track has its own detailed plan in its respective folder.

---

## Active Track
- [x] **Track: Implement unit tests for core Solution commands**
  *Link: [./tracks/solution_tests_20260408/](./tracks/solution_tests_20260408/)*

## Future Tracks (Backlog)
- [x] **Track: Establish Code Quality & Coverage Infrastructure**
  *Link: [./tracks/code_quality_20260408/](./tracks/code_quality_20260408/)*
  *Description: Introduce an `.editorconfig` for consistent formatting and configure `coverlet.collector` in the test suite to automatically measure and report code coverage.*

- [x] **Track: Update CI/CD Pipeline to Run Tests**
  *Link: [./tracks/cicd_tests_20260408/](./tracks/cicd_tests_20260408/)*
  *Description: Modify `.github/workflows/build-pipeline-01.yml` to execute `dotnet test` (with code coverage) so that tests are run automatically on PRs and commits.*

- [x] **Track: Incorporate PR #146**
  *Link: [./tracks/pr146_integration_20260408/](./tracks/pr146_integration_20260408/)*
  *Description: Review and integrate the pull request from https://github.com/neronotte/Greg.Xrm.Command/pull/146.*

- [x] **Track: Explore and Incorporate Branches**
  *Link: [./tracks/explore_branches_20260408/](./tracks/explore_branches_20260408/)*
  *Description: Review existing branches in the repository to evaluate their features/fixes and incorporate them.*

- [x] **Track: Review and Resolve GitHub Issues**
  *Link: [./tracks/resolve_issues_20260408/](./tracks/resolve_issues_20260408/)*
  *Description: Go through the open issues at https://github.com/neronotte/Greg.Xrm.Command/issues one by one and implement fixes or enhancements.*

- [x] **Track: Implement an MCP Server**
  *Link: [./tracks/mcp_server_20260408/](./tracks/mcp_server_20260408/)*
  *Description: Add support for an MCP (Model Context Protocol) server to expose the CLI's capabilities to AI agents. (Requires creating a GitHub issue first, then submitting a PR against it).*

- [x] **Track: Extend Automation Capabilities (as Plugin)**
  *Link: [./tracks/automation_plugin_20260408/](./tracks/automation_plugin_20260408/)*
  *Description: Expand tool functions to cover advanced platform automation, implementing them as a first-class PACX Plugin utilizing `McMaster.NETCore.Plugins` and Autofac `Module` registrations. (Requires creating a GitHub issue first, then submitting a PR against it).*
