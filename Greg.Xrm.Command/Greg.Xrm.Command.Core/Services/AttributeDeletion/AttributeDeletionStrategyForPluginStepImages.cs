using Greg.Xrm.Command.Model;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Metadata;

namespace Greg.Xrm.Command.Services.AttributeDeletion
{
	public class AttributeDeletionStrategyForPluginStepImages(
		IOutput output
	)
		: AttributeDeletionStrategyBase
	{
		public override ComponentType ComponentType => ComponentType.SDKMessageProcessingStepImage;


		protected override async Task HandleInternalAsync(
			IOrganizationServiceAsync2 crm,
			AttributeMetadata attribute,
			IReadOnlyList<Dependency> dependencies,
			CancellationToken cancellationToken)
		{
			var result = await RetrieveDataAsync(crm, dependencies, "sdkmessageprocessingstepimage", "sdkmessageprocessingstepimageid", cancellationToken, "name", "attributes1").ConfigureAwait(false);

			var i = 0;
			foreach (var e in result)
			{
				++i;
				output.Write($"Updating sdkmessageprocessingstepimage {i}/{result.Count} {e.GetAttributeValue<string>("name")}...");

				e["attributes1"] = RemoveValueFromCsvValues(e.GetAttributeValue<string>("attributes1"), attribute.LogicalName);
				await crm.UpdateAsync(e, cancellationToken).ConfigureAwait(false);
				output.WriteLine("Done", ConsoleColor.Green);
			}
		}
	}
}
