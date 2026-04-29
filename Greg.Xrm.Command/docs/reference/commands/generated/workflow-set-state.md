# workflow set-state

Activate or deactivate a workflow definition.

## Usage

```powershell
pacx workflow set-state
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--id` |  | string | True | Workflow definition ID. |
| `--state` | s | string | True | Target state: activated, deactivated. |
| `--format` | f | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Plugin.Automation/Commands/Workflow/WorkflowSetStateCommand.cs`
