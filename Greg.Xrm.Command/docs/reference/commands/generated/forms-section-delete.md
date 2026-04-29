# forms section delete

Delete a section from a Microsoft Forms authoring manifest.

## Usage

```powershell
pacx forms section delete --file <path> --id <section-id> --force
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the authoring manifest JSON. |
| `--id` |  | string | True | Section identifier. |
| `--force` |  | bool | False | Delete without confirmation. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsAuthoringCommands.cs`
