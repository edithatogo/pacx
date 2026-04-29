# PACX Distribution Plan

## Goal

Ship PACX as a fork-first tool with no dependency on upstream issue or PR workflows.

## Primary Channels

1. **Source distribution**
   - Keep the fork as the canonical development branch.
   - Contributors clone the fork, build locally, and run the test suite from source.
   - Upstream sync is handled by periodic `git fetch upstream` + selective rebase/merge when useful.
   - The fork's release workflow now owns tag-driven publishing and does not rely on upstream issues or pull requests.

2. **NuGet global tool**
   - Publish `Greg.Xrm.Command` as the primary install path for users.
   - Users install with `dotnet tool install -g Greg.Xrm.Command`.
   - Use semantic version tags such as `v1.2.3` and prerelease tags such as `v1.3.0-beta.1`.

3. **GitHub Releases**
   - Attach packed `.nupkg` files, release notes, and optional SBOM/provenance artifacts.
   - Use the staged PACX release workflow to upload `.pacx`, `pacx.release.json`, `RELEASE_NOTES.md`, and `checksums.txt` when a package release is ready.
   - Tag pushes on `v*` now fan out to the fork-owned `release.yml` NuGet publishing workflow plus the PACX package release workflow.
   - Use the dedicated `release-smoke.yml` workflow dispatch to verify the release path without publishing when smoke testing the fork.
   - Release notes are drafted from merged work using Release Drafter and attached to the published GitHub Release.
   - The release workflow emits a CycloneDX SBOM for the release solution and attaches it to the GitHub Release alongside the `.nupkg` artifacts.
   - The release workflow also keylessly signs the release assets with Sigstore and uploads the signing artifacts for auditability.
   - The release workflow also emits SLSA provenance for the release artifacts using the generic GitHub Actions generator and uploads the provenance with the release assets.
   - Treat the release as the user-facing artifact record for a given tag.

4. **Optional internal feed**
   - If a private team feed is needed, publish the same NuGet package there first.
   - Keep the release metadata and package contents identical across feeds.

5. **PACX-native package format**
   - Ship deployment bundles as `.pacx` containers with a root `pacx.package.json` manifest.
   - Treat `kind` as a first-class package contract: `bundle` for mixed/release packages, `solution` for solution-only packages, and `data` for data-only packages.
   - Keep the repository-owned release package source in `conductor/release-package` so CI has a concrete releaseable package root.
   - Stamp `conductor/release-package/data/release.json` from the workflow tag, commit, and run metadata before staging the release bundle.
   - CI stamps a temporary copy of the release bundle source so the committed files stay clean after release runs.
   - Use the package format for solution imports and data imports so deployment stays cross-platform.
   - Keep the package manifest source-controlled alongside the repo so package shape is explicit and reviewable.
   - Use `pacx package list` to inspect authoring readiness, missing artifacts, and deployment steps.
   - Use `pacx package add` and `pacx package remove` for direct authoring changes; both normalize the package kind from the files present.
   - Use `pacx package sync` to reconcile the manifest with `payload/`, `data/`, and `scripts/` contents and normalize the package kind from the files present.
   - Use `pacx package fix` to deduplicate, normalize, and kind-align a messy package before release.
   - Use `pacx package publish` to generate a release archive and machine-readable release manifest.
   - Use `pacx package publish --version` to stamp a release-tag version into the published archive and release manifest.
   - Use `pacx package release --version` to stage release folders for GitHub Releases or internal feeds.
   - Validate the package before build/deploy with `pacx package validate`.
   - Build the archive with `pacx package build` from a package folder, then publish or attach the resulting `.pacx`.

## Release Workflow

1. Merge to the fork's mainline branch.
2. Run CI, format checks, unit tests, and any required integration checks.
3. Tag the release commit with a semantic version tag.
4. Publish the CLI and interfaces projects through NuGet trusted publishing.
5. Create or update a GitHub Release with the packaged assets and Release Drafter notes.
6. Build or consume `.pacx` deployment bundles when a release needs an environment rollout artifact.

## Security and Supply Chain

1. Prefer GitHub Actions OIDC / trusted publishing for NuGet.org when available.
2. Publish signed or provenance-bearing artifacts when the release pipeline supports it.
3. Keep Dependabot security updates enabled, but avoid version-update noise where possible.
4. Run CodeQL and scorecard checks on the fork's CI branch.

## Fork Governance

1. Do not open upstream issues as part of normal delivery.
2. Do not create upstream PRs as part of the normal workflow.
3. Use the fork's own issues, discussions, and release notes for coordination.
4. Periodically sync upstream changes only to cherry-pick useful fixes or API drift updates.
