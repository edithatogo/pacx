# workflow run resubmit

Resubmit a failed or cancelled workflow run.

## Usage

```powershell
pacx workflow run resubmit
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--id` |  | string | True | Workflow run ID to resubmit. |
| `--wait` |  | bool | False | Wait for the resubmitted run to complete. |
| `--format` | f | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Plugin.Automation/Commands/Workflow/WorkflowRunResubmitCommand.cs`
