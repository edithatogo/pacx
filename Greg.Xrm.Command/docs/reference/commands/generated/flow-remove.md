# flow remove

Delete a Power Automate cloud flow.

## Usage

```powershell
pacx flow remove --environment <name> --name <id>
```

```powershell
pacx flows remove --environment <name> --name <id>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string | True | The environment name or ID. |
| `--name` | n | string | True | The name (ID) of the flow to remove. |
| `--as-admin` | a | bool | False | Run as admin. |
| `--confirm` | c | bool | False | Skip the confirmation prompt. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Flows/FlowCommands.cs`
