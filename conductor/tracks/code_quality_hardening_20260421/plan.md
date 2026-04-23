# Implementation Plan: Code Quality Hardening

## Context
`Directory.Build.props` has `AnalysisLevel=latest-recommended` and `Nullable=enable`, but `TreatWarningsAsErrors=false` and no Central Package Management. SonarAnalyzer is the only analyzer. This track tightens the build to fail fast on warnings and adds a full analyzer stack.

## Phase 1: Central Package Management
- [x] Task: Create `Directory.Packages.props` at repo root; hoist every `<PackageReference Include="X" Version="Y" />` into a single `<PackageVersion>` entry. Remove `Version=` from csproj files.
- [x] Task: Add `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>` to `Directory.Build.props`.
- [x] Task: Verify restore still succeeds across all projects.
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Warnings-as-Errors
- [ ] Task: Flip `TreatWarningsAsErrors=true` in `Directory.Build.props`.
- [ ] Task: Triage resulting errors in batches (by rule ID). Fix or justify with targeted `<NoWarn>` per project.
- [ ] Task: Add `<EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Analyzer Stack
- [ ] Task: Reference `Meziantou.Analyzer`, `Roslynator.Analyzers`, `Microsoft.VisualStudio.Threading.Analyzers`, `AsyncFixer`, `IDisposableAnalyzers`.
- [ ] Task: Reference `Microsoft.CodeAnalysis.BannedApiAnalyzers` + `banned-api.txt` banning `DateTime.Now`, `Thread.Sleep`, `Task.Wait`, `Task.Result`, `Assembly.LoadFrom`.
- [ ] Task: Expand `.editorconfig` with `dotnet_diagnostic.*` severity rules; naming conventions per project style.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: Deterministic Builds + SourceLink
- [x] Task: Add `Microsoft.SourceLink.GitHub`; enable `<PublishRepositoryUrl>true</PublishRepositoryUrl>`, `<EmbedUntrackedSources>true</EmbedUntrackedSources>`, `<DebugType>embedded</DebugType>`, `<Deterministic>true</Deterministic>`.
- [x] Task: Set `<ContinuousIntegrationBuild Condition="'$(GITHUB_ACTIONS)'=='true'">true</ContinuousIntegrationBuild>`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Lock Files
- [ ] Task: Set `<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>`; commit `packages.lock.json` for every project.
- [ ] Task: CI uses `dotnet restore --locked-mode` to prevent drift.
- [ ] Task: Set `<NuGetAudit>true</NuGetAudit>` + `<NuGetAuditMode>all</NuGetAuditMode>` (.NET 8+ vulnerability scan at restore).
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Architecture Tests
- [ ] Task: Add `TestSuite/ArchitectureTests` project using `NetArchTest.Rules`.
- [ ] Task: Enforce: Executors do not depend on each other; Commands do not reference `HttpClient` directly (must go through a service abstraction); all `*Executor` classes implement `ICommandExecutor<T>`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 7: PR Lifecycle
- [ ] Task: Upstream PR per phase; `/ralph-loop`; merge.
