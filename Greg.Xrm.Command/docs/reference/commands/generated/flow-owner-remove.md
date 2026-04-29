# flow owner remove

Remove a permission (owner) from a Power Automate cloud flow.

## Usage

```powershell
pacx flow owner remove --environment <name> --name <id> --principal-id <objectId>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string | True | The environment name or ID. |
| `--name` | n | string | True | The name (ID) of the flow. |
| `--principal-id` | p | string | True | The AAD object ID of the user or group to remove. |
| `--as-admin` | a | bool | False | Run as admin. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Flows/FlowCommands.cs`
