using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.CustomApi
{
	public class CustomApiCreateCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<CustomApiCreateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CustomApiCreateCommand command, CancellationToken cancellationToken)
		{
			output.Write("Connecting to the current Dataverse environment...");
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				var customApi = new Entity("customapi");
				customApi["uniquename"] = command.Name;
				customApi["displayname"] = command.DisplayName ?? command.Name;
				customApi["description"] = command.Description ?? "";
				customApi["iscustomapivisible"] = true;
				customApi["iscustomprocessed"] = false;
				customApi["boundentitylogicalname"] = command.EntityLogicalName;
				customApi["customapirequestprocessingtype"] = command.BindingType switch
				{
					"Entity" => 0,
					"EntityCollection" => 1,
					_ => 2
				};
				customApi["isfunction"] = command.IsFunction;

				if (!string.IsNullOrEmpty(command.ExecutePluginTypeName))
				{
					var pluginTypeQuery = new QueryExpression("plugintype");
					pluginTypeQuery.ColumnSet.AddColumn("plugintypeid");
					pluginTypeQuery.Criteria.AddCondition("typename", ConditionOperator.Equal, command.ExecutePluginTypeName);
					var pluginResult = await crm.RetrieveMultipleAsync(pluginTypeQuery, cancellationToken).ConfigureAwait(false);
					if (pluginResult.Entities.Count > 0)
					{
						customApi["workflowsdksteppluginTypeId"] = new EntityReference("plugintype", pluginResult.Entities[0].Id);
					}
				}

				output.Write($"Creating Custom API '{command.Name}'...");
				var apiId = await crm.CreateAsync(customApi, cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				if (command.Inputs != null && command.Inputs.Length > 0)
				{
					foreach (var inputDef in command.Inputs)
					{
						await CreateParameterAsync(crm, apiId, inputDef, true, cancellationToken).ConfigureAwait(false);
					}
				}

				if (command.Outputs != null && command.Outputs.Length > 0)
				{
					foreach (var outputDef in command.Outputs)
					{
						await CreateParameterAsync(crm, apiId, outputDef, false, cancellationToken).ConfigureAwait(false);
					}
				}

				output.WriteLine($"Custom API '{command.Name}' created successfully (ID: {apiId}).", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				return CommandResult.Fail($"Failed to create Custom API: {ex.Message}", ex);
			}
		}

		private async Task CreateParameterAsync(IOrganizationServiceAsync2 crm, Guid customApiId, string paramDef, bool isInput, CancellationToken ct)
		{
			var parts = paramDef.Split(':');
			if (parts.Length != 2)
			{
				output.WriteLine($"  WARNING: Invalid parameter format '{paramDef}'. Expected 'Type:Name'.", ConsoleColor.Yellow);
				return;
			}

			var typeName = parts[0];
			var paramName = parts[1];

			var param = new Entity(isInput ? "customapirequestparameter" : "customapiresponseproperty");
			param["customapiid"] = new EntityReference("customapi", customApiId);
			param["uniquename"] = paramName;
			param["displayname"] = paramName;

			var typeCode = typeName.ToLowerInvariant() switch
			{
				"string" => 10,
				"int" or "integer" => 6,
				"bool" or "boolean" => 7,
				"datetime" => 8,
				"decimal" or "money" => 5,
				"double" => 4,
				"guid" or "uniqueidentifier" => 11,
				"entity" or "entityreference" => 1,
				"picklist" or "optionset" => 2,
				"stringarray" => 13,
				_ => 10
			};

			param["type"] = typeCode;

			await crm.CreateAsync(param, ct).ConfigureAwait(false);
			output.WriteLine($"  Created {(isInput ? "input" : "output")} parameter: {paramName} ({typeName})", ConsoleColor.Green);
		}
	}
}
