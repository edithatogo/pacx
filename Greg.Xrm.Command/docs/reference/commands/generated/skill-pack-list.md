# skill pack list

Lists available skill packs from the catalog.

## Usage

```powershell
pacx skill pack list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--catalog` | c | string | False | Path to the skill pack catalog JSON file. |
| `--query` | q | string? | False | Filter packs by name, description, or tags. |
| `--tag` |  | string? | False | Filter packs by tag. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/SkillPack/ListCommand.cs`

