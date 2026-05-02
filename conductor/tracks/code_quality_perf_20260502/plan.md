# Implementation Plan: Code Quality & Performance

## Anti-Stub Preamble
Every task produces a measurable improvement: eliminated dead code, removed sync-over-async, reduced allocations, or removed a suppress warning. No TODOs. No stubs. `/conductor-review` at every phase boundary.

## Overview
Fix the P0 dead code bug in CommandRunnerBase. Eliminate sync-over-async in 70+ test files. Add source generators for zero-reflection command dispatch. Fix CommandResult allocation pattern. Add catch filters. Remove Autofac.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: Fix Dead Code Bug in CommandRunnerBase
- **Analyze:** Read `CommandRunnerBase.cs` lines 200-228 — `task.IsFaulted` and `task.IsCanceled` are unreachable after `await task`.
- **Implement:** Remove the dead code. Restructure to let exceptions propagate to the caller's `catch (Exception ex)` without trying to handle them twice. Add explicit `OperationCanceledException` catch.
- **Verify:** Tests confirm cancellation flow works. No unreachable code remains. Code coverage on the path increases.
- **Auto-Review:** `/conductor-review`.

### Phase 2: Eliminate Sync-over-Async in Tests
- **Analyze:** Search for `.GetAwaiter().GetResult()`, `.Result`, `.Wait()` in all test files (~70+ matches).
- **Implement:** Convert all affected test methods from `void` to `async Task`. Replace `.GetAwaiter().GetResult()` with `await`. Remove `.Wait()` calls.
- **Verify:** All tests pass. No sync-over-async patterns remain in test code. No deadlock risk.
- **Auto-Review:** `/conductor-review`.

### Phase 3: Redesign CommandResult to Avoid Dictionary Allocation
- **Analyze:** Read `CommandResult.cs` — extends `Dictionary<string, object>`. `CommandResult.Success()` allocates an empty dictionary.
- **Implement:** Decouple success/failure from data container. Make `Success()` return a lightweight object with no dictionary. Add `SuccessWithData(data)` for commands that return data. Update `PrintSuccess` method.
- **Verify:** `CommandResult.Success()` is allocation-free (no dictionary). Performance benchmarks show improvement.
- **Auto-Review:** `/conductor-review`.

### Phase 4: Add Catch Filters System-Wide
- **Analyze:** Search for all `catch (Exception ex)` patterns (~243 matches). Identify those that should propagate `OperationCanceledException`.
- **Implement:** Add `when (ex is not OperationCanceledException)` to all generic catch blocks in production code. Add `catch (OperationCanceledException)` where cancellation needs special handling.
- **Verify:** Cancellation propagates correctly. No `OperationCanceledException` is swallowed. Tests confirm cancellation flow.
- **Auto-Review:** `/conductor-review`.

### Phase 5: Replace Autofac with MS.DI Scopes
- **Analyze:** Read `IoCModule.cs` (109 lines), `CommandExecutorFactory.cs`, `Program.cs`. Autofac is used only for `BeginLifetimeScope`.
- **Implement:** Migrate all Autofac registrations to `IServiceCollection`. Use `IServiceScopeFactory` in `CommandExecutorFactory`. Remove `Autofac.Extensions.DependencyInjection` and `Autofac` NuGet dependencies.
- **Verify:** All services resolve correctly. Executor factory creates lifetime scopes. No registration leaks between scopes.
- **Auto-Review:** `/conductor-review`.

### Phase 6: ConfigureAwait Audit
- **Analyze:** Search for `await` without `ConfigureAwait(false)` in production code (not test code). Check `CommandRunnerBase.cs`, service clients.
- **Implement:** Add `ConfigureAwait(false)` to all missing production `await` calls. Add `Microsoft.VisualStudio.Threading.Analyzers` to catch regressions.
- **Verify:** All production `await` calls have `ConfigureAwait(false)`. Analyzer catches future violations.
- **Auto-Review:** `/conductor-review`.

## Rollback
Any change that breaks tests: revert, fix, re-verify. Test suite must pass after each phase.
