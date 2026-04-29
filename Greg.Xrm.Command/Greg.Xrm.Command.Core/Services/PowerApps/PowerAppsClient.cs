using System.Net.Http.Headers;
using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;

namespace Greg.Xrm.Command.Services.PowerApps
{
	public class PowerAppsClient(ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory) : IPowerAppsClient
	{
		private const string Resource = "https://management.azure.com";
		private const string BaseUrl = "https://management.azure.com";
		private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
		{
			WriteIndented = true
		};

		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public async Task<JsonDocument> ListAppsAsync(string? environmentName, bool asAdmin, CancellationToken cancellationToken)
		{
			var path = BuildAppPath(null, environmentName, asAdmin);
			return await GetAsync($"{path}?api-version=2017-08-01", cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> GetAppAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken)
		{
			var path = BuildAppPath(appName, environmentName, asAdmin);
			return await GetAsync($"{path}?api-version=2016-11-01", cancellationToken).ConfigureAwait(false);
		}

		public async Task DeleteAppAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken)
		{
			var path = BuildAppPath(appName, environmentName, asAdmin);
			await DeleteAsync($"{path}?api-version=2017-08-01", cancellationToken).ConfigureAwait(false);
		}

		public async Task<(byte[] content, string fileName)> ExportAppAsync(string appName, string environmentName, CancellationToken cancellationToken)
		{
			// Step 1: List package resources
			var listResourcesPath = $"/providers/Microsoft.BusinessAppPlatform/environments/{Uri.EscapeDataString(environmentName)}/listPackageResources?api-version=2016-11-01";
			var resourcesPayload = new { baseResourceIds = new[] { $"/providers/Microsoft.PowerApps/apps/{appName}" } };
			var resourcesResult = await PostAsync(listResourcesPath, resourcesPayload, cancellationToken).ConfigureAwait(false);

			if (resourcesResult.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
			{
				var firstError = errors[0].GetString();
				throw new InvalidOperationException($"Failed to list package resources: {firstError}");
			}

			// Step 2: Initiate export and capture the Location header (polling URL)
			var exportPath = $"/providers/Microsoft.BusinessAppPlatform/environments/{Uri.EscapeDataString(environmentName)}/exportPackage?api-version=2016-11-01";
			var exportPayload = new
			{
				includedResourceIds = new[] { $"/providers/Microsoft.PowerApps/apps/{appName}" },
				details = new
				{
					displayName = $"{appName}_export",
					description = $"Export of Power App {appName}",
					creator = "pacx CLI",
					sourceEnvironment = environmentName
				},
				resources = new { }
			};

			using var exportRequest = await CreateRequestAsync(HttpMethod.Post, exportPath, cancellationToken).ConfigureAwait(false);
			exportRequest.Content = new StringContent(JsonSerializer.Serialize(exportPayload, SerializerOptions), System.Text.Encoding.UTF8, "application/json");
			var httpClient = this.httpClientFactory.CreateClient();
			using var exportResponse = await httpClient.SendAsync(exportRequest, cancellationToken).ConfigureAwait(false);
			var exportContent = await exportResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
			if (!exportResponse.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Failed to initiate export: {exportContent}");
			}

			var pollingUrl = exportResponse.Headers.Location?.ToString()
				?? throw new InvalidOperationException("No polling URL returned for export.");

			// Step 3: Poll until the export completes
			string packageLink;
			using (var pollClient = this.httpClientFactory.CreateClient())
			{
				while (true)
				{
					using var pollResponse = await pollClient.GetAsync(pollingUrl, cancellationToken).ConfigureAwait(false);
					var pollBody = JsonDocument.Parse(await pollResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));

					var status = pollBody.RootElement
						.GetProperty("properties")
						.GetProperty("status")
						.GetString();

					if (status == "Succeeded")
					{
						packageLink = pollBody.RootElement
							.GetProperty("properties")
							.GetProperty("packageLink")
							.GetProperty("value")
							.GetString()
							?? throw new InvalidOperationException("No package download link returned.");
						break;
					}
					if (status == "Failed")
					{
						throw new InvalidOperationException("Export failed.");
					}

					await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
				}
			}

			var fileName = ExtractFileName(packageLink, appName);

			// Step 4: Download the zip
			using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, packageLink);
			downloadRequest.Headers.Add("x-anonymous", "true");
			using var downloadResponse = await httpClient.SendAsync(downloadRequest, cancellationToken).ConfigureAwait(false);
			downloadResponse.EnsureSuccessStatusCode();
			var content = await downloadResponse.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);

