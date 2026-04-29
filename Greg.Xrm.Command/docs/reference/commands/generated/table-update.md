# table update

Updates the metadata of an existing table.

## Usage

```powershell
pacx table update
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--plural` | p | string? | False | The collection name of the table to be created. |
| `--feedback` | fb | bool? | False | Indicates whether user can provide feedbacks to records in this table or not. It can only be set. |
| `--notes` | notes | bool? | False | Indicates whether user can add notes and attachments to the current table or not. It can only be set. |
| `--audit` | a | bool? | False | Indicates whether audit is enabled or not. |
| `--changeTracking` | ct | bool? | False | Indicates whether change tracking is enabled or not. |
| `--quickCreate` | qc | bool? | False | Indicates whether quick create form is enabled or not. |
| `--hasEmail` | email | bool? | False | Rows in this table can have email addresses (for example, info@contoso.com.). If the table didn’t have an email column, one will be added. This option can only be set. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Table/UpdateCommand.cs`
