# plugin step register

Registers a plugin step.

## Usage

```powershell
pacx plugin step register
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--table` | t | string | False | Primary table for the step, e.g., account, contact. Leave empty for global messages (e.g. Recalculate). |
| `--stage` | st | Stage | False | Pipeline stage when the step executes. Possible values: PreValidation (10), PreOperation (20), PostOperation (40) |
| `--filteringAttributes` | fa | string | False | Comma (,) separated list of columns acting as filtering attributes for the message |
| `--order` | o | int | False | Execution order of the step. Lower numbers execute first. |
| `--preImage` | preim | bool | False | Indicates whether a PreImage must be registered on the step. |
| `--preImageName` | preimn | string? | False | Name of the PreImage. If not specified, will be set automatically as <table name>_pre |
| `--postImage` | postim | bool | False | Indicates whether a PostImage must be registered on the step. |
| `--postImageName` | postimn | string? | False | Name of the PreImage. If not specified, will be set automatically as <table name>_pre |
| `--description` | d | string? | False | Description of the plugin step. |
| `--unsecureConfig` | uc | string? | False | Unsecure configuration string for the plugin step. |
| `--secureConfig` | sc | string? | False | Secure configuration string for the plugin step. |
| `--mode` | md | Mode | False | Execution mode of the step. Possible values: Sync, Async |
| `--deployment` | dep | Deployment | False | Deployment type |
| `--name` | n | string? | False | Name of the plugin step. If not specified, will be defined automatically by the platform |
| `--user` | u | Guid? | False | Specify this argument if you want to run the step in a specific user context. Provide the User's GUID. Leave empty to run the plugin in the calling user context. |
| `--solution` | s | string? | False | The name of the solution where step must be added (in case of creation). If not provided, the default solution will be used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Plugin/Step/RegisterCommand.cs`

