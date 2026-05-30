# alm pipeline create

Create a deployment pipeline stage from template.

## Usage

```powershell
pacx alm pipeline create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string | True | Pipeline name. |
| `--type` | t | string | False | Pipeline type: Deployment, Validation. |
| `--source-env` |  | string? | False | Source environment ID. |
| `--target-env` |  | string? | False | Target environment ID. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Alm/AlmCommands.cs`

