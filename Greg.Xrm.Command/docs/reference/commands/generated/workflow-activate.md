# workflow activate

Activates one or more workflows (Power Automate Flow)

## Usage

```powershell
pacx workflow activate
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--id` | id | Guid | False | The ID of the workflow to activate |
| `--name` | n | string | False | The unique name of the workflow to activate |
| `--solution` | s | string | False | The solution that contains the workflows to activate. If not provided, the default solution is used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Workflows/ActivateCommand.cs`

