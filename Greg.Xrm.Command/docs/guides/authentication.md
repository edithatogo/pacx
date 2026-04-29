# Authentication

PACX uses the existing connection repository abstraction for Dataverse and Power Platform API access.

## Local Development

Use interactive authentication for local command work:

```powershell
pacx auth
```

## Automation

For CI and scheduled jobs, prefer non-interactive credentials with the least privilege required for the target environment.

Document the chosen authentication mode in the workflow or runbook that invokes PACX.
