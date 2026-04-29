# workflow get

Downloads the JSON definition of a Power Automate Flow.

## Usage

```powershell
pacx workflow get
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--id` | id | Guid? | False | The ID of the workflow to retrieve. |
| `--name` | n | string? | False | The unique name of the workflow to retrieve. |
| `--output` | o | string? | False | The output file path (default: flow_definition.json). |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Plugin.Automation/Commands/Workflow/WorkflowGetCommand.cs`
