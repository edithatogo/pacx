# Implementation Plan: Documentation Site

## Context
README is 72 lines; per-command READMEs exist sporadically. No docs site, no API reference, no ADRs. At ~130 commands spanning 50 nouns, users cannot discover what pacx does without reading source.

## Phase 1: DocFX Scaffold
- [ ] Task: `docs/` folder — `docfx.json`, `toc.yml`, `index.md`.
- [ ] Task: Sections: Getting Started, Authentication, Commands (auto-generated), Recipes, ADRs, Contributing, Upgrade Guide.
- [ ] Task: `docs/_build/` gitignored; GitHub Pages deployment via `.github/workflows/docs.yml`.
- [ ] Task: Custom template matching pacx branding.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Auto-Generated Command Reference
- [ ] Task: Write a small tool (`Greg.Xrm.Command.DocGen`) that reflects on every `[Command]`-decorated type and emits a markdown page per command: synopsis, usage, flags (from `[Option]`), examples, related commands.
- [ ] Task: Run in CI before `docfx build`; commit generated docs under `docs/reference/commands/`.
- [ ] Task: Each page cross-links to the executor source on GitHub.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Recipes & Guides
- [ ] Task: `docs/recipes/` — one markdown per scenario: "Deploy a solution from CI", "Export form responses weekly", "Scaffold a virtual table", "Compare tabular models across environments".
- [ ] Task: `docs/guides/authentication.md` — interactive, device-code, client-secret, managed-identity.
- [ ] Task: `docs/guides/migration-from-pac.md` — command-by-command mapping for users coming from Microsoft's `pac`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: ADRs
- [ ] Task: `docs/adr/` with MADR template (`adr-0001-use-command-executor-pattern.md`, `adr-0002-central-package-management.md`, …).
- [ ] Task: Backfill ADRs for existing architectural decisions: command pattern, IOrganizationServiceRepository abstraction, MCP server design, plugin architecture.
- [ ] Task: Require a new ADR in PR template when touching `*.Core/Architecture/*`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: README Refresh
- [ ] Task: Trim root README to a clean landing; add badges (build, coverage, NuGet, OpenSSF Scorecard, Best Practices).
- [ ] Task: Move "Quick start" to docs; keep README focused on what pacx is and who it's for.
- [ ] Task: Add a command-count auto-generated table (X commands across Y nouns).
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: PR Lifecycle
- [ ] Task: Upstream PR per phase; `/ralph-loop`; merge.
