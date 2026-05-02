# Skill Packs

Skill packs are reusable capability bundles that group related PACX commands, configurations, and automation patterns into a single installable unit.

## Overview

A skill pack is a curated collection of CLI commands, configuration presets, and documentation designed to solve a specific domain problem. Instead of discovering and configuring individual commands, you install a skill pack and get everything you need for that domain.

### Available Skill Packs

| Pack | Description | Author |
|------|-------------|--------|
| `dataverse-devops` | CI/CD automation for Dataverse | PACX Team |
| `power-platform-governance` | Governance and security tooling | PACX Team |
| `flow-automation` | Power Automate management suite | PACX Team |
| `copilot-enablement` | Copilot Studio agent lifecycle | PACX Team |
| `power-bi-ops` | Power BI workspace management | PACX Team |

## Usage

### List available packs

```powershell
pacx skill pack list
```

Filter by tag:

```powershell
pacx skill pack list --tag governance
```

Search by keyword:

```powershell
pacx skill pack list --query dataverse
```

### Install a pack

```powershell
pacx skill pack install --id dataverse-devops
```

### Preview installation (dry-run)

```powershell
pacx skill pack install --id flow-automation --dry-run
```

## Catalog Format

Skill packs are distributed as JSON catalog entries. The catalog is at `conductor/skill-pack-catalog/packs.json`.

Each entry contains:

- **id** — Unique identifier for the pack
- **name** — Display name
- **description** — What the pack provides
- **version** — Semantic version
- **author** — Publisher
- **capabilities** — List of capabilities the pack enables
- **dependencies** — IDs of other packs required
- **tags** — Categorization tags

## Authoring a Skill Pack

1. Define a catalog entry in the JSON format above
2. Ensure any pack dependencies are listed first
3. Submit via PR to the PACX repository
