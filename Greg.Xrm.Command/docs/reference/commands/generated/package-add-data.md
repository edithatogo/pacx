# package add data

Add a data payload to a PACX package folder.

## Usage

```powershell
pacx package add data
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--table` | t | string? | False | Target Dataverse table name. Defaults to the source file name. |
| `--artifact` | a | string? | False | Relative artifact path inside the package. Defaults to data/<file>. |
| `--force` | f | bool | False | Overwrite an existing artifact with the same path. |
| `--mode` | m | string | False | Default deployment mode for data import. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Package/PackageCommands.cs`

