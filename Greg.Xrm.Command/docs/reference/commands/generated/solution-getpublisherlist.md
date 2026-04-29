# solution getPublisherList

Lists the available publishers in current Dataverse environment. It displays unique name, friendly name and prefix.

## Usage

```powershell
pacx solution getPublisherList
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--verbose` | v | bool | False | Add optionset prefix, created on, created by and description details. |
| `--publisherBlacklist` | pb | string? | False | Schema name of publishers to exclude separated by comma |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Solution/GetPublisherListCommand.cs`
