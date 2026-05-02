namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsCloseCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsCloseCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsCloseCommand command, CancellationToken cancellationToken)
		{
			output.WriteLine("Closing a form via API is not supported by the Forms Office API.", ConsoleColor.Yellow);
			output.WriteLine("To close a form manually, open it in the Forms web UI and select 'Stop accepting responses'.", ConsoleColor.Yellow);
			return CommandResult.Success();
		}
	}

	public class FormsReopenCommandExecutor(
		IOutput output) : ICommandExecutor<FormsReopenCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsReopenCommand command, CancellationToken cancellationToken)
		{
			output.WriteLine("Reopening a form via API is not supported by the Forms Office API.", ConsoleColor.Yellow);
			output.WriteLine("To reopen a form manually, open it in the Forms web UI and select 'Start accepting responses again'.", ConsoleColor.Yellow);
			return Task.FromResult(CommandResult.Success());
		}
	}

	public class FormsDuplicateCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsDuplicateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsDuplicateCommand command, CancellationToken cancellationToken)
		{
			output.WriteLine("Duplicating a form via API is not supported by the Forms Office API.", ConsoleColor.Yellow);
			output.WriteLine("To duplicate a form, use the Forms web UI or manually create a copy from the form definition.", ConsoleColor.Yellow);
			return CommandResult.Success();
		}
	}
}
