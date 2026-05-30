# workflow run get

Get details of a specific workflow run including action outputs.

## Usage

```powershell
pacx workflow run get
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--id` |  | string | True | Workflow run ID. |
| `--workflow-id` |  | string? | False | Workflow definition ID (if run ID is not known). |
| `--actions` |  | bool | False | Include action output details. |
| `--format` | f | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Plugin.Automation/Commands/Workflow/WorkflowRunGetCommand.cs`

