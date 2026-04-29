# solution generateLateBoundConstants

Generates C# and/or JavaScript constants files from Dataverse metadata for a solution.

## Usage

```powershell
pacx solution generateLateBoundConstants
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--solutionName` | sn | string? | False | The unique name of the solution to extract constants for. Uses the current default solution if omitted. |
| `--outputCs` | ocs | string? | False | Output folder path for generated C# constants files. If omitted, no C# files are generated. |
| `--namespaceCs` | ncs | string? | False | C# namespace for the generated constants classes. Required when --outputCs is specified. |
| `--outputJs` | ojs | string? | False | Output folder path for generated JavaScript constants files. If omitted, no JS files are generated. |
| `--namespaceJs` | njs | string? | False | JavaScript root namespace object for the generated constants. Required when --outputJs is specified. |
| `--jsHeader` | jsh | string? | False | Header lines to prepend to each generated JavaScript file. Use \\n to separate multiple lines. |
| `--withTypes` | wt | bool | False | Include attribute type information in C# XML doc comments. |
| `--withDescriptions` | wd | bool | False | Include attribute description in C# XML doc comments. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Solution/GenerateLateBoundConstantsCommand.cs`
