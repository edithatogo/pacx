# plugin register-attributes

Scan DLLs for [CrmPluginStep] attributes and auto-register plugin steps in Dataverse.

## Usage

```powershell
pacx plugin register-attributes
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--dll` | d | string | True | Path to the plugin DLL file or directory containing DLLs. |
| `--solution` | s | string? | False | Solution unique name to register plugins into. Defaults to active solution. |
| `--publisher` | p | string | False | Publisher unique name. Defaults to 'devkit'. |
| `--publisher-name` |  | string | False | Publisher friendly name. Defaults to 'Development Toolkit'. |
| `--dry-run` |  | bool | False | Scan and show what would be registered without actually registering. |
| `--format` | f | string | False | Output format: table, json. |
| `--isolation` | None | string | False | Plugin isolation mode: None, Sandbox. Default is None. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Plugin/Registration/PluginRegisterAttributesCommand.cs`

