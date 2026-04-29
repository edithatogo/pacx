# tool run

Run or open a tool from the PACX tool catalog.

## Usage

```bash
pacx tool run --name <name>
```

## Options

| Name | Alias | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--catalog` | c | string | True | Path to the tool catalog JSON file. |
| `--name` | n | string | True | Tool id or name. |
| `--open` | o | bool | False | Open the tool's homepage if one is available. |
