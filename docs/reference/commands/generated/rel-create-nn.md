# rel create nn

Creates a many-to-many relationship between two tables

## Usage

```powershell
pacx rel create nn
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--schemaName` | sn | string? | False | The name of the table that manages the intersection between the two tables.\nIf not specified, is calculated concatenating the schema names of the two entities. |
| `--suffix` | sns | string? | False | The suffix to be appended to the schema name of the intersection table.\nIs considered only if --schemaName is not provided. |
| `--menuBehavior1` | m1 | AssociatedMenuBehavior | False | Indicates how the table1 is displayed in the table2 navbar |
| `--menuLabel1` | ml1 | string? | False | Indicates the menu label used to display table1 records in table2 navbar. To be specified only if the menuBehavior arg is set to UseLabel |
| `--menuGroup1` | mg1 | AssociatedMenuGroup | False | Indicates the menu group that will contain table1 label in table2 navbar. To be specified only if the menuBehavior arg is set to UseLabel or UseCollectionName |
| `--menuOrder1` | mo1 | int | False | Indicates the sequence used to display table1 label in table2 navbar. To be specified only if the menuBehavior arg is set to UseLabel or UseCollectionName |
| `--menuBehavior2` | m2 | AssociatedMenuBehavior | False | Indicates how the table2 entity is displayed in the table1 navbar |
| `--menuLabel2` | ml2 | string? | False | Indicates the menu label used to display table2 records in table1 navbar. To be specified only if the menuBehavior arg is set to UseLabel |
| `--menuGroup2` | mg2 | AssociatedMenuGroup | False | Indicates the menu group that will contain table2 label in table1 navbar. To be specified only if the menuBehavior arg is set to UseLabel or UseCollectionName |
| `--menuOrder2` | mo2 | int | False | Indicates the sequence used to display table2 label in table1 navbar. To be specified only if the menuBehavior arg is set to UseLabel or UseCollectionName |
| `--solution` | s | string? | False | The name of the unmanaged solution to which you want to add this relationship. |
| `--explicit` | e | bool | False | Indicates whether the relationship is an explicit or implicit relationship. |
| `--name` | n | string? | False | Only for explicit relationships. The display name of the intersect table. |
| `--plural` | p | string? | False | Only for explicit relationships. The plural display name of the intersect table. |
| `--description` | d | string? | False | Only for explicit relationships. The description of the intersect table. |
| `--ownership` | o | OwnershipTypes | False | Only for explicit relationships. The ownership of the intersect table |
| `--audit` | a | bool | False | Only for explicit relationships. Indicates whether audit is enabled |
| `--primaryAttributeName` | pan | string | False | Only for explicit relationships. The display name of the primary attribute of the intersect table. |
| `--primaryAttributeSchemaName` | pas | string? | False | Only for explicit relationships. The schema name of the primary attribute of the intersect table. If not specified, is deducted from the display name |
| `--primaryAttributeDescription` | pad | string? | False | Only for explicit relationships. The description of the primary attribute of the intersect table. |
| `--primaryAttributeAutoNumberFormat` | paan | string? | False | Only for explicit relationships. If not specified, it is assumed as {initial of table1}{initial of table2}-{SEQNUM:10}.\nTo generate a simple text field instead, pass \ |
| `--primaryAttributeRequiredLevel` | par | AttributeRequiredLevel | False | Only for explicit relationships. Indicates whether the primary attribute of the intersect table is required or not. |
| `--primaryAttributeMaxLength` | palen | int? | False | Only for explicit relationship. Indicates the len of the primary attribute. Is set to 20 in case of autonumber field, 100 in case of text field. |
| `--cascadeAssign1` | caass1 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table1 record is assigned to another owner\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeArchive1` | caarc1 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table1 record is archived\n(not available via UI)\n(default: NoCascade) |
| `--cascadeShare1` | cas1 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table1 record is shared\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeUnshare1` | cau1 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table1 record is unshared\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeDelete1` | cad1 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table when the table1 record is deleted\n(values: Restrict, RemoveLink)\n(default: Restrict) |
| `--cascadeMerge1` | cam1 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table1 record is merged to another one\n(not available via UI)\n(default: NoCascade) |
| `--cascadeReparent1` | car1 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table1 record is reparented\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeAssign2` | caass2 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table2 record is assigned to another owner\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeArchive2` | caarc2 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table2 record is archived\n(not available via UI)\n(default: NoCascade) |
| `--cascadeShare2` | cas2 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table2 record is shared\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeUnshare2` | cau2 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table2 record is unshared\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--cascadeDelete2` | cad2 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table2 record is deleted\n(values: Restrict, RemoveLink)\n(default: Restrict) |
| `--cascadeMerge2` | cam2 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table2 record is merged to another one\n(not available via UI)\n(default: NoCascade) |
| `--cascadeReparent2` | car2 | CascadeType? | False | Only for explicit relationship. The behavior to apply to relationship table records when the table2 record is reparented\n(values: Cascade, Active, UserOwned, NoCascade)\n(default: NoCascade) |
| `--lookupDisplayName1` | ldn1 | string? | False | Only for explicit relationship. The display name of the lookup attribute vs table1. If not specified, the display name of the parent table is taken as default. |
| `--lookupSchemaName1` | lsn1 | string? | False | Only for explicit relationship. The schema name of the lookup attribute vs table1. If not specified, the |
| `--requiredLevel1` | r1 | AttributeRequiredLevel | False | Only for explicit relationship. The required level of the lookup attribute vs table1. |
| `--lookupDisplayName2` | ldn2 | string? | False | Only for explicit relationships. The display name of the lookup attribute vs table2. If not specified, the display name of the parent table is taken as default. |
| `--lookupSchemaName2` | lsn2 | string? | False | Only for explicit relationships. The schema name of the lookup attribute vs table2. If not specified, the |
| `--requiredLevel2` | r2 | AttributeRequiredLevel | False | Only for explicit relationships. The required level of the lookup attribute vs table2. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Relationship/CreateNNCommand.cs`

