# flow owner list

List owners/permissions of a Power Automate cloud flow.

## Usage

```powershell
pacx flow owner list --environment <name> --name <id>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string | True | The environment name or ID. |
| `--name` | n | string | True | The name (ID) of the flow. |
| `--as-admin` | a | bool | False | Run as admin. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Flows/FlowCommands.cs`
