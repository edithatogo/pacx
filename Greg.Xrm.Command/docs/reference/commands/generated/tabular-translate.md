# tabular translate

Manage and deploy multi-language translations for Power BI semantic model measures and columns.

## Usage

```powershell
pacx tabular translate
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--model` | m | string | True | Power BI dataset/model ID or name. |
| `--file` | f | string | True | Path to translation file (.json or .bim with translations). |
| `--language` | l | string | True | Target language code (e.g., en-US, fr-FR, ja-JP). |
| `--mode` | deploy | string | False | Operation mode: deploy, export, diff. |
| `--workspace` | w | string? | False | Power BI workspace ID. |
| `--format` | table | string | False | Output format: table, json. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Tabular/TabularAdvancedCommands.cs`

