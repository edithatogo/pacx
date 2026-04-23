# Implementation Plan: CLI UX

## Context
Current CLI has no shell completion, no exit-code taxonomy, no first-run wizard, and inconsistent output conventions. Rich terminal experience is an expected baseline for 2026 CLIs (gh, azd, pac all have this).

## Phase 1: Shell Completion
- [ ] Task: Generate completion scripts from `[Command]` metadata for PowerShell, Bash, Zsh, Fish.
- [ ] Task: Add `pacx completions <shell>` command that prints to stdout; `pacx completions pwsh | Out-String | Invoke-Expression` for on-the-fly load.
- [ ] Task: Recipe in docs for permanent install per shell.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Exit Code Taxonomy
- [ ] Task: Define `ExitCodes` constants: `0 Success`, `1 UsageError`, `2 AuthError`, `3 ApiError`, `4 ValidationError`, `5 NetworkError`, `64 InternalError`.
- [ ] Task: Wrap `CommandRunner` to translate exceptions to codes; document in `docs/guides/exit-codes.md`.
- [ ] Task: Tests asserting expected code per failure scenario.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Uniform Global Flags
- [ ] Task: Global `--verbose`, `--quiet`, `--format {table|json|yaml}`, `--no-color`, `--correlation-id`, `--profile <name>` available on every command.
- [ ] Task: Respect `NO_COLOR` env var (nocolor.org convention).
- [ ] Task: Respect `DOTNET_CLI_TELEMETRY_OPTOUT`, `PACX_TELEMETRY_OPTOUT`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: First-Run Wizard
- [ ] Task: On first invocation without auth, print `pacx auth create` interactive flow (Spectre.Console `Prompt`), offer device-code or interactive.
- [ ] Task: Store profile in `%APPDATA%/pacx/profiles.json` (encrypted via DPAPI on Windows, libsecret on Linux, Keychain on macOS).
- [ ] Task: `pacx auth list`, `pacx auth select`, `pacx auth delete`, `pacx auth whoami` already exist — audit for gaps.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Telemetry (Opt-In, OpenTelemetry)
- [ ] Task: Emit anonymized command-usage counters via OpenTelemetry OTLP to a configurable endpoint; defaults to **disabled**.
- [ ] Task: `pacx telemetry enable|disable|status`.
- [ ] Task: First-run prompt asking for consent (one-time).
- [ ] Task: Document exactly what is collected + provide a sample OTLP dump in docs.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Rich Output
- [ ] Task: Upgrade to `Spectre.Console.Cli` where it improves help rendering; live progress for long operations (solution import, tabular deploy).
- [ ] Task: `--watch` flag on commands that make sense (workflow run list, dataset refresh status).
- [ ] Task: Structured JSON output is round-trippable through `jq`; snapshot-tested via Verify.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 7: PR Lifecycle
- [ ] Task: One PR per phase; `/ralph-loop`; merge.
