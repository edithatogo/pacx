using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Greg.Xrm.Command.Services.Forms
{
	public sealed class FormsApiClient(IFormsTokenProvider tokenProvider, IHttpClientFactory httpClientFactory) : IFormsApiClient
	{
		private const string FormsApiBase = "https://forms.office.com/formapi/api";
		private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
		private static readonly JsonSerializerOptions PrettyOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };
		private static readonly Regex TenantIdPattern = new(@"^[0-9a-fA-F-]{36}$", RegexOptions.Compiled);
		private static readonly Regex FormIdPattern = new(@"^[a-zA-Z0-9-]+$", RegexOptions.Compiled);

		private readonly IFormsTokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public async Task<IReadOnlyList<FormsForm>> GetFormsAsync(string tenantId, string ownerId, string ownerType, CancellationToken ct)
		{
			var ctx = OwnerContext(ownerType);
			var json = await GetAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/{ctx}/{ValidateAndEscape(ownerId, null, nameof(ownerId))}/light/forms?$select=id,status,title,createdDate,modifiedDate,ownerId,version,softDeleted,type", ct);
			return DeserializeList<FormsForm>(json);
		}

		public async Task<FormsForm?> GetFormDetailAsync(string tenantId, string ownerId, string ownerType, string formId, CancellationToken ct)
		{
			var ctx = OwnerContext(ownerType);
			var json = await GetAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/{ctx}/{ValidateAndEscape(ownerId, null, nameof(ownerId))}/light/forms('{ValidateAndEscape(formId, FormIdPattern, nameof(formId))}')?$select=rowCount,id,status,title,createdDate,modifiedDate,ownerId", ct);
			return JsonSerializer.Deserialize<FormsForm>(json, JsonOptions);
		}

		public async Task<int> GetResponseCountAsync(string tenantId, string ownerId, string ownerType, string formId, CancellationToken ct)
		{
			var detail = await GetFormDetailAsync(tenantId, ownerId, ownerType, formId, ct);
			return detail?.RowCount ?? 0;
		}

		public async Task<IReadOnlyList<FormsResponse>> GetResponsesAsync(string tenantId, string ownerId, string ownerType, string formId, int top, int skip, CancellationToken ct)
		{
			var ctx = OwnerContext(ownerType);
			var json = await GetAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/{ctx}/{ValidateAndEscape(ownerId, null, nameof(ownerId))}/light/forms('{ValidateAndEscape(formId, FormIdPattern, nameof(formId))}')/responses?$expand=comments&$top={top}&$skip={skip}", ct);
			return DeserializeList<FormsResponse>(json);
		}

		public async Task<FormsResponse?> GetResponseAsync(string tenantId, string ownerId, string ownerType, string formId, int responseId, CancellationToken ct)
		{
			var ctx = OwnerContext(ownerType);
			var json = await GetAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/{ctx}/{ValidateAndEscape(ownerId, null, nameof(ownerId))}/light/forms('{ValidateAndEscape(formId, FormIdPattern, nameof(formId))}')/responses?$filter=id eq {responseId}", ct);
			var list = DeserializeList<FormsResponse>(json);
			return list.FirstOrDefault();
		}

		public async Task<JsonDocument?> GetBranchingAsync(string tenantId, string ownerId, string ownerType, string formId, CancellationToken ct)
		{
			var ctx = OwnerContext(ownerType);
			var json = await GetAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/{ctx}/{ValidateAndEscape(ownerId, null, nameof(ownerId))}/light/forms('{ValidateAndEscape(formId, FormIdPattern, nameof(formId))}')/branching", ct);
			return JsonDocument.Parse(json);
		}

		public async Task<JsonDocument?> GetAnalyticsAsync(string tenantId, string ownerId, string ownerType, string formId, CancellationToken ct)
		{
			var ctx = OwnerContext(ownerType);
			var json = await GetAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/{ctx}/{ValidateAndEscape(ownerId, null, nameof(ownerId))}/light/forms('{ValidateAndEscape(formId, FormIdPattern, nameof(formId))}')/analytics", ct);
			return JsonDocument.Parse(json);
		}

		public async Task ShareFormAsync(string tenantId, string ownerId, string ownerType, string formId, string groupId, string role, CancellationToken ct)
		{
			var ctx = OwnerContext(ownerType);
			var payload = new { groupId, role };
			await PostAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/{ctx}/{ValidateAndEscape(ownerId, null, nameof(ownerId))}/light/forms('{ValidateAndEscape(formId, FormIdPattern, nameof(formId))}')/permissions", payload, ct);
		}

		public async Task TransferOwnershipAsync(string tenantId, string ownerId, string ownerType, string formId, string targetUpn, CancellationToken ct)
		{
			var ctx = OwnerContext(ownerType);
			var payload = new { targetUserPrincipalName = targetUpn };
			await PostAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/{ctx}/{ValidateAndEscape(ownerId, null, nameof(ownerId))}/light/forms('{ValidateAndEscape(formId, FormIdPattern, nameof(formId))}')/owner", payload, ct);
		}

		public async Task<IReadOnlyList<FormsForm>> ListTemplatesAsync(string tenantId, CancellationToken ct)
		{
			var json = await GetAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/templates", ct);
			return DeserializeList<FormsForm>(json);
		}

		public async Task CreateTemplateAsync(string tenantId, string formId, string scope, CancellationToken ct)
		{
			var payload = new { formId, scope };
			await PostAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/templates", payload, ct);
		}

		public async Task ShareTemplateAsync(string tenantId, string templateId, string groupId, CancellationToken ct)
		{
			var payload = new { groupId };
			await PostAsync($"{FormsApiBase}/{ValidateAndEscape(tenantId, TenantIdPattern, nameof(tenantId))}/templates('{ValidateAndEscape(templateId, null, nameof(templateId))}')/shares", payload, ct);
		}

		private async Task<string> GetAsync(string url, CancellationToken ct)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, url);
			await AttachTokenAsync(request, ct);
			return await SendAsync(request, ct);
		}

		private async Task PostAsync(string url, object payload, CancellationToken ct)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = new StringContent(JsonSerializer.Serialize(payload, PrettyOptions), System.Text.Encoding.UTF8, "application/json")
			};
			await AttachTokenAsync(request, ct);
			await SendAsync(request, ct);
		}

		private async Task AttachTokenAsync(HttpRequestMessage request, CancellationToken ct)
		{
			var tenantId = ExtractTenantId(request.RequestUri?.AbsoluteUri ?? "");
			var token = await tokenProvider.GetTokenAsync(tenantId, ct);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		private async Task<string> SendAsync(HttpRequestMessage request, CancellationToken ct)
		{
			var client = httpClientFactory.CreateClient();
			using var response = await client.SendAsync(request, ct);
			var content = await response.Content.ReadAsStringAsync(ct);
			if (!response.IsSuccessStatusCode)
				throw new InvalidOperationException($"Forms API error ({response.StatusCode}): {RedactError(content)}");
			return content;
		}

		private static List<T> DeserializeList<T>(string json)
		{
			var wrapper = JsonSerializer.Deserialize<FormsValueWrapper<T>>(json, JsonOptions);
			return wrapper?.Value ?? [];
		}

		private static string ValidateAndEscape(string value, Regex? pattern, string paramName)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
			if (pattern != null && !pattern.IsMatch(value))
				throw new ArgumentException($"{paramName} '{value}' has an invalid format.", paramName);
			return Uri.EscapeDataString(value);
		}

		private static string RedactError(string content)
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

		private static string ExtractTenantId(string url)
		{
			var parts = url.Split('/');
			return parts.Length > 4 ? parts[4] : string.Empty;
		}

		private static string OwnerContext(string ownerType)
			=> string.Equals(ownerType, "Group", StringComparison.OrdinalIgnoreCase) ? "groups" : "users";

		private sealed class FormsValueWrapper<T>
		{
			public List<T>? Value { get; set; }
		}
	}
}

