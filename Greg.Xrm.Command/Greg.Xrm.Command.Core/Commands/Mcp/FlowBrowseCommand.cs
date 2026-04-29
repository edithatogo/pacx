using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Mcp
{
		[Command("mcp", "flow", "browse", HelpText = "Browse Flow Studio-style flow operations.")]
	public class FlowBrowseCommand
	{
		[Option("catalog", "c", Order = 1, DefaultValue = "conductor/flow-mcp-catalog/flows.json", HelpText = "Path to the flow MCP catalog JSON file.")]
		public string CatalogPath { get; set; } = "conductor/flow-mcp-catalog/flows.json";

		[Option("category", Order = 2, HelpText = "Filter by category.")]
		public string? Category { get; set; }

		[Option("query", "q", Order = 3, HelpText = "Filter by name, provider, kind, summary, or operation.")]
		public string? Query { get; set; }
	}
}
