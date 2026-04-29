# custom-api list

List all Custom APIs in the current environment.

## Usage

```powershell
pacx custom-api list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--format` | f | string | False | Output format: table, json. |
| `--entity` | e | string? | False | Filter by bound entity logical name. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/CustomApi/CustomApiListCommand.cs`
