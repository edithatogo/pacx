using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	public sealed class FormsQuestionAddCommandExecutor(IOutput output) : ICommandExecutor<FormsQuestionAddCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsQuestionAddCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);
				var question = store.AddQuestion(document, command.QuestionText, command.Type, command.Required, FormsAuthoringValueParser.ParseOptions(command.OptionsCsv));
				store.Save(command.FilePath, document);

				output.WriteLine("Added Microsoft Forms question.", ConsoleColor.Green);
				output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
				output.WriteLine($"  Id: {question.Id}");
				output.WriteLine($"  Type: {question.Type}");
				output.WriteLine($"  Required: {question.Required}");
				output.WriteLine($"  Options: {question.Options.Count}");
				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to add question to <{command.FilePath}>: {ex.Message}", ex));
			}
		}
	}

	public sealed class FormsQuestionUpdateCommandExecutor(IOutput output) : ICommandExecutor<FormsQuestionUpdateCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsQuestionUpdateCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);
				var question = store.UpdateQuestion(
					document,
					command.QuestionId,
					command.QuestionText,
					command.Type,
					command.Required,
					command.OptionsCsv is null ? null : FormsAuthoringValueParser.ParseOptions(command.OptionsCsv));
				store.Save(command.FilePath, document);

				output.WriteLine("Updated Microsoft Forms question.", ConsoleColor.Green);
				output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
				output.WriteLine($"  Id: {question.Id}");
				output.WriteLine($"  Type: {question.Type}");
				output.WriteLine($"  Required: {question.Required}");
				output.WriteLine($"  Options: {question.Options.Count}");
				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to update question <{command.QuestionId}>: {ex.Message}", ex));
			}
		}
	}

	public sealed class FormsQuestionDeleteCommandExecutor(IOutput output) : ICommandExecutor<FormsQuestionDeleteCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsQuestionDeleteCommand command, CancellationToken cancellationToken)
		{
			if (!command.Force)
			{
				return Task.FromResult(CommandResult.Fail("Refusing to delete a forms question without --force."));
			}

			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);
				store.DeleteQuestion(document, command.QuestionId);
				store.Save(command.FilePath, document);

				output.WriteLine("Deleted Microsoft Forms question.", ConsoleColor.Green);
				output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
				output.WriteLine($"  Id: {command.QuestionId}");
				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to delete question <{command.QuestionId}>: {ex.Message}", ex));
			}
		}
	}

	public sealed class FormsSectionAddCommandExecutor(IOutput output) : ICommandExecutor<FormsSectionAddCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsSectionAddCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);
				var section = store.AddSection(document, command.Title, command.Description, command.Order);
				store.Save(command.FilePath, document);

				output.WriteLine("Added Microsoft Forms section.", ConsoleColor.Green);
				output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
				output.WriteLine($"  Id: {section.Id}");
				output.WriteLine($"  Title: {section.Title}");
				output.WriteLine($"  Order: {section.Order}");
				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to add section to <{command.FilePath}>: {ex.Message}", ex));
			}
		}
	}

	public sealed class FormsSectionUpdateCommandExecutor(IOutput output) : ICommandExecutor<FormsSectionUpdateCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsSectionUpdateCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);
				var section = store.UpdateSection(document, command.SectionId, command.Title, command.Description, command.Order);
				store.Save(command.FilePath, document);

				output.WriteLine("Updated Microsoft Forms section.", ConsoleColor.Green);
				output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
				output.WriteLine($"  Id: {section.Id}");
				output.WriteLine($"  Title: {section.Title}");
				output.WriteLine($"  Order: {section.Order}");
				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to update section <{command.SectionId}>: {ex.Message}", ex));
			}
		}
	}

	internal static class FormsAuthoringValueParser
	{
		public static string[] ParseOptions(string? csv)
		{
			if (string.IsNullOrWhiteSpace(csv))
			{
				return [];
			}

			return csv
				.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Where(option => !string.IsNullOrWhiteSpace(option))
				.ToArray();
		}
	}
}
