# usersettings list

Lists the current values of all tracked user settings for the specified or currently logged-in user.

## Usage

```powershell
pacx usersettings list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--user` | u | string? | False | Domain name of the user whose settings to read (e.g. DOMAIN\\john.doe). If omitted, the current user's settings are shown. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/UserSettings/ListCommand.cs`

