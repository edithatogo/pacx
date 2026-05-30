# pa app list

List Power Apps in an environment.

## Usage

```powershell
pacx pa app list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string? | False | The environment name or ID. Required when using --as-admin. |
| `--as-admin` | a | bool | False | Run as admin to list apps across the environment. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/PowerApps/PaCommands.cs`

