using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.Connector
{
	public class ConnectorImportCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository,
		IConnectorSchemaValidator validator) : ICommandExecutor<ConnectorImportCommand>
	{
		private readonly IOutput output = output ?? throw new ArgumentNullException(nameof(output));
		private readonly IOrganizationServiceRepository organizationServiceRepository = organizationServiceRepository ?? throw new ArgumentNullException(nameof(organizationServiceRepository));
		private readonly IConnectorSchemaValidator validator = validator ?? throw new ArgumentNullException(nameof(validator));

		public ConnectorImportCommandExecutor(IOutput output, IOrganizationServiceRepository organizationServiceRepository)
			: this(output, organizationServiceRepository, new ConnectorSchemaValidator())
		{
		}

		public async Task<CommandResult> ExecuteAsync(ConnectorImportCommand command, CancellationToken cancellationToken)
		{
			if (!File.Exists(command.FilePath))
			{
				return CommandResult.Fail($"Connector definition not found: {command.FilePath}");
			}

			var content = await File.ReadAllTextAsync(command.FilePath, cancellationToken).ConfigureAwait(false);
			var validationResult = this.validator.Validate(content);
			var validationCommandResult = ConnectorValidationOutput.WriteIssues(this.output, validationResult, strict: false);
			if (validationCommandResult != null)
			{
				return CommandResult.Fail(
					$"Connector import validation failed: {validationResult.Errors.Count} error(s), {validationResult.Warnings.Count} warning(s)",
					validationResult.Exception);
			}

			this.output.Write("Connecting to the current Dataverse environment...");
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			this.output.WriteLine("Done", ConsoleColor.Green);

			this.output.WriteLine($"Importing connector from: {command.FilePath}", ConsoleColor.Cyan);

			if (command.DryRun)
			{
				this.output.WriteLine("[DRY RUN] Would import:", ConsoleColor.Yellow);
				this.output.WriteLine($"  File: {command.FilePath}");
				this.output.WriteLine($"  Size: {content.Length} bytes");
				if (!string.IsNullOrEmpty(command.SolutionUniqueName))
					this.output.WriteLine($"  Solution: {command.SolutionUniqueName}");
				return CommandResult.Success();
			}

			try
			{
				var connector = new Entity("connector");
				connector["name"] = Path.GetFileNameWithoutExtension(command.FilePath);
				connector["openapidefinition"] = content;
				connector["connectortype"] = new OptionSetValue(1); // Custom

				this.output.Write("Creating custom connector in Dataverse...");
				var connectorId = await crm.CreateAsync(connector, cancellationToken).ConfigureAwait(false);
				this.output.WriteLine("Done", ConsoleColor.Green);
				this.output.WriteLine($"Connector ID: {connectorId}");

				if (!string.IsNullOrEmpty(command.SolutionUniqueName))
				{
					var request = new Microsoft.Crm.Sdk.Messages.AddSolutionComponentRequest
					{
						ComponentId = connectorId,
						ComponentType = 371, // Connector
						SolutionUniqueName = command.SolutionUniqueName,
						AddRequiredComponents = true
					};
					await crm.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
					this.output.WriteLine($"Added connector to solution: {command.SolutionUniqueName}", ConsoleColor.Green);
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Connector import error: {ex.Message}", ex);
			}
		}
	}

	public class ConnectorExportCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<ConnectorExportCommand>
	{
		private readonly IOutput output = output ?? throw new ArgumentNullException(nameof(output));
		private readonly IOrganizationServiceRepository organizationServiceRepository = organizationServiceRepository ?? throw new ArgumentNullException(nameof(organizationServiceRepository));

		public async Task<CommandResult> ExecuteAsync(ConnectorExportCommand command, CancellationToken cancellationToken)
		{
			this.output.Write("Connecting to the current Dataverse environment...");
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			this.output.WriteLine("Done", ConsoleColor.Green);

			this.output.WriteLine($"Exporting connector: {command.ConnectorName}", ConsoleColor.Cyan);

			try
			{
				var query = new QueryExpression("connector");
				query.ColumnSet.AddColumns("name", "openapidefinition");
				query.Criteria.AddCondition("name", ConditionOperator.Equal, command.ConnectorName);

				var result = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
				if (result.Entities.Count == 0)
				{
					return CommandResult.Fail($"Connector '{command.ConnectorName}' not found.");
				}

				var connector = result.Entities[0];
				var definition = connector.GetAttributeValue<string>("openapidefinition");

				if (string.IsNullOrEmpty(definition))
				{
					return CommandResult.Fail($"Connector '{command.ConnectorName}' has no definition.");
				}

				await File.WriteAllTextAsync(command.OutputPath, definition, cancellationToken).ConfigureAwait(false);
				this.output.WriteLine($"Connector definition exported to: {command.OutputPath}", ConsoleColor.Green);

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Connector export error: {ex.Message}", ex);
			}
		}
	}

	public class ConnectorTestCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository,
		ITokenProvider tokenProvider,
		IHttpClientFactory httpClientFactory,
		IConnectorSchemaValidator validator) : ICommandExecutor<ConnectorTestCommand>
	{
		private readonly IOutput output = output ?? throw new ArgumentNullException(nameof(output));
		private readonly IOrganizationServiceRepository organizationServiceRepository = organizationServiceRepository ?? throw new ArgumentNullException(nameof(organizationServiceRepository));
		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		private readonly IConnectorSchemaValidator validator = validator ?? throw new ArgumentNullException(nameof(validator));

		public ConnectorTestCommandExecutor(
			IOutput output,
			IOrganizationServiceRepository organizationServiceRepository,
			ITokenProvider tokenProvider,
			IHttpClientFactory httpClientFactory)
			: this(output, organizationServiceRepository, tokenProvider, httpClientFactory, new ConnectorSchemaValidator())
		{
		}

		public async Task<CommandResult> ExecuteAsync(ConnectorTestCommand command, CancellationToken cancellationToken)
		{
			this.output.Write("Connecting to the current Dataverse environment...");
			var crmBase = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			this.output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				var connectorDefinitionValidation = await this.ValidateStoredConnectorDefinitionAsync(crmBase, command.ConnectorName, cancellationToken).ConfigureAwait(false);
				if (connectorDefinitionValidation != null)
				{
					return connectorDefinitionValidation;
				}

				if (crmBase is not ServiceClient crm)
				{
					return CommandResult.Fail("Connector testing requires a ServiceClient connection.");
				}

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

				var url = $"https://api.bap.microsoft.com/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{envId}/connectors/{command.ConnectorName}/test?api-version=2020-10-01&operationId={command.OperationName}";

				string payload = "{}";
				if (!string.IsNullOrEmpty(command.PayloadPath))
				{
					if (!File.Exists(command.PayloadPath))
						return CommandResult.Fail($"Payload file not found: {command.PayloadPath}");
					payload = await File.ReadAllTextAsync(command.PayloadPath, cancellationToken).ConfigureAwait(false);
				}

				this.output.WriteLine($"Testing connector: {command.ConnectorName}", ConsoleColor.Cyan);
				this.output.WriteLine($"  Operation: {command.OperationName}");
				this.output.Write("Sending test request...");

				var response = await client.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

				if (!response.IsSuccessStatusCode)
				{
					var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
					return CommandResult.Fail($"API error ({response.StatusCode}): {error}");
				}

				this.output.WriteLine("Done", ConsoleColor.Green);
				var resultJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
				this.output.WriteLine("Response Received:", ConsoleColor.Cyan);
				this.output.WriteLine(resultJson);

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Connector test error: {ex.Message}", ex);
			}
		}

		private async Task<CommandResult?> ValidateStoredConnectorDefinitionAsync(IOrganizationServiceAsync2 crm, string connectorName, CancellationToken cancellationToken)
		{
			var query = new QueryExpression("connector");
			query.ColumnSet.AddColumns("name", "openapidefinition");
			query.Criteria.AddCondition("name", ConditionOperator.Equal, connectorName);

			var result = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
			if (result.Entities.Count == 0)
			{
				return CommandResult.Fail($"Connector '{connectorName}' not found.");
			}

			var definition = result.Entities[0].GetAttributeValue<string>("openapidefinition");
			if (string.IsNullOrWhiteSpace(definition))
			{
				return CommandResult.Fail($"Connector '{connectorName}' has no definition.");
			}

			var validationResult = this.validator.Validate(definition);
			var validationCommandResult = ConnectorValidationOutput.WriteIssues(this.output, validationResult, strict: false);
			if (validationCommandResult != null)
			{
				return CommandResult.Fail(
					$"Connector test validation failed: {validationResult.Errors.Count} error(s), {validationResult.Warnings.Count} warning(s)",
					validationResult.Exception);
			}

			return null;
		}
	}

	public class ConnectorValidateCommandExecutor : ICommandExecutor<ConnectorValidateCommand>
	{
		private readonly IOutput output;
		private readonly IConnectorSchemaValidator validator;

		public ConnectorValidateCommandExecutor(IOutput output)
			: this(output, new ConnectorSchemaValidator())
		{
		}

		public ConnectorValidateCommandExecutor(IOutput output, IConnectorSchemaValidator validator)
		{
			this.output = output ?? throw new ArgumentNullException(nameof(output));
			this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
		}

		public async Task<CommandResult> ExecuteAsync(ConnectorValidateCommand command, CancellationToken cancellationToken)
		{
			if (!File.Exists(command.FilePath))
			{
				return CommandResult.Fail($"Connector definition not found: {command.FilePath}");
			}

			var content = await File.ReadAllTextAsync(command.FilePath, cancellationToken).ConfigureAwait(false);

			this.output.WriteLine($"Validating connector: {command.FilePath}", ConsoleColor.Cyan);

			var validationResult = this.validator.Validate(content);
			if (!string.IsNullOrWhiteSpace(command.SchemaFilePath))
			{
				validationResult = Merge(validationResult, await ValidateOrganizationSchemaAsync(content, command.SchemaFilePath, cancellationToken).ConfigureAwait(false));
			}

			var validationCommandResult = ConnectorValidationOutput.WriteIssues(this.output, validationResult, command.Strict);
			if (validationCommandResult != null)
			{
				return validationCommandResult;
			}

			if (validationResult.Warnings.Count > 0)
			{
				this.output.WriteLine($"\nFound {validationResult.Warnings.Count} warning(s).", ConsoleColor.Yellow);
				return CommandResult.Success();
			}

			this.output.WriteLine("Validation passed. No issues found.", ConsoleColor.Green);
			return CommandResult.Success();
		}

		private static async Task<ConnectorSchemaValidationResult> ValidateOrganizationSchemaAsync(string connectorDefinition, string schemaFilePath, CancellationToken cancellationToken)
		{
			if (!File.Exists(schemaFilePath))
			{
				return new ConnectorSchemaValidationResult([$"Schema file not found: {schemaFilePath}"], Array.Empty<string>());
			}

			try
			{
				using var definition = JsonDocument.Parse(connectorDefinition);
				using var schema = JsonDocument.Parse(await File.ReadAllTextAsync(schemaFilePath, cancellationToken).ConfigureAwait(false));

				var errors = new List<string>();
				var warnings = new List<string>();
				if (schema.RootElement.TryGetProperty("required", out var required) && required.ValueKind == JsonValueKind.Array)
				{
					foreach (var item in required.EnumerateArray())
					{
						var propertyName = item.GetString();
						if (!string.IsNullOrWhiteSpace(propertyName) && !definition.RootElement.TryGetProperty(propertyName, out _))
						{
							errors.Add($"Organization schema requires root property '{propertyName}'.");
						}
					}
				}
				else
				{
					warnings.Add("Organization schema has no 'required' array; no custom policy rules were applied.");
				}

				return new ConnectorSchemaValidationResult(errors, warnings);
			}
			catch (JsonException ex)
			{
				return new ConnectorSchemaValidationResult(["Invalid organization schema JSON: " + ex.Message], Array.Empty<string>(), ex);
			}
		}

		private static ConnectorSchemaValidationResult Merge(ConnectorSchemaValidationResult first, ConnectorSchemaValidationResult second)
		{
			return new ConnectorSchemaValidationResult(
				first.Errors.Concat(second.Errors).ToArray(),
				first.Warnings.Concat(second.Warnings).ToArray(),
				second.Exception ?? first.Exception);
		}
	}

	internal static class ConnectorValidationOutput
	{
		public static CommandResult? WriteIssues(IOutput output, ConnectorSchemaValidationResult validationResult, bool strict)
		{
			foreach (var warning in validationResult.Warnings)
			{
				output.WriteLine($"  WARNING: {warning}", ConsoleColor.Yellow);
			}

			foreach (var error in validationResult.Errors)
			{
				output.WriteLine($"  ERROR: {error}", ConsoleColor.Red);
			}

			if (validationResult.IsValid && (!strict || validationResult.Warnings.Count == 0))
			{
				return null;
			}

			var totalIssues = validationResult.Errors.Count + validationResult.Warnings.Count;
			output.WriteLine($"\nFound {totalIssues} issue(s).", strict ? ConsoleColor.Red : ConsoleColor.Yellow);
			var message = $"Validation failed: {validationResult.Errors.Count} error(s), {validationResult.Warnings.Count} warning(s)";
			return CommandResult.Fail(message, new ValidationException(message, validationResult.Exception));
		}
	}
}
