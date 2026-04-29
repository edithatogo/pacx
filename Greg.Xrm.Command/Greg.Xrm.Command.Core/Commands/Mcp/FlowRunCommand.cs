using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[Command("mcp", "flow", "run", HelpText = "Run or open a flow entry from the PACX flow catalog.")]
	public class FlowRunCommand
	{
		[Option("catalog", "c", Order = 1, DefaultValue = "conductor/flow-mcp-catalog/flows.json", HelpText = "Path to the flow MCP catalog JSON file.")]
		public string CatalogPath { get; set; } = "conductor/flow-mcp-catalog/flows.json";

		[Option("name", "n", Order = 2, Required = true, HelpText = "Flow entry id or name.")]
		public string Name { get; set; } = string.Empty;

		[Option("open", "o", Order = 3, HelpText = "Open the flow's homepage if one is available.")]
		public bool OpenHomePage { get; set; }
	}
}
