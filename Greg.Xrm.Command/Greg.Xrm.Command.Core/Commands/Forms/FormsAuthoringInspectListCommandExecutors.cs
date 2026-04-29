using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	public sealed class FormsInspectCommandExecutor(IOutput output) : ICommandExecutor<FormsInspectCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsInspectCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);

				output.WriteLine("Microsoft Forms authoring manifest.", ConsoleColor.Green);
				output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
				output.WriteLine($"  Id: {document.Id}");
				output.WriteLine($"  Title: {document.Title}");
				output.WriteLine($"  Published: {document.Published}");
				output.WriteLine($"  Questions: {document.Questions.Count}");
				output.WriteLine($"  Sections: {document.Sections.Count}");
				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to inspect forms authoring manifest <{command.FilePath}>: {ex.Message}", ex));
			}
		}
	}

	public sealed class FormsQuestionListCommandExecutor(IOutput output) : ICommandExecutor<FormsQuestionListCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsQuestionListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);

				output.WriteLine("Microsoft Forms questions.", ConsoleColor.Green);
				output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
				output.WriteLine();

				if (document.Questions.Count == 0)
				{
					output.WriteLine("  No questions defined.", ConsoleColor.DarkGray);
					return Task.FromResult(CommandResult.Success());
				}

				foreach (var question in document.Questions)
				{
					var options = question.Options.Count == 0 ? string.Empty : $" | options={string.Join(", ", question.Options)}";
					output.WriteLine($"  {question.Id} | {question.Type} | required={question.Required} | {question.Text}{options}");
				}

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to list questions in <{command.FilePath}>: {ex.Message}", ex));
			}
		}
	}

	public sealed class FormsSectionListCommandExecutor(IOutput output) : ICommandExecutor<FormsSectionListCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsSectionListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);

				output.WriteLine("Microsoft Forms sections.", ConsoleColor.Green);
				output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
				output.WriteLine();

				if (document.Sections.Count == 0)
				{
					output.WriteLine("  No sections defined.", ConsoleColor.DarkGray);
					return Task.FromResult(CommandResult.Success());
				}

				foreach (var section in document.Sections)
				{
					var description = string.IsNullOrWhiteSpace(section.Description) ? string.Empty : $" | {section.Description}";
					output.WriteLine($"  {section.Id} | order={section.Order} | {section.Title}{description}");
				}

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to list sections in <{command.FilePath}>: {ex.Message}", ex));
			}
		}
	}

	public sealed class FormsSectionDeleteCommandExecutor(IOutput output) : ICommandExecutor<FormsSectionDeleteCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsSectionDeleteCommand command, CancellationToken cancellationToken)
		{
			if (!command.Force)
			{
				return Task.FromResult(CommandResult.Fail("Refusing to delete a forms section without --force."));
			}

			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);
				store.DeleteSection(document, command.SectionId);
				store.Save(command.FilePath, document);

				output.WriteLine("Deleted Microsoft Forms section.", ConsoleColor.Green);
				output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
				output.WriteLine($"  Id: {command.SectionId}");
				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to delete section <{command.SectionId}>: {ex.Message}", ex));
			}
		}
	}
}
