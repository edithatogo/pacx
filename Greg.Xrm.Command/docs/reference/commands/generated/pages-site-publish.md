# pages site publish

Publish a Power Pages site from local source.

## Usage

```powershell
pacx pages site publish
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--source` | s | string | True | Path to the Power Pages site source directory. |
| `--website` | w | string? | False | Website record GUID or unique name. |
| `--dry-run` |  | bool | False | Show what would be published without actually publishing. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Pages/PagesCommands.cs`
