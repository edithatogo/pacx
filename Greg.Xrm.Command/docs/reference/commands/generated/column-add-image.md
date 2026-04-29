# column add image

Creates an image column.

## Usage

```powershell
pacx column add image
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--maxSizeInKB` | maxKb | int? | False | For File or Image type columns indicates the maximum size in KB for the column. Do not provide a value if you want to stay with the default (32Mb for file columns, 10Mb for image columns). The value must be lower than 10485760 (1Gb) for file columns, and lower than 30720 (30Mb) for image columns. |
| `--storeOnlyThumbnailImage` | thumb | bool? | False | For Image type columns indicates if the column stores only thumbnail-sized images. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/Create/CreateImageCommand.cs`
