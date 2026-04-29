# forms update

Update a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms update
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the authoring manifest JSON. |
| `--title` | t | string? | False | Updated form title. |
| `--description` | d | string? | False | Updated form description. |
| `--published` | p | bool? | False | Updated published state. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`
