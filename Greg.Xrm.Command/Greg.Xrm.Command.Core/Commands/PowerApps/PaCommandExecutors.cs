using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerApps;

namespace Greg.Xrm.Command.Commands.PowerApps
{
	internal static class PaCommandOutput
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

	public class PaAppListCommandExecutor(
		IPowerAppsClient client,
		IOutput output) : ICommandExecutor<PaAppListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PaAppListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.ListAppsAsync(command.EnvironmentName, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				return PaCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list Power Apps: {ex.Message}", ex);
			}
		}
	}

	public class PaAppGetCommandExecutor(
		IPowerAppsClient client,
		IOutput output) : ICommandExecutor<PaAppGetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PaAppGetCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.GetAppAsync(command.AppName, command.EnvironmentName, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				return PaCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to get Power App: {ex.Message}", ex);
			}
		}
	}

	public class PaAppRemoveCommandExecutor(
		IPowerAppsClient client,
		IOutput output) : ICommandExecutor<PaAppRemoveCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PaAppRemoveCommand command, CancellationToken cancellationToken)
		{
			try
			{
				if (!command.Confirm)
				{
					output.WriteLine($"Are you sure you want to delete the Power App '{command.AppName}'? Use --confirm to skip this prompt.");
					return CommandResult.Success();
				}

				await client.DeleteAppAsync(command.AppName, command.EnvironmentName, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Power App '{command.AppName}' deleted successfully.");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to delete Power App: {ex.Message}", ex);
			}
		}
	}

	public class PaAppExportCommandExecutor(
		IPowerAppsClient client,
		IOutput output) : ICommandExecutor<PaAppExportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PaAppExportCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var (content, fileName) = await client.ExportAppAsync(command.AppName, command.EnvironmentName, cancellationToken).ConfigureAwait(false);

				var outputPath = command.OutputPath;
				if (string.IsNullOrWhiteSpace(outputPath))
				{
					outputPath = Path.Combine(Environment.CurrentDirectory, fileName);
				}

				await File.WriteAllBytesAsync(outputPath, content, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Power App exported to: {outputPath}");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to export Power App: {ex.Message}", ex);
			}
		}
	}

	public class PaAppConsentSetCommandExecutor(
		IPowerAppsClient client,
		IOutput output) : ICommandExecutor<PaAppConsentSetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PaAppConsentSetCommand command, CancellationToken cancellationToken)
		{
			try
			{
				await client.SetAppConsentAsync(command.AppName, command.EnvironmentName, command.BypassConsent, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Consent bypass set to {command.BypassConsent} for Power App '{command.AppName}'.");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to set consent: {ex.Message}", ex);
			}
		}
	}

	public class PaAppOwnerSetCommandExecutor(
		IPowerAppsClient client,
		IOutput output) : ICommandExecutor<PaAppOwnerSetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PaAppOwnerSetCommand command, CancellationToken cancellationToken)
		{
			try
			{
				await client.SetAppOwnerAsync(command.AppName, command.EnvironmentName, command.NewOwnerId, command.RoleForOldOwner, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Ownership of Power App '{command.AppName}' transferred to '{command.NewOwnerId}'.");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to set app owner: {ex.Message}", ex);
			}
		}
	}

	public class PaAppPermissionListCommandExecutor(
		IPowerAppsClient client,
		IOutput output) : ICommandExecutor<PaAppPermissionListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PaAppPermissionListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.ListAppPermissionsAsync(command.AppName, command.EnvironmentName, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				return PaCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list permissions: {ex.Message}", ex);
			}
		}
	}

	public class PaAppPermissionAddCommandExecutor(
		IPowerAppsClient client,
		IOutput output) : ICommandExecutor<PaAppPermissionAddCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PaAppPermissionAddCommand command, CancellationToken cancellationToken)
		{
			try
			{
				await client.AddAppPermissionAsync(command.AppName, command.EnvironmentName, command.AsAdmin, command.PrincipalId, command.PrincipalType, command.RoleName, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Permission added for principal '{command.PrincipalId}' on Power App '{command.AppName}'.");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to add permission: {ex.Message}", ex);
			}
		}
	}

	public class PaAppPermissionRemoveCommandExecutor(
		IPowerAppsClient client,
		IOutput output) : ICommandExecutor<PaAppPermissionRemoveCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PaAppPermissionRemoveCommand command, CancellationToken cancellationToken)
		{
			try
			{
				await client.RemoveAppPermissionAsync(command.AppName, command.EnvironmentName, command.AsAdmin, command.PrincipalId, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Permission removed for principal '{command.PrincipalId}' on Power App '{command.AppName}'.");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to remove permission: {ex.Message}", ex);
			}
		}
	}
}
