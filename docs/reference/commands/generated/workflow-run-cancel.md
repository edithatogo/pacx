# workflow run cancel

Cancel a running workflow.

## Usage

```powershell
pacx workflow run cancel
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--id` |  | string | True | Workflow run ID to cancel. |
| `--force` | F | bool | False | Force cancellation without confirmation. |
| `--format` | f | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Plugin.Automation/Commands/Workflow/WorkflowRunCancelCommand.cs`

