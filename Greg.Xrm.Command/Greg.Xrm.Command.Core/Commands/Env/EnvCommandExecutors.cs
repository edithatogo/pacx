using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerPlatformAdmin;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.Env
{
	public class EnvCreateCommandExecutor(
		IOutput output,
		IPowerPlatformAdminClient adminClient) : ICommandExecutor<EnvCreateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(EnvCreateCommand command, CancellationToken cancellationToken)
		{
			output.Write($"Creating {command.Type} environment: {command.Name}...");
			try
			{
				var result = await adminClient.CreateEnvironmentAsync(command.Name, command.Type, command.Region ?? "", command.Currency ?? "USD", command.Language ?? "en", cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				if (result.RootElement.TryGetProperty("id", out var id))
					output.WriteLine($"  Environment ID: {id}");
				if (result.RootElement.TryGetProperty("properties", out var props) && props.TryGetProperty("provisioningState", out var state))
					output.WriteLine($"  State: {state}");

				if (command.Wait)
				{
					output.WriteLine("Waiting for provisioning (polling every 30s)...", ConsoleColor.Yellow);
					await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to create environment: {ex.Message}", ex);
			}
		}
	}

	public class EnvCloneCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository,
		IPowerPlatformAdminClient adminClient,
		Services.AsyncJobPoller.IAsyncJobPoller jobPoller) : ICommandExecutor<EnvCloneCommand>
	{
		public async Task<CommandResult> ExecuteAsync(EnvCloneCommand command, CancellationToken cancellationToken)
		{
			try
			{
				output.Write("Connecting to Dataverse...");
				var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				var query = new QueryExpression("environment");
				query.ColumnSet.AddColumn("environmentid");
				query.ColumnSet.AddColumn("friendlyname");
				query.Criteria.AddCondition("environmentid", ConditionOperator.Equal, command.SourceEnvironmentId);

				var results = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
				if (results.Entities.Count == 0)
				{
					return CommandResult.Fail($"Source environment '{command.SourceEnvironmentId}' not found.");
				}

				var envName = results.Entities[0].GetAttributeValue<string>("friendlyname");
				var modeMap = command.Mode switch
				{
					"schema-only" => "MinimalCopy",
					"schema-data" => "FullCopy",
					"data-only" => "DataOnly",
					_ => command.Mode
				};

				output.WriteLine($"Cloning environment: {envName} ({command.SourceEnvironmentId}) -> {command.Name}", ConsoleColor.Cyan);
				output.WriteLine($"  Mode: {command.Mode} ({modeMap})");

				if (command.Tables != null && command.Tables.Length > 0)
					output.WriteLine($"  Tables: {string.Join(", ", command.Tables)}");

				output.Write("Submitting clone request...");
				var result = await adminClient.CopyEnvironmentAsync(command.SourceEnvironmentId, command.Name, modeMap, cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				string? newEnvId = null;
				if (result.RootElement.TryGetProperty("id", out var idProp))
				{
					var idStr = idProp.GetString();
					if (!string.IsNullOrEmpty(idStr))
					{
						var parts = idStr.Split('/');
						newEnvId = parts[^1];
						output.WriteLine($"  New environment ID: {newEnvId}");
					}
				}

				if (command.Wait)
				{
					if (!string.IsNullOrEmpty(newEnvId))
					{
						output.WriteLine("Waiting for clone to complete (polling every 30s)...", ConsoleColor.Yellow);
						var success = await jobPoller.WaitForEnvironmentProvisioningAsync(
							newEnvId,
							TimeSpan.FromMinutes(60),
							TimeSpan.FromSeconds(30),
							output,
							cancellationToken).ConfigureAwait(false);

						if (success)
						{
							output.WriteLine("Clone completed successfully.", ConsoleColor.Green);
						}
						else
						{
							return CommandResult.Fail("Clone operation did not complete successfully within the timeout period.");
						}
					}
					else
					{
						output.WriteLine("Clone submitted. Monitor progress in Power Platform Admin Center.", ConsoleColor.Yellow);
					}
				}

				return CommandResult.Success();
			}
			catch (Exception ex) when (ex is not InvalidOperationException)
			{
				return CommandResult.Fail($"Error cloning environment: {ex.Message}", ex);
			}
		}
	}

	public class EnvCapacityReportCommandExecutor(
		IOutput output,
		IPowerPlatformAdminClient adminClient) : ICommandExecutor<EnvCapacityReportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(EnvCapacityReportCommand command, CancellationToken cancellationToken)
		{
			output.WriteLine("Environment Capacity Report", ConsoleColor.Cyan);
			try
			{
				JsonDocument result;
				if (!string.IsNullOrEmpty(command.EnvironmentId))
				{
					result = await adminClient.GetEnvironmentCapacityAsync(command.EnvironmentId, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					result = await adminClient.ListEnvironmentsAsync(cancellationToken).ConfigureAwait(false);
				}

				if (result.RootElement.TryGetProperty("capacity", out var cap))
				{
					var dbCap = cap.TryGetProperty("databaseCapacity", out var db) ? db.ToString() : "N/A";
					var fileCap = cap.TryGetProperty("fileCapacity", out var fc) ? fc.ToString() : "N/A";
					var logCap = cap.TryGetProperty("logCapacity", out var lc) ? lc.ToString() : "N/A";
					output.WriteLine($"  Database: {dbCap}");
					output.WriteLine($"  File: {fileCap}");
					output.WriteLine($"  Log: {logCap}");
				}
				else if (result.RootElement.TryGetProperty("value", out var list))
				{
					foreach (var env in list.EnumerateArray())
					{
						var name = env.TryGetProperty("properties", out var p) && p.TryGetProperty("displayName", out var dn) ? dn.GetString() : "?";
						var eid = env.TryGetProperty("id", out var eidProp) ? eidProp.GetString() : "?";
						output.WriteLine($"  {name}: {eid}");
					}
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to get capacity: {ex.Message}", ex);
			}
		}
	}
}
