# api ratelimit monitor

Monitor API rate limit usage and alert when approaching thresholds.

## Usage

```powershell
pacx api ratelimit monitor
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--period` | p | string | False | Time period: minute, hour, day. |
| `--threshold` | t | int | False | Alert threshold as percentage of limit (default: 80%). |
| `--format` | f | string | False | Output format: table, json. |
| `--alert` |  | bool | False | Send alert when threshold is exceeded. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Governance/ApiRateLimitMonitorCommand.cs`

