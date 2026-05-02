namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsTemplateCreateCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsTemplateCreateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsTemplateCreateCommand command, CancellationToken cancellationToken)
		{
			try
			{
				await formsApiClient.CreateTemplateAsync("common", command.FormId, command.Scope, cancellationToken);
				output.WriteLine($"Template created from form <{command.FormId}> (scope: {command.Scope}).", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to create template: {ex.Message}", ex);
			}
		}
	}
}
