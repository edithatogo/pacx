# Implementation Plan: .NET 10/11 SOTA Modernization

## Anti-Stub Preamble
Every task produces a measurable code improvement: replaced legacy pattern with modern .NET API, reduced allocations, improved testability. No stubs, no TODOs. Build must pass after each phase.

## Overview
Apply .NET 10 and .NET 11 preview features across the codebase to improve performance, readability, and testability.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: `System.Threading.Lock` (Replace `object lock`)
- **Analyze:** Search for `lock (.*)` patterns and `Lock syncRoot` in production code
- **Implement:** Replace `private readonly object syncRoot = new();` with `private readonly Lock syncRoot = new();`. The `Lock` class in .NET 10 provides better performance and clearer semantics.
- **Files to modify:** `OutputToConsole.cs`, `CommandRegistry.cs`, any other files using `lock(object)`
- **Verify:** Build passes. No `lock(object)` remains in production code.
- **Auto-Review:** `/conductor-review`

### Phase 2: `TimeProvider` Abstraction
- **Analyze:** Search for `DateTime.UtcNow`, `DateTimeOffset.UtcNow`, `DateTime.Now`, `DateTimeOffset.Now`, `Stopwatch.StartNew()` in production code
- **Implement:** Add `TimeProvider` as a DI service. Replace direct `DateTime.UtcNow` calls with `timeProvider.GetUtcNow()`. Makes time testable without mocking static methods.
- **Files to modify:** All service classes using current time, `CommandRunnerBase` for telemetry timestamps, `TokenProvider` for expiry calculations
- **Verify:** Build passes. Tests can inject fake `TimeProvider` for deterministic time assertions.
- **Auto-Review:** `/conductor-review`

### Phase 3: `SearchValues<T>` for Fast String Matching
- **Analyze:** Search for `Regex` in hot paths (command parsing, option matching)
- **Implement:** Use `SearchValues.Create(['-', '/'])` for option prefix checking instead of `Regex` or `String.StartsWith` multiple calls. `SearchValues` is hardware-accelerated.
- **Files to modify:** `CommandRunArgs.cs` `IsOption()` method, `CommandParser.cs` string matching
- **Verify:** Build passes. Benchmarks show improved parsing throughput.
- **Auto-Review:** `/conductor-review`

### Phase 4: Collection Expressions
- **Analyze:** Search for `new List<T> { ... }`, `new string[] { ... }`, `new Dictionary<string, T> { ... }` in test and production code
- **Implement:** Replace with collection expressions: `[item1, item2]`, `[.. existing]`. This reduces allocations and improves readability.
- **Files to modify:** All files with collection initializers. Focus on test files first where this improves readability most.
- **Verify:** Build passes. No functional changes.
- **Auto-Review:** `/conductor-review`

### Phase 5: UTF-8 String Literals
- **Analyze:** Search for `Encoding.UTF8.GetBytes(...)` calls in HTTP-heavy areas
- **Implement:** Use `"string"u8` for string literals that are converted to `byte[]`. This is allocation-free.
- **Files to modify:** Service clients that construct HTTP content (`PowerAutomateClient.cs`, `FormsApiClient.cs`, `PowerPlatformAdminClient.cs`)
- **Verify:** Build passes. Reduced allocations in hot HTTP paths.
- **Auto-Review:** `/conductor-review`

### Phase 6: Semi-Auto Properties (`field` keyword) — .NET 11 preview
- **Analyze:** Search for manual backing fields in DTOs and models
- **Implement:** Use `public string Name { get; field; }` pattern where applicable. The `field` keyword eliminates the need for explicit backing fields with property validation.
- **Files to modify:** DTO classes like `FormsForm`, `FormsResponse`, `PacxConfig`, `CommandDefinition`
- **Verify:** Build passes. Cleaner property declarations.
- **Auto-Review:** `/conductor-review`

### Phase 7: `IIncrementalGenerator` for Command Registration
- **Analyze:** Review current reflection-heavy command pipeline (`CommandRegistry.ScanForCommands`, `CommandDefinition.CreateCommand`)
- **Implement:** Create a Roslyn source generator that produces compile-time command registry with zero reflection on startup
- **Files to create:** `Greg.Xrm.Command.SourceGenerator/CommandSourceGenerator.cs`
- **Verify:** Build passes. Command registration is compile-time verified. Startup time improved.
- **Auto-Review:** `/conductor-review`
