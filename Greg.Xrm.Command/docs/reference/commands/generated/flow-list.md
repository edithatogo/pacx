# flow list

List Power Automate cloud flows in an environment.

## Usage

```powershell
pacx flow list --environment <name>
```

```powershell
pacx flows list --environment <name>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string | True | The environment name or ID. |
| `--sharing-status` | s | string | False | Filter by sharing status: `personal`, `sharedWithMe`, or `all`. Defaults to all. |
| `--with-solutions` | w | bool | False | Include solution cloud flows. |
| `--as-admin` | a | bool | False | Run as admin against all flows in the environment. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Flows/FlowCommands.cs`
