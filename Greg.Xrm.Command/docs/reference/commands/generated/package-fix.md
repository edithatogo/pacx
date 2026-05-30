# package fix

Fix and normalize a PACX package manifest from folder contents.

## Usage

```powershell
pacx package fix
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--prune-missing` |  | bool | False | Remove manifest entries for artifacts that no longer exist in the folder. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Package/PackageCommands.cs`

