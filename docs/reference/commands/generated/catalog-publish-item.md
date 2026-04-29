# catalog publish-item

Publish an item to the Dataverse Catalog for Business Events.

## Usage

```powershell
pacx catalog publish-item
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--type` | t | string | False | Catalog item type: BusinessEvent, ApiDefinition. |
| `--description` |  | string? | False | Description of the catalog item. |
| `--version` |  | string? | False | Version of the catalog item. |
| `--definition` | d | string? | False | Path to the JSON/YAML definition file. |
| `--dry-run` |  | bool | False | Show what would be published without actually publishing. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Catalog/CatalogPublishCommand.cs`

