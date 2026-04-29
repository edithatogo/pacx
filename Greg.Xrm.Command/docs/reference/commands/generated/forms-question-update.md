# forms question update

Update a question in a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms question update --file <path> --id <question-id>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the authoring manifest JSON. |
| `--id` |  | string | True | Question identifier. |
| `--text` | t | string? | False | Updated question text. |
| `--type` |  | string? | False | Updated question type. |
| `--required` | r | bool? | False | Updated required flag. |
| `--options` | o | string? | False | Comma-separated answer options for choice questions. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`
