# forms section add

Add a section to a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms section add --file <path> --title <section>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the authoring manifest JSON. |
| `--title` | t | string | True | Section title. |
| `--description` | d | string? | False | Optional section description. |
| `--order` | n | int? | False | Display order for the section. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`
