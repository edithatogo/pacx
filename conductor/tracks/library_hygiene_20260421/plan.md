# Implementation Plan: Library Hygiene

## Context
Static audit identified three async/threading antipatterns: blocking `Task.Result` on awaited tasks, `DateTime.Now` instead of `DateTimeOffset.UtcNow`, and zero `ConfigureAwait(false)` across the Core library. CancellationTokens are also not threaded through command execution. These are minor but widely visible library-quality signals.

## Current Status
Phase 1 and Phase 2 are now satisfied in the repository. The remaining open work is the `ConfigureAwait(false)` audit and cancellation-token plumbing.

## Phase 1: Blocking-call fixes
- [x] Task: Fix `Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Plugin/ListCommandExecutor.cs:58-61` — replace triple `Task.Result` after `Task.WhenAll` with awaited tuple destructuring. *Repository scan confirmed the executor already uses `await`/`Task.WhenAll`; no blocking `Result` call remains in core.*
- [x] Task: Ban `.Result`, `.Wait()`, `GetAwaiter().GetResult()` via a Roslyn analyzer rule in `.editorconfig` (severity: error). Use `AsyncFixer` package. *Already present in `.editorconfig`.*
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase. *Phase 1 normalized after repo scan and status cleanup.*

## Phase 2: Clock hygiene
- [x] Task: Codemod `DateTime.Now` → `DateTimeOffset.UtcNow` across the repo; identified files: `PrTrackCommandExecutor.cs`, `ExportCommandExecutor.cs`, `UninstallCommandExecutor.cs`, `WebResourceWatchCommandExecutor.cs`, and any others found by a fresh scan. *Remaining `DateTime.Now` usages were removed from the test/helper call sites; the only match left is the guardrail entry in `BannedApi.txt`.*
- [x] Task: Introduce `IClock` abstraction and inject it into executors that emit timestamps so tests can fake time. *`Greg.Xrm.Command.Interfaces.Services.IClock` and `Greg.Xrm.Command.Core.Services.Clock` already exist.*
- [x] Task: Ban `DateTime.Now` via `BannedApiAnalyzers` with `banned-api.txt`. *Already present in `Greg.Xrm.Command.Core/BannedApi.txt`.*
- [x] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase. *Phase 2 normalized after repo scan and status cleanup.*

## Phase 3: ConfigureAwait audit
- [ ] Task: Add `<NoWarn>CA2007</NoWarn>` opt-out OR add `.ConfigureAwait(false)` to every async call in `Greg.Xrm.Command.Core`. Chose: add `ConfigureAwait(false)` (library best practice).
- [ ] Task: Enable Roslynator `RCS1090` / `Microsoft.VisualStudio.Threading.Analyzers` `VSTHRD111` as error.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: CancellationToken plumbing
- [ ] Task: Extend `ICommandExecutor<T>` with a `CancellationToken ct` parameter (additive — existing impls adopt via default parameter during migration).
- [ ] Task: `CommandRunner` forwards `Console.CancelKeyPress` → linked cancellation source.
- [ ] Task: Thread `ct` through every `*Async` call down to `IOrganizationService` and `HttpClient.SendAsync`.
- [ ] Task: Integration test: start long-running command, send SIGINT, confirm clean abort.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: PR Lifecycle
- [ ] Task: Open one PR for Phases 1+2, a second for 3+4.
- [ ] Task: `/ralph-loop` with completion promise: "All analyzer rules pass at error severity; no regression in TestSuite".
- [ ] Task: Confirm merged or document blockers.
