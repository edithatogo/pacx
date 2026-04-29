# flow owner ensure

Add or update a permission (owner) on a Power Automate cloud flow.

## Usage

```powershell
pacx flow owner ensure --environment <name> --name <id> --principal-id <objectId>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string | True | The environment name or ID. |
| `--name` | n | string | True | The name (ID) of the flow. |
| `--principal-id` | p | string | True | The AAD object ID of the user or group. |
| `--principal-type` | t | string | False | Principal type: `User` or `Group`. Default is `User`. |
| `--role` | r | string | False | Role to assign: `CanView` or `CanEdit`. Default is `CanEdit`. |
| `--as-admin` | a | bool | False | Run as admin. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Flows/FlowCommands.cs`
