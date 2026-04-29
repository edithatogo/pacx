# package release

Stage a PACX release folder with archive, manifest, notes, and checksums.

## Usage

```powershell
pacx package release
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--version` | v | string? | False | Override the package version for the staged release folder. |
| `--force` | f | bool | False | Overwrite existing release folders and files. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Package/PackageCommands.cs`
