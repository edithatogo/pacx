# Implementation Plan: Documentation Overhaul

## Anti-Stub Preamble
Every task produces a user-facing document change. No placeholder content. No TBDs. No stub pages. `/conductor-review` at every phase boundary.

## Overview
Fix the #1 user adoption blocker: no install path in README, no quick-start, no search, stub recipes, flat 189-command index, missing CHANGELOG and LICENSE.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: README Overhaul
- **Analyze:** Read current `README.md`. Identify missing: install command, quick-start flow, NuGet badge link, license section.
- **Implement:** Add `dotnet tool install -g Greg.Xrm.Command` as the first code block. Add a Quick-Start section: `pacx auth` → `pacx solution list` → expected output. Link NuGet badge to package page. Add screenshot/gif of terminal output. Remove "TBD" license note.
- **Verify:** README has install + quick-start + badges + license. New user can run first command in under 2 minutes.
- **Auto-Review:** `/conductor-review`.

### Phase 2: Add Search
- **Analyze:** Read `docs/docfx.json` — check search configuration. Read DocFx docs for Lunr search integration.
- **Implement:** Enable DocFx built-in search (set `"enableSearch": true` in docfx.json). Add `lunr.min.js` index generation. Configure search scope.
- **Verify:** Search box appears on docs site. Searching for a command finds the right page.
- **Auto-Review:** `/conductor-review`.

### Phase 3: Group Command Index
- **Analyze:** Read generated command reference — 189 items in flat list. Identify natural groupings (auth, solution, env, forms, powerbi, etc.).
- **Implement:** Add taxonomy YAML file mapping commands to groups. Regenerate index with grouped sections. Each group has a header and collapsible list.
- **Verify:** Command reference shows grouped by area. Each group has a meaningful heading.
- **Auto-Review:** `/conductor-review`.

### Phase 4: Fill Recipe Stubs
- **Analyze:** Read all 12 recipe files — 4 are stubs (8-9 lines, zero `pacx` commands): `deploy-solution-from-ci.md`, `scaffold-virtual-table.md`, `export-form-responses-weekly.md`, `copilot-studio.md`.
- **Implement:** Fill each recipe with actual `pacx` commands. Include: prerequisites, full command sequence, expected output, CI/CD integration example where applicable.
- **Verify:** Every recipe contains at least 3 `pacx` commands. Recipes are actionable (copy-paste into terminal).
- **Auto-Review:** `/conductor-review`.

### Phase 5: Write Troubleshooting Guide
- **Analyze:** Identify common failure modes from error handling patterns.
- **Implement:** Create `docs/guides/troubleshooting.md`. Sections: Auth failures, Connection issues, API rate limits, Solution errors, Environment errors, Power BI errors, Forms errors, Plugin errors. Each section: symptom → cause → fix.
- **Verify:** Guide covers top 10 most common issues. Each issue has actionable fix.
- **Auto-Review:** `/conductor-review`.

### Phase 6: Add CHANGELOG
- **Analyze:** Read `release-drafter.yml` config. Check if any changelog tooling exists.
- **Implement:** Create `CHANGELOG.md` in Keep a Changelog format. Backfill from git log or release-drafter history. Add link in README.
- **Verify:** CHANGELOG.md exists with at least current version entry.
- **Auto-Review:** `/conductor-review`.

### Phase 7: Add LICENSE
- **Analyze:** Read existing license files (none at root, `LICENSE.txt` in subdir with placeholders). README says "TBD".
- **Implement:** Create root `LICENSE` file with MIT license text. Fill in copyright: `Copyright (c) 2026 PACX Contributors`. Update README badge.
- **Verify:** `LICENSE` exists at root. No "TBD" license notes remain in README.
- **Auto-Review:** `/conductor-review`.

## Rollback
Undo any document change that makes the docs worse. All pages must still be valid after each phase.
