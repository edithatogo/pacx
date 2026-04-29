# webresources push

Push web resources from a local folder to the target environment

## Usage

```powershell
pacx webresources push
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--path` | p | string? | False | The path to the folder containing the webresources to deploy, or to a specific webresource file to deploy. If not provided, the current folder will be used. |
| `--no-publish` | np | bool | False | Indicates that the webresources should be published after the deployment |
| `--no-action` | nop | bool | False | If specified, the command will not perform any action, but it will show what it would do. |
| `--solution` | s | string? | False | The name of the solution that will contain the WebResources. If empty, the default solution for the current environment is used as default |
| `--reference` | r | string? | False | Indicates if it should also push the webresources set as project reference. See wiki for additional details. |
| `--verbose` | v | bool | False | If specified, the command will output more details about the operations performed |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/WebResources/PushCommand.cs`
