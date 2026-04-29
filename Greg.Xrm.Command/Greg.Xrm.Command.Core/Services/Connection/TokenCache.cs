using System.Collections.Concurrent;

namespace Greg.Xrm.Command.Services.Connection
{
	public class TokenCache : Dictionary<string, TokenDefinition>
	{
		private static readonly ConcurrentDictionary<string, TokenDefinition> InMemoryCache = new(StringComparer.OrdinalIgnoreCase);
		private const string TokenCachePathEnvironmentVariable = "GREG_XRM_COMMAND_TOKEN_CACHE_PATH";

		public void Set(string name, Uri serviceUri, string token)
		{
			this[name] = new TokenDefinition(serviceUri, token);
		}

		public Task SaveAsync()
		{
			return Task.CompletedTask;
		}



		public static string GetTokenCachePath()
		{
			var overridePath = Environment.GetEnvironmentVariable(TokenCachePathEnvironmentVariable);
			if (!string.IsNullOrWhiteSpace(overridePath))
			{
				if (!Directory.Exists(overridePath))
				{
					Directory.CreateDirectory(overridePath);
				}

				return overridePath;
			}

			var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			folderPath = Path.Combine(folderPath, "Greg.Xrm.Command");
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			folderPath = Path.Combine(folderPath, "tokenCache");
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			return folderPath;
		}


		public static async Task<TokenDefinition?> TryGetAccessTokenAsync(string connectionName, CancellationToken cancellationToken = default)
		{
			await Task.CompletedTask.ConfigureAwait(false);
			if (!InMemoryCache.TryGetValue(connectionName, out var accessToken)) return null;
			if (accessToken == null || !accessToken.IsValid) return null;
			return accessToken;
		}


		public static async Task SaveAccessTokenAsync(string name, Uri serviceUri, string currentAccessToken, CancellationToken cancellationToken = default)
		{
			await Task.CompletedTask.ConfigureAwait(false);
			InMemoryCache[name] = new TokenDefinition(serviceUri, currentAccessToken);
		}

		public static async Task ClearAccessTokenAsync(string connectionName, CancellationToken cancellationToken = default)
		{
			await Task.CompletedTask.ConfigureAwait(false);
			InMemoryCache.TryRemove(connectionName, out _);
		}
	}
}
