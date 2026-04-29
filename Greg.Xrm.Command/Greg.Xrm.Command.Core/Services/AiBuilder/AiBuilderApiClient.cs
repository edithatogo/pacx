using Greg.Xrm.Command.Services.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Services.AiBuilder
{
	public class AiBuilderApiClient : IAiBuilderApiClient
	{
		private const int MaxRetryAttempts = 3;
		private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromSeconds(1);
		private readonly ServiceClient _serviceClient;
		private readonly ITokenProvider _tokenProvider;
		private readonly IHttpClientFactory _httpClientFactory;
		private string? _baseUrl;

		public AiBuilderApiClient(ServiceClient serviceClient, ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory)
		{
			_serviceClient = serviceClient ?? throw new ArgumentNullException(nameof(serviceClient));
			_tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		}

		private async Task<string> GetBaseUrlAsync(CancellationToken ct)
		{
			if (_baseUrl != null) return _baseUrl;

			var whoAmI = await _serviceClient.ExecuteAsync(new WhoAmIRequest(), ct).ConfigureAwait(false) as WhoAmIResponse;
			if (whoAmI is null)
			{
				throw new InvalidOperationException("Unable to determine the current Dataverse user.");
			}

			var orgDetail = await _serviceClient.RetrieveAsync(
				"organization",
				whoAmI.UserId,
				new ColumnSet("organizationid", "weburl", "friendlyname"),
				ct).ConfigureAwait(false);
			if (orgDetail is null)
			{
				throw new InvalidOperationException("Unable to retrieve the current organization details.");
			}

			var webUrl = orgDetail.GetAttributeValue<string>("weburl");
			if (string.IsNullOrEmpty(webUrl))
			{
				throw new InvalidOperationException("Could not determine organization Web URL. Please ensure the environment is properly configured.");
			}

			_baseUrl = webUrl.TrimEnd('/');
			return _baseUrl;
		}

		public async Task<IEnumerable<AiModelInfo>> ListModelsAsync(CancellationToken ct = default)
		{
			var query = new QueryExpression("aimodel");
			query.ColumnSet.AddColumns(
				"aimodelid",
				"name",
				"statuscode",
				"createdon",
				"description",
				"versionnumber");
			query.AddOrder("createdon", OrderType.Descending);

			var result = await _serviceClient.RetrieveMultipleAsync(query, ct).ConfigureAwait(false);
			var models = new List<AiModelInfo>();

			foreach (var entity in result.Entities)
			{
				var statusCode = entity.GetAttributeValue<int?>("statuscode") ?? 0;
				models.Add(new AiModelInfo
				{
					Id = entity.GetAttributeValue<Guid?>("aimodelid")?.ToString() ?? "",
					Name = entity.GetAttributeValue<string>("name") ?? "",
					Status = GetStatusText(statusCode),
					CreatedOn = entity.GetAttributeValue<DateTime?>("createdon")
				});
			}

			return models;
		}

		public async Task TrainModelAsync(string modelId, bool wait, CancellationToken ct = default)
		{
			await TrainModelAsync(modelId, wait, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(60), ct).ConfigureAwait(false);
		}

		public async Task TrainModelAsync(string modelId, bool wait, TimeSpan pollInterval, TimeSpan timeout, CancellationToken ct = default)
		{
			var baseUrl = await GetBaseUrlAsync(ct).ConfigureAwait(false);
			var aiBuilderUrl = BuildAiBuilderUrl(baseUrl);

			var token = await GetAccessTokenAsync(aiBuilderUrl, ct).ConfigureAwait(false);

			using var httpClient = _httpClientFactory.CreateClient();
			var response = await SendWithRetryAsync(
				httpClient,
				HttpMethod.Post,
				$"{aiBuilderUrl}/models/{modelId}/train",
				token,
				ct).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
				throw new InvalidOperationException($"Failed to train model: {response.StatusCode} - {error}");
			}

			if (wait)
			{
				await PollForTrainingComplete(modelId, aiBuilderUrl, token, pollInterval, timeout, ct).ConfigureAwait(false);
			}
		}

		public async Task PublishModelAsync(string modelId, CancellationToken ct = default)
		{
			var baseUrl = await GetBaseUrlAsync(ct).ConfigureAwait(false);
			var aiBuilderUrl = BuildAiBuilderUrl(baseUrl);

			var token = await GetAccessTokenAsync(aiBuilderUrl, ct).ConfigureAwait(false);

			using var httpClient = _httpClientFactory.CreateClient();
			var response = await SendWithRetryAsync(
				httpClient,
				HttpMethod.Post,
				$"{aiBuilderUrl}/models/{modelId}/publish",
				token,
				ct).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
				throw new InvalidOperationException($"Failed to publish model: {response.StatusCode} - {error}");
			}
		}

		public async Task ConfigureFormProcessorAsync(string modelId, string documentType, string[]? fields, string[]? tables, CancellationToken ct = default)
		{
			var baseUrl = await GetBaseUrlAsync(ct).ConfigureAwait(false);
			var aiBuilderUrl = BuildAiBuilderUrl(baseUrl);

			var token = await GetAccessTokenAsync(aiBuilderUrl, ct).ConfigureAwait(false);

			var config = new
			{
				documentType = documentType,
				fields = fields?.ToList() ?? new List<string>(),
				tables = tables?.ToList() ?? new List<string>()
			};

			var json = JsonSerializer.Serialize(config);
			using var httpClient = _httpClientFactory.CreateClient();
			var response = await SendWithRetryAsync(
				httpClient,
				HttpMethod.Patch,
				$"{aiBuilderUrl}/models/{modelId}/formprocessor/config",
				token,
				() => new StringContent(json, Encoding.UTF8, "application/json"),
				ct).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
				throw new InvalidOperationException($"Failed to configure form processor: {response.StatusCode} - {error}");
			}
		}

		private async Task<string> GetAccessTokenAsync(string resource, CancellationToken ct)
		{
			var token = await _tokenProvider.GetTokenAsync(resource, ct).ConfigureAwait(false);
			if (string.IsNullOrEmpty(token))
			{
				throw new InvalidOperationException($"Failed to acquire token for resource: {resource}");
			}
			return token;
		}

		internal static string BuildAiBuilderUrl(string baseUrl)
		{
			var uri = new Uri(baseUrl);
			var authority = uri.Authority;
			
			if (authority.Contains("dynamics.com"))
			{
				var parts = authority.Split('.');
				var environment = parts[0];
				var region = parts[1];
				return $"https://{environment}.api.{region}.powerplatform.com/aiBuilder";
			}
			
			return $"{baseUrl}/api/aiBuilder";
		}

		internal static async Task<HttpResponseMessage> SendWithRetryAsync(
			HttpClient httpClient,
			HttpMethod method,
			string requestUri,
			string token,
			CancellationToken ct = default)
		{
			return await SendWithRetryAsync(httpClient, method, requestUri, token, null, Task.Delay, ct).ConfigureAwait(false);
		}

		internal static async Task<HttpResponseMessage> SendWithRetryAsync(
			HttpClient httpClient,
			HttpMethod method,
			string requestUri,
			string token,
			Func<HttpContent?>? contentFactory,
			CancellationToken ct = default)
		{
			return await SendWithRetryAsync(httpClient, method, requestUri, token, contentFactory, Task.Delay, ct).ConfigureAwait(false);
		}

		internal static async Task<HttpResponseMessage> SendWithRetryAsync(
			HttpClient httpClient,
			HttpMethod method,
			string requestUri,
			string token,
			Func<HttpContent?>? contentFactory,
			Func<TimeSpan, CancellationToken, Task> delayAsync,
			CancellationToken ct = default)
		{
			if (httpClient is null) throw new ArgumentNullException(nameof(httpClient));
			if (method is null) throw new ArgumentNullException(nameof(method));
			if (string.IsNullOrWhiteSpace(requestUri)) throw new ArgumentException("Request URI is required.", nameof(requestUri));
			if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Access token is required.", nameof(token));
			if (delayAsync is null) throw new ArgumentNullException(nameof(delayAsync));

			for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
			{
				using var request = new HttpRequestMessage(method, requestUri);
				request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				if (contentFactory is not null)
				{
					request.Content = contentFactory();
				}

				var response = await httpClient.SendAsync(request, ct).ConfigureAwait(false);
				if (!IsTransientStatusCode(response.StatusCode) || attempt == MaxRetryAttempts)
				{
					return response;
				}

				var delay = GetRetryDelay(response, attempt);
				response.Dispose();
				await delayAsync(delay, ct).ConfigureAwait(false);
			}

			throw new InvalidOperationException("Retry loop exited unexpectedly.");
		}

		internal static TimeSpan GetRetryDelay(HttpResponseMessage response, int attempt)
		{
			if (response.Headers.RetryAfter?.Delta is TimeSpan delta && delta > TimeSpan.Zero)
			{
				return delta;
			}

			if (response.Headers.RetryAfter?.Date is DateTimeOffset retryAfterDate)
			{
				var remaining = retryAfterDate - DateTimeOffset.UtcNow;
				if (remaining > TimeSpan.Zero)
				{
					return remaining;
				}
			}

			var retrySeconds = Math.Min(30, InitialRetryDelay.TotalSeconds * Math.Pow(2, attempt - 1));
			return TimeSpan.FromSeconds(Math.Max(1, retrySeconds));
		}

		internal static bool IsTransientStatusCode(HttpStatusCode statusCode)
		{
			var code = (int)statusCode;
			return code == 408 || code == 429 || (code >= 500 && code <= 599);
		}

		private async Task PollForTrainingComplete(
			string modelId,
			string aiBuilderUrl,
			string token,
			TimeSpan pollInterval,
			TimeSpan timeout,
			CancellationToken ct)
		{
			if (pollInterval <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException(nameof(pollInterval), "Poll interval must be greater than zero.");
			}

			if (timeout <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than zero.");
			}

			var startedAt = DateTimeOffset.UtcNow;
			while (DateTimeOffset.UtcNow - startedAt < timeout)
			{
				using var httpClient = _httpClientFactory.CreateClient();
				var response = await SendWithRetryAsync(
					httpClient,
					HttpMethod.Get,
					$"{aiBuilderUrl}/models/{modelId}",
					token,
					ct).ConfigureAwait(false);

				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
					var doc = JsonDocument.Parse(json);
					var status = doc.RootElement.GetProperty("status").GetString();

					if (status == "Published" || status == "Ready")
					{
						return;
					}

					if (status == "Failed")
					{
						var error = doc.RootElement.TryGetProperty("errorMessage", out var errorMsg) 
							? errorMsg.GetString() 
							: "Unknown error";
						throw new InvalidOperationException($"Training failed: {error}");
					}
				}

				await Task.Delay(pollInterval, ct).ConfigureAwait(false);
			}

			throw new TimeoutException($"Training polling timed out after {timeout.TotalSeconds:0} seconds.");
		}

		private static string GetStatusText(int statusCode) => statusCode switch
		{
			0 => "Draft",
			1 => "Training",
			2 => "Trained",
			3 => "Compiled",
			4 => "Ready",
			5 => "Published",
			192350000 => "Not Started",
			192350001 => "Training",
			192350002 => "Training Complete",
			192350003 => "Training Failed",
			192350004 => "Publishing",
			192350005 => "Published",
			192350006 => "Publish Failed",
			_ => $"Unknown ({statusCode})"
		};
	}
}
