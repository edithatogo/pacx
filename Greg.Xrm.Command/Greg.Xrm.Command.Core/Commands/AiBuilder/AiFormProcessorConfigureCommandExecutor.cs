using Greg.Xrm.Command.Services.AiBuilder;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.AiBuilder
{
	public class AiFormProcessorConfigureCommandExecutor(
		IOutput output,
		IAiBuilderService aiBuilderService) : ICommandExecutor<AiFormProcessorConfigureCommand>
	{
		public AiFormProcessorConfigureCommandExecutor(IOutput output, IAiBuilderApiClientFactory aiBuilderApiClientFactory)
			: this(output, new AiBuilderService(aiBuilderApiClientFactory))
		{
		}

		public async Task<CommandResult> ExecuteAsync(AiFormProcessorConfigureCommand command, CancellationToken cancellationToken)
		{
			var validationError = Validate(command);
			if (validationError != null)
			{
				return CommandResult.Fail(validationError);
			}

			output.Write("Connecting to AI Builder...");
			output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				output.WriteLine($"Configuring Form Processor:", ConsoleColor.Cyan);
				output.WriteLine($"  Model ID: {command.ModelId}");
				output.WriteLine($"  Document Type: {command.DocumentType}");

				var configureResult = await aiBuilderService.ConfigureFormProcessorAsync(
					command.ModelId,
					command.DocumentType,
					command.Fields,
					command.Tables,
					cancellationToken).ConfigureAwait(false);
				if (!configureResult.IsSuccess)
				{
					return CommandResult.Fail(configureResult.ErrorMessage ?? "Form processor configuration error.", configureResult.Exception);
				}

				output.WriteLine("Form processor configured successfully!", ConsoleColor.Green);
				output.WriteLine("Use 'ai model list' to verify the model status before publishing.", ConsoleColor.Cyan);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Form processor configuration error: {ex.Message}", ex);
			}
		}

		private static string? Validate(AiFormProcessorConfigureCommand command)
		{
			if (string.IsNullOrWhiteSpace(command.ModelId))
			{
				return "Form processor configuration error: --model-id is required.";
			}

			if (string.IsNullOrWhiteSpace(command.DocumentType))
			{
				return "Form processor configuration error: --doc-type is required.";
			}

			if (ContainsBlank(command.Fields))
			{
				return "Form processor configuration error: --fields must not contain blank field names.";
			}

			if (ContainsBlank(command.Tables))
			{
				return "Form processor configuration error: --tables must not contain blank table names.";
			}

			return null;
		}

		private static bool ContainsBlank(string[]? values)
		{
			return values?.Any(string.IsNullOrWhiteSpace) == true;
		}
	}
}
