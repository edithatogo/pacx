# ribbon get

Returns the full definition of a specific (application or table) ribbon (command bar).

## Usage

```powershell
pacx ribbon get
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--table` | t | string | False | The logical name of the table to get the ribbon for. If not specified, application ribbons are returned |
| `--output` | o | string | False | When specified, saves the ribbon definition in a local file. Should contain the name (absolute or relative to the current path) of the file that will contain the ribbon definition. |
| `--autorun` | r | bool | False | When specified, automatically opens the file containing the ribbon definition after export. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Ribbon/GetRibbonCommand.cs`

