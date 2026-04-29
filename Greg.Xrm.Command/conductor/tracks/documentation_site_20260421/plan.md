# Implementation Plan: Documentation Site

## Context
README is 72 lines; per-command READMEs exist sporadically. No docs site, no API reference, no ADRs. At ~130 commands spanning 50 nouns, users cannot discover what pacx does without reading source.

## Phase 1: DocFX Scaffold
- [x] Task: `docs/` folder — `docfx.json`, `toc.yml`, `index.md`.
- [x] Task: Sections: Getting Started, Authentication, Commands (auto-generated), Recipes, ADRs, Contributing, Upgrade Guide.
- [x] Task: `docs/_build/` gitignored; GitHub Pages deployment via `.github/workflows/docs.yml`.
- [x] Task: Custom template matching pacx branding.
- [x] Task: Run local file/artifact verification.

## Phase 2: Auto-Generated Command Reference
- [x] Task: Write a small tool that scans every `[Command]`-decorated type and emits a markdown page per command: synopsis, usage, flags (from `[Option]`), examples, related commands.
- [x] Task: Run in CI before `docfx build`; commit generated docs under `docs/reference/commands/generated/`.
- [x] Task: Each page links to the source path.
- [x] Task: Run local file/artifact verification.

## Phase 3: Recipes & Guides
- [x] Task: `docs/recipes/` — one markdown per scenario: "Deploy a solution from CI", "Export form responses weekly", "Scaffold a virtual table", "Compare tabular models across environments".
- [x] Task: `docs/guides/authentication.md` — interactive, device-code, client-secret, managed-identity.
- [x] Task: `docs/guides/migration-from-pac.md` — command-by-command mapping for users coming from Microsoft's `pac`.
- [x] Task: Run local file/artifact verification.

## Phase 4: ADRs
- [x] Task: `docs/adr/` with MADR template (`adr-0001-use-command-executor-pattern.md`, `adr-0002-central-package-management.md`, …).
- [x] Task: Backfill ADRs for existing architectural decisions: IOrganizationServiceRepository abstraction, MCP server design, plugin architecture.
- [x] Task: Require a new ADR in PR template when touching stable architecture boundaries.
- [x] Task: Run local file/artifact verification.

## Phase 5: README Refresh
- [x] Task: Trim root README to a clean landing; add badges (build, coverage, NuGet, OpenSSF Scorecard, Best Practices).
- [x] Task: Move "Quick start" to docs; keep README focused on what pacx is and who it's for.
- [x] Task: Add a command-count auto-generated table (X commands across Y nouns).
- [x] Task: Run local review checks and apply fixes.

## Phase 6: PR Lifecycle
- [x] Task: Fork-local PR lifecycle handoff captured via docs workflow and conductor completion state.

---

## Validation Snapshot (2026-04-28)

Local scaffold validation:

- `docs/docfx.json` parses as JSON.
- `docs/toc.yml`, `.github/workflows/docs.yml`, ADR seed files, and recipe pages exist.
- `docs/_build/` is ignored in `.gitignore`.
- `scripts/generate-command-docs.ps1` generated 189 command reference pages under `docs/reference/commands/generated/`.
- `git diff --check` passes for docs-related files, with only Windows line-ending notices.

README refresh:

- Root README now acts as a clean landing page with CI, docs, coverage, NuGet, OpenSSF Scorecard, and OpenSSF Best Practices status badges.
- Quick-start detail moved behind docs links; README keeps only build and contributor entry points.
- Command coverage table documents 189 generated command pages across 50 top-level command areas.
