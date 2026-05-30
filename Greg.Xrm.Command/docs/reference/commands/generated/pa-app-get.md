# pa app get

Get details of a specific Power App.

## Usage

```powershell
pacx pa app get
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string | True | The name (ID) of the Power App. |
| `--environment` | env | string? | False | The environment name or ID. Required when using --as-admin. |
| `--as-admin` | a | bool | False | Run as admin. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/PowerApps/PaCommands.cs`

