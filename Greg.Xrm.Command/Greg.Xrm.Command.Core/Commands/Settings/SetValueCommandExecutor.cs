
using System.ServiceModel;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk;

namespace Greg.Xrm.Command.Commands.Settings
{
	public class SetValueCommandExecutor : ICommandExecutor<SetValueCommand>
	{
		private readonly IOutput output;
		private readonly IOrganizationServiceRepository organizationServiceRepository;

		public SetValueCommandExecutor(
			IOutput output,
			IOrganizationServiceRepository organizationServiceRepository)
		{
			ArgumentNullException.ThrowIfNull(output);
			ArgumentNullException.ThrowIfNull(organizationServiceRepository);
			this.output = output;
			this.organizationServiceRepository = organizationServiceRepository;
		}
		public async Task<CommandResult> ExecuteAsync(SetValueCommand command, CancellationToken cancellationToken)
		{

			this.output.Write($"Connecting to the current dataverse environment...");
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			this.output.WriteLine("Done", ConsoleColor.Green);

			var currentSolutionName = command.SolutionName;
			if (string.IsNullOrWhiteSpace(currentSolutionName))
			{
				currentSolutionName = await organizationServiceRepository.GetCurrentDefaultSolutionAsync().ConfigureAwait(false);
				if (currentSolutionName == null)
				{
					return CommandResult.Fail("No solution name provided and no current solution name found in the settings. Please provide a solution name or set a current solution name in the settings.");
				}
			}

			try
			{
				this.output.Write("Updating setting value...");

				var request = new OrganizationRequest("SaveSettingValue");
				request["SettingName"] = command.Name;
				request["Value"] = command.Value;
				if (!string.IsNullOrWhiteSpace(command.AppUniqueName))
				{
					request["AppUniqueName"] = command.AppUniqueName;
				}
				request["SolutionUniqueName"] = currentSolutionName;


				await crm.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

				this.output.WriteLine("Done", ConsoleColor.Green);

				return CommandResult.Success();
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				this.output.WriteLine("ERROR", ConsoleColor.Red);
				this.output.WriteLine(ex.Message, ConsoleColor.Red);

				return CommandResult.Fail("Error updating setting value: " + ex.Message, ex);
			}


		}
	}
}
