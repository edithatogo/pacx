# plugin step enable

Reactivates a plugin step registration that has previously been disabled.

## Usage

```powershell
pacx plugin step enable
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--class` | c | string | False | Name of the plugin type that executes when the step is triggered. |
| `--message` | m | string | False | Message that triggers the step, e.g., Create, Update, Delete. |
| `--stage` | st | Stage? | False | Pipeline stage when the step executes. Possible values: PreValidation (10), PreOperation (20), PostOperation (40) |
| `--table` | t | string | False | Primary table for the step, e.g., account, contact. Leave empty for global messages (e.g. Recalculate). |
| `--id` | id | Guid? | False | The unique identifier of the plugin step to be removed. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Plugin/Step/EnableCommand.cs`

