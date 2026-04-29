# dlp policy-audit

Review and report on DLP policy coverage across connectors and environments.

## Usage

```powershell
pacx dlp policy-audit
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | e | string? | False | Filter by environment ID. |
| `--connector` | c | string? | False | Filter by connector ID or name. |
| `--format` | f | string | False | Output format: table, json. |
| `--show-gaps` |  | bool | False | Show connectors without DLP policies. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Dlp/DlpPolicyAuditCommand.cs`
