using Greg.Xrm.Command.Services.Output;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Services.Connection
{
	public class TokenProvider(
		IOrganizationServiceRepository organizationServiceRepository,
		IOutput output) : ITokenProvider
	{
		private readonly IOrganizationServiceRepository organizationServiceRepository = organizationServiceRepository ?? throw new ArgumentNullException(nameof(organizationServiceRepository));
		private readonly IOutput output = output ?? throw new ArgumentNullException(nameof(output));

		public async Task<string?> GetTokenAsync(string resource, CancellationToken cancellationToken = default)
		{
			var connectionString = await this.organizationServiceRepository.GetConnectionStringAsync().ConfigureAwait(false);
			if (string.IsNullOrEmpty(connectionString)) return null;

			var parts = connectionString.Split(';')
				.Select(x => x.Split('='))
				.Where(x => x.Length == 2)
				.ToDictionary(x => x[0].Trim(), x => x[1].Trim(), StringComparer.OrdinalIgnoreCase);

			if (!parts.TryGetValue("AuthType", out var authType)) return null;

			if (authType.Equals("ClientSecret", StringComparison.OrdinalIgnoreCase))
			{
				return await GetTokenViaClientSecretAsync(parts, resource, cancellationToken).ConfigureAwait(false);
			}

			if (authType.Equals("OAuth", StringComparison.OrdinalIgnoreCase))
			{
				return await GetTokenViaOAuthAsync(parts, resource, cancellationToken).ConfigureAwait(false);
			}

			return null;
		}

		private async Task<string?> GetTokenViaClientSecretAsync(Dictionary<string, string> parts, string resource, CancellationToken cancellationToken)
		{
			if (!parts.TryGetValue("ClientId", out var clientId)) return null;
			if (!parts.TryGetValue("ClientSecret", out var clientSecret)) return null;

			string tenantId = "common";
			if (parts.TryGetValue("TenantId", out var tid)) tenantId = tid;

			var authority = $"https://login.microsoftonline.com/{tenantId}";
			var scopes = BuildScopes(resource);

			var app = ConfidentialClientApplicationBuilder.Create(clientId)
				.WithClientSecret(clientSecret)
				.WithAuthority(new Uri(authority))
				.Build();

			try
			{
				var result = await app.AcquireTokenForClient(scopes).ExecuteAsync(cancellationToken).ConfigureAwait(false);
				return result.AccessToken;
			}
			catch (Exception ex)
			{
				output.WriteLine($"Error acquiring token via ClientSecret: {ex.Message}", ConsoleColor.Yellow);
				return null;
			}
		}

		private async Task<string?> GetTokenViaOAuthAsync(Dictionary<string, string> parts, string resource, CancellationToken cancellationToken)
		{
			// Default PAC client ID if not specified
			string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
			if (parts.TryGetValue("ClientId", out var cid)) clientId = cid;

			string tenantId = "common";
			if (parts.TryGetValue("TenantId", out var tid)) tenantId = tid;

			var authority = $"https://login.microsoftonline.com/{tenantId}";
			var scopes = BuildScopes(resource);

			var app = PublicClientApplicationBuilder.Create(clientId)
				.WithAuthority(new Uri(authority))
				.WithRedirectUri("http://localhost")
				.Build();

			// In a real implementation, we should attach the MSAL cache from TokenCacheStorePath
			// For now, we attempt silent acquisition which will likely fail if not cached in this process
			try
			{
				var accounts = await app.GetAccountsAsync().ConfigureAwait(false);
				var result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
					.ExecuteAsync(cancellationToken).ConfigureAwait(false);
				return result.AccessToken;
			}
			catch (MsalUiRequiredException)
			{
				// Device-code acquisition uses the standard bearer-token flow and
				// avoids WAM/broker token protection for management-plane APIs that
				// advertise Bearer and MSAuth1.0 but reject PoP.
				try
				{
					var deviceCode = await app.AcquireTokenWithDeviceCode(scopes, deviceCodeResult =>
					{
						output.WriteLine(deviceCodeResult.Message, ConsoleColor.Yellow);
						return Task.CompletedTask;
					}).ExecuteAsync(cancellationToken).ConfigureAwait(false);
					return deviceCode.AccessToken;
				}
				catch (Exception deviceCodeException)
				{
					output.WriteLine($"Device-code authentication unavailable: {deviceCodeException.Message}", ConsoleColor.Yellow);
				}

				// Keep a browser fallback for hosts where device code is disabled.
				try
				{
					var interactive = await app.AcquireTokenInteractive(scopes)
						.WithPrompt(Prompt.SelectAccount)
						.ExecuteAsync(cancellationToken).ConfigureAwait(false);
					return interactive.AccessToken;
				}
				catch (Exception interactiveException)
				{
					output.WriteLine($"Interactive authentication required for the requested resource: {interactiveException.Message}", ConsoleColor.Yellow);
					return null;
				}
			}
			catch (Exception ex)
			{
				output.WriteLine($"Error acquiring token via OAuth: {ex.Message}", ConsoleColor.Yellow);
				return null;
			}
		}

		internal static string[] BuildScopes(string resource)
			=> new[] { $"{resource.TrimEnd('/')}/.default" };
	}
}
