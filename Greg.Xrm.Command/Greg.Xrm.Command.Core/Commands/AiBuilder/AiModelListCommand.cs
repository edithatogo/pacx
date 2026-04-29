using Greg.Xrm.Command.Parsing;
using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.Commands.AiBuilder
{
	[Command("ai", "model", "list", HelpText = "List all AI Builder models with training status and accuracy.")]
	public class AiModelListCommand
	{
		[Option("format", "f", Order = 1, DefaultValue = "table", HelpText = "Output format: table, json.")]
		public string Format { get; set; } = "table";

		[Option("status", "s", Order = 2, HelpText = "Filter by training status: NotStarted, Training, Completed, Failed.")]
		public string? Status { get; set; }
	}
}
