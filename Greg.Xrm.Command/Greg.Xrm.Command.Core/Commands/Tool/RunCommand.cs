using System.ComponentModel.DataAnnotations;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Tool
{
	[Command("tool", "run", HelpText = "Run or open a tool from the PACX tool catalog.")]
	public class RunCommand
	{
		[Option("catalog", "c", Order = 1, DefaultValue = "conductor/tool-catalog/tools.json", HelpText = "Path to the tool catalog JSON file.")]
		public string CatalogPath { get; set; } = "conductor/tool-catalog/tools.json";

		[Option("name", "n", Order = 2, Required = true, HelpText = "Tool id or name.")]
		[Required]
		public string Name { get; set; } = string.Empty;

		[Option("open", "o", Order = 3, HelpText = "Open the tool's homepage if one is available.")]
		public bool OpenHomePage { get; set; }
	}
}
