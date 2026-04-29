using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Metadata;
using System.Threading;

namespace Greg.Xrm.Command.Commands.Column.Builders
{
	public interface IAttributeMetadataBuilder
	{
		Task<AttributeMetadata> CreateFromAsync(
			IOrganizationServiceAsync2 crm,
			CreateCommand command,
			int languageCode,
			string publisherPrefix,
			int customizationOptionValuePrefix,
			CancellationToken cancellationToken = default);
	}
}
