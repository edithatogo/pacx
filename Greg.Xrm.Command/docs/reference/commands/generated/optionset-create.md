# optionset create

Create a new global optionset

## Usage

```powershell
pacx optionset create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--schemaName` | sn | string? | False | The schema name of the global optionset.\nIf not specified, is deducted from the display name. |
| `--colors` | c | string? | False | The list of colors for each option, in exadecimal format, as a single string separated by comma (,). |
| `--solution` | s | string? | False | The name of the unmanaged solution to which you want to add this attribute. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/OptionSet/CreateCommand.cs`
