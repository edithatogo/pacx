# column add memo

Creates a memo column.

## Usage

```powershell
pacx column add memo
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--memoFormat` | mf | MemoFormatName1 | False | The format of the memo attribute (default: Text). |
| `--len` | l | int? | False | The maximum length for string attribute. |
| `--imeMode` | ime | ImeMode | False | Indicates the input method editor (IME) mode for the column. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/Create/CreateMemoCommand.cs`
