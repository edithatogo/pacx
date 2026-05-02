# Implementation Plan: PCF Tooling

## Overview
PCF test, publish, and dependency-check commands are stubbed — they print documentation text instead of performing real operations.

## Scope
- PCF test runner using the official PCF test harness
- PCF publish via pac CLI solution packaging/import
- PCF dependency checking against target environment

## Success Criteria
- `pacx pcf test` launches PCF test harness in specified browser mode
- `pacx pcf publish` packages component and imports via pac CLI
- `pacx pcf dependency-check` verifies prerequisites in target Dataverse environment
- All commands provide clear output and error messages

## Phases

### Phase 1: PCF Test
- [x] Task: Research PCF test harness CLI arguments (browser, reporter, output)
- [x] Task: De-fake `PcfTestCommandExecutor` — launch `pcf test` via Process
- [x] Task: Support browser selection, reporter, project path options
- [x] Task: Tests with mock Process invocation

### Phase 2: PCF Publish
- [x] Task: De-fake `PcfPublishCommandExecutor` — invoke `pac solution` commands via Process
- [x] Task: Build solution from PCF project, import to Dataverse
- [x] Task: Support dry-run, solution targeting, version handling
- [x] Task: Tests

### Phase 3: PCF Dependency Check
- [x] Task: De-fake `PcfDependencyCheckCommandExecutor` — query Dataverse for PCF prerequisites
- [x] Task: Check Component Framework enabled, required web resources, Dataverse version
- [x] Task: Tests

### Phase 4: Documentation
- [x] Task: Update Known Stubs in tracks.md
