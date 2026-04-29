# flow disable

Disable/stop a Power Automate cloud flow.

## Usage

```powershell
pacx flow disable --environment <name> --name <id>
```

```powershell
pacx flows disable --environment <name> --name <id>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string | True | The environment name or ID. |
| `--name` | n | string | True | The name (ID) of the flow to disable. |
| `--as-admin` | a | bool | False | Run as admin. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Flows/FlowCommands.cs`
