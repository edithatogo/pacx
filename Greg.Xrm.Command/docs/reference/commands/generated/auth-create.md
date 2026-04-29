# auth create

Create and store authentication profiles on this computer. Can be also used to update an existing authentication profile.

## Usage

```powershell
pacx auth create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--environment` | env | string? | False | If you want to connect to your environment via OAuth or Client ID / Secret, specify the environment URL here. |
| `--applicationId` | id | string? | False | The Application ID (Client ID) to authenticate with when using Client ID/Secret. |
| `--clientSecret` | s | string? | False | The Client Secret to authenticate with when using Client ID/Secret. |
| `--conn` | cs | string? | False | The [connection string](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect) that will be used to connect to the dataverse. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Auth/CreateCommand.cs`
