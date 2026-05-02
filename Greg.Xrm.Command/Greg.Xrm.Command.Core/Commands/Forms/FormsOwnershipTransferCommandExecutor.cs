namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsOwnershipTransferCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsOwnershipTransferCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsOwnershipTransferCommand command, CancellationToken cancellationToken)
		{
			var ownerId = command.OwnerId ?? "me";
			try
			{
				await formsApiClient.TransferOwnershipAsync(command.TenantId!, ownerId, command.OwnerType, command.FormId, command.TargetUserPrincipalName, cancellationToken);
				output.WriteLine($"Ownership of form <{command.FormId}> transferred to <{command.TargetUserPrincipalName}>.", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to transfer ownership: {ex.Message}", ex);
			}
		}
	}
}
