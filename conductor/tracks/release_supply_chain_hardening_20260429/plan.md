# Implementation Plan: Release Supply Chain Hardening

## Overview
Harden release supply chain — provenance, SBOM, signing, verification, and release gates.

## Scope
- SLSA provenance attestation for release artifacts.
- SBOM generation and verification.
- Authenticode/StrongName signing of assemblies.
- Release gate verification in CI/CD.
- Release verification commands.

## Dependencies
- `ci_cd_hardening_20260421` — CI/CD pipeline foundation needed for gate integration.

## Success Criteria
- Release artifacts carry SLSA provenance attestations.
- SBOM generated and published with each release.
- Assemblies strong-name signed.
- Release gates verify provenance, SBOM, and signatures before promotion.
- `pacx package verify` checks supply chain artifacts.

## Phases

### Phase 1: Provenance & SBOM (DONE)
- [x] Task: SLSA provenance generation for NuGet packages via GitHub Actions.
- [x] Task: SPDX SBOM generation during build.
- [x] Task: `PacxPackageReleaseProvenance` service implementation.
- [x] Task: `PacxPackageReleaseSbom` service implementation.
- [x] Task: `PacxPackageReleaseVerifier` — verification logic.
- [x] Task: Tests for provenance/SBOM.

### Phase 2: Signing (COMPLETED)
- [x] Task: Configure strong-name signing key (.snk) management. [b5ed258]
- [x] Task: Sign assemblies during build (`.targets` / `.props` integration). [b5ed258]
- [x] Task: Sign NuGet packages during release workflow. [b5ed258] (Implemented via Sigstore signing in release.yml — line 219: sigstore/gh-action-sigstore-python signs .nupkg files and SBOM)
- [x] Task: Sign Git tags for release commits. (Already implemented via `release-tag.yml` — Gitsign Sigstore-based, active prior to this track)
- [x] Task: Tests for signature verification. [b5ed258]

### Phase 3: Release gates (COMPLETED)
- [x] Task: Verify provenance before NuGet publish. [1026ee3]
- [x] Task: Verify SBOM completeness before release. [1026ee3]
- [x] Task: Verify signatures before release promotion. [1026ee3]
- [x] Task: CI gate integration in release workflow. [1026ee3]
- [x] Task: Tests. [1026ee3]

### Phase 4: PR Lifecycle
- [x] Task: Upstream PR; merge. (N/A — submodule embedded, no separate upstream)
