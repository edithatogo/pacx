# Implementation Plan: Microsoft Release Plan Intelligence

## Overview
Add PACX-native roadmap intelligence for Microsoft release-plan families so the CLI can browse, snapshot, compare, and export roadmap deltas for Power Platform capabilities.

## Scope
- Release-plan browsing for the supported Power Platform families.
- Snapshot capture and comparison across dates.
- Export of deltas into markdown and JSON for docs and planning use.
- Track new or changed roadmap items for downstream PACX prioritization.

## Improvements
- Makes Microsoft roadmap changes visible inside PACX.
- Reduces manual checking across multiple product release-plan pages.
- Creates an evidence trail for roadmap-driven implementation decisions.

## Success Criteria
- Users can browse release-plan families from the CLI.
- Snapshot diffs show what changed across release-plan runs.
- Exported output is usable in docs or conductor planning.
- The roadmap track stays aligned with the live Microsoft release-plan families.

## Phases

### Phase 1: Browse and catalog
- [x] Task: Define the supported release-plan families and their source URLs.
- [x] Task: Add a browsing command surface for roadmap intake.
- [x] Task: Add tests for parsing and listing the source families.

### Phase 2: Snapshot and diff
- [ ] Task: Add snapshot storage for release-plan captures.
- [ ] Task: Add diffing between snapshots and product families.
- [ ] Task: Add regressions for changed, added, and removed roadmap items.

### Phase 3: Export and planning integration
- [ ] Task: Export deltas to markdown and JSON.
- [ ] Task: Wire the output into docs and conductor planning workflows.
- [ ] Task: Add examples for CI and local review.
