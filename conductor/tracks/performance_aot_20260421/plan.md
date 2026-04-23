# Implementation Plan: Performance & Native AOT Feasibility

## Context
Cold startup of a dotnet-tool on Windows can take 1-2s; a 10-command shell script compounds that. Native AOT could bring us near-instant startup if dependencies are compatible. Also no perf regression gates today.

## Phase 1: Benchmarks
- [ ] Task: `Greg.Xrm.Command.Benchmarks` project using BenchmarkDotNet.
- [ ] Task: Scenarios: cold-start for `pacx --help`, command parsing, metadata reflection, JSON serialization of common responses.
- [ ] Task: CI job: nightly run, publish markdown report as artifact.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Regression Gates
- [ ] Task: Baseline results checked into `bench/baseline.json`.
- [ ] Task: PR comment bot compares run-to-baseline; flags >20% regressions.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: AOT Feasibility Study
- [ ] Task: Try `dotnet publish -c Release -r win-x64 --self-contained /p:PublishAot=true`.
- [ ] Task: Enumerate AOT-incompatible dependencies (reflection-heavy, dynamic codegen) — typical suspects: `Microsoft.Xrm.Sdk`, `Autofac`, `Newtonsoft.Json`.
- [ ] Task: Write up findings in `docs/adr/adr-00NN-native-aot.md`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: Trim-Safe Subset
- [ ] Task: Identify a subset of commands that don't touch AOT-blocking deps (e.g., `pacx help`, `pacx completions`, static validators).
- [ ] Task: Build an AOT-optimized "pacx-lite" binary for those.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Hot-path Optimizations
- [ ] Task: Profile top-10 commands; look for reflection caches, allocator churn, string concatenation in loops.
- [ ] Task: Upgrade reflection-heavy paths to `[Command]` source generators (see `documentation_site` Phase 2).
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.
