using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;

namespace Greg.Xrm.Command.Services.Tabular
{
	public class TabularClient(ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory) : ITabularClient
	{
		public const string Resource = "https://analysis.windows.net/powerbi/api";
		private const string BaseUrl = "https://api.powerbi.com/v1.0/myorg";

		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public async Task<string> DeployBimAsync(string workspaceId, string datasetName, string bimContent, CancellationToken ct)
		{
			var path = $"/groups/{Uri.EscapeDataString(workspaceId)}/imports?datasetDisplayName={Uri.EscapeDataString(datasetName)}";
			using var request = await CreateRequestAsync(HttpMethod.Post, path, ct).ConfigureAwait(false);

			using var form = new MultipartFormDataContent();
			form.Add(new StringContent(bimContent, Encoding.UTF8, "application/json"), "file", $"{datasetName}.bim");
			request.Content = form;

			var response = await SendWithFactoryAsync(request, ct).ConfigureAwait(false);
			return response.RootElement.TryGetProperty("id", out var id) ? id.GetString() ?? string.Empty : string.Empty;
		}

		public async Task<string?> GetDeployedBimAsync(string workspaceId, string datasetId, CancellationToken ct)
		{
			var path = $"/groups/{Uri.EscapeDataString(workspaceId)}/datasets/{Uri.EscapeDataString(datasetId)}";
			var response = await SendAsync(HttpMethod.Get, path, null, ct).ConfigureAwait(false);
			return response.RootElement.ToString();
		}

		public async Task<string?> GetDatasetIdByNameAsync(string workspaceId, string datasetName, CancellationToken ct)
		{
			var path = $"/groups/{Uri.EscapeDataString(workspaceId)}/datasets";
			var response = await SendAsync(HttpMethod.Get, path, null, ct).ConfigureAwait(false);

			if (response.RootElement.TryGetProperty("value", out var datasets) && datasets.ValueKind == JsonValueKind.Array)
			{
				foreach (var ds in datasets.EnumerateArray())
				{
					if (ds.TryGetProperty("name", out var name) && name.GetString() == datasetName)
					{
						return ds.TryGetProperty("id", out var id) ? id.GetString() : null;
					}
				}
			}

			return null;
		}

		public async Task UpdateDefinitionAsync(string workspaceId, string datasetId, string bimContent, CancellationToken ct)
		{
			var path = $"/groups/{Uri.EscapeDataString(workspaceId)}/datasets/{Uri.EscapeDataString(datasetId)}/Default.UpdateDefinition";
			var payload = new
			{
				definition = new
				{
					model = new Dictionary<string, string>
					{
						["model.json"] = bimContent
					}
				}
			};
			await SendVoidAsync(HttpMethod.Put, path, payload, ct).ConfigureAwait(false);
		}

		private async Task<JsonDocument> SendAsync(HttpMethod method, string path, object? payload, CancellationToken ct)
		{
			using var request = await CreateRequestAsync(method, path, ct).ConfigureAwait(false);
			if (payload != null)
			{
				request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
			}
			return await SendWithFactoryAsync(request, ct).ConfigureAwait(false);
		}

		private async Task SendVoidAsync(HttpMethod method, string path, object? payload, CancellationToken ct)
		{
			using var request = await CreateRequestAsync(method, path, ct).ConfigureAwait(false);
			if (payload != null)
			{
				request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
			}
			var client = this.httpClientFactory.CreateClient();
			using var response = await client.SendAsync(request, ct).ConfigureAwait(false);
			var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Power BI Tabular API error ({response.StatusCode}): {content}");
			}
		}

		private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string path, CancellationToken ct)
		{
			var token = await this.tokenProvider.GetTokenAsync(Resource, ct).ConfigureAwait(false);
			if (string.IsNullOrWhiteSpace(token))
			{
				throw new InvalidOperationException("Unable to acquire a Power BI access token from the current connection.");
			}

			var normalizedPath = path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
			var request = new HttpRequestMessage(method, BaseUrl + normalizedPath);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			return request;
		}

		private async Task<JsonDocument> SendWithFactoryAsync(HttpRequestMessage request, CancellationToken ct)
		{
			var client = this.httpClientFactory.CreateClient();
			using var response = await client.SendAsync(request, ct).ConfigureAwait(false);
			var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Power BI Tabular API error ({response.StatusCode}): {content}");
			}
			return JsonDocument.Parse(string.IsNullOrWhiteSpace(content) ? "{}" : content);
		}
	}
}
