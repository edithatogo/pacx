# webresources js create

(Preview) Creates a new Javascript webresource from a template

## Usage

```powershell
pacx webresources js create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--for` | f | JavascriptWebResourceType | False | Indicates if the JS web resource to create is for a form, a ribbon command, or other |
| `--table` | t | string? | False | Name of the table related to the JS. Mandatory for form JS. Optional for Ribbon JS (if not specified, is assumed as a global ribbon command). Must not be specified for Other JS. |
| `--namespace` | ns | string? | False | Namespace for the generated webresources. If not specified, the **uniquename** of the default solution publisher will be used. |
| `--solution` | s | string? | False | The name of the solution that will contain the creted WebResource. Is used to deduct the namespace, if not explicitly set. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/WebResources/JsCreateCommand.cs`