			return (content, fileName);
		}

		public async Task SetAppConsentAsync(string appName, string environmentName, bool bypass, CancellationToken cancellationToken)
		{
			var path = $"/providers/Microsoft.PowerApps/scopes/admin/environments/{Uri.EscapeDataString(environmentName)}/apps/{Uri.EscapeDataString(appName)}/setPowerAppConnectionDirectConsentBypass?api-version=2021-02-01";
			await SendVoidAsync(HttpMethod.Post, path, new { bypassconsent = bypass }, cancellationToken).ConfigureAwait(false);
		}

		public async Task SetAppOwnerAsync(string appName, string environmentName, string newOwnerId, string? roleForOldOwner, CancellationToken cancellationToken)
		{
			var path = $"/providers/Microsoft.PowerApps/scopes/admin/environments/{Uri.EscapeDataString(environmentName)}/apps/{Uri.EscapeDataString(appName)}/modifyAppOwner?api-version=2022-11-01";
			var body = new Dictionary<string, object>
			{
				["newAppOwner"] = newOwnerId
			};
			if (!string.IsNullOrWhiteSpace(roleForOldOwner))
			{
				body["roleForOldAppOwner"] = roleForOldOwner;
			}
			await SendVoidAsync(HttpMethod.Post, path, body, cancellationToken).ConfigureAwait(false);
		}

		public async Task<JsonDocument> ListAppPermissionsAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken)
		{
			var path = BuildAppPermissionPath(appName, environmentName, asAdmin);
			return await GetAsync($"{path}?api-version=2022-11-01", cancellationToken).ConfigureAwait(false);
		}

		public async Task AddAppPermissionAsync(string appName, string? environmentName, bool asAdmin, string principalId, string principalType, string roleName, CancellationToken cancellationToken)
		{
			var path = BuildAppPermissionModifyPath(appName, environmentName, asAdmin);
			var body = new
			{
				put = new[]
				{
					new
					{
						properties = new
						{
							principal = new
							{
								id = principalId,
								type = principalType
							},
							roleName
						}
					}
				}
			};
			await SendVoidAsync(HttpMethod.Post, path, body, cancellationToken).ConfigureAwait(false);
		}

		public async Task RemoveAppPermissionAsync(string appName, string? environmentName, bool asAdmin, string principalId, CancellationToken cancellationToken)
		{
			var path = BuildAppPermissionModifyPath(appName, environmentName, asAdmin);
			var body = new
			{
				delete = new[]
				{
					new { id = principalId }
				}
			};
			await SendVoidAsync(HttpMethod.Post, path, body, cancellationToken).ConfigureAwait(false);
		}

		private static string BuildAppPath(string? appName, string? environmentName, bool asAdmin)
		{
			var path = "/providers/Microsoft.PowerApps";
			if (asAdmin)
			{
				path += "/scopes/admin";
				if (!string.IsNullOrWhiteSpace(environmentName))
				{
					path += $"/environments/{Uri.EscapeDataString(environmentName)}";
				}
			}
			path += "/apps";
			if (!string.IsNullOrWhiteSpace(appName))
			{
				path += $"/{Uri.EscapeDataString(appName)}";
			}
			return path;
		}

		private static string BuildAppPermissionPath(string appName, string? environmentName, bool asAdmin)
		{
			if (asAdmin)
			{
				if (!string.IsNullOrWhiteSpace(environmentName))
				{
					return $"/providers/Microsoft.PowerApps/scopes/admin/environments/{Uri.EscapeDataString(environmentName)}/apps/{Uri.EscapeDataString(appName)}/permissions";
				}
				return $"/providers/Microsoft.PowerApps/scopes/admin/apps/{Uri.EscapeDataString(appName)}/permissions";
			}
			return $"/providers/Microsoft.PowerApps/apps/{Uri.EscapeDataString(appName)}/permissions";
		}

		private static string BuildAppPermissionModifyPath(string appName, string? environmentName, bool asAdmin)
		{
			if (asAdmin && !string.IsNullOrWhiteSpace(environmentName))
			{
				return $"/providers/Microsoft.PowerApps/scopes/admin/environments/{Uri.EscapeDataString(environmentName)}/apps/{Uri.EscapeDataString(appName)}/modifyPermissions";
			}
			return $"/providers/Microsoft.PowerApps/apps/{Uri.EscapeDataString(appName)}/modifyPermissions";
		}

		private static string ExtractFileName(string url, string fallbackName)
		{
			var match = System.Text.RegularExpressions.Regex.Match(url, @"([^/]+\.zip)");
			return match.Success ? match.Groups[1].Value : $"{fallbackName}.zip";
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
				throw new InvalidOperationException($"Power Apps API error ({response.StatusCode}): {content}");
			}
		}

		private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string path, CancellationToken cancellationToken)
		{
			var token = await this.tokenProvider.GetTokenAsync(Resource, cancellationToken).ConfigureAwait(false);
			if (string.IsNullOrWhiteSpace(token))
			{
				throw new InvalidOperationException("Unable to acquire a Power Apps access token from the current connection.");
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
				throw new InvalidOperationException($"Power Apps API error ({response.StatusCode}): {content}");
			}

			return JsonDocument.Parse(string.IsNullOrWhiteSpace(content) ? "{}" : content);
		}

	}
}
