# Implementation Plan: CLI UX & Command Polish

## Anti-Stub Preamble
Every task produces a measurable UX improvement: a fixed verb, a working `--version`, an example in help output. No stubs. No TODOs. `/conductor-review` at every phase boundary.

## Overview
Fix verb inconsistencies, standardize output format across all commands, add `--version`, fix plural bugs, add inline help examples, improve error messages.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: Fix Verb Inconsistencies
- **Analyze:** Read all command definitions. Identify multi-word kebab verbs (`data init-schema-from-solution`, `data seed-mock`, `table define-migration-strategy`). Identify depth inconsistencies (`pcf version bump` depth-3 vs flat `pcf publish` depth-2).
- **Implement:** Break multi-word verbs into sub-commands: `data init-schema-from-solution` → `data schema init --from-solution`. `data seed-mock` → `data mock seed`. `table define-migration-strategy` → `table migration define`. Add backward-compatible aliases for all broken verbs.
- **Verify:** Old commands still work via aliases. New commands follow `noun verb` or `noun verb subverb` pattern. Help output shows correct hierarchy.
- **Auto-Review:** `/conductor-review`.

### Phase 2: Centralize Output Format
- **Analyze:** Read all commands with `--format` options — some use `enum OutputFormat`, some use `string`. Several ignore the flag entirely (env create, env clone, env backup).
- **Implement:** Create global `OutputFormat` enum (`Table`, `Json`, `Yaml`, `TableCompact`). Remove per-command `--format` strings. Replace with shared option. Fix env commands to actually check `--format`. Wire `YamlSerializer` into the dispatch.
- **Verify:** All commands respect `--format json`, `--format yaml`, `--format table`. Output is consistent across modules.
- **Auto-Review:** `/conductor-review`.

### Phase 3: Add `--version` and `pacx version`
- **Analyze:** Read `Update/SelfUpdateCommand.cs` — handles updates but not version display. Read `Assembly.GetEntryAssembly().GetName().Version` pattern.
- **Implement:** Add `[Command("version")]` that displays current assembly version. Wire `--version` global flag in `Program.cs` or `CommandRunnerBase.cs`.
- **Verify:** `pacx --version` and `pacx version` both work. Output matches NuGet package version.
- **Auto-Review:** `/conductor-review`.

### Phase 4: Fix Plural Inconsistencies
- **Analyze:** `forms response count` vs `forms responses export` — inconsistent singular/plural.
- **Implement:** Standardize on singular: `forms response list`, `forms response export`, `forms response get`, `forms response count`. Add backward-compatible aliases for the old plural forms.
- **Verify:** Singular forms work as primary. Plural forms work via aliases. Help shows singular only.
- **Auto-Review:** `/conductor-review`.

### Phase 5: Add Inline Help Examples
- **Analyze:** Read `ICanProvideUsageExample` interface. Read `HelpCommand` — how `--help` output is generated. Check if examples from the interface appear in terminal help.
- **Implement:** Wire `ICanProvideUsageExample` into the terminal `--help` output (currently it only feeds markdown export). If no interface implementation exists, add 1-2 examples directly to `HelpText`.
- **Verify:** `pacx command --help` shows at least one usage example. Example is contextually appropriate.
- **Auto-Review:** `/conductor-review`.

### Phase 6: Improve Error Messages
- **Analyze:** Read common error paths across executors. Generic `catch (Exception ex) { return CommandResult.Fail(ex.Message); }` patterns.
- **Implement:** Add contextual "how to fix" guidance to the top 10 most common errors: auth failures, connection failures, solution not found, environment not found, invalid tenant ID, API errors, rate limiting, validation errors, missing permissions, network errors.
- **Verify:** Each error message includes: what went wrong, why it might have happened, and how to fix it.
- **Auto-Review:** `/conductor-review`.

## Rollback
Any change that breaks command parsing: revert immediately. All commands must work after each phase.
