# pcf version bump

Semantic version management for PCF components with changelog.

## Usage

```powershell
pacx pcf version bump
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--path` | p | string? | False | Path to the PCF project directory. |
| `--type` | t | string | True | Version bump type: major, minor, patch. |
| `--message` | m | string? | False | Changelog message for the version bump. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Pcf/PcfCommands.cs`

