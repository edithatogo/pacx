# self-update

Check for and install the latest version of PACX.

## Usage

```powershell
pacx self-update
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--check` | c | bool | False | Only check for updates without installing. |
| `--version` | v | string? | False | Install a specific version instead of latest. |
| `--pre-release` | p | bool | False | Include pre-release versions. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Update/SelfUpdateCommand.cs`

