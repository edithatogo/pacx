# usersettings set

Sets one or more user setting properties for the specified or currently logged-in user.

## Usage

```powershell
pacx usersettings set
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--user` | u | string? | False | Domain name of the user whose settings to update (e.g. DOMAIN\\john.doe). If omitted, the current user's settings are updated. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/UserSettings/SetCommand.cs`
