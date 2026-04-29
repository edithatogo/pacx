# forms question add

Add a question to a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms question add --file <path> --text <question>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the authoring manifest JSON. |
| `--text` | t | string | True | Question text. |
| `--type` |  | string | False | Question type. |
| `--required` | r | bool | False | Mark the question as required. |
| `--options` | o | string? | False | Comma-separated answer options for choice questions. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`
