# connection-ref map

Map connection references across solutions and environments.

## Usage

```powershell
pacx connection-ref map
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--solution` | s | string? | False | Filter by solution unique name. |
| `--connector` | c | string? | False | Filter by connector ID or name. |
| `--format` | f | string | False | Output format: table, json. |
| `--interactive` | i | bool | False | Interactive mode to update connection references. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/ConnectionRef/ConnectionRefMapCommand.cs`
