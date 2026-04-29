using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.CustomApi
{
	[Command("custom-api", "list", HelpText = "List all Custom APIs in the current environment.")]
	public class CustomApiListCommand
	{
		[Option("format", "f", Order = 1, DefaultValue = "table", HelpText = "Output format: table, json.")]
		public string Format { get; set; } = "table";

		[Option("entity", "e", Order = 2, HelpText = "Filter by bound entity logical name.")]
		public string? EntityLogicalName { get; set; }
	}
}
