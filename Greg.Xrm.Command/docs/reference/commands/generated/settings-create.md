# settings create

Creates a new setting in the current environment

## Usage

```powershell
pacx settings create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string? | False | The unique name of the setting in an environment. If not provided, the name is automatically generated based on the display name provided but can be changed before the setting is created. Once a setting is created, the Name can't be changed as it may be referenced in applications or code. Name has a prefix that corresponds to the solution publisher. This prefix is intended to give the setting a unique name if you want to import them into another solution or environment in the future (which would have a different prefix). |
| `--description` | d | string? | False | A description of the setting that helps others understand what the setting is used for in all user interfaces where settings are displayed. |
| `--type` | t | SettingDefinitionDataType | False | The data type of a setting controls how the setting’s value is stored. Data type can be set to Number, String, or Yes/No. Data type can't be changed after the setting is created. |
| `--defaultValue` | dv | string? | False | The default value of the setting. It specifies the setting's value that will be used unless it is overridden by a setting environment value or a setting app value. It should match the setting type. For booleans you can also provide an int value: 0 means false, any other value means true. |
| `--rel` | r | SettingDefinitionReleaseLevel | False | Release level is used to inform the framework and other consumers of the setting about the state of the feature that the setting is used with. Release level can be set to Generally available or Preview. |
| `--url` | u | string? | False | A link to documentation to help consumers of the setting understand the purpose of the setting. Will be used as a Learn more link in all user interfaces where settings are displayed. |
| `--solution` | s | string? | False | The solution where to save the created setting. If not specified, the default solution is considered. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Settings/CreateCommand.cs`
