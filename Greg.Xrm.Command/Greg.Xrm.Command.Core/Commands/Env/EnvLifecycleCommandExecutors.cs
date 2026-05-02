using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerPlatformAdmin;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.Env
{
	public class EnvResetCommandExecutor(
		IOutput output,
		IPowerPlatformAdminClient adminClient) : ICommandExecutor<EnvResetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(EnvResetCommand command, CancellationToken cancellationToken)
		{
			try
			{
				if (!command.Force)
				{
					output.WriteLine($"WARNING: This will reset environment '{command.EnvironmentId}' with type '{command.ResetType}'.", ConsoleColor.Red);
					output.WriteLine("This operation cannot be undone. Use --force to skip this warning.", ConsoleColor.Red);
					return CommandResult.Fail("Reset aborted. Use --force to confirm.");
				}

				output.Write($"Resetting environment {command.EnvironmentId}...");
				var result = await adminClient.ResetEnvironmentAsync(command.EnvironmentId, command.ResetType, cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				if (result.RootElement.TryGetProperty("properties", out var props) && props.TryGetProperty("provisioningState", out var state))
					output.WriteLine($"  State: {state}");

				if (command.Wait)
				{
					output.WriteLine("Reset initiated. Monitor progress in Power Platform Admin Center.");
				}

				return CommandResult.Success();
			}
			catch (Exception ex) when (ex is not InvalidOperationException)
			{
				return CommandResult.Fail($"Error resetting environment: {ex.Message}", ex);
			}
		}
	}

	public class EnvBackupCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository,
		IPowerPlatformAdminClient adminClient,
		Services.AsyncJobPoller.IAsyncJobPoller jobPoller) : ICommandExecutor<EnvBackupCommand>
	{
		public async Task<CommandResult> ExecuteAsync(EnvBackupCommand command, CancellationToken cancellationToken)
		{
			try
			{
				output.Write("Connecting to Dataverse...");
				var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				var query = new QueryExpression("environment");
				query.ColumnSet.AddColumn("environmentid");
				query.ColumnSet.AddColumn("friendlyname");
				query.Criteria.AddCondition("environmentid", ConditionOperator.Equal, command.EnvironmentId);

				var results = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
				if (results.Entities.Count == 0)
				{
					return CommandResult.Fail($"Environment '{command.EnvironmentId}' not found.");
				}

				var envName = results.Entities[0].GetAttributeValue<string>("friendlyname");
				var backupName = command.BackupName ?? $"backup-{DateTime.UtcNow:yyyy-MM-dd-HHmmss}";
				var mode = command.IncludeData ? "schema+data" : "schema-only";

				output.WriteLine($"Backing up environment: {envName} ({command.EnvironmentId})", ConsoleColor.Cyan);
				output.WriteLine($"  Backup name: {backupName}");
				output.WriteLine($"  Mode: {mode}");

				output.Write("Submitting backup request...");
				var result = await adminClient.BackupEnvironmentAsync(command.EnvironmentId, backupName, cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				if (command.Wait)
				{
					output.WriteLine("Waiting for backup to complete (polling every 30s)...", ConsoleColor.Yellow);
					var success = await jobPoller.WaitForEnvironmentProvisioningAsync(
						command.EnvironmentId,
						TimeSpan.FromMinutes(30),
						TimeSpan.FromSeconds(30),
						output,
						cancellationToken).ConfigureAwait(false);

					if (success)
					{
						output.WriteLine("Backup completed successfully.", ConsoleColor.Green);
					}
					else
					{
						return CommandResult.Fail("Backup operation did not complete successfully within the timeout period.");
					}
				}

				return CommandResult.Success();
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				return CommandResult.Fail($"Error backing up environment: {ex.Message}", ex);
			}
			catch (Exception ex) when (ex is not InvalidOperationException)
			{
				return CommandResult.Fail($"Error backing up environment: {ex.Message}", ex);
			}
		}
	}

	public class EnvRestoreCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository,
		IPowerPlatformAdminClient adminClient,
		Services.AsyncJobPoller.IAsyncJobPoller jobPoller) : ICommandExecutor<EnvRestoreCommand>
	{
		public async Task<CommandResult> ExecuteAsync(EnvRestoreCommand command, CancellationToken cancellationToken)
		{
			try
			{
				if (!command.Force)
				{
					output.WriteLine($"WARNING: This will restore environment '{command.EnvironmentId}' from backup '{command.BackupId}'.", ConsoleColor.Red);
					output.WriteLine("This operation cannot be undone. Use --force to skip this warning.", ConsoleColor.Red);
					return CommandResult.Fail("Restore aborted. Use --force to confirm.");
				}

				output.Write("Connecting to Dataverse...");
				var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				var query = new QueryExpression("environment");
				query.ColumnSet.AddColumn("environmentid");
				query.ColumnSet.AddColumn("friendlyname");
				query.Criteria.AddCondition("environmentid", ConditionOperator.Equal, command.EnvironmentId);

				var results = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
				if (results.Entities.Count == 0)
				{
					return CommandResult.Fail($"Environment '{command.EnvironmentId}' not found.");
				}

				var envName = results.Entities[0].GetAttributeValue<string>("friendlyname");
				output.WriteLine($"Restoring environment: {envName} ({command.EnvironmentId})", ConsoleColor.Cyan);
				output.WriteLine($"  From backup: {command.BackupId}");

				output.Write("Submitting restore request...");
				var result = await adminClient.RestoreEnvironmentAsync(command.EnvironmentId, command.BackupId, cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				if (command.Wait)
				{
					output.WriteLine("Waiting for restore to complete (polling every 30s)...", ConsoleColor.Yellow);
					var success = await jobPoller.WaitForEnvironmentProvisioningAsync(
						command.EnvironmentId,
						TimeSpan.FromMinutes(60),
						TimeSpan.FromSeconds(30),
						output,
						cancellationToken).ConfigureAwait(false);

					if (success)
					{
						output.WriteLine("Restore completed successfully.", ConsoleColor.Green);
					}
					else
					{
						return CommandResult.Fail("Restore operation did not complete successfully within the timeout period.");
					}
				}

				return CommandResult.Success();
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				return CommandResult.Fail($"Error restoring environment: {ex.Message}", ex);
			}
			catch (Exception ex) when (ex is not InvalidOperationException)
			{
				return CommandResult.Fail($"Error restoring environment: {ex.Message}", ex);
			}
		}
	}
}
