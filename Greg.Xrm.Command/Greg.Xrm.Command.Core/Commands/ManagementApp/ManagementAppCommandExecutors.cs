using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerPlatformAdmin;

namespace Greg.Xrm.Command.Commands.ManagementApp
{
	internal static class ManagementAppCommandOutput
	{
		public static CommandResult WriteJson(IOutput output, JsonDocument document)
		{
			using (document)
			{
				output.WriteLine(JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = true }));
			}
			return CommandResult.Success();
		}
	}

	public class ManagementAppListCommandExecutor(
		IPowerPlatformAdminClient client,
		IOutput output) : ICommandExecutor<ManagementAppListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(ManagementAppListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.ListManagementAppsAsync(cancellationToken).ConfigureAwait(false);
				return ManagementAppCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list management apps: {ex.Message}", ex);
			}
		}
	}

	public class ManagementAppAddCommandExecutor(
		IOutput output) : ICommandExecutor<ManagementAppAddCommand>
	{
		public async Task<CommandResult> ExecuteAsync(ManagementAppAddCommand command, CancellationToken cancellationToken)
		{
			try
			{
				output.WriteLine("Warning: 'managementapp add' is not yet implemented. Use the Power Platform admin center to register management applications.");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to add management app: {ex.Message}", ex);
			}
		}
	}
}
