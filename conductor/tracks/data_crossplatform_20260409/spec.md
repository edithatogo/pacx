# Specification: Data & Cross-Platform Engine

## Overview
Rewrite data export/import using pure .NET 6+ to eliminate the WPF/CMT dependency that blocks Mac/Linux support. Additionally, add data schema generation and mock data seeding capabilities.

## Scope
- **Pure .NET 6+ Data Engine:** Replace the legacy Configuration Migration Tool (CMT) dependency with a pure .NET 6+ data engine using `ServiceClient` — enabling Mac/Linux support.
- **Schema from Solution:** Generate a schema definition from an existing solution (tables, columns, relationships, choice values).
- **Mock Data Seed:** Generate realistic mock/seed data for development environments based on schema definitions.

## Constraints
- Must be fully cross-platform (Windows, Mac, Linux).
- Must support the same data formats as CMT (ZIP with import/export XML) for compatibility.
- Mock data must respect field constraints (required fields, choice values, lookups).

## Dependencies
- `Microsoft.PowerPlatform.Dataverse.ServiceClient` (cross-platform compatible).
- Community libraries: Capgemini.Xrm.DataMigration for reference patterns.

## Success Criteria
- `pacx data export` and `pacx data import` work identically on Windows, Mac, and Linux.
- A developer can run `pacx data init-schema-from-solution --solution MySolution --output ./schema.yaml` to generate a schema definition.
- A developer can run `pacx data seed-mock --schema ./schema.yaml --count 100 --output ./data.zip` to generate mock data.

## API Readiness
- **Data Engine:** Dataverse Web API via `ServiceClient` — fully cross-platform
- **Schema Generation:** Dataverse Web API (RetrieveEntity, RetrieveAllEntities)
- **Mock Data:** Local schema parsing + data generation (no API needed for generation)
