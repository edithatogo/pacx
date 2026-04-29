# forms publish

Publish or unpublish a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms publish
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the authoring manifest JSON. |
| `--published` | p | bool | False | Set published to true or false. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`
