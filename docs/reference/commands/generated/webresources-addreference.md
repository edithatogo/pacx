# webresources addReference

Adds an external web resource to the current webresource project **as a reference**, without copying it locally.

## Usage

```powershell
pacx webresources addReference
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--path` | p | string? | False | The folder containing the .wr.pacx project where the reference should be added. If not specified, the command will find it recoursing up from the current folder. |
| `--solution` | s | string? | False | The name of the solution that will contain the WebResources. If empty, the default solution for the current environment is used as default |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/WebResources/AddReferenceCommand.cs`

