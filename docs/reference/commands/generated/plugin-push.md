# plugin push

Push plugins packages or plugin assemblies into a Dataverse instance.

## Usage

```powershell
pacx plugin push
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--solution` | s | string? | False | The name of the solution where package/assembly must be added (in case of creation). If not provided, the default solution will be used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Plugin/PushCommand.cs`

