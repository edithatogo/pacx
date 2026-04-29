# webresources init

Set-up the folder that will host the Dataverse WebResources

## Usage

```powershell
pacx webresources init
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--folder` | f | string? | False | The folder where the webresources will be stored. If not provided, the current folder will be used. |
| `--remote` | r | bool | False | Indicates that the folder will be set-up synchronizing the webresources from the Dataverse environment |
| `--solution` | s | string? | False | In case you want to init the folder from the contents of a solution that is not the default one for the current environment, specifies the name of the solution to take as source |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/WebResources/InitCommand.cs`

