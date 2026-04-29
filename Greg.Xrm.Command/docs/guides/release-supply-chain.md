# Release Supply Chain

PACX release artifacts are staged from the package workflow and emitted as a directory containing:

- the `.pacx` archive
- `pacx.release.json`
- `provenance.json`
- `sbom.json`
- `RELEASE_NOTES.md`
- `checksums.txt`

The release workflow runs `pacx package release` to stage the folder and then `pacx package release verify` to fail fast when required artifacts are missing, hashes do not line up, or the provenance/SBOM content drifts from the staged files.

## Promotion Path

1. Merge the release-ready code to `master`.
2. Run the package release workflow against the package folder or `.pacx` archive.
3. Verify the staged release folder.
4. Publish the release artifacts from the verified folder.

## Release Metadata

- `pacx.release.json` records the package identity, archive hash, and packaging metadata.
- `provenance.json` records repository, commit, workflow, and hash information for traceability.
- `sbom.json` lists the release contents and the package manifest artifacts, including component hashes.
- `checksums.txt` provides the local hash set used by the verifier.

## Notes

The current implementation is local-manifest and workflow driven. The repo does not yet emit cryptographic signatures for the package-release staging flow, but the staged release folder is now explicit about provenance, SBOM, and hash verification.
