# rel poly remove

Removes a parent from an existing many-to-one **polymorphic** relationship between Dataverse tables

## Usage

```powershell
pacx rel poly remove
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--lookup` | l | string | False | The lookup column that represent the relationship to update. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Relationship/RemovePolyCommand.cs`
