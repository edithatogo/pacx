# security audit-user

Full privilege audit for a user — what can they actually do?

## Usage

```powershell
pacx security audit-user
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--user` | u | string | True | User email, domain\\username, or systemuserid. |
| `--format` | f | string | False | Output format: table, json. |
| `--detail` | d | string | False | Detail level: summary (default), full (all privileges). |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Security/SecurityCommands.cs`
