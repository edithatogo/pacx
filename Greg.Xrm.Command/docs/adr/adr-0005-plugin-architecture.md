# ADR 0005: Plugin Architecture

## Status

Accepted

## Context

PACX supports a growing command surface and plugin automation features. Loading everything as hard-coded command registrations would make extension points difficult to maintain.

## Decision

Use assembly scanning and plugin-oriented command registration so new commands can be discovered from command assemblies and plugin folders.

## Consequences

- Plugin commands can extend PACX without changing the central command runner.
- Command metadata attributes remain important for discovery and documentation generation.
- Plugin loading tests are required to protect command discovery behavior.
