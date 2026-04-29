# ADR 0002: Central Package Management

## Status

Accepted

## Context

Package versions were previously distributed across project files, increasing drift risk and making upgrades harder to review.

## Decision

Use `Directory.Packages.props` for central package versions.

## Consequences

- Dependency upgrades are reviewed in one file.
- Project files remain focused on dependency intent.
- Restore lock files provide drift detection in CI.
