# solution list

Lists all solutions in the current environment.

## Usage

```powershell
pacx solution list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--type` | t | SolutionType | False | Type of solutions to list (Managed, Unmanaged, Both). |
| `--hidden` | hid | bool | False | Shows all solutions, including the ones that are not visible via make.powerapps.com UI. |
| `--format` | f | OutputFormat | False | Chooses how to generate the output. |
| `--orderby` | o | OutputOrder | False | Order of the output. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Solution/ListCommand.cs`
