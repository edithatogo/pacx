# table print

Returns the Mermaid (https://mermaid.js.org/) classDiagram representation of the set of tables contained in a given solution

## Usage

```powershell
pacx table print
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--solution` | s | string? | False | The name of the solution containing the entities to export. If not specified, the default solution is used instead. |
| `--include-security-tables` | ist | bool | False | If false, the security tables (organization, systemuser, businessunit, team, position, fieldsecurityprofile) are not taken consideration in the export. |
| `--skip-missing-tables` | skip | bool | False | If true, the command will not fail if some tables are missing in the solution. The missing tables will be skipped. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Table/TablePrintMermaidCommand.cs`

