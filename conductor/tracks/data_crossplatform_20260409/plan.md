# Implementation Plan: Data & Cross-Platform Engine

## Phase 1: Pure .NET 6+ Data Engine
- [ ] Task: Research CMT data format (ZIP structure, import/export XML schema).
- [ ] Task: Implement pure .NET 6+ data export using ServiceClient (no WPF).
- [ ] Task: Implement pure .NET 6+ data import using ServiceClient.
- [ ] Task: Verify cross-platform compatibility (Windows, Mac, Linux).
- [ ] Task: Write unit tests with mock data and mocked ServiceClient.
- [ ] Task: Run automated /conductor:review

## Phase 2: Schema Generation
- [ ] Task: Implement `data init-schema-from-solution` — generate schema from existing solution.
- [ ] Task: Include tables, columns, relationships, choice values, and constraints.
- [ ] Task: Output format: YAML (human-readable) + JSON (machine-readable).
- [ ] Task: Write unit tests.
- [ ] Task: Run automated /conductor:review

## Phase 3: Mock Data Generation
- [ ] Task: Implement `data seed-mock` — generate mock data from schema definition.
- [ ] Task: Support configurable record count, field value strategies (random, sequential, fixed).
- [ ] Task: Respect field constraints: required fields, choice values, lookup references.
- [ ] Task: Output: ZIP file compatible with the data import engine.
- [ ] Task: Write unit tests.
- [ ] Task: Run automated /conductor:review

## Phase 4: Integration & Verification
- [ ] Task: End-to-end test: export → schema generate → mock seed → import (round-trip).
- [ ] Task: Cross-platform test on Mac and Linux (or WSL).
- [ ] Task: Document all commands with usage examples.
- [ ] Task: Verify code coverage >80%.
- [ ] Task: Open PR against upstream repo.
- [ ] Task: Run automated /conductor:review
