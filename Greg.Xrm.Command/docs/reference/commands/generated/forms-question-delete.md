# forms question delete

Delete a question from a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms question delete --file <path> --id <question-id> --force
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the authoring manifest JSON. |
| `--id` |  | string | True | Question identifier. |
| `--force` |  | bool | False | Delete without confirmation. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`
