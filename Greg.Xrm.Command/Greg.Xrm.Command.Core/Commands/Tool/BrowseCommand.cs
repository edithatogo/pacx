using System.ComponentModel.DataAnnotations;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Tool
{
	[Command("tool", "browse", HelpText = "Browse the PACX tool catalog.")]
	public class BrowseCommand
	{
		[Option("catalog", "c", Order = 1, DefaultValue = "conductor/tool-catalog/tools.json", HelpText = "Path to the tool catalog JSON file.")]
		public string CatalogPath { get; set; } = "conductor/tool-catalog/tools.json";

		[Option("category", Order = 2, HelpText = "Filter by category.")]
		public string? Category { get; set; }

		[Option("query", "q", Order = 3, HelpText = "Filter by name, provider, kind, or summary.")]
		public string? Query { get; set; }
	}
}
