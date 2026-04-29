# flow get

Get a specific Power Automate cloud flow definition.

## Usage

```powershell
pacx flow get --environment <name> --name <id>
```

```powershell
pacx flows get --environment <name> --name <id>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string | True | The environment name or ID. |
| `--name` | n | string | True | The name (ID) of the flow to get. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Flows/FlowCommands.cs`
