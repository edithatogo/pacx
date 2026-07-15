# PACX Harness Manual Verification

After `codex/harness-engineering-20260714` is pushed, verify that branch in GitHub. Until publication succeeds, this checklist remains pending and must not be represented as hosted evidence. It is intentionally read-only except for creating a disposable pull request or using workflow dispatch where stated.

## Evidence checklist

- [ ] Open the latest `Harness Engineering` workflow run and confirm `Repository controls` passes.
- [ ] Confirm the upstream evidence artifact reports the expected canonical repository and `structurally-divergent` status.
- [ ] Confirm the Zizmor artifact is present and contains no high-severity findings.
- [ ] Open the latest pull request checks and confirm build, test, format, coverage, and vulnerable-package gates are visible.
- [ ] Confirm Dependabot metadata and auto-merge workflows only run for Dependabot-authored dependency updates.
- [ ] Confirm the Scorecard workflow publishes SARIF only on trusted branch events, not pull requests.
- [ ] Confirm documentation deployment is limited to the configured GitHub Pages environment.
- [ ] Confirm release, rollback, tag-signing, NuGet, Docker, and provenance workflows require their declared environment or dispatch inputs before any write operation.
- [ ] Confirm no workflow run exposes secrets in logs or artifacts.

## Recording

Record the date, verifier, branch SHA, workflow run URLs, and any exceptions in the Conductor track checkpoint. A failed item should remain open and include the run URL and remediation issue.
