using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerPlatformAdmin;

namespace Greg.Xrm.Command.Commands.TenantSettings
{
	internal static class TenantSettingsCommandOutput
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

	public class TenantSettingsListCommandExecutor(
		IPowerPlatformAdminClient client,
		IOutput output) : ICommandExecutor<TenantSettingsListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TenantSettingsListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.GetTenantSettingsAsync(cancellationToken).ConfigureAwait(false);
				return TenantSettingsCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list tenant settings: {ex.Message}", ex);
			}
		}
	}

	public class TenantSettingsSetCommandExecutor(
		IOutput output) : ICommandExecutor<TenantSettingsSetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TenantSettingsSetCommand command, CancellationToken cancellationToken)
		{
			try
			{
				output.WriteLine("Tenant settings update is not yet implemented. Use the Power Platform admin center to configure tenant settings.");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to update tenant settings: {ex.Message}", ex);
			}
		}
	}
}
