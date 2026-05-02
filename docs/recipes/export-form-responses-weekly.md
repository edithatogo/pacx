# Export Form Responses Weekly

Use a scheduled workflow to export Microsoft Forms response data into a controlled storage location.

## Prerequisites

- PACX CLI installed (`dotnet tool install -g Greg.Xrm.Command`)
- Dataverse environment with Microsoft Forms integration configured
- Connection profile with automation credentials

## Scheduled Export Steps

### 1. Configure authentication for the scheduled job

```powershell
pacx auth create --name scheduler-env --url https://myorg.crm.dynamics.com --client-id ${{ secrets.SCHEDULER_CLIENT_ID }} --client-secret ${{ secrets.SCHEDULER_CLIENT_SECRET }} --tenant ${{ secrets.TENANT_ID }}
```

### 2. Query available forms and identify the target

```powershell
pacx forms list --env scheduler-env
```

Select the form identifier from the output.

### 3. Run the export command

```powershell
pacx data export --env scheduler-env --entity "msfp_surveyresponse" --output ./exports --format csv --filter "modifiedon ge @(Get-Date (Get-Date).AddDays(-7).ToString('yyyy-MM-dd'))"
```

### 4. Archive the output and notify owners on failure

```powershell
pacx settings set --env scheduler-env --name "pacx/weekly-forms-export/last-run" --value (Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ")
```

## Expected Output

```
> pacx forms list --env scheduler-env
┌──────────────────────────────────┬──────────────────────────────────────┐
│ Form Name                        │ Form ID                             │
├──────────────────────────────────┼──────────────────────────────────────┤
│ Customer Satisfaction Survey     │ 00000000-0000-0000-0000-000000000001 │
└──────────────────────────────────┴──────────────────────────────────────┘

> pacx data export --env scheduler-env --entity "msfp_surveyresponse"
Exporting msfp_surveyresponse...
  143 records exported to ./exports/msfp_surveyresponse_2026-05-02.csv
```

## CI/CD Integration (Scheduled Action)

```yaml
schedule:
  - cron: "0 6 * * 1"  # Every Monday at 06:00

steps:
  - run: |
      pacx auth create --name scheduler-env --url ${{ vars.ENV_URL }} --client-id ${{ secrets.CLIENT_ID }} --client-secret ${{ secrets.CLIENT_SECRET }} --tenant ${{ secrets.TENANT_ID }}
      pacx data export --env scheduler-env --entity "msfp_surveyresponse" --output ./exports
      pacx settings set --env scheduler-env --name "pacx/weekly-forms-export/last-run" --value "$(date -u +%Y-%m-%dT%H:%M:%SZ)"
```
