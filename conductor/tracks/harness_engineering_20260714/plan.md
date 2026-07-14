# Implementation plan

- [x] Pin all GitHub Actions references to immutable commit SHAs.
- [x] Add repository harness checks for action pins, central package management, secret-pattern hygiene, and upstream manifest integrity.
- [x] Add scheduled upstream comparison with explicit structural-divergence detection.
- [x] Add build, locked restore, test, coverage, format, and vulnerable-package gates.
- [x] Add scheduled Zizmor SARIF evidence collection.
- [x] Remediate high-risk trust boundaries in auto-merge, welcome, branch protection, reusable build, rollback, stale maintenance, and Scorecard (commit `058165b`).
- [~] Remediate residual Zizmor findings in benchmark, release, dependency, and legacy integration surfaces.
- [ ] Port bearer-auth changes onto an upstream-related baseline after an explicit architecture decision.
- [ ] Conductor - User Manual Verification 'PACX maximal harness engineering' (Protocol in workflow.md)

Checkpoint: local harness passed; upstream comparison reports structural divergence with no common ancestor.
