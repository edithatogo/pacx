# telemetry enable

Enable opt-in PACX telemetry.

## Usage

```powershell
pacx telemetry enable
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--endpoint` | e | string? | False | Optional OTLP endpoint. If omitted, PACX records consent but does not export telemetry. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Telemetry/TelemetryCommands.cs`

