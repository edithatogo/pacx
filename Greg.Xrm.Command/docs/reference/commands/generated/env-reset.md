# env reset

Reset an environment to a clean state — removes customizations, data, or both.

## Usage

```powershell
pacx env reset
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--id` |  | string | True | Environment ID to reset. |
| `--type` | t | string | False | Reset type: full (data + customizations), customizations-only, data-only. |
| `--force` | y | bool | False | Skip confirmation prompt. |
| `--wait` |  | bool | False | Wait for reset operation to complete. |
| `--format` | f | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Env/EnvLifecycleCommands.cs`

