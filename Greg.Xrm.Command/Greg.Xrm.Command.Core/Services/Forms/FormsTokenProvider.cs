using System.Net.Http.Headers;
using System.Text.Json;

namespace Greg.Xrm.Command.Services.Forms
{
	public interface IFormsTokenProvider
	{
		Task<string> GetTokenAsync(string tenantId, CancellationToken ct);
	}

	public sealed class FormsTokenProvider : IFormsTokenProvider
	{
		private string? cachedToken;
		private DateTimeOffset tokenExpiry;

		public async Task<string> GetTokenAsync(string tenantId, CancellationToken ct)
		{
			if (!string.IsNullOrEmpty(cachedToken) && DateTimeOffset.UtcNow < tokenExpiry)
				return cachedToken;

			var clientId = Environment.GetEnvironmentVariable("MSAL_CLIENT_ID");
			var clientSecret = Environment.GetEnvironmentVariable("MSAL_CLIENT_SECRET");

			if (string.IsNullOrWhiteSpace(clientId))
				throw new InvalidOperationException("MSAL_CLIENT_ID environment variable is required.");

			using var httpClient = new HttpClient();
			var body = new Dictionary<string, string>
			{
				["client_id"] = clientId,
				["scope"] = "https://forms.office.com/.default",
				["grant_type"] = "client_credentials"
			};

			if (!string.IsNullOrWhiteSpace(clientSecret))
				body["client_secret"] = clientSecret;

			var username = Environment.GetEnvironmentVariable("MSAL_USERNAME");
			var password = Environment.GetEnvironmentVariable("MSAL_PASSWORD");
			if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
			{
				body["grant_type"] = "password";
				body["username"] = username;
				body["password"] = password;
			}

			var response = await httpClient.PostAsync(
				$"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token",
				new FormUrlEncodedContent(body), ct);

			var json = await response.Content.ReadAsStringAsync(ct);
			if (!response.IsSuccessStatusCode)
				throw new InvalidOperationException($"Token acquisition failed: {json}");

			using var doc = JsonDocument.Parse(json);
			var root = doc.RootElement;
			cachedToken = root.GetProperty("access_token").GetString()!;
			var expiresIn = root.GetProperty("expires_in").GetInt32();
			tokenExpiry = DateTimeOffset.UtcNow.AddSeconds(expiresIn - 60);

			return cachedToken;
		}
	}
}
