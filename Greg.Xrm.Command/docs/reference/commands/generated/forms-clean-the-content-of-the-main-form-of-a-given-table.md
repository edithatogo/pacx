# forms clean  the content of the main form of a given table

\

## Usage

```powershell
pacx forms clean  the content of the main form of a given table
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--form` | f | string | False | The name of the form to initialize. It is required only if the table has more than one Main form. |
| `--solution` | s | string? | False | The name of the solution that contains the table. If not provided, the default solution will be used |
| `--output` | out | string? | False | If specified, the command will export the original and updated version of the temporary solution in the specified folder. The folder must exist. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/CleanCommand.cs`
