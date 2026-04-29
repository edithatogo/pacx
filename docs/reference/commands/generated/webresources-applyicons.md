# webresources applyIcons

(Preview) Applies icons to custom tables, starting from a given solution

## Usage

```powershell
pacx webresources applyIcons
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--table-solution` | ts | string? | False | The name of the solution that contains the tables to update with icons. If not specified, the default solution is considered. |
| `--wr-solution` | wrs | string? | False | The name of the solution that contains the web resources to set. If not specified, the default solution is considered. |
| `--no-action` | nop | bool | False | If specified, the command will not perform any action, but it will show what it would do. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/WebResources/ApplyIconsCommand.cs`

