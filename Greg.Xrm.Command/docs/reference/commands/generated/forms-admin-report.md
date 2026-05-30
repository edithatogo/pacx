# forms admin report

Generate a tenant-wide Microsoft Forms inventory report.

## Usage

```powershell
pacx forms admin report
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--tenant` | t | string | True | Tenant ID or domain. |
| `--output` | o | string | False | Output file path for the report. |
| `--include-groups` | g | bool | False | Include group-owned forms (requires ROPC auth). |
| `--token` |  | string? | False | OAuth2 access token. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAdminReportCommand.cs`

