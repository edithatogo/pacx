# Ecosystem Intake

PACX now carries a lightweight catalog model for adjacent ecosystems so users can discover related tools, packages, and providers from one place.

## Tool Catalog

Use `pacx tool browse` to list tool-style utilities and `pacx tool run` to inspect or open a selected entry.

```powershell
pacx tool browse --category Dataverse
pacx tool run --name XrmToolBox
pacx tool run --name Flow Studio --open
```

The default catalog lives at `conductor/tool-catalog/tools.json` and includes entries for:

- XrmToolBox
- Flow Studio
- Dataverse MCP
- Power Platform CLI
- Power BI API

## Package and Source Catalog

Use `pacx package source browse` to inspect package feeds, SDK repositories, and provider ecosystems.

```powershell
pacx package source browse --category Packages
pacx package source browse --query Dataverse
```

The default catalog lives at `conductor/source-catalog/sources.json` and includes entries for:

- NuGet
- PowerShell Gallery
- Dataverse
- Power BI

## Mapping Notes

This repo does not mirror the upstream products one-for-one. The catalogs are a discovery layer that points back to the upstream sources and records the kinds of capabilities PACX can absorb over time.

Relevant upstream sources include:

- XrmToolBox and its plugin gallery
- Flow Studio capability surfaces and Flow Studio
- Dataverse MCP and Dataverse skills
- Microsoft Power Platform CLI
- NuGet packages for Dataverse, Power BI, and Power Platform automation
- PowerShell Gallery packages for operator workflows
