# webresource watch

Watch local files and sync changes to Dataverse web resources on file save.

## Usage

```powershell
pacx webresource watch
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--config` | c | string | True | Path to the web resource mapping config file (YAML or JSON). |
| `--solution` | s | string? | False | Solution unique name to publish web resources into. |
| `--debounce` |  | int | False | Debounce delay in milliseconds before syncing after file change. Default is 500ms. |
| `--publish` | p | bool | False | Publish web resources after each upload. |
| `--poll` |  | bool | False | Use polling instead of FileSystemWatcher (fallback for network shares). |
| `--poll-interval` |  | int | False | Polling interval in milliseconds. Default is 2000ms. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/WebResources/WebResourceWatchCommand.cs`
