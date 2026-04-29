# unifiedrouting agentStatus

Get the agent status with the primary email/domain name provided. Optionally, you can specify a date in order to get agent status at that time. It uses the Dataverse environment selected using `pacx auth select`

## Usage

```powershell
pacx unifiedrouting agentStatus
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--dateTime` | t | string? | False | Date and time (local time) used to perform the query. Format dd/MM/yyyy HH:mm. |
| `--isUnitBased` | ub | bool | False |  |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/UnifiedRouting/GetAgentStatusCommand.cs`
