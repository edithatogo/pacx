# flow recyclebin restore

Restore a soft-deleted Power Automate cloud flow from the recycle bin.

## Usage

```powershell
pacx flow recyclebin restore --environment <name> --name <id>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string | True | The environment name or ID. |
| `--name` | n | string | True | The name (ID) of the flow to restore. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Flows/FlowCommands.cs`
