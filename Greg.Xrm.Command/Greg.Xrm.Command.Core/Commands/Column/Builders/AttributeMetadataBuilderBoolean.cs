using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System.Threading;

namespace Greg.Xrm.Command.Commands.Column.Builders
{
	internal sealed class AttributeMetadataBuilderBoolean : AttributeMetadataBuilderBase
	{
		public override Task<AttributeMetadata> CreateFromAsync(IOrganizationServiceAsync2 crm, CreateCommand command, int languageCode, string publisherPrefix, int customizationOptionValuePrefix, CancellationToken cancellationToken = default)
		{
			var attribute = new BooleanAttributeMetadata();
			SetCommonProperties(attribute, command, languageCode, publisherPrefix);

			// Set extended properties
			attribute.OptionSet = new BooleanOptionSetMetadata(
				new OptionMetadata(new Label(command.TrueLabel, languageCode), 1),
				new OptionMetadata(new Label(command.FalseLabel, languageCode), 0)
			);

			return Task.FromResult((AttributeMetadata)attribute);
		}
	}
}
