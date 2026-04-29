# package sync

Synchronize a PACX package manifest with folder contents.

## Usage

```powershell
pacx package sync
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--prune-missing` |  | bool | False | Remove manifest entries for artifacts that no longer exist in the folder. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Package/PackageCommands.cs`

