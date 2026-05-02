using System.Text.Json;

namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsBranchingExportCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsBranchingExportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsBranchingExportCommand command, CancellationToken cancellationToken)
		{
			var ownerId = command.OwnerId ?? "me";
			try
			{
				var branching = await formsApiClient.GetBranchingAsync(command.TenantId!, ownerId, command.OwnerType, command.FormId, cancellationToken);
				output.WriteLine(JsonSerializer.Serialize(branching, new JsonSerializerOptions { WriteIndented = true }));
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to export branching: {ex.Message}", ex);
			}
		}
	}
}
