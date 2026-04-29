# column add file

Creates a file column.

## Usage

```powershell
pacx column add file
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--maxSizeInKB` | maxKb | int? | False | For File or Image type columns indicates the maximum size in KB for the column. Do not provide a value if you want to stay with the default (32Mb for file columns, 10Mb for image columns). The value must be lower than 10485760 (1Gb) for file columns, and lower than 30720 (30Mb) for image columns. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/Create/CreateFileCommand.cs`

