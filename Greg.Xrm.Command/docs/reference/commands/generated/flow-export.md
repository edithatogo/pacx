# flow export

Export a Power Automate cloud flow as a package or ARM template.

## Usage

```powershell
pacx flow export --environment <name> --name <id>
```

```powershell
pacx flows export --environment <name> --name <id>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string | True | The environment name or ID. |
| `--name` | n | string | True | The name (ID) of the flow to export. |
| `--format` | f | string | False | Export format: `zip` (package) or `json` (ARM template). Default is `zip`. |
| `--output` | o | string | False | Output file path. Defaults to current directory with the flow display name. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Flows/FlowCommands.cs`
