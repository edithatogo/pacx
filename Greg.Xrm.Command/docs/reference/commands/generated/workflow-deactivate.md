# workflow deactivate

Deactivates one or more workflows (Power Automate Flow)

## Usage

```powershell
pacx workflow deactivate
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--id` | id | Guid | False | The ID of the workflow to deactivate |
| `--name` | n | string | False | The unique name of the workflow to deactivate |
| `--solution` | s | string | False | The solution that contains the workflows to deactivate. If not provided, the default solution is used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Workflows/DeactivateCommand.cs`
