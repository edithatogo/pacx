# ai model train

Trigger AI Builder model training from labeled data.

## Usage

```powershell
pacx ai model train
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--model-id` | m | string | True | AI Builder model ID to train. |
| `--wait` |  | bool | False | Wait for training to complete. |
| `--poll-interval` |  | int | False | Polling interval in seconds when --wait is used. |
| `--timeout` |  | int | False | Timeout in seconds when --wait is used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/AiBuilder/AiModelTrainCommand.cs`

