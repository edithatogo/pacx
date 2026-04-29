using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.CustomApi
{
	public class CustomApiListCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<CustomApiListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CustomApiListCommand command, CancellationToken cancellationToken)
		{
			output.Write("Connecting to the current Dataverse environment...");
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				var query = new QueryExpression("customapi");
				query.ColumnSet.AddColumns("uniquename", "displayname", "description", "boundentitylogicalname", "iscustomprocessed", "isfunction", "createdon");
				query.AddOrder("uniquename", OrderType.Ascending);

				if (!string.IsNullOrEmpty(command.EntityLogicalName))
				{
					query.Criteria.AddCondition("boundentitylogicalname", ConditionOperator.Equal, command.EntityLogicalName);
				}

				var result = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);

				if (result.Entities.Count == 0)
				{
					output.WriteLine("No Custom APIs found.", ConsoleColor.Yellow);
					return CommandResult.Success();
				}

				if (command.Format == "json")
				{
					var json = Newtonsoft.Json.JsonConvert.SerializeObject(
						result.Entities.Select(MapToJson).ToList(),
						Newtonsoft.Json.Formatting.Indented);
					output.WriteLine(json);
				}
				else
				{
					output.WriteTable(result.Entities,
						() => new[] { "Name", "Display Name", "Bound Entity", "Function", "Created" },
						e => new[] {
							e.GetAttributeValue<string>("uniquename") ?? "-",
							e.GetAttributeValue<string>("displayname") ?? "-",
							e.GetAttributeValue<string>("boundentitylogicalname") ?? "Global",
							e.GetAttributeValue<bool?>("isfunction") == true ? "Yes" : "No",
							e.GetAttributeValue<DateTime?>("createdon")?.ToString("yyyy-MM-dd") ?? "-"
						}
					);
				}

				output.WriteLine($"\nTotal: {result.Entities.Count} Custom API(s)", ConsoleColor.Cyan);
				return CommandResult.Success();
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				return CommandResult.Fail($"Failed to list Custom APIs: {ex.Message}", ex);
			}
		}

		private static object MapToJson(Entity e) => new
		{
			UniqueName = e.GetAttributeValue<string>("uniquename"),
			DisplayName = e.GetAttributeValue<string>("displayname"),
			Description = e.GetAttributeValue<string>("description"),
			BoundEntity = e.GetAttributeValue<string>("boundentitylogicalname"),
			IsFunction = e.GetAttributeValue<bool?>("isfunction") ?? false,
			CreatedOn = e.GetAttributeValue<DateTime?>("createdon")
		};
	}
}
