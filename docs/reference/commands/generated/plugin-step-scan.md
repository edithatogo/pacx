# plugin step-scan

Validate plugin step definitions in compiled DLLs without deploying to Dataverse.

## Usage

```powershell
pacx plugin step-scan
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--dll` | d | string | True | Path to the plugin DLL file or directory containing DLLs. |
| `--format` | f | string | False | Output format: table, json. |
| `--strict` |  | bool | False | Fail if any validation warnings are found. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Plugin/Step/PluginStepScanCommand.cs`

