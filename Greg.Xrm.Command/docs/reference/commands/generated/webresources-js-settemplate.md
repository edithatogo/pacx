# webresources js setTemplate

Allows to override the default template used when creating custom JS WebResources.

## Usage

```powershell
pacx webresources js setTemplate
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--type` | t | JavascriptWebResourceType | False | The type of the template to override. |
| `--forTable` | ft | bool | False | To be used in conjunction with `--type Ribbon`, indicates if the template is for a table command bar. If not specified, is assumed as a global command bar. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/WebResources/JsSetTemplateCommand.cs`
