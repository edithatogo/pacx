# solution create

Creates a new unmanaged solution in the current Dataverse environment,\nalso creating the publisher, if needed.

## Usage

```powershell
pacx solution create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--uniqueName` | un | string? | False | The unique name of the solution to create. If not specified, is deducted from the display name |
| `--publisherPrefix` | pp | string? | False | The customization prefix of the publisher to create. If not specified, is deducted from the unique name. |
| `--publisherUniqueName` | pun | string? | False | The unique name of the publisher to create. If not specified, is deducted from the friendly name or customization prefix |
| `--publisherFriendlyName` | puf | string? | False | The friendly name of the publisher to create. If not specified, is deducted from the unique name or customization prefix |
| `--publisherOptionSetPrefix` | pop | int? | False | The option set prefix of the publisher to create (5 digit number). |
| `--applicationRibbons` | ar | bool | False | Once the solution has been created, adds the application ribbons |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Solution/CreateCommand.cs`

