using Greg.Xrm.Command.Services.Connection;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Services.AiBuilder
{
	public class AiBuilderApiClientFactory : IAiBuilderApiClientFactory
	{
		private readonly IOrganizationServiceRepository _organizationServiceRepository;
		private readonly ITokenProvider _tokenProvider;
		private readonly IHttpClientFactory _httpClientFactory;

		public AiBuilderApiClientFactory(
			IOrganizationServiceRepository organizationServiceRepository,
			ITokenProvider tokenProvider,
			IHttpClientFactory httpClientFactory)
		{
			_organizationServiceRepository = organizationServiceRepository;
			_tokenProvider = tokenProvider;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IAiBuilderApiClient> CreateAsync(CancellationToken ct = default)
		{
			var crm = await _organizationServiceRepository.GetCurrentConnectionAsync(ct).ConfigureAwait(false);
			
			if (crm is ServiceClient serviceClient)
			{
				return new AiBuilderApiClient(serviceClient, _tokenProvider, _httpClientFactory);
			}

			throw new InvalidOperationException("AI Builder API requires a ServiceClient connection.");
		}
	}
}
