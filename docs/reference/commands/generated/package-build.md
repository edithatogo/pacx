# package build

Build a PACX-native package archive from a package folder.

## Usage

```powershell
pacx package build
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--output` | o | string? | False | Output .pacx file. If omitted, PACX chooses a name based on the manifest. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Package/PackageCommands.cs`

