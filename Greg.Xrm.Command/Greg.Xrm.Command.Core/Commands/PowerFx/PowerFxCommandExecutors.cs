using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerFx;

namespace Greg.Xrm.Command.Commands.PowerFx
{
	public class PowerFxValidateCommandExecutor(IPowerFxValidationService validationService, IOutput output) : ICommandExecutor<PowerFxValidateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PowerFxValidateCommand command, CancellationToken cancellationToken)
		{
			var expressions = await ReadExpressionsAsync(command.Expression, command.FilePath, cancellationToken).ConfigureAwait(false);
			if (expressions.Count == 0)
			{
				return CommandResult.Fail("Provide --expression or --file.");
			}

			var failures = 0;
			foreach (var item in expressions)
			{
				var result = validationService.ValidateExpression(item.Value);
				output.WriteLine($"{item.Key}: {(result.IsValid ? "valid" : "invalid")}", result.IsValid ? ConsoleColor.Green : ConsoleColor.Red);
				foreach (var warning in result.Warnings)
				{
					output.WriteLine($"  WARNING: {warning}", ConsoleColor.Yellow);
				}

				foreach (var error in result.Errors)
				{
					output.WriteLine($"  ERROR: {error}", ConsoleColor.Red);
				}

				if (!result.IsValid)
				{
					failures++;
				}
			}

			if (!string.IsNullOrWhiteSpace(command.TableName))
			{
				output.WriteLine($"Table binding requested for '{command.TableName}'. Binding-aware validation will run when table metadata binding is configured.", ConsoleColor.Yellow);
			}

			return failures == 0 ? CommandResult.Success() : CommandResult.Fail($"{failures} Power Fx expression(s) failed validation.");
		}

		private static async Task<IReadOnlyDictionary<string, string>> ReadExpressionsAsync(string? expression, string? filePath, CancellationToken cancellationToken)
		{
			if (!string.IsNullOrWhiteSpace(expression))
			{
				return new Dictionary<string, string> { ["expression"] = expression };
			}

			if (string.IsNullOrWhiteSpace(filePath))
			{
				return new Dictionary<string, string>();
			}

			var content = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
			if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
			{
				using var document = JsonDocument.Parse(content);
				if (document.RootElement.ValueKind == JsonValueKind.Object)
				{
					return document.RootElement.EnumerateObject()
						.Where(property => property.Value.ValueKind == JsonValueKind.String)
						.ToDictionary(property => property.Name, property => property.Value.GetString() ?? string.Empty, StringComparer.OrdinalIgnoreCase);
				}
			}

			return new Dictionary<string, string> { [Path.GetFileName(filePath)] = content };
		}
	}

	public class PowerFxFormatCommandExecutor(IPowerFxValidationService validationService, IOutput output) : ICommandExecutor<PowerFxFormatCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PowerFxFormatCommand command, CancellationToken cancellationToken)
		{
			var expression = command.Expression;
			if (string.IsNullOrWhiteSpace(expression) && !string.IsNullOrWhiteSpace(command.FilePath))
			{
				expression = await File.ReadAllTextAsync(command.FilePath, cancellationToken).ConfigureAwait(false);
			}

			if (string.IsNullOrWhiteSpace(expression))
			{
				return CommandResult.Fail("Provide --expression or --file.");
			}

			var formatted = validationService.FormatExpression(expression);
			if (command.InPlace)
			{
				if (string.IsNullOrWhiteSpace(command.FilePath))
				{
					return CommandResult.Fail("--in-place requires --file.");
				}

				await File.WriteAllTextAsync(command.FilePath, formatted, cancellationToken).ConfigureAwait(false);
			}

			output.WriteLine(formatted);
			return CommandResult.Success();
		}
	}

	public class PowerFxReplCommandExecutor(IOutput output) : ICommandExecutor<PowerFxReplCommand>
	{
		public Task<CommandResult> ExecuteAsync(PowerFxReplCommand command, CancellationToken cancellationToken)
		{
			output.WriteLine("Power Fx REPL bootstrap is available through the validation service; interactive evaluation requires a bound Dataverse context.", ConsoleColor.Yellow);
			return Task.FromResult(CommandResult.Success());
		}
	}
}
