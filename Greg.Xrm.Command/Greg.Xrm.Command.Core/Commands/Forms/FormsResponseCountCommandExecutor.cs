namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsResponseCountCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsResponseCountCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsResponseCountCommand command, CancellationToken cancellationToken)
		{
			var ownerId = command.OwnerId ?? "me";
			var ownerType = command.OwnerType ?? "User";

			try
			{
				var count = await formsApiClient.GetResponseCountAsync(command.TenantId, ownerId, ownerType, command.FormId, cancellationToken);
				output.WriteLine($"Form <{command.FormId}> has {count} response(s).", ConsoleColor.Cyan);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to get response count: {ex.Message}", ex);
			}
		}
	}
}
