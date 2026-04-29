# Implementation Plan: Release & Supply Chain Hardening

## Overview
Harden the release path so binaries and packages are signed, traceable, and promoted through a documented workflow with provenance and SBOMs attached.

## Scope
- Signed releases for binaries and packages.
- Provenance generation and verification.
- SBOM generation and publication.
- A documented promotion path from `master` to release artifacts.
- Release gating for signature and provenance checks.

## Improvements
- Reduce artifact tampering risk.
- Make releases easier to audit.
- Give consumers a clear trust chain for published outputs.
- Lower the chance that release packaging diverges from the code that was tested.

## Success Criteria
- Release artifacts are signed.
- Provenance is generated and published alongside artifacts.
- SBOM output is produced for release builds.
- The release path is documented and repeatable.
- Release verification checks fail fast when signatures or provenance are missing.

## Phases

### Phase 1: Artifact trust chain
- [ ] Task: Define signing requirements for packages and binaries.
- [x] Task: Generate provenance metadata for release outputs.
- [x] Task: Emit SBOM artifacts alongside builds.
- [ ] Task: Add verification tests for signature and provenance presence.
- [x] Task: Add provenance and SBOM consistency checks for staged release folders.
- [x] Task: Add staged release verification command and CI gate for release folders.

### Phase 2: Promotion workflow
- [x] Task: Document the promotion path from `master` to release artifacts.
- [ ] Task: Add release gates so unsigned or unprovenanced artifacts cannot publish.
- [ ] Task: Add release verification checks in CI.
- [x] Task: Add operational docs for maintainers.

### Phase 3: Track closure
- [ ] Task: Split any remaining release-specific work into narrower child tracks only if needed.
