namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsShareCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsShareCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsShareCommand command, CancellationToken cancellationToken)
		{
			var ownerId = command.OwnerId ?? "me";
			try
			{
				await formsApiClient.ShareFormAsync(command.TenantId!, ownerId, command.OwnerType, command.FormId, command.GroupId, command.Role, cancellationToken);
				output.WriteLine($"Form <{command.FormId}> shared with group <{command.GroupId}> (role: {command.Role}).", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to share form: {ex.Message}", ex);
			}
		}
	}
}
