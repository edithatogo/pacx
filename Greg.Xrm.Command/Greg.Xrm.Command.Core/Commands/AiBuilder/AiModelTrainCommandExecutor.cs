using Greg.Xrm.Command.Services.AiBuilder;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.AiBuilder
{
	public class AiModelTrainCommandExecutor(
		IOutput output,
		IAiBuilderService aiBuilderService) : ICommandExecutor<AiModelTrainCommand>
	{
		public AiModelTrainCommandExecutor(IOutput output, IAiBuilderApiClientFactory aiBuilderApiClientFactory)
			: this(output, new AiBuilderService(aiBuilderApiClientFactory))
		{
		}

		public async Task<CommandResult> ExecuteAsync(AiModelTrainCommand command, CancellationToken cancellationToken)
		{
			try
			{
				if (command.PollIntervalSeconds <= 0)
				{
					return CommandResult.Fail("AI model train error: --poll-interval must be greater than zero.");
				}

				if (command.TimeoutSeconds <= 0)
				{
					return CommandResult.Fail("AI model train error: --timeout must be greater than zero.");
				}

				output.Write("Connecting to AI Builder...");
				output.WriteLine("Done", ConsoleColor.Green);
				output.WriteLine($"Triggering training for model: {command.ModelId}", ConsoleColor.Cyan);
				var trainResult = await aiBuilderService.TrainModelWithRetryAsync(
					command.ModelId,
					command.Wait,
					TimeSpan.FromSeconds(command.PollIntervalSeconds),
					TimeSpan.FromSeconds(command.TimeoutSeconds),
					cancellationToken).ConfigureAwait(false);
				if (!trainResult.IsSuccess)
				{
					return CommandResult.Fail(trainResult.ErrorMessage ?? "AI model train error.", trainResult.Exception);
				}

				if (command.Wait)
				{
					output.WriteLine("Model training triggered successfully and completed!", ConsoleColor.Green);
				}
				else
				{
					output.WriteLine("Model training triggered successfully!", ConsoleColor.Green);
				}

				output.WriteLine("Use 'ai model list' to check the latest training status.", ConsoleColor.Cyan);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"AI model train error: {ex.Message}", ex);
			}
		}
	}
}
