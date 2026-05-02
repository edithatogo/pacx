namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsResponseGetCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsResponseGetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsResponseGetCommand command, CancellationToken cancellationToken)
		{
			var ownerId = command.OwnerId ?? "me";
			try
			{
				var response = await formsApiClient.GetResponseAsync(command.TenantId!, ownerId, command.OwnerType, command.FormId, command.ResponseId, cancellationToken);
				if (response == null)
				{
					output.WriteLine($"Response <{command.ResponseId}> not found.", ConsoleColor.Yellow);
					return CommandResult.Success();
				}
				output.WriteLine($"Response {response.Id} — Submitted: {response.SubmittedAt:O}");
				output.WriteLine($"Answers: {response.Answers ?? "(none)"}");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to get response: {ex.Message}", ex);
			}
		}
	}
}
