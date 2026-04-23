# PACX Release Bundle

This folder is the repository-owned PACX package source that CI uses when it stages a release bundle.

It is intentionally small and deterministic:

- `pacx.package.json` defines the bundle
- `data/release.json` carries release metadata for the package archive

The bundle uses `kind: bundle` because it is a release artifact container rather than a solution-only or data-only deployment package.

The bundle is published by `pacx package release` during tag-driven CI runs.
