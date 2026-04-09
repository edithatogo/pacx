# Specification: Power Pages CLI

## Overview
Provide CLI commands for Power Pages website management. Currently, all Power Pages operations require the maker portal GUI. This track brings the entire Power Pages development workflow into the CLI/CI/CD space.

## Scope
- **Site Publish:** Deploy a Power Pages site from local source to an environment.
- **Web Template Sync:** Synchronize web templates, page templates, and content snippets between environments.
- **Site Config Export/Import:** Export and import portal configuration (authentication, navigation, themes) with conflict resolution.
- **Liquid Lint:** Validate Liquid templates for common errors before deployment.

## Constraints
- Power Pages configuration is stored as Dataverse records (adx_* tables) — all operations go through the Dataverse Web API.
- Liquid linting is client-side — requires a custom parser/analyzer.
- Must support incremental publish (only changed files).

## Dependencies
- None — operates on existing adx_* tables in Dataverse.

## Success Criteria
- A developer can run `pacx pages site publish --source ./site --env prod` to deploy a site.
- A developer can run `pacx pages liquid lint --file ./template.liquid` and get validation errors.
- Site configuration can be exported from dev and imported to prod with value mapping.

## API Readiness
- **Power Pages Tables:** Dataverse Web API (adx_website, adx_webtemplate, adx_page, adx_contentsnippet, adx_siteSetting)
- **Publish:** adx_website record activation via Dataverse API
- **Liquid Lint:** Client-side — custom Liquid parser/validator
