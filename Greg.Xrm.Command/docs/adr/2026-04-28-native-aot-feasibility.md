# ADR: Native AOT Feasibility

## Status
Accepted

## Context
The CLI has reflection-heavy command discovery, Autofac-based activation, Dataverse SDK dependencies, Newtonsoft.Json usage, and plugin-loading paths. Those are useful for the full `pacx` experience but are poor Native AOT candidates.

## Decision
Keep the full CLI on the normal .NET toolchain and evaluate Native AOT only for a `pacx-lite` subset. The candidate subset is static command help, shell completions, connector schema validation, and other commands that avoid Dataverse SDK, Autofac dynamic resolution, plugin loading, and runtime assembly scanning.

## Consequences
- BenchmarkDotNet remains the primary performance measurement tool for the full CLI.
- Native AOT experiments should not block the full CLI release path.
- Any future `pacx-lite` binary needs explicit command registration or generated metadata instead of reflection-driven discovery.
