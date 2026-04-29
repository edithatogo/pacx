# Dataverse Skill Pack

This guidance pack captures the Dataverse operator workflows PACX already knows how to support, plus the places where future MCP and automation features should align.

## What It Covers

- Environment discovery and identity checks
- Solution inventory and lifecycle operations
- Table and column schema management
- Connector and metadata validation
- Package and release preparation

## What It Does Not Cover

- Tenant-specific governance policy
- Secrets handling or credential storage
- Irreversible production changes without explicit review

## Core Workflows

### Discover the current context

```powershell
pacx auth whoami
pacx org info
pacx solution list
```

### Validate before you change

```powershell
pacx connector validate --file connector.json --strict
pacx package validate --file package.json
pacx validate all
```

### Inspect schema and metadata

```powershell
pacx table exportMetadata --table account
pacx column getDependencies --table account --column name
```

## Alignment Notes

The Dataverse skill-pack is intentionally lightweight. It is a reference surface for agents and docs, not a replacement for the CLI or the upstream Dataverse guidance published by Microsoft and the Dataverse MCP / Dataverse-skills projects.

The machine-readable catalog entry lives at `conductor/skill-pack-catalog/skill-packs.json`. Future automation can use that file as the stable entry point for discovery and linking.

The pack should stay close to existing PACX command names and document the gap between:

- what PACX can already do
- what can be safely automated next
- what still needs an upstream or platform-specific workflow
