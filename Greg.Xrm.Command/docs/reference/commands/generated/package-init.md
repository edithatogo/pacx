# package init

Scaffold a starter PACX package folder.

## Usage

```powershell
pacx package init
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--package-id` | id | string? | False | Package identifier. Defaults to the folder name. |
| `--version` | v | string | False | Package version. |
| `--name` | n | string? | False | Display name. Defaults to the package identifier. |
| `--description` | d | string? | False | Package description. |
| `--kind` | k | string | False | Package kind. Use bundle, solution, or data. |
| `--force` | f | bool | False | Overwrite existing files and folders. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Package/PackageCommands.cs`
