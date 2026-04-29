using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Metadata;
using System.Threading;

namespace Greg.Xrm.Command.Commands.Column.Builders
{
	internal sealed class AttributeMetadataBuilderInteger : AttributeMetadataBuilderNumericBase
	{

		public override Task<AttributeMetadata> CreateFromAsync(IOrganizationServiceAsync2 crm, CreateCommand command, int languageCode, string publisherPrefix, int customizationOptionValuePrefix, CancellationToken cancellationToken = default)
		{
			var attribute = new IntegerAttributeMetadata();
			SetCommonProperties(attribute, command, languageCode, publisherPrefix);

			attribute.MinValue = GetIntValue(command.MinValue, Limit.Min);
			attribute.MaxValue = GetIntValue(command.MaxValue, Limit.Max);
			attribute.Format = command.IntegerFormat;

			return Task.FromResult((AttributeMetadata)attribute);
		}
	}
}
