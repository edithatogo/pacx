using Greg.Xrm.Command.Model;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Metadata;

namespace Greg.Xrm.Command.Services.AttributeDeletion
{
	public class AttributeDeletionStrategyForPluginSteps(
		IOutput output
	)
		: AttributeDeletionStrategyBase
	{
		public override ComponentType ComponentType => ComponentType.SDKMessageProcessingStep;


		protected override async Task HandleInternalAsync(
			IOrganizationServiceAsync2 crm,
			AttributeMetadata attribute,
			IReadOnlyList<Dependency> dependencies,
			CancellationToken cancellationToken)
		{
			var result = await RetrieveDataAsync(crm, dependencies, "sdkmessageprocessingstep", "sdkmessageprocessingstepid", cancellationToken, "name", "filteringattributes").ConfigureAwait(false);

			var i = 0;
			foreach (var e in result)
			{
				++i;
				output.Write($"Updating sdkmessageprocessingstep {i}/{result.Count} {e.GetAttributeValue<string>("name")}...");
				e["filteringattributes"] = RemoveValueFromCsvValues(e.GetAttributeValue<string>("filteringattributes"), attribute.LogicalName);
				await crm.UpdateAsync(e, cancellationToken).ConfigureAwait(false);
				output.WriteLine("Done", ConsoleColor.Green);
			}
		}
	}
}
