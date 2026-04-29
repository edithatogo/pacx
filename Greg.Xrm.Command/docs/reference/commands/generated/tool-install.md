# tool install

Installs or updates a PACX plugin.

## Usage

```powershell
pacx tool install
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--name` | n | string | False | To install from NuGet. The unique name of the NuGet package containing the plugin to install. |
| `--version` | v | string | False | To install from NuGet. Allows to explicit select the version of the plugin to install. |
| `--source` | s | string | False | To install from other NuGet feed. Allows to explicit select the version of the plugin to install. |
| `--personalaccesstoken` | pat | string | False | Personal Access Token to authenticate to private NuGet feeds. |
| `--file` | f | string | False | To install from a local file. The full path + file name of the nuget package containing the plugin to install. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Tool/InstallCommand.cs`
