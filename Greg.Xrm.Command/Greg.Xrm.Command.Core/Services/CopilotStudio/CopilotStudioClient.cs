using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;

namespace Greg.Xrm.Command.Services.CopilotStudio
{
	public class CopilotStudioClient(ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory) : ICopilotStudioClient
	{
		public const string Resource = "https://api.powerplatform.com/";
		private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
		{
			WriteIndented = true
		};

		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public Task<JsonDocument> GetAsync(string environmentId, string path, CancellationToken cancellationToken)
			=> SendAsync(HttpMethod.Get, environmentId, path, null, cancellationToken);

		public Task<JsonDocument> PostAsync(string environmentId, string path, object payload, CancellationToken cancellationToken)
			=> SendAsync(HttpMethod.Post, environmentId, path, payload, cancellationToken);

		private async Task<JsonDocument> SendAsync(HttpMethod method, string environmentId, string path, object? payload, CancellationToken cancellationToken)
		{
			var token = await this.tokenProvider.GetTokenAsync(Resource, cancellationToken).ConfigureAwait(false);
			if (string.IsNullOrWhiteSpace(token))
			{
				throw new InvalidOperationException("Unable to acquire a Copilot Studio access token from the current connection.");
			}

			var normalizedPath = path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
			using var request = new HttpRequestMessage(method, $"https://api.powerplatform.com/copilotstudio/environments/{Uri.EscapeDataString(environmentId)}{normalizedPath}");
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
			if (payload != null)
			{
				request.Content = new StringContent(JsonSerializer.Serialize(payload, SerializerOptions), System.Text.Encoding.UTF8, "application/json");
			}

			var client = this.httpClientFactory.CreateClient();
			using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
			var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Copilot Studio API error ({response.StatusCode}): {content}");
			}

			return JsonDocument.Parse(string.IsNullOrWhiteSpace(content) ? "{}" : content);
		}
	}
}
