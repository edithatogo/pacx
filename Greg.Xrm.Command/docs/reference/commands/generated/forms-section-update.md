# forms section update

Update a section in a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms section update --file <path> --id <section-id>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the authoring manifest JSON. |
| `--id` |  | string | True | Section identifier. |
| `--title` | t | string? | False | Updated section title. |
| `--description` | d | string? | False | Updated section description. |
| `--order` | n | int? | False | Updated display order. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`
