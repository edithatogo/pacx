# Telemetry

PACX telemetry is opt-in and disabled by default.

Commands:

```powershell
pacx telemetry status
pacx telemetry enable --endpoint https://collector.example/v1/traces
pacx telemetry disable
```

`DOTNET_CLI_TELEMETRY_OPTOUT=1` and `PACX_TELEMETRY_OPTOUT=1` disable telemetry even when local PACX settings are enabled.

When enabled with an endpoint, PACX emits a command execution event after each command. Telemetry stays anonymized: command name, result code, duration, timestamp, and correlation ID are allowed; arguments, connection strings, tenant IDs, user IDs, URLs, and secrets are not.
