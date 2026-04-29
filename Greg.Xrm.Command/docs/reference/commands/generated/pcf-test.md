# pcf test

Run PCF component tests in headless mode for CI/CD.

## Usage

```powershell
pacx pcf test
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--path` | p | string? | False | Path to the PCF project directory. Defaults to current directory. |
| `--browser` | b | string | False | Browser mode: headless, chrome, firefox, edge. |
| `--reporter` | r | string | False | Test reporter: spec, json, junit. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Pcf/PcfCommands.cs`
