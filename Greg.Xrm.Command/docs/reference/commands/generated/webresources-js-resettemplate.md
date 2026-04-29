# webresources js resetTemplate

Allows to restore the default templates used for JS WebResources to the one shipped by default by PACX.

## Usage

```powershell
pacx webresources js resetTemplate
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--forTable` | ft | bool | False | To be used in conjunction with `--type Ribbon`, indicates if the template is for a table command bar. If not specified, is assumed as a global command bar. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/WebResources/JsResetTemplateCommand.cs`
