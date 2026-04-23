# PACX Package Template

This folder is a minimal starter for a PACX-native package.

The starter manifest uses `kind: bundle`. Use `pacx package init --kind solution` or `pacx package init --kind data` if you want a stricter package contract from day one. Those starter kinds now scaffold valid package contents and deployment steps immediately, and `add`, `remove`, `sync`, and `fix` will keep the kind aligned with the package contents.

## Structure

```text
pacx.package.json
payload/
data/
scripts/
```

Use `pacx package validate --path <folder>` to verify the package, then `pacx package build --path <folder>` to emit a `.pacx` archive.
For a clean starter with immediate validation success, run `pacx package init --kind solution` or `pacx package init --kind data`.

`pacx package init --path <folder>` creates the same starter layout from the CLI.
Use `pacx package add solution` and `pacx package add data` to populate the manifest and copy artifacts into the folder.
Use `pacx package remove solution` and `pacx package remove data` to remove payloads cleanly.
Use `pacx package list` to inspect manifest readiness and deployment steps.
Use `pacx package add`, `pacx package remove`, `pacx package sync`, and `pacx package fix` to keep the manifest and package kind aligned with the files present. `add` and `remove` are the direct authoring commands, while `sync` and `fix` reconcile the folder after manual edits.
Use `pacx package publish` to generate a release artifact bundle.
Use `pacx package release` to stage a release folder with notes and checksums.

Recommended flow:

1. Run `pacx package init`.
2. Add solution and data artifacts with `pacx package add`.
3. Validate with `pacx package validate`.
4. Build with `pacx package build`.
