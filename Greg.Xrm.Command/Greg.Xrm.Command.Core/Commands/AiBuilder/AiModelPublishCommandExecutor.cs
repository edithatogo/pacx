using Greg.Xrm.Command.Services.AiBuilder;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.AiBuilder
{
	public class AiModelPublishCommandExecutor(
		IOutput output,
		IAiBuilderService aiBuilderService) : ICommandExecutor<AiModelPublishCommand>
	{
		public AiModelPublishCommandExecutor(IOutput output, IAiBuilderApiClientFactory aiBuilderApiClientFactory)
			: this(output, new AiBuilderService(aiBuilderApiClientFactory))
		{
		}

		public async Task<CommandResult> ExecuteAsync(AiModelPublishCommand command, CancellationToken cancellationToken)
		{
			if (command.DryRun)
			{
				output.WriteLine("[DRY RUN] Would publish:", ConsoleColor.Yellow);
				output.WriteLine($"  Model ID: {command.ModelId}");
				return CommandResult.Success();
			}

			try
			{
				output.Write("Connecting to AI Builder...");
				output.WriteLine("Done", ConsoleColor.Green);
				output.WriteLine($"Publishing AI model: {command.ModelId}", ConsoleColor.Cyan);
				var publishResult = await aiBuilderService.PublishModelAsync(command.ModelId, cancellationToken).ConfigureAwait(false);
				if (!publishResult.IsSuccess)
				{
					return CommandResult.Fail(publishResult.ErrorMessage ?? "AI model publish error.", publishResult.Exception);
				}
				output.WriteLine("Model published successfully!", ConsoleColor.Green);
				output.WriteLine("Use 'ai model list' to confirm publication status.", ConsoleColor.Cyan);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"AI model publish error: {ex.Message}", ex);
			}
		}
	}
}
