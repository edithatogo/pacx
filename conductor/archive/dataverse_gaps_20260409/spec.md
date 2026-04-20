# Specification: Dataverse Platform Gaps

## Overview
Address CLI-accessible Dataverse features that are currently GUI-only or API-only, including Custom APIs, Catalog/Business Events, Elastic Tables, Virtual Tables, and Connection References.

## Scope
- **Custom API Create:** Create Custom APIs (Custom Actions) via CLI — currently requires GUI or messy solution manipulation.
- **Catalog Publish:** Manage Catalog & Business Events for exposing Dataverse events externally.
- **Elastic Table Manage:** Manage retention policies and scaling for Elastic Tables.
- **Virtual Table Scaffold:** Scaffold virtual table definitions from external data sources.
- **Connection Reference Map:** Interactive mapping of connection references across solutions.

## Constraints
- Custom APIs require careful handling of request/response parameter definitions.
- Catalog is a newer feature — API may be less documented.
- Virtual tables require external data source connectivity validation.

## Dependencies
- None — operates on core Dataverse Web API tables.

## Success Criteria
- A developer can run `pacx custom-api create --name "MyAction" --inputs "String:Target" --outputs "Entity:Result"` to create a Custom API.
- An admin can run `pacx elastic-table manage --table MyElasticTable --retention 90d` to configure retention.
- A developer can scaffold a virtual table from a SQL Server data source.

## API Readiness
- **Custom APIs:** Dataverse Web API (customapi, customapirequestparameter, customapiresponseproperty)
- **Catalog:** Dataverse Web API (catalog, catalogitem) — newer, less documented
- **Elastic Tables:** Dataverse Web API (table metadata with changefeed)
- **Virtual Tables:** Dataverse Web API (externaldatasource, entitymap)
- **Connection References:** Dataverse Web API (connectionreference)
