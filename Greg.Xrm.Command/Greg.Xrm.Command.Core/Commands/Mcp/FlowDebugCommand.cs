using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[Command("mcp", "flow", "debug", HelpText = "Browse debug-oriented flow entries.")]
	public class FlowDebugCommand
	{
		[Option("catalog", "c", Order = 1, DefaultValue = "conductor/flow-mcp-catalog/flows.json", HelpText = "Path to the flow MCP catalog JSON file.")]
		public string CatalogPath { get; set; } = "conductor/flow-mcp-catalog/flows.json";

		[Option("query", "q", Order = 2, HelpText = "Filter by name, provider, kind, summary, or operation.")]
		public string? Query { get; set; }
	}
}
