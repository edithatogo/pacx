# table create

Creates a new table in the dataverse environment that has previously been selected via `pacx auth select`

## Usage

```powershell
pacx table create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--plural` | p | string? | False | The collection name of the table to be created. |
| `--schemaName` | sn | string? | False | Technical schema name of the table to be created. If not specified, it is inferred from the display name. |
| `--primaryAttributeName` | pan | string? | False | The display name of the primary attribute for the table. If not specified, is used **Name**, unless it is required to be an autonumber. In that case, **Code** is used. |
| `--primaryAttributeSchemaName` | pas | string? | False | The schema name of the primary attribute for the table. If not specified, it's inferred from the primary attribute name. |
| `--primaryAttributeDescription` | pad | string? | False | A description for the primary attribute of the table. |
| `--primaryAttributeAutoNumberFormat` | paan | string? | False | If the primary attribute should be an autonumber, indicates the format for the autonumber (https://learn.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/create-auto-number-attributes?view=op-9-1#autonumberformat-options). |
| `--primaryAttributeRequiredLevel` | par | AttributeRequiredLevel? | False | Requirement level for the primary attribute. If not specified, and autonumber, it's None, otherwise it's ApplicationRequired |
| `--primaryAttributeMaxLength` | palen | int? | False | Max length of the primary attribute for the table. |
| `--description` | d | string? | False | Meaningful description of the table contents/purpose. |
| `--ownership` | o | OwnershipTypes | False | Defines if the table records can belong to an user or are organization-owned. |
| `--isActivity` | act | bool | False | Indicates whether the table is an activity or not. |
| `--offline` | off | bool | False | Indicates whether the table should be enabled for offline or not. |
| `--queue` | queue | bool | False | Indicates whether records of this table can be added to a queue or not. |
| `--feedback` | fb | bool | False | Indicates whether user can provide feedbacks to records in this table or not. |
| `--notes` | notes | bool | False | Indicates whether user can add notes and attachments to the current table or not. |
| `--audit` | a | bool | False | Indicates whether audit is enabled or not. |
| `--connection` | conn | bool | False | Indicates whether the current table can partecipate in connection relationships or not. |
| `--changeTracking` | ct | bool? | False | Indicates whether change tracking is enabled or not. |
| `--quickCreate` | qc | bool? | False | Indicates whether quick create form is enabled or not. |
| `--hasEmail` | email | bool? | False | Rows in this table can have email addresses (for example, info@contoso.com.). If the table didn’t have an email column, one will be added. This option can only be set. |
| `--solution` | s | string? | False | The name of the solution where the table will be created. If not provided, the default solution will be used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Table/CreateCommand.cs`

