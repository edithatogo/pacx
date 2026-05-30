# power-fx format

Format a Power Fx expression or expression file.

## Usage

```powershell
pacx power-fx format
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--expression` | e | string? | False | Power Fx expression to format. |
| `--file` | f | string? | False | Text file containing a Power Fx expression. |
| `--in-place` |  | bool | False | Write formatted output back to the file. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/PowerFx/PowerFxCommands.cs`

