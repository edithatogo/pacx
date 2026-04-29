# project init

Initializes a new PACX project

## Usage

```powershell
pacx project init
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--conn` | c | string? | False | The name of the authentication profile that allows to connect to the environment associated to the current command. It overrides the default auth profile set via `pacx auth select`. If not provided, the current default auth profile is used. |
| `--solution` | s | string? | False | The default solution to use to store the customizations for the current project. It overrides any solution set via `pacx solution setDefault`. If not provided, the default solution for the selected auth profile is used. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Projects/InitProjectCommand.cs`
