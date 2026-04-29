# package add solution

Add a solution payload to a PACX package folder.

## Usage

```powershell
pacx package add solution
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--artifact` | a | string? | False | Relative artifact path inside the package. Defaults to payload/<file>. |
| `--force` | f | bool | False | Overwrite an existing artifact with the same path. |
| `--overwrite-unmanaged-customizations` |  | bool | False | Default deployment flag for solution import. |
| `--publish-workflows` |  | bool | False | Default deployment flag for solution import. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Package/PackageCommands.cs`
