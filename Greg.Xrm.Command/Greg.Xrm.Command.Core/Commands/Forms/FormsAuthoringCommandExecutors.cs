using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	public sealed class FormsCreateCommandExecutor(IOutput output) : ICommandExecutor<FormsCreateCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsCreateCommand command, CancellationToken cancellationToken)
		{
			var store = new FormsAuthoringDocumentStore();
			var document = store.Create(command.Title, command.Description, command.Published);
			store.Save(command.OutputPath, document);

			output.WriteLine("Created Microsoft Forms authoring manifest.", ConsoleColor.Green);
			output.WriteLine($"  File: {Path.GetFullPath(command.OutputPath)}");
			output.WriteLine($"  Id: {document.Id}");
			output.WriteLine($"  Title: {document.Title}");
			output.WriteLine($"  Published: {document.Published}");
			return Task.FromResult(CommandResult.Success());
		}
	}

	public sealed class FormsUpdateCommandExecutor(IOutput output) : ICommandExecutor<FormsUpdateCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsUpdateCommand command, CancellationToken cancellationToken)
		{
			var store = new FormsAuthoringDocumentStore();
			var document = store.Load(command.FilePath);

			if (!string.IsNullOrWhiteSpace(command.Title))
			{
				document.Title = command.Title;
			}

			if (command.Description is not null)
			{
				document.Description = command.Description;
			}

			if (command.Published.HasValue)
			{
				document.Published = command.Published.Value;
			}

			store.Save(command.FilePath, document);

			output.WriteLine("Updated Microsoft Forms authoring manifest.", ConsoleColor.Green);
			output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
			output.WriteLine($"  Title: {document.Title}");
			output.WriteLine($"  Published: {document.Published}");
			return Task.FromResult(CommandResult.Success());
		}
	}

	public sealed class FormsDeleteCommandExecutor(IOutput output) : ICommandExecutor<FormsDeleteCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsDeleteCommand command, CancellationToken cancellationToken)
		{
			var fullPath = Path.GetFullPath(command.FilePath);
			if (!File.Exists(fullPath))
			{
				return Task.FromResult(CommandResult.Fail($"Forms authoring manifest not found: {fullPath}"));
			}

			if (!command.Force)
			{
				return Task.FromResult(CommandResult.Fail("Refusing to delete a forms authoring manifest without --force."));
			}

			File.Delete(fullPath);
			output.WriteLine($"Deleted Microsoft Forms authoring manifest: {fullPath}", ConsoleColor.Green);
			return Task.FromResult(CommandResult.Success());
		}
	}

	public sealed class FormsPublishCommandExecutor(IOutput output) : ICommandExecutor<FormsPublishCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsPublishCommand command, CancellationToken cancellationToken)
		{
			var store = new FormsAuthoringDocumentStore();
			var document = store.Load(command.FilePath);
			document.Published = command.Published;
			store.Save(command.FilePath, document);

			output.WriteLine(command.Published
				? "Published Microsoft Forms authoring manifest."
				: "Unpublished Microsoft Forms authoring manifest.", ConsoleColor.Green);
			output.WriteLine($"  File: {Path.GetFullPath(command.FilePath)}");
			output.WriteLine($"  Published: {document.Published}");
			return Task.FromResult(CommandResult.Success());
		}
	}
}
