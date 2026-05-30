# forms list

List all Microsoft Forms with metadata (ID, title, status, response count, owner).

## Usage

```powershell
pacx forms list
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--tenant` | t | string | True | Tenant ID or domain (e.g., contoso.onmicrosoft.com). |
| `--owner` | o | string? | False | Owner user ID. If not provided, lists current user's forms. |
| `--owner-type` | User | string | False | Owner type: User or Group. |
| `--format` | f | string | False | Output format: table, json. |
| `--token` |  | string? | False | OAuth2 access token. Reads from MSAL cache or environment if not provided. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Forms/FormsCommands.cs`

