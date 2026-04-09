# Specification: Power BI Semantic Model CLI (Tabular Editor Parity)

## Overview
Replicate Tabular Editor 3's core CI/CD capabilities as CLI commands. This enables fully automated, scriptable Power BI semantic model management without a desktop GUI.

## Scope
- **BIM Deploy:** Deploy a `.bim` (Tabular Object Model) file to a Power BI semantic model with idempotent operations.
- **Model Diff:** Compare local `.bim` against deployed model — report drift for CI/CD validation.
- **Model Validation:** Validate `.bim` files for circular dependencies, invalid references, and best practices.
- **Translation Management:** Manage and deploy multi-language translations for measures, columns, and tables.
- **Role & Measure Operations:** Bulk-add measures to security roles, manage perspectives.
- **BIM Compare:** Compare two `.bim` files and output structural differences.

## Constraints
- Deploy must be idempotent — only push changes, not full model overwrites.
- Must support both XMLA endpoint (Premium/Embedded) and REST API (Pro) connectivity.
- BIM files are JSON-based TOM serialization — must handle TOM schema correctly.

## Dependencies
- `Microsoft.PowerBI.Api` NuGet package.
- `Microsoft.AnalysisServices.Core` or open-source TOM wrapper libraries.

## Success Criteria
- A DevOps engineer can run `pacx tabular deploy --bim ./model.bim --workspace MyWorkspace` in a CI/CD pipeline.
- A developer can run `pacx tabular diff --bim ./model.bim --workspace MyWorkspace` and see drift.
- `pacx tabular validate --bim ./model.bim` catches circular dependencies and invalid references.

## API Readiness
- **Deploy:** Power BI REST API (datasets) + XMLA endpoint (Premium)
- **Diff:** XMLA endpoint TOM comparison or REST API metadata
- **Validate:** Client-side TOM validation
- **Translations:** Power BI REST API (locale translations)
- **BIM Compare:** Local JSON diff
