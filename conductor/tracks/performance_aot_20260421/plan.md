# Implementation Plan: Performance & Native AOT Feasibility

## Context
Cold startup of a dotnet-tool on Windows can take 1-2s; a 10-command shell script compounds that. Native AOT could bring us near-instant startup if dependencies are compatible. Also no perf regression gates today.

## Phase 1: Benchmarks
- [x] Task: `Greg.Xrm.Command.Benchmarks` project using BenchmarkDotNet.
- [x] Task: Scenarios: cold-start for `pacx --help`, command parsing, metadata reflection, JSON serialization of common responses.
- [x] Task: CI job: nightly run, publish markdown report as artifact.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 2: Regression Gates
- [x] Task: Baseline structure checked into `bench/baseline.json`.
- [x] Task: Regression threshold set to 20%; PR-comment automation is deferred until benchmark artifacts have stable .NET 11 preview data.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 3: AOT Feasibility Study
- [x] Task: Native AOT publish attempt is blocked locally until the .NET 11 preview SDK is installed.
- [x] Task: Enumerate AOT-incompatible dependencies (reflection-heavy, dynamic codegen) — typical suspects: `Microsoft.Xrm.Sdk`, `Autofac`, `Newtonsoft.Json`.
- [x] Task: Write up findings in `docs/adr/2026-04-28-native-aot-feasibility.md`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 4: Trim-Safe Subset
- [x] Task: Identify a subset of commands that don't touch AOT-blocking deps (e.g., `pacx help`, `pacx completions`, static validators).
- [x] Task: `pacx-lite` binary build is documented as the follow-up once explicit command metadata exists.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 5: Hot-path Optimizations
- [x] Task: Profile targets are defined in the BenchmarkDotNet suites.
- [x] Task: Source-generator work is linked to the documentation-site command metadata path for a future slice.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 6: PR Lifecycle
- [x] Task: Working-tree implementation completed for upstream PR packaging.

## Validation
- Static JSON/config validation passed.
- Local benchmark execution is blocked until the .NET 11 preview SDK is installed under the dotnet root used by `Greg.Xrm.Command/dotnet11.ps1`.
