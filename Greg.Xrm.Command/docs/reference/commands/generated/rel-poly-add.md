# rel poly add

Adds a new parent to an existing many-to-one **polymorphic** relationship between Dataverse tables

## Usage

```powershell
pacx rel poly add
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--lookup` | l | string | False | The lookup column that represent the relationship to update. |
| `--relNameSuffix` | suff | string? | False | The suffix to append to the relationship name. If not provided, will be set equal to the display name of the lookup attribute (only letters, numbers, or underscores, lowercase). |
| `--cascadeAssign` | caass | CascadeType? | False | The behavior to apply to child records when the parent record is assigned to another owner\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeArchive` | caarc | CascadeType? | False | The behavior to apply to child records when the parent record is archived\n(not available via UI)\n(default: NoCascade) |
| `--cascadeShare` | cas | CascadeType? | False | The behavior to apply to child records when the parent record is shared\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeUnshare` | cau | CascadeType? | False | The behavior to apply to child records when the parent record is unshared\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeDelete` | cad | CascadeType? | False | The behavior to apply when the parent record is deleted\n(values: Restrict, RemoveLink)\n(default: Restrict) |
| `--cascadeMerge` | cam | CascadeType? | False | The behavior to apply to child records when the parent record is merged to another one\n(not available via UI)\n(default: NoCascade) |
| `--cascadeReparent` | car | CascadeType? | False | The behavior to apply to child records when the parent record is reparented\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--solution` | s | string? | False | The name of the unmanaged solution that contains the table (used to get the publisher prefix). If not provided, the default table for the environment will be used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Relationship/AddPolyCommand.cs`
