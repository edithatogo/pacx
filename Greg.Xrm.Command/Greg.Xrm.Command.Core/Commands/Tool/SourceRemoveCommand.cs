using System.ComponentModel.DataAnnotations;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Tool
{
	[Command("tool", "source", "remove", HelpText = "Unregisters a tool source feed.")]
	[Alias("tool", "source", "delete")]
	[Alias("tool", "source", "unregister")]
	public class SourceRemoveCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "The name of the source to remove.")]
		[Required]
		public string Name { get; set; } = string.Empty;
	}
}
