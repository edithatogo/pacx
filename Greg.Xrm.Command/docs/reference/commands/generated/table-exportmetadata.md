# table exportMetadata

Exports the metadata definition of a given table (for documentation purpose)

## Usage

```powershell
pacx table exportMetadata
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--what` | w | EntityFilters | False | The level of details to export. |
| `--output` | o | string | False | The name of the folder that will contain the file with the exported metadata. (default: current folder) |
| `--run` | r | bool | False | Automatically opens the file containing the exported metadata after export. |
| `--format` | f | ExportMetadataFormat | False | The format of the exported metadata file. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Table/ExportMetadata/ExportMetadataCommand.cs`
