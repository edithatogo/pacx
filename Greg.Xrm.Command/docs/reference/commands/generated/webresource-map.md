# webresource map

Map local files to Dataverse web resources with flexible file-to-resource mapping.

## Usage

```powershell
pacx webresource map
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--config` | c | string | True | Path to the web resource mapping config file (YAML or JSON). |
| `--solution` | s | string? | False | Solution unique name to publish web resources into. |
| `--dry-run` |  | bool | False | Show what would be mapped/updated without actually uploading. |
| `--publish` | p | bool | False | Publish web resources after uploading. |
| `--force` | f | bool | False | Force overwrite of existing web resources without prompting. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/WebResources/WebResourceMapCommand.cs`
