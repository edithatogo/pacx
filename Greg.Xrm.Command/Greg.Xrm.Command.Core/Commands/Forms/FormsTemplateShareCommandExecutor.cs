namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsTemplateShareCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsTemplateShareCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsTemplateShareCommand command, CancellationToken cancellationToken)
		{
			try
			{
				await formsApiClient.ShareTemplateAsync("common", command.TemplateId, command.GroupId, cancellationToken);
				output.WriteLine($"Template <{command.TemplateId}> shared with group <{command.GroupId}>.", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to share template: {ex.Message}", ex);
			}
		}
	}
}
