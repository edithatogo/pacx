# forms delete

Delete a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms delete
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the authoring manifest JSON. |
| `--force` |  | bool | False | Delete without an interactive confirmation prompt. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`
