using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;

namespace Greg.Xrm.Command.Services.PowerPlatformAdmin
{
	public class PowerPlatformAdminClient(ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory) : IPowerPlatformAdminClient
	{
		private const string Resource = "https://management.azure.com";
		private const string BaseUrl = "https://management.azure.com";
		private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
		{
			WriteIndented = true
		};

		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public async Task<JsonDocument> ListManagementAppsAsync(CancellationToken cancellationToken)
		{
			return await GetAsync("/providers/Microsoft.BusinessAppPlatform/adminApplications?api-version=2020-06-01", cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> GetTenantSettingsAsync(CancellationToken cancellationToken)
		{
			return await PostAsync("/providers/Microsoft.BusinessAppPlatform/listtenantsettings?api-version=2020-10-01", null, cancellationToken).ConfigureAwait(false);
		}

		public async Task SetTenantSettingsAsync(object settings, CancellationToken cancellationToken)
		{
			await SendVoidAsync(HttpMethod.Post, "/providers/Microsoft.BusinessAppPlatform/scopes/admin/updateTenantSettings?api-version=2020-10-01", settings, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> CreateEnvironmentAsync(string name, string type, string region, string currency, string language, CancellationToken ct)
		{
			var payload = new
			{
				properties = new
				{
					displayName = name,
					environmentType = type,
					location = region ?? "australiaeast"
				}
			};
			return await PostAsync("/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments?api-version=2020-10-01", payload, ct).ConfigureAwait(false);
		}

		public async Task<JsonDocument> GetEnvironmentAsync(string environmentId, CancellationToken ct)
		{
			return await GetAsync($"/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{environmentId}?api-version=2020-10-01", ct).ConfigureAwait(false);
		}

		public async Task<JsonDocument> ListEnvironmentsAsync(CancellationToken ct)
		{
			return await GetAsync("/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments?api-version=2020-10-01", ct).ConfigureAwait(false);
		}

		public async Task<JsonDocument> ResetEnvironmentAsync(string environmentId, string resetType, CancellationToken ct)
		{
			var payload = new { resetType };
			return await PostAsync($"/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{environmentId}/reset?api-version=2020-10-01", payload, ct).ConfigureAwait(false);
		}

		public async Task<JsonDocument> CopyEnvironmentAsync(string sourceEnvId, string targetName, string mode, CancellationToken ct)
		{
			var payload = new
			{
				properties = new
				{
					displayName = targetName,
					copyType = mode ?? "MinimalCopy"
				}
			};
			return await PostAsync($"/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{sourceEnvId}/copy?api-version=2020-10-01", payload, ct).ConfigureAwait(false);
		}

		public async Task<JsonDocument> BackupEnvironmentAsync(string environmentId, string label, CancellationToken ct)
		{
			var payload = new { Label = label };
			return await PostAsync($"/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{environmentId}/backup?api-version=2020-10-01", payload, ct).ConfigureAwait(false);
		}

		public async Task<JsonDocument> RestoreEnvironmentAsync(string environmentId, string backupId, CancellationToken ct)
		{
			var payload = new { BackupId = backupId };
			return await PostAsync($"/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{environmentId}/restore?api-version=2020-10-01", payload, ct).ConfigureAwait(false);
		}

		public async Task<JsonDocument> GetEnvironmentCapacityAsync(string environmentId, CancellationToken ct)
		{
			return await GetAsync($"/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{environmentId}?api-version=2020-10-01&$expand=capacity", ct).ConfigureAwait(false);
		}

		private async Task<JsonDocument> GetAsync(string path, CancellationToken cancellationToken)
			=> await SendAsync(HttpMethod.Get, path, null, cancellationToken).ConfigureAwait(false);

		private async Task<JsonDocument> PostAsync(string path, object? payload, CancellationToken cancellationToken)
			=> await SendAsync(HttpMethod.Post, path, payload, cancellationToken).ConfigureAwait(false);

		private async Task<JsonDocument> SendAsync(HttpMethod method, string path, object? payload, CancellationToken cancellationToken)
		{
			using var request = await CreateRequestAsync(method, path, cancellationToken).ConfigureAwait(false);
			if (payload != null)
			{
				request.Content = JsonContent.Create(payload, options: SerializerOptions);
			}
			return await SendWithFactoryAsync(request, cancellationToken).ConfigureAwait(false);
		}

		private async Task SendVoidAsync(HttpMethod method, string path, object? payload, CancellationToken cancellationToken)
		{
			using var request = await CreateRequestAsync(method, path, cancellationToken).ConfigureAwait(false);
			if (payload != null)
			{
				request.Content = JsonContent.Create(payload, options: SerializerOptions);
			}
			var client = this.httpClientFactory.CreateClient();
			using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
			var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Power Platform Admin API error ({response.StatusCode}): {RedactErrorContent(content)}");
			}
		}

		private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string path, CancellationToken cancellationToken)
		{
			var token = await this.tokenProvider.GetTokenAsync(Resource, cancellationToken).ConfigureAwait(false);
			if (string.IsNullOrWhiteSpace(token))
			{
				throw new InvalidOperationException("Unable to acquire a Power Platform Admin access token from the current connection.");
			}

			var normalizedPath = path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
			var request = new HttpRequestMessage(method, BaseUrl + normalizedPath);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			return request;
		}

		private async Task<JsonDocument> SendWithFactoryAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var client = this.httpClientFactory.CreateClient();
			using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
			var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Power Platform Admin API error ({response.StatusCode}): {RedactErrorContent(content)}");
			}

			return JsonDocument.Parse(string.IsNullOrWhiteSpace(content) ? "{}" : content);
		}

		private static string RedactErrorContent(string content)
		{
			try
			{
				using var doc = JsonDocument.Parse(content);
				if (doc.RootElement.TryGetProperty("error", out var error)
obj&& error.TryGetProperty("message", out var message))
				{
					return message.GetString() ?? "API error";
				}
			}
			catch
			{
			}
			return content.Length <= 200 ? content : content[..200] + "...";
		}
	}
}

