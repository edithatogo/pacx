# Specification: Upstream Baseline Sync

## Overview
Integrate upstream work from `neronotte/Greg.Xrm.Command` into the local PACX baseline so downstream planning tracks inherit the latest merged command and model changes.

## Scope
- Review upstream branches that are ahead of `master`.
- Determine which changes should be merged directly and which should be cherry-picked.
- Integrate the selected upstream changes into the local baseline with minimal conflict risk.
- Verify the resulting baseline before downstream work proceeds.

## Known Current Candidates
- `copilot/add-change-language-command`
- `capacityProfileInfo`

## Constraints
- Preserve the local dirty worktree; do not discard unrelated user changes.
- Prefer branch-by-branch integration rather than broad blind merges when branches are diverged.
- Treat verification failures as blockers before progressing to downstream tracks.

## Success Criteria
- The ahead upstream branches have been evaluated and a merge strategy is recorded.
- Applicable upstream changes are integrated into the local baseline.
- Verification is run after each integration step.
- `conductor/tracks.md` reflects that this track gates the downstream planning set.

## Pipeline Position
- This track should be completed before the 2026-04-27 planning tracks begin.
