# package source browse

Browse package and source ecosystems.

## Usage

```powershell
pacx package source browse
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--catalog` | c | string | False | Path to the package source catalog JSON file. |
| `--category` |  | string? | False | Filter by category. |
| `--query` | q | string? | False | Filter by name, provider, kind, summary, or package name. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Package/PackageSourceBrowseCommand.cs`

