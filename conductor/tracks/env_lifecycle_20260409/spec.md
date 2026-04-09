# Specification: Environment Lifecycle Management

## Overview
Enable programmatic environment provisioning, cloning, backup, and capacity management. Currently, all environment lifecycle operations require the Power Platform Admin Center GUI.

## Scope
- **Environment Create:** Create Developer, Sandbox, or Production environments with configurable settings (region, language, currency, security groups).
- **Environment Clone:** Clone environments with selective options — schema only, schema + data, or specific tables.
- **Backup/Restore:** Trigger and monitor database backup/restore operations.
- **Capacity Report:** Report database and file storage capacity across all environments.
- **Environment Reset:** Reset a sandbox environment to factory state.

## Constraints
- Environment creation requires appropriate licensing and permissions.
- Clone operations are asynchronous — must support status polling.
- Backup/restore operations may take extended time — must support progress monitoring.

## Dependencies
- Requires Power Platform Admin API (BAP) access with appropriate admin permissions.
- `alm_center` track — shares Admin API patterns.

## Success Criteria
- A DevOps engineer can run `pacx env create --type Sandbox --region europe --name "Dev-EU"` to create an environment.
- A developer can run `pacx env clone --source prod --target sandbox --mode schema-only` to create a dev copy.
- Capacity reports are available in both table and JSON formats.

## API Readiness
- **Environment CRUD:** Power Platform Admin API (`api.bap.microsoft.com`)
- **Clone:** BAP API environment copy
- **Backup/Restore:** BAP API backup operations
- **Capacity:** BAP Admin API + Dataverse storage endpoints
