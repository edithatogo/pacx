# Flow Studio Capability Surfaces

PACX now includes a small Flow Studio-style catalog for browsing, inspecting, and monitoring flow-oriented operations.

## Surfaces

- `pacx mcp flow browse`
- `pacx mcp flow inspect --name <name>`
- `pacx mcp flow monitor`
- `pacx mcp flow run --name <name>`
- `pacx mcp flow debug`
- `pacx mcp flow govern`

## Default Catalog

The default catalog lives at `conductor/flow-mcp-catalog/flows.json`.

It is intended to act as a discovery layer for Flow Studio-style capabilities, not as a mirror or integration with upstream products.

## Practical Use

Use the browse command when you want to scan the catalog by category or query. Use inspect when you already know the flow entry you want. Use monitor when you want the flows that expose monitoring-related operations.

## Next Steps

The current surfaces cover catalog discovery, inspection, execution-style detail, and monitoring. Future work can extend the same model with richer diagnostics views or deeper workflow-specific helpers.
