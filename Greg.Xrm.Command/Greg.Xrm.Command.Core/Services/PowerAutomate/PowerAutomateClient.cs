using System.Net.Http.Headers;
using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;

namespace Greg.Xrm.Command.Services.PowerAutomate
{
	public class PowerAutomateClient(ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory) : IPowerAutomateClient
	{
		public const string Resource = "https://management.azure.com";
		private const string FlowBaseUrl = "https://management.azure.com";
		private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
		{
			WriteIndented = true
		};

		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public async Task<JsonDocument> ListFlowsAsync(string environmentName, string? sharingStatus, bool withSolutions, bool asAdmin, CancellationToken cancellationToken)
		{
			var path = BuildFlowsPath(environmentName, asAdmin);
			var query = $"api-version=2016-11-01";

			if (!string.IsNullOrWhiteSpace(sharingStatus) && sharingStatus != "all")
			{
				query += $"&$filter=search('{Uri.EscapeDataString(sharingStatus)}')";
			}
			if (withSolutions)
			{
				query += "&include=includeSolutionCloudFlows";
			}

			return await GetAsync($"{path}?{query}", cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> GetFlowAsync(string environmentName, string flowName, CancellationToken cancellationToken)
		{
			var path = $"/providers/Microsoft.ProcessSimple/environments/{Uri.EscapeDataString(environmentName)}/flows/{Uri.EscapeDataString(flowName)}?api-version=2016-11-01";
			return await GetAsync(path, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> EnableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
		{
			var adminScope = asAdmin ? "/scopes/admin" : string.Empty;
			var path = $"/providers/Microsoft.ProcessSimple{adminScope}/environments/{Uri.EscapeDataString(environmentName)}/flows/{Uri.EscapeDataString(flowName)}/start?api-version=2016-11-01";
			return await PostAsync(path, null, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> DisableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
		{
			var adminScope = asAdmin ? "/scopes/admin" : string.Empty;
			var path = $"/providers/Microsoft.ProcessSimple{adminScope}/environments/{Uri.EscapeDataString(environmentName)}/flows/{Uri.EscapeDataString(flowName)}/stop?api-version=2016-11-01";
			return await PostAsync(path, null, cancellationToken).ConfigureAwait(false);
		}

		public async Task DeleteFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
		{
			var adminScope = asAdmin ? "/scopes/admin" : string.Empty;
			var path = $"/providers/Microsoft.ProcessSimple{adminScope}/environments/{Uri.EscapeDataString(environmentName)}/flows/{Uri.EscapeDataString(flowName)}?api-version=2016-11-01";
			await DeleteAsync(path, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> ExportFlowAsJsonAsync(string environmentName, string flowName, CancellationToken cancellationToken)
		{
			var path = $"/providers/Microsoft.ProcessSimple/environments/{Uri.EscapeDataString(environmentName)}/flows/{Uri.EscapeDataString(flowName)}/exportToARMTemplate?api-version=2016-11-01";
			using var request = await CreateRequestAsync(HttpMethod.Post, path, cancellationToken).ConfigureAwait(false);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			return await SendWithFactoryAsync(request, cancellationToken).ConfigureAwait(false);
		}

		public async Task<(byte[] content, string fileName)> ExportFlowAsZipAsync(string environmentName, string flowName, CancellationToken cancellationToken)
		{
			// Step 1: List package resources
			var listResourcesPath = $"/providers/Microsoft.BusinessAppPlatform/environments/{Uri.EscapeDataString(environmentName)}/listPackageResources?api-version=2016-11-01";
			var resourcesPayload = new { baseResourceIds = new[] { $"/providers/Microsoft.Flow/flows/{flowName}" } };

			var resourcesResult = await PostAsync(listResourcesPath, resourcesPayload, cancellationToken).ConfigureAwait(false);
			if (resourcesResult.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
			{
				var firstError = errors[0].GetString();
				throw new InvalidOperationException($"Failed to list package resources: {firstError}");
			}

			// Step 2: Get the flow definition to obtain display name
			var flowDef = await GetFlowAsync(environmentName, flowName, cancellationToken).ConfigureAwait(false);
			var displayName = flowDef.RootElement.TryGetProperty("properties", out var props) && props.TryGetProperty("displayName", out var dn)
				? dn.GetString() ?? flowName
				: flowName;

			// Step 3: Export package
			var exportPath = $"/providers/Microsoft.BusinessAppPlatform/environments/{Uri.EscapeDataString(environmentName)}/exportPackage?api-version=2016-11-01";
			var exportPayload = new
			{
				includedResourceIds = new[] { $"/providers/Microsoft.Flow/flows/{flowName}" },
				details = new
				{
					displayName = $"{displayName}_export",
					description = $"Export of {displayName}",
					creator = "pacx CLI",
					sourceEnvironment = environmentName
				},
				resources = new { }
			};

			var exportResult = await PostAsync(exportPath, exportPayload, cancellationToken).ConfigureAwait(false);
			var packageLink = exportResult.RootElement.GetProperty("packageLink").GetProperty("value").GetString()
				?? throw new InvalidOperationException("No package download link returned.");

			var fileName = ExtractFileName(packageLink, flowName);

			// Step 4: Download the zip
			var client = this.httpClientFactory.CreateClient();
			using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, packageLink);
			downloadRequest.Headers.Add("x-anonymous", "true");
			using var downloadResponse = await client.SendAsync(downloadRequest, cancellationToken).ConfigureAwait(false);
			downloadResponse.EnsureSuccessStatusCode();
			var content = await downloadResponse.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);

			return (content, fileName);
		}

		private static string BuildFlowsPath(string environmentName, bool asAdmin)
		{
			if (asAdmin)
			{
				return $"/providers/Microsoft.ProcessSimple/scopes/admin/environments/{Uri.EscapeDataString(environmentName)}/v2/flows";
			}
			return $"/providers/Microsoft.ProcessSimple/environments/{Uri.EscapeDataString(environmentName)}/flows";
		}

		private static string ExtractFileName(string url, string fallbackName)
		{
			var match = System.Text.RegularExpressions.Regex.Match(url, @"([^/]+\.zip)");
			return match.Success ? match.Groups[1].Value : $"{fallbackName}.zip";
		}

		public async Task<JsonDocument> ListFlowPermissionsAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
		{
			var adminScope = asAdmin ? "/scopes/admin" : string.Empty;
			var path = $"/providers/Microsoft.ProcessSimple{adminScope}/environments/{Uri.EscapeDataString(environmentName)}/flows/{Uri.EscapeDataString(flowName)}/permissions?api-version=2016-11-01";
			return await GetAsync(path, cancellationToken).ConfigureAwait(false);
		}

		public async Task ModifyFlowPermissionsAsync(string environmentName, string flowName, object putPrincipals, object deletePrincipals, bool asAdmin, CancellationToken cancellationToken)
		{
			var adminScope = asAdmin ? "/scopes/admin" : string.Empty;
			var path = $"/providers/Microsoft.ProcessSimple{adminScope}/environments/{Uri.EscapeDataString(environmentName)}/flows/{Uri.EscapeDataString(flowName)}/modifyPermissions?api-version=2016-11-01";

			var body = new Dictionary<string, object>();
			if (putPrincipals != null)
				body["put"] = putPrincipals;
			if (deletePrincipals != null)
				body["delete"] = deletePrincipals;

			await SendVoidAsync(HttpMethod.Post, path, body, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> ListEnvironmentsAsync(CancellationToken cancellationToken)
		{
			var path = $"/providers/Microsoft.ProcessSimple/environments?api-version=2016-11-01";
			return await GetAsync(path, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> GetEnvironmentAsync(string environmentName, CancellationToken cancellationToken)
		{
			var path = $"/providers/Microsoft.ProcessSimple/environments/{Uri.EscapeDataString(environmentName)}?api-version=2016-11-01";
			return await GetAsync(path, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> ListRecycleBinFlowsAsync(string environmentName, CancellationToken cancellationToken)
		{
			var path = $"/providers/Microsoft.ProcessSimple/scopes/admin/environments/{Uri.EscapeDataString(environmentName)}/v2/flows?api-version=2016-11-01&include=softDeletedFlows";
			return await GetAsync(path, cancellationToken).ConfigureAwait(false);
		}

		public async Task RestoreRecycleBinFlowAsync(string environmentName, string flowName, CancellationToken cancellationToken)
		{
			var path = $"/providers/Microsoft.ProcessSimple/scopes/admin/environments/{Uri.EscapeDataString(environmentName)}/flows/{Uri.EscapeDataString(flowName)}/restore?api-version=2016-11-01";
			await SendVoidAsync(HttpMethod.Post, path, null, cancellationToken).ConfigureAwait(false);
		}

		private Task<JsonDocument> GetAsync(string path, CancellationToken cancellationToken)
			=> SendAsync(HttpMethod.Get, path, null, cancellationToken);

		private Task<JsonDocument> PostAsync(string path, object? payload, CancellationToken cancellationToken)
			=> SendAsync(HttpMethod.Post, path, payload, cancellationToken);

		private Task DeleteAsync(string path, CancellationToken cancellationToken)
			=> SendVoidAsync(HttpMethod.Delete, path, null, cancellationToken);

		private async Task<JsonDocument> SendAsync(HttpMethod method, string path, object? payload, CancellationToken cancellationToken)
		{
			using var request = await CreateRequestAsync(method, path, cancellationToken).ConfigureAwait(false);
			if (payload != null)
			{
				request.Content = new StringContent(JsonSerializer.Serialize(payload, SerializerOptions), System.Text.Encoding.UTF8, "application/json");
			}
			return await SendWithFactoryAsync(request, cancellationToken).ConfigureAwait(false);
		}

		private async Task SendVoidAsync(HttpMethod method, string path, object? payload, CancellationToken cancellationToken)
		{
			using var request = await CreateRequestAsync(method, path, cancellationToken).ConfigureAwait(false);
			if (payload != null)
			{
				request.Content = new StringContent(JsonSerializer.Serialize(payload, SerializerOptions), System.Text.Encoding.UTF8, "application/json");
			}
			var client = this.httpClientFactory.CreateClient();
			using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
			var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Power Automate API error ({response.StatusCode}): {RedactErrorContent(content)}");
			}
		}

		private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string path, CancellationToken cancellationToken)
		{
			var token = await this.tokenProvider.GetTokenAsync(Resource, cancellationToken).ConfigureAwait(false);
			if (string.IsNullOrWhiteSpace(token))
			{
				throw new InvalidOperationException("Unable to acquire a Power Automate access token from the current connection.");
			}

			var normalizedPath = path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
			var request = new HttpRequestMessage(method, FlowBaseUrl + normalizedPath);
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
				throw new InvalidOperationException($"Power Automate API error ({response.StatusCode}): {RedactErrorContent(content)}");
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

