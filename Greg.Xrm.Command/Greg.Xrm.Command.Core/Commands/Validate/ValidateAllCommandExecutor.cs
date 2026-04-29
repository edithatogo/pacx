using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Validate
{
	public class ValidateAllCommandExecutor(
		IOutput output,
		ICommandRegistry registry,
		ICommandReferenceParityValidator parityValidator,
		ICatalogContractValidator catalogValidator) : ICommandExecutor<ValidateAllCommand>
	{
		public Task<CommandResult> ExecuteAsync(ValidateAllCommand command, CancellationToken cancellationToken)
		{
			var repoRoot = Path.GetFullPath(command.CatalogRootPath);
			var result = parityValidator.Validate(registry, command.DocsIndexPath);
			var catalogResult = catalogValidator.Validate(repoRoot);

			if (result.Exception is not null)
			{
				return Task.FromResult(CommandResult.Fail($"Command reference validation error: {result.Exception.Message}", result.Exception));
			}

			if (catalogResult.Exception is not null)
			{
				return Task.FromResult(CommandResult.Fail($"Catalog validation error: {catalogResult.Exception.Message}", catalogResult.Exception));
			}

			if (result.IsValid && catalogResult.IsValid)
			{
				output.WriteLine("Command reference parity passed.", ConsoleColor.Green);
				output.WriteLine("Catalog contract validation passed.", ConsoleColor.Green);
				return Task.FromResult(CommandResult.Success());
			}

			if (result.MissingPages.Count > 0)
			{
				output.WriteLine("Missing generated docs pages:", ConsoleColor.Red);
				foreach (var page in result.MissingPages)
				{
					output.WriteLine($"- {page}", ConsoleColor.Red);
				}
			}

			if (result.ExtraPages.Count > 0)
			{
				output.WriteLine("Unexpected generated docs pages:", ConsoleColor.Yellow);
				foreach (var page in result.ExtraPages)
				{
					output.WriteLine($"- {page}", ConsoleColor.Yellow);
				}
			}

			if (result.ContentIssues.Count > 0)
			{
				output.WriteLine("Command reference content issues:", ConsoleColor.Red);
				foreach (var issue in result.ContentIssues)
				{
					output.WriteLine($"- {issue}", ConsoleColor.Red);
				}
			}

			if (!catalogResult.IsValid)
			{
				output.WriteLine("Catalog contract issues:", ConsoleColor.Red);
				foreach (var error in catalogResult.Errors)
				{
					output.WriteLine($"- {error}", ConsoleColor.Red);
				}
			}

			return Task.FromResult(CommandResult.Fail(
				$"Validation failed: {result.MissingPages.Count} missing page(s), {result.ExtraPages.Count} unexpected page(s), {result.ContentIssues.Count} content issue(s), {catalogResult.Errors.Count} catalog error(s)."));
		}
	}
}
