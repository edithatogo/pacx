using Greg.Xrm.Command.Parsing;
using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.Commands.AiBuilder
{
	[Command("ai", "model", "publish", HelpText = "Publish a trained AI Builder model to an environment.")]
	public class AiModelPublishCommand
	{
		[Option("model-id", "m", Order = 1, Required = true, HelpText = "AI Builder model ID to publish.")]
		public string ModelId { get; set; } = "";

		[Option("dry-run", Order = 2, HelpText = "Show what would be published without actually publishing.")]
		public bool DryRun { get; set; }
	}
}
