# Implementation plan

- [x] Pin all GitHub Actions references to immutable commit SHAs.
- [x] Add repository harness checks for action pins, central package management, secret-pattern hygiene, and upstream manifest integrity.
- [x] Add scheduled upstream comparison with explicit structural-divergence detection.
- [x] Add build, locked restore, test, coverage, format, and vulnerable-package gates.
- [x] Add scheduled Zizmor SARIF evidence collection.
- [x] Remediate high-risk trust boundaries in auto-merge, welcome, branch protection, reusable build, rollback, stale maintenance, and Scorecard (commit `058165b`).
- [x] Remediate residual high-severity Zizmor findings in benchmark, release, dependency, documentation, container, and integration workflows (commit `e03f597`).
- [x] Decide and document the upstream integration boundary: retain PACX as the bearer-auth baseline and use upstream for evidence-only comparison while histories remain structurally divergent (ADR 0006).
- [ ] Port an individually selected upstream capability through a reviewed, tested commit after a bounded capability comparison.
- [ ] Conductor - User Manual Verification 'PACX maximal harness engineering' (Protocol in workflow.md)

Checkpoint: local harness passed; upstream comparison reports structural divergence with no common ancestor.
