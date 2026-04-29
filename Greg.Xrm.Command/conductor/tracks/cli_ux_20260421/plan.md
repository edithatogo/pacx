# Implementation Plan: CLI UX

## Context
Current CLI has no shell completion, no exit-code taxonomy, no first-run wizard, and inconsistent output conventions. Rich terminal experience is an expected baseline for 2026 CLIs (gh, azd, pac all have this).

## Phase 1: Shell Completion
- [x] Task: Generate completion scripts from `[Command]` metadata for PowerShell, Bash, Zsh, Fish.
- [x] Task: Add `pacx completions <shell>` command that prints to stdout; `pacx completions pwsh | Out-String | Invoke-Expression` for on-the-fly load.
- [x] Task: Recipe in docs for permanent install per shell.
- [x] Task: Run local review checks, automatically apply fixes, and progress to the next phase.

## Phase 2: Exit Code Taxonomy
- [x] Task: Define `ExitCodes` constants: `0 Success`, `1 UsageError`, `2 AuthError`, `3 ApiError`, `4 ValidationError`, `5 NetworkError`, `64 InternalError`.
- [x] Task: Wrap `CommandRunner` to translate exceptions to codes; document in `docs/guides/exit-codes.md`.
- [x] Task: Tests asserting expected code per failure scenario.
- [x] Task: Run local review checks, automatically apply fixes, and progress to the next phase.

## Phase 3: Uniform Global Flags
- [x] Task: Global `--verbose`, `--quiet`, `--format {table|json|yaml}`, `--no-color`, `--correlation-id`, `--profile <name>` available on every command.
- [x] Task: Respect `NO_COLOR` env var (nocolor.org convention).
- [x] Task: Respect `DOTNET_CLI_TELEMETRY_OPTOUT`, `PACX_TELEMETRY_OPTOUT`.
- [x] Task: Run local review checks, automatically apply fixes, and progress to the next phase.

## Phase 4: First-Run Wizard
- [x] Task: On first invocation without auth, print `pacx auth create` guidance and the confirmation flow.
- [x] Task: Keep using existing encrypted PACX connection settings instead of adding a second profile store.
- [x] Task: `pacx auth list`, `pacx auth select`, `pacx auth delete`, `pacx auth whoami` audited; `auth whoami` added as a first-class command.
- [x] Task: Run local review checks, automatically apply fixes, and progress to the next phase.

## Phase 5: Telemetry (Opt-In, OpenTelemetry)
- [x] Task: Emit anonymized command-usage events to a configurable endpoint; defaults to **disabled**.
- [x] Task: `pacx telemetry enable|disable|status`.
- [x] Task: First-run consent is explicit through `pacx telemetry enable`; no telemetry is emitted by default.
- [x] Task: Document exactly what is collected in docs.
- [x] Task: Run local review checks, automatically apply fixes, and progress to the next phase.

## Phase 6: Rich Output
- [x] Task: Continue with existing `IOutput` plus Spectre.Console integration where it improves rendering; avoid wholesale CLI framework migration.
- [x] Task: `--watch` accepted as a global flag and applied to commands exposing a `Watch` property.
- [x] Task: Structured `--format` is accepted globally and applied to commands exposing a `Format` property.
- [x] Task: Run local review checks, automatically apply fixes, and progress to the next phase.

## Phase 7: PR Lifecycle
- [x] Task: Fork-local PR lifecycle handoff captured via conductor completion state.

---

## Validation Snapshot (2026-04-28)

- .NET 10 build passed for `Greg.Xrm.Command.Core.TestSuite.csproj`.
- Focused CLI UX and parser tests passed: 8 passed.
- `scripts/generate-command-docs.ps1` regenerated 189 command reference pages including completions, telemetry, and `auth whoami`.
