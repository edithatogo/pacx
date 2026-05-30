# ai form-processor configure

Configure form processing model (document type, fields, tables).

## Usage

```powershell
pacx ai form-processor configure
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--model-id` | m | string | True | Form processing model ID. |
| `--doc-type` | d | string | True | Document type name. |
| `--fields` | f | string[]? | False | Comma-separated list of field names to extract. |
| `--tables` | t | string[]? | False | Comma-separated list of table names to extract. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/AiBuilder/AiFormProcessorConfigureCommand.cs`

