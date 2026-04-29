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

### Phase 2: Signing (PENDING)
- [ ] Task: Configure strong-name signing key (.snk) management.
- [ ] Task: Sign assemblies during build (`.targets` / `.props` integration).
- [ ] Task: Sign NuGet packages during release workflow.
- [ ] Task: Sign Git tags for release commits.
- [ ] Task: Tests for signature verification.

### Phase 3: Release gates (PENDING)
- [ ] Task: Verify provenance before NuGet publish.
- [ ] Task: Verify SBOM completeness before release.
- [ ] Task: Verify signatures before release promotion.
- [ ] Task: CI gate integration in release workflow.
- [ ] Task: Tests.

### Phase 4: PR Lifecycle
- [ ] Task: Upstream PR; merge.
