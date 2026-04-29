using Greg.Xrm.Command.Model;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Metadata;

namespace Greg.Xrm.Command.Services.AttributeDeletion
{
	public class AttributeDeletionService(
		IOutput output,
		IEnumerable<IAttributeDeletionStrategy> strategies)
		: IAttributeDeletionService
	{
		public async Task DeleteAttributeAsync(IOrganizationServiceAsync2 crm, AttributeMetadata attribute, DependencyList dependencies, bool? simulation = false, CancellationToken cancellationToken = default)
		{
			foreach (var strategy in strategies)
			{
				try
				{
					await strategy.HandleAsync(crm, attribute, dependencies, cancellationToken).ConfigureAwait(false);
				}
				catch (AttributeDeletionException ex)
				{
					output.WriteLine($"Error while trying to remove a dependency on {strategy.GetType().Name}: " + ex.Message, ConsoleColor.Red);
				}

			}

			await PublishAll(crm, cancellationToken).ConfigureAwait(false);
		}


		private async Task PublishAll(IOrganizationServiceAsync2 crm, CancellationToken cancellationToken)
		{
			output.Write("Publishing all...");
			await crm.ExecuteAsync(new PublishAllXmlRequest(), cancellationToken).ConfigureAwait(false);
			output.WriteLine("Done", ConsoleColor.Green);
		}
	}
}
