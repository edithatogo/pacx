using System.Net.Http.Json;
using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;

namespace Greg.Xrm.Command.Services.Fabric
{
	public class FabricClient(ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory) : IFabricClient
	{
		public const string Resource = "https://api.fabric.microsoft.com/";
		private const string BaseUrl = "https://api.fabric.microsoft.com/v1";
		private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
		{
			WriteIndented = true
		};

		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public async Task<JsonDocument> GetAsync(string path, CancellationToken cancellationToken)
		{
			using var request = await CreateRequestAsync(HttpMethod.Get, path, cancellationToken).ConfigureAwait(false);
			return await SendWithFactoryAsync(request, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> PostAsync(string path, object payload, CancellationToken cancellationToken)
		{
			using var request = await CreateRequestAsync(HttpMethod.Post, path, cancellationToken).ConfigureAwait(false);
			request.Content = JsonContent.Create(payload, options: SerializerOptions);
			return await SendWithFactoryAsync(request, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> DeleteAsync(string path, CancellationToken cancellationToken)
		{
			using var request = await CreateRequestAsync(HttpMethod.Delete, path, cancellationToken).ConfigureAwait(false);
			return await SendWithFactoryAsync(request, cancellationToken).ConfigureAwait(false);
		}

		private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string path, CancellationToken cancellationToken)
		{
			var token = await this.tokenProvider.GetTokenAsync(Resource, cancellationToken).ConfigureAwait(false);
			if (string.IsNullOrWhiteSpace(token))
			{
				throw new InvalidOperationException("Unable to acquire a Fabric access token from the current connection.");
			}

			var request = new HttpRequestMessage(method, BuildUri(path));
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
			return request;
		}

		private async Task<JsonDocument> SendWithFactoryAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var client = this.httpClientFactory.CreateClient();
			using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
			var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Fabric API error ({response.StatusCode}): {content}");
			}

			if (string.IsNullOrWhiteSpace(content))
			{
				content = "{}";
			}

			return JsonDocument.Parse(content);
		}

		private static Uri BuildUri(string path)
		{
			var normalizedPath = path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
			return new Uri(BaseUrl + normalizedPath, UriKind.Absolute);
		}
	}
}
