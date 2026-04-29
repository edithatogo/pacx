using Greg.Xrm.Command.Services.AiBuilder;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.AiBuilder
{
	public class AiModelListCommandExecutor(
		IOutput output,
		IAiBuilderService aiBuilderService) : ICommandExecutor<AiModelListCommand>
	{
		public AiModelListCommandExecutor(IOutput output, IAiBuilderApiClientFactory aiBuilderApiClientFactory)
			: this(output, new AiBuilderService(aiBuilderApiClientFactory))
		{
		}

		public async Task<CommandResult> ExecuteAsync(AiModelListCommand command, CancellationToken cancellationToken)
		{
			output.Write("Connecting to the current Dataverse environment...");
			var listResult = await aiBuilderService.ListModelsAsync(cancellationToken).ConfigureAwait(false);
			if (!listResult.IsSuccess)
			{
				return CommandResult.Fail(listResult.ErrorMessage ?? "AI model list error.", listResult.Exception);
			}
			output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				var models = (listResult.Value ?? []).ToList();
				var statusFilter = NormalizeStatusFilter(command.Status);
				if (statusFilter is null && !string.IsNullOrWhiteSpace(command.Status))
				{
					return CommandResult.Fail("AI model list error: unsupported status filter. Use NotStarted, Training, Completed, or Failed.");
				}

				if (statusFilter is not null)
				{
					models = models.Where(m => NormalizeStatusCategory(m.Status) == statusFilter).ToList();
				}

				if (models.Count == 0)
				{
					output.WriteLine("No AI Builder models found.", ConsoleColor.Yellow);
					return CommandResult.Success();
				}

				output.WriteLine($"AI Builder Models ({models.Count}):", ConsoleColor.Cyan);

				if (command.Format == "json")
				{
					var json = Newtonsoft.Json.JsonConvert.SerializeObject(models, Newtonsoft.Json.Formatting.Indented);
					output.WriteLine(json);
				}
				else
				{
					output.WriteTable(models,
						() => new[] { "Name", "Status", "Created" },
						m => new[] {
							m.Name,
							m.Status,
							m.CreatedOn?.ToString("yyyy-MM-dd") ?? "-"
						}
					);
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"AI model list error: {ex.Message}", ex);
			}
		}

		internal static string? NormalizeStatusFilter(string? status) => status?.Trim().ToLowerInvariant() switch
		{
			null or "" => null,
			"notstarted" => "notstarted",
			"training" => "training",
			"completed" => "completed",
			"failed" => "failed",
			_ => null
		};

		internal static string NormalizeStatusCategory(string status) => status.Trim().ToLowerInvariant() switch
		{
			"draft" => "notstarted",
			"not started" => "notstarted",
			"training" => "training",
			"trained" => "completed",
			"training complete" => "completed",
			"compiled" => "completed",
			"ready" => "completed",
			"published" => "completed",
			"training failed" => "failed",
			"publish failed" => "failed",
			"failed" => "failed",
			_ => "unknown"
		};
	}
}
