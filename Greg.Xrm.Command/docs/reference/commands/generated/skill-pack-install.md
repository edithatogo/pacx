# skill pack install

Installs a skill pack from the catalog.

## Usage

```powershell
pacx skill pack install
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--catalog` | c | string | False | Path to the skill pack catalog JSON file. |
| `--dry-run` | d | bool | False | Preview what would be installed without making changes. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/SkillPack/InstallCommand.cs`

