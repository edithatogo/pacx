namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsTemplateListCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsTemplateListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsTemplateListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var templates = await formsApiClient.ListTemplatesAsync("common", cancellationToken);
				if (templates.Count == 0)
				{
					output.WriteLine("No templates found.", ConsoleColor.Yellow);
					return CommandResult.Success();
				}
				output.WriteTable(templates,
					() => ["ID", "Title", "Status"],
					t => [t.Id, t.Title, t.Status]);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list templates: {ex.Message}", ex);
			}
		}
	}
}
