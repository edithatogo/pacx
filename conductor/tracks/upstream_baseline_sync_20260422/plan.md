# Implementation Plan: Upstream Baseline Sync

## Overview
Bring the local PACX baseline up to date with upstream work that is already ahead of `master`, then point the rest of the planned pipeline at that synced baseline.

## Scope
- Evaluate upstream branches that are ahead of `master`.
- Integrate the high-value ahead branches into the local baseline.
- Resolve any conflicts introduced by the integration.
- Verify the baseline before downstream track work continues.

## Integration Notes
- Current confirmed ahead branches:
  - `copilot/add-change-language-command`
  - `capacityProfileInfo`
- Prefer cherry-pick or selective merge when a branch is heavily diverged.
- Record any branch that is no longer suitable for merge as a tracked exception in the plan.

## Success Criteria
- A concrete merge strategy exists for every ahead branch currently selected for integration.
- Local baseline contains the chosen upstream work.
- Verification passes after integration.
- Downstream planning tracks are explicitly gated on this sync.

---

## Phases

### Phase 1: Branch inventory and merge strategy
- [x] Task: Reconfirm the upstream branches that are ahead of `master` and capture their current divergence. Result: `copilot/add-change-language-command` is 11 commits ahead / 41 behind `upstream/master`; `capacityProfileInfo` is 1 commit ahead / 304 behind `upstream/master`.
- [x] Task: Classify each ahead branch as merge, cherry-pick, or defer. Result: cherry-pick `copilot/add-change-language-command` as a focused usersettings series; defer `capacityProfileInfo` as stale/superseded relative to the current baseline.
- [x] Task: Document the integration order for the selected branches. Result: integrate the copilot usersettings branch first, then revisit any remaining capacity-profile work only if it still applies cleanly after the baseline sync.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 2: Integrate the primary ahead branch
- [x] Task: Integrate `copilot/add-change-language-command` into the local baseline. Result: already present on `master`; `git branch --contains 2d5d148` shows the branch head is an ancestor of the local baseline.
- [x] Task: Run the relevant command and test coverage for the changed user-settings flow. Result: `Greg.Xrm.Command.Core.TestSuite` passed in the clean scratch clone with .NET 10.0.100.
- [x] Task: Capture any follow-up fixes required by the integration. Result: none required; the local baseline already contains the usersettings series.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 3: Integrate the secondary ahead branch
- [x] Task: Evaluate `capacityProfileInfo` and integrate it if it still applies cleanly to the current baseline. Result: already present on `master`; `git branch --contains 25aa0db` shows the branch head is an ancestor of the local baseline.
- [x] Task: If the branch is stale or superseded, record the reason and the replacement plan. Result: stale/superseded by the current baseline; no additional replacement work required beyond keeping it as a tracked exception.
- [x] Task: Run the affected tests or verification steps after the decision. Result: same `Greg.Xrm.Command.Core.TestSuite` verification pass applies to the synced baseline.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 4: Pipeline sync
- [x] Task: Update `conductor/tracks.md` so this track gates the downstream planning work.
- [x] Task: Confirm the downstream track ordering reflects the synced baseline.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

### Phase 5: Fork-only closure
- [x] Task: Record that no upstream issue or PR will be opened for the baseline sync work. Result: this fork is maintained independently; the local baseline sync is complete and no upstream PR lifecycle will be used.
- [x] Task: Mark the track complete locally and keep downstream planning gated on the synced baseline. Result: the downstream planning tracks already point at the synced baseline.
