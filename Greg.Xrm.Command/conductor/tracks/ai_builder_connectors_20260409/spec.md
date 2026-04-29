# Specification: AI Builder & Custom Connectors

## Overview
Provide CLI management for AI Builder models and custom connectors — two growing areas of the Power Platform that currently have minimal CLI surface area.

## Scope
- **AI Model Management:** List models with training status/accuracy, trigger training, publish trained models.
- **Form Processor Configuration:** Configure form processing models (document type, fields, tables).
- **Custom Connector Management:** Import/export custom connectors, test with sample payloads, validate against OpenAPI schema.

## Constraints
- AI Builder training is asynchronous — must support status polling.
- Custom connector testing requires actual HTTP calls to the connector's backend.
- Connector import/export uses solution component packaging.

## Dependencies
- None — operates on existing Dataverse and Power Platform APIs.

## Success Criteria
- An AI admin can run `pacx ai model list` and see all models with training status.
- A developer can run `pacx connector validate --file ./connector-definition.json` to validate against OpenAPI.
- A form processing model can be fully configured via CLI without the maker portal.

## API Readiness
- **AI Models:** Dataverse Web API (aimodel) + AI Builder API
- **Form Processor:** Dataverse Web API (aibuilder_formprocessing)
- **Custom Connectors:** Dataverse Web API (connector) — import/export as solution components
- **Connector Test:** HTTP calls to connector backend using stored authentication
