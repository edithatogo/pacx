# settings update

Updates metadata of a setting

## Usage

```powershell
pacx settings update
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--description` | d | string? | False | If specified, updates the description of the setting. |
| `--defaultValue` | dv | string? | False | If specified, updates the default value of the setting. It should match the setting type. For booleans you can also provide an int value: 0 means false, any other value means true. |
| `--change` | c | OverridableLevel? | False |  |
| `--rel` | r | SettingDefinitionReleaseLevel? | False | If specified, updates the release level of the setting. |
| `--url` | u | string? | False | If specified, updates the information URL of the setting. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Settings/UpdateCommand.cs`

