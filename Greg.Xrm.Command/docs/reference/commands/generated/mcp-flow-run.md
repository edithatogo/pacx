# mcp flow run

Run or open a flow entry from the PACX flow catalog.

## Usage

```powershell
pacx mcp flow run --name <name>
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--catalog` | c | string | False | Path to the flow MCP catalog JSON file. |
| `--name` | n | string | True | Flow entry id or name. |
| `--open` | o | bool | False | Open the flow's homepage if one is available. |
