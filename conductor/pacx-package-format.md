# PACX Package Format

PACX packages are cross-platform deployment bundles. The container is either:

- a directory that contains `pacx.package.json` at its root, or
- a `.pacx` file, which is a standard ZIP archive with the same layout.

## Package kinds

`kind` is required and must be one of:

- `bundle`: a mixed package or release bundle. This is the default kind for starter packages and repository-owned release bundles.
- `solution`: a solution-focused package. It must include at least one artifact with role `solution` and at least one deployment step of type `solutionImport`.
- `data`: a data-focused package. It must include at least one artifact with role `data` and at least one deployment step of type `dataImport`.

PACX can build a `.pacx` archive from a package directory with `pacx package build`.
PACX can also validate a folder or archive with `pacx package validate`.
PACX can scaffold a starter package folder with `pacx package init`.
PACX can set the package kind at scaffold time with `pacx package init --kind`.
PACX can add solution and data payloads to a package folder with `pacx package add solution` and `pacx package add data`.
PACX can remove solution and data payloads with `pacx package remove solution` and `pacx package remove data`.
PACX can list package readiness and manifest contents with `pacx package list`.
PACX can synchronize package metadata with folder contents using `pacx package sync`.
`pacx package add`, `pacx package remove`, `pacx package sync`, and `pacx package fix` all normalize the manifest kind from the package contents, promoting the package to `solution`, `data`, or `bundle` as needed.
PACX can fix duplicates, stale entries, and ordering using `pacx package fix`.
PACX can publish a release archive plus release manifest using `pacx package publish`.
PACX can override the published version with `--version` when a release tag should control the output name and manifest.
PACX can stage a release folder with notes and checksums using `pacx package release`.
PACX can also override the staged release version with `--version`.
PACX can deploy a package with `pacx package deploy --dry-run` to validate the deployment plan and show step readiness before applying changes.

## Required layout

```text
pacx.package.json
payload/
data/
scripts/
```

Only `pacx.package.json` is required. Other files and folders are package-specific.

## Manifest schema

Minimum required manifest fields:

- `schemaVersion`: integer, currently `1`
- `packageId`: stable package identifier
- `version`: package version string
- `name`: display name
- `kind`: package category, one of `bundle`, `solution`, or `data`
- `artifacts`: list of files included in the package
- `deployment`: ordered list of deployment steps

### Artifact entry

Each artifact entry has:

- `path`: package-relative path
- `role`: descriptive role such as `solution` or `data`
- `required`: whether the artifact must exist
- `contentType`: optional MIME type
- `sha256`: optional integrity hash

### Deployment step

Each deployment step has:

- `type`: `solutionImport` or `dataImport`
- `artifact`: package-relative path to the payload
- `table`: required for data imports when the table cannot be inferred from the file name
- `mode`: optional import mode, typically `upsert` or `create`
- `overwriteUnmanagedCustomizations`: optional solution import flag
- `publishWorkflows`: optional solution import flag

## Execution semantics

- `solutionImport` reads the referenced ZIP payload and passes it to Dataverse solution import.
- `dataImport` reads a JSON array of records and writes them to Dataverse using the current connection.
- Unknown step types are rejected.

## Compatibility rule

If a package can be represented as a single `.pacx` file, it can also be unpacked into a directory with the same manifest and payload paths. Tooling should treat both forms identically.
