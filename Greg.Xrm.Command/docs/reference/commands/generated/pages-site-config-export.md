# pages site config export

Export Power Pages portal configuration (auth, navigation, themes).

## Usage

```powershell
pacx pages site config export
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--site` | s | string | True | Power Pages site name or ID. |
| `--output` | o | string | True | Output directory for exported configuration. |
| `--scope` | all | string | False | Export scope: all, auth, navigation, themes, snippets. |
| `--format` | f | string | False | Export format: json, xml. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Pages/PagesSiteConfigCommands.cs`
