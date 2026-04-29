# rel poly create

Creates a new many-to-one **polymorphic** relationship between Dataverse tables

## Usage

```powershell
pacx rel poly create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--lookupSchemaName` | lsn | string? | False | The schema name of the lookup attribute. If not specified, it is inferred by the display name. |
| `--requiredLevel` | r | AttributeRequiredLevel | False | The required level of the lookup attribute. |
| `--relNameSuffix` | suff | string? | False | The suffix to append to the relationship name. If not provided, will be set equal to the display name of the lookup attribute (only letters, numbers, or underscores, lowercase). |
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

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Relationship/CreatePolyCommand.cs`
