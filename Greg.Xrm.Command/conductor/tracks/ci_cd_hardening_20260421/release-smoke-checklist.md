# Release Smoke Checklist

Use `.github/workflows/release-smoke.yml` with the default `v0.0.0-ci-smoke` tag to verify the release path without publishing.

Expected results:

- The reusable `release.yml` workflow runs in dry-run mode.
- Coverage is collected and summarized.
- The CLI and interfaces packages are built.
- PACX package release artifacts are staged.
- NuGet publishing is skipped.
- GitHub Release creation is skipped.
- The workflow prints a release preview with the selected tag and version.

If all of the above happen, the fork-owned release pipeline is wired correctly for a real tagged release.
