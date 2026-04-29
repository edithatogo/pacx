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
	public class CustomApiDeleteCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<CustomApiDeleteCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CustomApiDeleteCommand command, CancellationToken cancellationToken)
		{
			output.Write("Connecting to the current Dataverse environment...");
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			output.WriteLine("Done", ConsoleColor.Green);

			try
			{
				var query = new QueryExpression("customapi");
				query.ColumnSet.AddColumn("customapiid");
				query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, command.Name);

				var result = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);

				if (result.Entities.Count == 0)
				{
					output.WriteLine($"Custom API '{command.Name}' not found.", ConsoleColor.Red);
					return CommandResult.Fail($"Custom API '{command.Name}' not found.");
				}

				var apiRef = result.Entities[0].ToEntityReference();
				output.Write($"Deleting Custom API '{command.Name}'...");
				await crm.DeleteAsync(apiRef.LogicalName, apiRef.Id, cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				return CommandResult.Success();
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				return CommandResult.Fail($"Failed to delete Custom API: {ex.Message}", ex);
			}
		}
	}
}
