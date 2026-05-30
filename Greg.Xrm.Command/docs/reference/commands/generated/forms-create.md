# forms create

Create a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--output` | o | string | True | Output path for the authoring manifest JSON. |
| `--title` | t | string | True | Form title. |
| `--description` | d | string? | False | Optional form description. |
| `--published` | p | bool | False | Set the initial published state. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`

