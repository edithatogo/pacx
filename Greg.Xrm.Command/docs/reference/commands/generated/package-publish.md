# package publish

Publish a PACX package archive and release manifest to a destination folder.

## Usage

```powershell
pacx package publish
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--version` | v | string? | False | Override the package version for the published archive and release manifest. |
| `--force` | f | bool | False | Overwrite existing published artifacts in the destination folder. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Package/PackageCommands.cs`
