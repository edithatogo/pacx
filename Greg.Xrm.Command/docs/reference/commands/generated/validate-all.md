# validate all

Validate the generated command reference, command-page content, and catalog contracts against the live command registry.

## Usage

```powershell
pacx validate all
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--docs-index` | d | string | False | Path to the generated command reference index. |
| `--catalog-root` | c | string | False | Repository root used to validate catalog JSON files. |

## Source

`Greg.Xrm.Command.Core/Commands/Validate/ValidateAllCommand.cs`
