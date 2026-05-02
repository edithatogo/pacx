# Microsoft Forms PowerShell Guide

This guide explains how to use PACX Forms commands from PowerShell for automation and reporting.

## Quick Start

```powershell
# List forms
pacx forms list --tenant contoso.onmicrosoft.com

# Get response count
pacx forms response count --tenant contoso.onmicrosoft.com --form-id "form-id-123"

# Export responses to CSV
pacx forms responses export --tenant contoso.onmicrosoft.com --form-id "form-id-123" --output responses.csv
```

## Authentication

Set environment variables before running:

```powershell
# Client credentials (user-owned forms)
$env:MSAL_CLIENT_ID = "your-app-client-id"
$env:MSAL_CLIENT_SECRET = "your-client-secret"

# ROPC (group-owned forms — requires MFA-excluded account)
$env:MSAL_USERNAME = "service-account@contoso.com"
$env:MSAL_PASSWORD = "your-password"
```

## Scheduled Reporting

Create a scheduled task for nightly exports:

```powershell
# export_forms.ps1
$tenantId = "contoso.onmicrosoft.com"
$date = Get-Date -Format "yyyyMMdd"

pacx forms admin report --tenant $tenantId --output "forms-report-$date.xlsx"
```

Register as a scheduled task:

```powershell
$action = New-ScheduledTaskAction -Execute "pwsh" -Argument "-File C:\scripts\export_forms.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At 2am
Register-ScheduledTask -TaskName "FormsNightlyExport" -Action $action -Trigger $trigger
```

## CI/CD Integration

### GitHub Actions

```yaml
- name: Export Forms responses
  shell: pwsh
  env:
    MSAL_CLIENT_ID: ${{ secrets.MSAL_CLIENT_ID }}
    MSAL_CLIENT_SECRET: ${{ secrets.MSAL_CLIENT_SECRET }}
  run: |
    pacx forms list --tenant ${{ vars.FORMS_TENANT_ID }} --format json
    pacx forms responses export `
      --tenant ${{ vars.FORMS_TENANT_ID }} `
      --form-id ${{ vars.FORM_ID }} `
      --output responses.csv
```

### Azure DevOps

```yaml
- powershell: |
    pacx forms responses export `
      --tenant $(FORMS_TENANT_ID) `
      --form-id $(FORM_ID) `
      --output responses.csv `
      --incremental
  displayName: 'Export Forms responses'
  env:
    MSAL_CLIENT_ID: $(MSAL_CLIENT_ID)
    MSAL_CLIENT_SECRET: $(MSAL_CLIENT_SECRET)
```

## Incremental Export Pattern

Track only new responses since the last run:

```powershell
# First run (exports all)
pacx forms responses export --tenant contoso.onmicrosoft.com --form-id "f1" --output data.csv

# Subsequent runs (exports only new)
pacx forms responses export --tenant contoso.onmicrosoft.com --form-id "f1" --output data.csv --incremental
```

The incremental flag tracks the last exported response offset in a `.last` file alongside the output.

## Admin Reporting

Generate a tenant-wide form inventory:

```powershell
pacx forms admin report --tenant contoso.onmicrosoft.com --output report.xlsx
pacx forms admin report --tenant contoso.onmicrosoft.com --include-groups
```

## Full Command Reference

| Command | Description |
|---------|-------------|
| `forms list` | List all forms |
| `forms response count` | Count responses |
| `forms response get` | Get single response |
| `forms responses export` | Export responses (CSV/JSON/SQL) |
| `forms admin report` | Tenant-wide inventory |
| `forms share` | Share form with group |
| `forms ownership transfer` | Transfer form ownership |
| `forms branching export` | Export branching rules |
| `forms analytics summary` | Form analytics summary |
| `forms template list` | List templates |
| `forms template create` | Create template from form |
| `forms template share` | Share template with group |
