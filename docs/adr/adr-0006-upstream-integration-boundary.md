# ADR 0006: Upstream Integration Boundary

## Status

Accepted — 2026-07-15

## Context

PACX currently has no common ancestor with `neronotte/Greg.Xrm.Command`. The PACX tree contains the bearer-safe authentication baseline and the repository-native harness, while the upstream tree is a separate structural history. A merge or rebase would therefore combine unrelated project roots and create a high-risk, low-auditability change.

## Decision

PACX remains the implementation baseline. The upstream repository is maintained as a reference-only comparison source until a future explicit porting effort identifies a bounded capability and tests it independently.

The repository will:

- retain the `upstream` remote and weekly comparison workflow;
- record upstream head, local head, and common-ancestor status as evidence;
- never force-merge, force-rebase, or silently replace PACX files from upstream;
- port individual upstream capabilities only through reviewed, tested commits;
- preserve bearer-auth behavior as a PACX compatibility requirement.

## Consequences

This avoids accidental loss of PACX security and harness work, but upstream updates require manual capability-level comparison. The current bearer-auth work is considered integrated into the PACX baseline; it is not rebased onto an unrelated upstream root.

## Verification

Run `scripts/Test-UpstreamSync.ps1` and require `commonAncestor` to be non-null before considering a merge or rebase. For the current repositories, the expected result is `structurally-divergent` with no common ancestor.
