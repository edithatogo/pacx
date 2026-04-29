# ai model list

List all AI Builder models with training status and accuracy.

## Usage

```powershell
pacx ai model list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--format` | f | string | False | Output format: table, json. |
| `--status` | s | string? | False | Filter by training status: NotStarted, Training, Completed, Failed. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/AiBuilder/AiModelListCommand.cs`
