# env create

Create a new Power Platform environment (Developer, Sandbox, Production).

## Usage

```powershell
pacx env create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string | True | Environment display name. |
| `--type` | t | string | False | Environment type: Developer, Sandbox, Production, Trial. |
| `--region` | r | string? | False | Geographic region (e.g., unitedstates, europe, asia). |
| `--currency` | USD | string | False | Base currency code. |
| `--language` | en-US | string | False | Base language code. |
| `--security-group` |  | string? | False | Azure AD security group ID for access control. |
| `--wait` |  | bool | False | Wait for environment provisioning to complete. |
| `--format` | f | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Env/EnvCommands.cs`

