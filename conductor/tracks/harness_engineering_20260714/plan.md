# Implementation plan

- [x] Pin all GitHub Actions references to immutable commit SHAs.
- [x] Add repository harness checks for action pins, central package management, secret-pattern hygiene, and upstream manifest integrity.
- [x] Add scheduled upstream comparison with explicit structural-divergence detection.
- [x] Add build, locked restore, test, coverage, format, and vulnerable-package gates.
- [x] Add scheduled Zizmor SARIF evidence collection.
- [x] Remediate high-risk trust boundaries in auto-merge, welcome, branch protection, reusable build, rollback, stale maintenance, and Scorecard (commit `058165b`).
- [x] Remediate residual high-severity Zizmor findings in benchmark, release, dependency, documentation, container, and integration workflows (commit `e03f597`).
- [x] Decide and document the upstream integration boundary: retain PACX as the bearer-auth baseline and use upstream for evidence-only comparison while histories remain structurally divergent (ADR 0006).
- [x] Port an individually selected upstream capability through a bounded comparison: read-only `custom-api describe` (commit `531e1da`) with tests and documentation.
- [x] Add the Conductor verification protocol and manual evidence checklist (`workflow.md`, `docs/verification/pacx-harness-manual-verification.md`).
- [x] Add normalized upstream capability inventory and CI artifact reporting (`scripts/Compare-UpstreamCapabilities.ps1`).
- [x] Port a bounded Custom API capability independently: add read-only `custom-api describe` with request/response metadata tests and documentation.
- [x] Validate the bounded capability on both target frameworks: 2/2 focused tests passed on .NET 10 and .NET 11.
- [x] Reconcile pre-existing full-suite failures in source and fixtures: architecture-test assembly resolution, SBOM archive/manifest hashes, release-plan cache isolation, and command-reference parity fixture.
- [ ] Conductor - User Manual Verification 'PACX maximal harness engineering' (record run URLs and outcomes).

Checkpoint: local harness passed; upstream comparison reports structural divergence with no common ancestor.
