# workflow list

Returns a list of workflows (Power Automate Flow)

## Usage

```powershell
pacx workflow list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string | False | The unique name (or part of it) of the workflow to retrieve |
| `--solution` | s | string | False | The solution that contains the workflows to return. If not provided, the default solution is used. Pass * to avoid filtering by solution. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Workflows/ListCommand.cs`

