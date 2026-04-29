using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerAutomate;

namespace Greg.Xrm.Command.Commands.Flows
{
	internal static class FlowCommandOutput
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

	public class FlowListCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.ListFlowsAsync(command.EnvironmentName, command.SharingStatus, command.WithSolutions, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				return FlowCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list flows: {ex.Message}", ex);
			}
		}
	}

	public class FlowGetCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowGetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowGetCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.GetFlowAsync(command.EnvironmentName, command.FlowName, cancellationToken).ConfigureAwait(false);
				return FlowCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to get flow: {ex.Message}", ex);
			}
		}
	}

	public class FlowEnableCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowEnableCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowEnableCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.EnableFlowAsync(command.EnvironmentName, command.FlowName, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				return FlowCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to enable flow: {ex.Message}", ex);
			}
		}
	}

	public class FlowDisableCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowDisableCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowDisableCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.DisableFlowAsync(command.EnvironmentName, command.FlowName, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				return FlowCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to disable flow: {ex.Message}", ex);
			}
		}
	}

	public class FlowRemoveCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowRemoveCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowRemoveCommand command, CancellationToken cancellationToken)
		{
			try
			{
				await client.DeleteFlowAsync(command.EnvironmentName, command.FlowName, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Flow '{command.FlowName}' has been deleted.", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to delete flow: {ex.Message}", ex);
			}
		}
	}

	public class FlowExportCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowExportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowExportCommand command, CancellationToken cancellationToken)
		{
			try
			{
				if (string.Equals(command.Format, "json", StringComparison.OrdinalIgnoreCase))
				{
					var result = await client.ExportFlowAsJsonAsync(command.EnvironmentName, command.FlowName, cancellationToken).ConfigureAwait(false);
					var outputPath = command.OutputPath ?? $"{command.FlowName}.json";
					var json = JsonSerializer.Serialize(result.RootElement, new JsonSerializerOptions { WriteIndented = true });
					await File.WriteAllTextAsync(outputPath, json, cancellationToken).ConfigureAwait(false);
					output.WriteLine($"Flow exported to {outputPath}", ConsoleColor.Green);
				}
				else
				{
					var (content, fileName) = await client.ExportFlowAsZipAsync(command.EnvironmentName, command.FlowName, cancellationToken).ConfigureAwait(false);
					var outputPath = command.OutputPath ?? fileName;
					await File.WriteAllBytesAsync(outputPath, content, cancellationToken).ConfigureAwait(false);
					output.WriteLine($"Flow exported to {outputPath}", ConsoleColor.Green);
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to export flow: {ex.Message}", ex);
			}
		}
	}

	// Phase 2: Flow owner management executors
	public class FlowOwnerListCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowOwnerListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowOwnerListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.ListFlowPermissionsAsync(command.EnvironmentName, command.FlowName, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				return FlowCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list flow owners: {ex.Message}", ex);
			}
		}
	}

	public class FlowOwnerEnsureCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowOwnerEnsureCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowOwnerEnsureCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var putPrincipals = new[]
				{
					new
					{
						properties = new
						{
							principal = new
							{
								id = command.PrincipalId,
								type = command.PrincipalType
							},
							roleName = command.Role
						}
					}
				};

				await client.ModifyFlowPermissionsAsync(command.EnvironmentName, command.FlowName, putPrincipals, null!, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Owner added/updated for flow '{command.FlowName}'.", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to ensure flow owner: {ex.Message}", ex);
			}
		}
	}

	public class FlowOwnerRemoveCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowOwnerRemoveCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowOwnerRemoveCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var deletePrincipals = new[]
				{
					new
					{
						id = command.PrincipalId
					}
				};

				await client.ModifyFlowPermissionsAsync(command.EnvironmentName, command.FlowName, null!, deletePrincipals, command.AsAdmin, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Owner removed from flow '{command.FlowName}'.", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to remove flow owner: {ex.Message}", ex);
			}
		}
	}

	// Phase 2: Flow environment executors
	public class FlowEnvironmentListCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowEnvironmentListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowEnvironmentListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.ListEnvironmentsAsync(cancellationToken).ConfigureAwait(false);
				return FlowCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list environments: {ex.Message}", ex);
			}
		}
	}

	public class FlowEnvironmentGetCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowEnvironmentGetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowEnvironmentGetCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.GetEnvironmentAsync(command.EnvironmentName, cancellationToken).ConfigureAwait(false);
				return FlowCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to get environment: {ex.Message}", ex);
			}
		}
	}

	// Phase 2: Flow recycle bin executors
	public class FlowRecycleBinListCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowRecycleBinListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowRecycleBinListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.ListRecycleBinFlowsAsync(command.EnvironmentName, cancellationToken).ConfigureAwait(false);
				return FlowCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list recycle bin flows: {ex.Message}", ex);
			}
		}
	}

	public class FlowRecycleBinRestoreCommandExecutor(
		IPowerAutomateClient client,
		IOutput output) : ICommandExecutor<FlowRecycleBinRestoreCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FlowRecycleBinRestoreCommand command, CancellationToken cancellationToken)
		{
			try
			{
				await client.RestoreRecycleBinFlowAsync(command.EnvironmentName, command.FlowName, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Flow '{command.FlowName}' has been restored.", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to restore flow: {ex.Message}", ex);
			}
		}
	}
}
