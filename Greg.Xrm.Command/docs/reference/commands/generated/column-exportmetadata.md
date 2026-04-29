# column exportMetadata

Exports the metadata definition of a given column (for documentation purpose)

## Usage

```powershell
pacx column exportMetadata
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--output` | o | string | False | The name of the folder that will contain the file with the exported metadata. (default: current folder) |
| `--run` | r | bool | False | Automatically opens the file containing the exported metadata after export. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/ExportMetadataCommand.cs`
