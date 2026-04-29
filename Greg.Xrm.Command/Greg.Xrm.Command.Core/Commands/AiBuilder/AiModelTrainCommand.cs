using Greg.Xrm.Command.Parsing;
using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.Commands.AiBuilder
{
	[Command("ai", "model", "train", HelpText = "Trigger AI Builder model training from labeled data.")]
	public class AiModelTrainCommand
	{
		[Option("model-id", "m", Order = 1, Required = true, HelpText = "AI Builder model ID to train.")]
		public string ModelId { get; set; } = "";

		[Option("wait", Order = 2, HelpText = "Wait for training to complete.")]
		public bool Wait { get; set; }

		[Option("poll-interval", Order = 3, DefaultValue = 5, HelpText = "Polling interval in seconds when --wait is used.")]
		public int PollIntervalSeconds { get; set; } = 5;

		[Option("timeout", Order = 4, DefaultValue = 600, HelpText = "Timeout in seconds when --wait is used.")]
		public int TimeoutSeconds { get; set; } = 600;
	}
}
