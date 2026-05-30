# forms push

Push/publish a local Microsoft Forms authoring manifest to the online Microsoft Forms service.

## Usage

```powershell
pacx forms push
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--file` | f | string | True | Path to the local authoring manifest JSON file. |
| `--tenant` | t | string? | False | Tenant ID or domain. |
| `--owner` | o | string? | False | Owner user ID. |
| `--owner-type` | User | string | False | Owner type: User or Group. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsPushCommand.cs`

