# rel create n1

Creates a new many-to-one relationship between two tables

## Usage

```powershell
pacx rel create n1
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--relName` | rn | string? | False | The name of the relationship. If not provided, the relationship name will be created\nconcatenating the names of the child and the parent table, with a suffix (if specified). |
| `--relNameSuffix` | suff | string? | False | The suffix to append to the relationship name. If not provided, the relationship name will contain only\nthe concatenated names of the child and the parent table. |
| `--lookupDisplayName` | ldn | string? | False | The display name of the lookup attribute. If not specified, the display name of the parent table is taken as default. |
| `--lookupSchemaName` | lsn | string? | False | The schema name of the lookup attribute. If not specified, it is inferred by the display name. |
| `--requiredLevel` | r | AttributeRequiredLevel | False | The required level of the lookup attribute. |
| `--cascadeAssign` | caass | CascadeType? | False | The behavior to apply to child records when the parent record is assigned to another owner\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeArchive` | caarc | CascadeType? | False | The behavior to apply to child records when the parent record is archived\n(not available via UI)\n(default: NoCascade) |
| `--cascadeShare` | cas | CascadeType? | False | The behavior to apply to child records when the parent record is shared\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeUnshare` | cau | CascadeType? | False | The behavior to apply to child records when the parent record is unshared\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeDelete` | cad | CascadeType? | False | The behavior to apply when the parent record is deleted\n(values: Restrict, RemoveLink)\n(default: Restrict) |
| `--cascadeMerge` | cam | CascadeType? | False | The behavior to apply to child records when the parent record is merged to another one\n(not available via UI)\n(default: NoCascade) |
| `--cascadeReparent` | car | CascadeType? | False | The behavior to apply to child records when the parent record is reparented\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--menuBehavior` | m | AssociatedMenuBehavior | False | Indicates how the child entity is displayed in the parent navbar |
| `--menuLabel` | ml | string? | False | Associated menu label. To be specified only if the menuBehavior arg is set to UseLabel |
| `--menuGroup` | mg | AssociatedMenuGroup | False | Associated menu group. To be specified only if the menuBehavior arg is set to UseLabel or UseCollectionName |
| `--menuOrder` | mo | int | False | Associated menu order. To be specified only if the menuBehavior arg is set to UseLabel or UseCollectionName |
| `--solution` | s | string? | False | The name of the unmanaged solution to which you want to add this relationship. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Relationship/CreateN1Command.cs`

