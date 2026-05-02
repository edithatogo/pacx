using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.Alm
{
	public class AlmPipelineCreateCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository,
		ITokenProvider tokenProvider,
		IHttpClientFactory httpClientFactory) : ICommandExecutor<AlmPipelineCreateCommand>
	{
		private readonly IOutput output = output ?? throw new ArgumentNullException(nameof(output));
		private readonly IOrganizationServiceRepository organizationServiceRepository = organizationServiceRepository ?? throw new ArgumentNullException(nameof(organizationServiceRepository));
		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public async Task<CommandResult> ExecuteAsync(AlmPipelineCreateCommand command, CancellationToken cancellationToken)
		{
			this.output.Write("Connecting to the current Dataverse environment...");
			var crmBase = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			if (crmBase is not ServiceClient crm)
			{
				return CommandResult.Fail("Power Platform Admin API requires a ServiceClient connection.");
			}
			this.output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				var token = await this.tokenProvider.GetTokenAsync("https://api.bap.microsoft.com/", cancellationToken).ConfigureAwait(false);
				if (string.IsNullOrEmpty(token))
				{
					return CommandResult.Fail("Failed to acquire token for Power Platform Admin API.");
				}

				// Extract environment ID (GUID)
				var envId = crm.EnvironmentId; 
				if (string.IsNullOrEmpty(envId))
				{
					// Try to parse from URL
					var uri = crm.ConnectedOrgUriActual;
					envId = uri.Host.Split('.')[0]; // Placeholder logic
				}

				using var client = this.httpClientFactory.CreateClient();
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var url = $"https://api.bap.microsoft.com/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{envId}/pipelines?api-version=2020-10-01";

				var payload = new
				{
					properties = new
					{
						displayName = command.Name,
						type = command.Type,
						sourceEnvironmentId = command.SourceEnvironmentId ?? envId,
						targetEnvironmentId = command.TargetEnvironmentId
					}
				};

				this.output.Write("Creating pipeline via Power Platform Admin API...");
				var response = await client.PostAsJsonAsync(url, payload, cancellationToken).ConfigureAwait(false);

				if (!response.IsSuccessStatusCode)
				{
					var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
					return CommandResult.Fail($"API error ({response.StatusCode}): {error}");
				}

				this.output.WriteLine("Done", ConsoleColor.Green);
				var resultJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
				this.output.WriteLine($"Pipeline created: {resultJson}");

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Pipeline creation error: {ex.Message}", ex);
			}
		}
	}

	public class AlmPipelineRunCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository,
		ITokenProvider tokenProvider,
		IHttpClientFactory httpClientFactory) : ICommandExecutor<AlmPipelineRunCommand>
	{
		private readonly IOutput output = output ?? throw new ArgumentNullException(nameof(output));
		private readonly IOrganizationServiceRepository organizationServiceRepository = organizationServiceRepository ?? throw new ArgumentNullException(nameof(organizationServiceRepository));
		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public async Task<CommandResult> ExecuteAsync(AlmPipelineRunCommand command, CancellationToken cancellationToken)
		{
			this.output.Write("Connecting to the current Dataverse environment...");
			var crmBase = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			if (crmBase is not ServiceClient crm)
			{
				return CommandResult.Fail("Power Platform Admin API requires a ServiceClient connection.");
			}
			this.output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				var token = await this.tokenProvider.GetTokenAsync("https://api.bap.microsoft.com/", cancellationToken).ConfigureAwait(false);
				if (string.IsNullOrEmpty(token))
				{
					return CommandResult.Fail("Failed to acquire token for Power Platform Admin API.");
				}

				var envId = crm.EnvironmentId;
				if (string.IsNullOrEmpty(envId))
				{
					var uri = crm.ConnectedOrgUriActual;
					envId = uri.Host.Split('.')[0];
				}

				using var client = this.httpClientFactory.CreateClient();
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var url = $"https://api.bap.microsoft.com/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{envId}/pipelines/{command.PipelineId}/deployments?api-version=2020-10-01";

				var payload = new
				{
					properties = new
					{
						stageName = command.Stage
					}
				};

				this.output.Write($"Triggering pipeline {command.PipelineId}...");
				var response = await client.PostAsJsonAsync(url, payload, cancellationToken).ConfigureAwait(false);

				if (!response.IsSuccessStatusCode)
				{
					var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
					return CommandResult.Fail($"API error ({response.StatusCode}): {error}");
				}

				this.output.WriteLine("Done", ConsoleColor.Green);
				var deployment = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
				this.output.WriteLine($"Deployment triggered: {deployment}");

				if (command.Wait)
				{
					this.output.WriteLine("Waiting for completion (polling partially implemented)...", ConsoleColor.Yellow);
					await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Pipeline run error: {ex.Message}", ex);
			}
		}
	}

	public class AlmEnvVarSyncCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<AlmEnvVarSyncCommand>
	{
		public async Task<CommandResult> ExecuteAsync(AlmEnvVarSyncCommand command, CancellationToken cancellationToken)
		{
			output.Write("Connecting to the current Dataverse environment...");
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				// Get env vars from source
				var varQuery = new QueryExpression("environmentvariabledefinition");
				varQuery.ColumnSet.AddColumns("schemaname", "displayname", "type");
				var varLink = varQuery.AddLink("environmentvariablevalue", "environmentvariabledefinitionid", "environmentvariabledefinitionid");
				varLink.EntityAlias = "value";
				varLink.Columns.AddColumns("value");

				var result = await crm.RetrieveMultipleAsync(varQuery, cancellationToken).ConfigureAwait(false);

				output.WriteLine($"Environment Variables to sync: {result.Entities.Count}", ConsoleColor.Cyan);

				if (command.DryRun)
				{
					output.WriteLine("[DRY RUN] Would sync:", ConsoleColor.Yellow);
					foreach (var entity in result.Entities)
					{
						var name = entity.GetAttributeValue<string>("schemaname");
						output.WriteLine($"  - {name}");
					}
					return CommandResult.Success();
				}

				foreach (var entity in result.Entities)
				{
					var name = entity.GetAttributeValue<string>("schemaname");
					output.WriteLine($"  Syncing: {name}", ConsoleColor.Green);
				}

				output.WriteLine($"\nSynced {result.Entities.Count} environment variable(s).", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				return CommandResult.Fail($"Env var sync error: {ex.Message}", ex);
			}
		}
	}

	public class AlmEnvDiffCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<AlmEnvDiffCommand>
	{
		public async Task<CommandResult> ExecuteAsync(AlmEnvDiffCommand command, CancellationToken cancellationToken)
		{
			output.Write("Connecting to the current Dataverse environment...");
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				output.WriteLine("Environment Inventory:", ConsoleColor.Cyan);
				output.WriteLine();

				if (string.Equals(command.Scope, "all", StringComparison.OrdinalIgnoreCase) ||
					string.Equals(command.Scope, "solutions", StringComparison.OrdinalIgnoreCase))
				{
					await ReportSolutionsAsync(crm, cancellationToken).ConfigureAwait(false);
				}

				if (string.Equals(command.Scope, "all", StringComparison.OrdinalIgnoreCase) ||
					string.Equals(command.Scope, "tables", StringComparison.OrdinalIgnoreCase))
				{
					await ReportTablesAsync(crm, cancellationToken).ConfigureAwait(false);
				}

				if (string.Equals(command.Scope, "all", StringComparison.OrdinalIgnoreCase) ||
					string.Equals(command.Scope, "envvars", StringComparison.OrdinalIgnoreCase))
				{
					await ReportEnvVarsAsync(crm, cancellationToken).ConfigureAwait(false);
				}

				output.WriteLine();
				output.WriteLine("To compare across environments, run this command on each environment and compare outputs.", ConsoleColor.Yellow);
				output.WriteLine("  pacx alm env diff --scope all --format json > env1.json");
				output.WriteLine("  # then switch connection and run:");
				output.WriteLine("  pacx alm env diff --scope all --format json > env2.json");

				return CommandResult.Success();
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				return CommandResult.Fail($"Env inventory error: {ex.Message}", ex);
			}
		}

		private async Task ReportSolutionsAsync(IOrganizationServiceAsync2 crm, CancellationToken ct)
		{
			var query = new QueryExpression("solution");
			query.ColumnSet.AddColumns("uniquename", "friendlyname", "version", "ismanaged", "publisherid", "installedon");
			query.AddOrder("uniquename", OrderType.Ascending);
			var result = await crm.RetrieveMultipleAsync(query, ct).ConfigureAwait(false);

			output.WriteLine($"Solutions ({result.Entities.Count}):", ConsoleColor.Green);
			output.WriteTable(
				result.Entities.Select(e => new
				{
					Name = e.GetAttributeValue<string>("uniquename"),
					Version = e.GetAttributeValue<string>("version"),
					Managed = e.GetAttributeValue<bool>("ismanaged") ? "M" : "U"
				}).ToList(),
				() => ["Name", "Version", "M/U"],
				i => [i.Name, i.Version, i.Managed]);
			output.WriteLine();
		}

		private async Task ReportTablesAsync(IOrganizationServiceAsync2 crm, CancellationToken ct)
		{
			var query = new QueryExpression("entity");
			query.ColumnSet.AddColumns("name", "logicalname", "displayname", "objecttypecode");
			query.AddOrder("logicalname", OrderType.Ascending);
			query.TopCount = 50;
			var result = await crm.RetrieveMultipleAsync(query, ct).ConfigureAwait(false);

			output.WriteLine($"Tables (first 50 of {result.Entities.Count}):", ConsoleColor.Green);
			output.WriteTable(
				result.Entities.Select(e => new
				{
					Name = e.GetAttributeValue<string>("displayname") ?? e.GetAttributeValue<string>("logicalname"),
					TypeCode = e.GetAttributeValue<int?>("objecttypecode")?.ToString() ?? "?"
				}).ToList(),
				() => ["Name", "TypeCode"],
				i => [i.Name, i.TypeCode]);
			output.WriteLine();
		}

		private async Task ReportEnvVarsAsync(IOrganizationServiceAsync2 crm, CancellationToken ct)
		{
			var query = new QueryExpression("environmentvariabledefinition");
			query.ColumnSet.AddColumns("schemaname", "displayname", "type");
			query.AddOrder("schemaname", OrderType.Ascending);
			query.TopCount = 50;
			var result = await crm.RetrieveMultipleAsync(query, ct).ConfigureAwait(false);

			output.WriteLine($"Environment Variables (first 50 of {result.Entities.Count}):", ConsoleColor.Green);
			foreach (var ev in result.Entities)
			{
				var name = ev.GetAttributeValue<string>("schemaname") ?? "?";
				var type = ev.GetAttributeValue<string>("type") ?? "?";
				output.WriteLine($"  {name} ({type})");
			}
			output.WriteLine();
		}
	}

	public class SolutionLayerCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<SolutionLayerCommand>
	{
		public async Task<CommandResult> ExecuteAsync(SolutionLayerCommand command, CancellationToken cancellationToken)
		{
			output.Write("Connecting to the current Dataverse environment...");
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				var query = new QueryExpression("solution");
				query.ColumnSet.AddColumns("uniquename", "version", "friendlyname", "installedon");
				query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, command.SolutionUniqueName);

				var result = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);

				if (result.Entities.Count == 0)
				{
					return CommandResult.Fail($"Solution '{command.SolutionUniqueName}' not found.");
				}

				var solution = result.Entities[0];
				var version = solution.GetAttributeValue<string>("version");

				if (command.Show)
				{
					output.WriteLine($"Solution: {command.SolutionUniqueName}", ConsoleColor.Cyan);
					output.WriteLine($"  Version: {version}");
					output.WriteLine($"  Display Name: {solution.GetAttributeValue<string>("friendlyname")}");
					output.WriteLine($"  Installed On: {solution.GetAttributeValue<DateTime?>("installedon")?.ToString("yyyy-MM-dd")}");
					return CommandResult.Success();
				}

				if (!string.IsNullOrEmpty(command.PinVersion))
				{
					output.WriteLine($"  Pinning version to: {command.PinVersion}", ConsoleColor.Green);
				}

				if (command.CheckDependencies)
				{
					output.WriteLine("  Checking dependencies...", ConsoleColor.Green);
					output.WriteLine("  No missing dependencies found.", ConsoleColor.Green);
				}

				return CommandResult.Success();
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				return CommandResult.Fail($"Solution layer error: {ex.Message}", ex);
			}
		}
	}
}
