using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.CustomApi
{
	public class CustomApiDescribeCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<CustomApiDescribeCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CustomApiDescribeCommand command, CancellationToken cancellationToken)
		{
			output.Write("Connecting to the current Dataverse environment...");
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				var apiQuery = new QueryExpression("customapi")
				{
					ColumnSet = new ColumnSet("uniquename", "displayname", "description", "isfunction", "boundentitylogicalname")
				};
				apiQuery.Criteria.AddCondition("uniquename", ConditionOperator.Equal, command.Name);
				apiQuery.TopCount = 1;
				var apiResult = await crm.RetrieveMultipleAsync(apiQuery, cancellationToken).ConfigureAwait(false);

				if (apiResult.Entities.Count == 0)
				{
					return CommandResult.Fail($"Custom API '{command.Name}' was not found.");
				}

				var api = apiResult.Entities[0];
				var requestParameters = await RetrieveParametersAsync(
					crm, "customapirequestparameter", api.Id, cancellationToken).ConfigureAwait(false);
				var responseProperties = await RetrieveParametersAsync(
					crm, "customapiresponseproperty", api.Id, cancellationToken).ConfigureAwait(false);

				output.WriteLine();
				output.WriteLine($"Custom API: {api.GetAttributeValue<string>("uniquename")}", ConsoleColor.Cyan);
				output.WriteLine($"Display name: {api.GetAttributeValue<string>("displayname") ?? "-"}");
				output.WriteLine($"Description: {api.GetAttributeValue<string>("description") ?? "-"}");
				output.WriteLine($"Type: {(api.GetAttributeValue<bool?>("isfunction") == true ? "Function" : "Action")}");
				output.WriteLine($"Bound entity: {api.GetAttributeValue<string>("boundentitylogicalname") ?? "Global"}");
				WriteParameterSection(output, "Request parameters", requestParameters, includeOptional: true);
				WriteParameterSection(output, "Response properties", responseProperties, includeOptional: false);

				var result = CommandResult.Success();
				result["UniqueName"] = api.GetAttributeValue<string>("uniquename") ?? command.Name;
				result["RequestParameterCount"] = requestParameters.Entities.Count;
				result["ResponsePropertyCount"] = responseProperties.Entities.Count;
				return result;
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				return CommandResult.Fail($"Failed to describe Custom API: {ex.Message}", ex);
			}
		}

		private static async Task<EntityCollection> RetrieveParametersAsync(
			IOrganizationServiceAsync2 crm,
			string entityName,
			Guid customApiId,
			CancellationToken cancellationToken)
		{
			var query = new QueryExpression(entityName)
			{
				ColumnSet = new ColumnSet("name", "uniquename", "description", "type", "isoptional")
			};
			query.Criteria.AddCondition("customapiid", ConditionOperator.Equal, customApiId);
			query.AddOrder("uniquename", OrderType.Ascending);
			return await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
		}

		private static void WriteParameterSection(IOutput output, string heading, EntityCollection parameters, bool includeOptional)
		{
			output.WriteLine();
			output.WriteLine($"{heading}: {(parameters.Entities.Count == 0 ? "none" : parameters.Entities.Count.ToString())}", ConsoleColor.DarkGray);
			foreach (var parameter in parameters.Entities)
			{
				var name = parameter.GetAttributeValue<string>("uniquename")
					?? parameter.GetAttributeValue<string>("name")
					?? "-";
				var type = parameter.GetAttributeValue<OptionSetValue>("type")?.Value.ToString() ?? "unknown";
				var optional = includeOptional && parameter.GetAttributeValue<bool?>("isoptional") == true ? "optional" : "required";
				output.WriteLine($"- {name} (type {type}, {optional}): {parameter.GetAttributeValue<string>("description") ?? "-"}");
			}
		}
	}
}
