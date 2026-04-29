using Greg.Xrm.Command.Services.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Greg.Xrm.Command.Core.TestSuite.Services.Connection
{
	[TestClass]
	public class TokenCacheTest
	{
		[TestMethod]
		public async Task SaveAndClearShouldStayInMemory()
		{
			var cacheRoot = Path.Combine(Path.GetTempPath(), "pacx-token-cache-tests", Guid.NewGuid().ToString("N"));
			var previous = Environment.GetEnvironmentVariable("GREG_XRM_COMMAND_TOKEN_CACHE_PATH");
			Environment.SetEnvironmentVariable("GREG_XRM_COMMAND_TOKEN_CACHE_PATH", cacheRoot);

			try
			{
				var connectionName = $"connection-{Guid.NewGuid():N}";
				var serviceUri = new Uri("https://example.crm.dynamics.com/");
				var token = $"token-{Guid.NewGuid():N}";

				await TokenCache.SaveAccessTokenAsync(connectionName, serviceUri, token).ConfigureAwait(false);

				var cached = await TokenCache.TryGetAccessTokenAsync(connectionName).ConfigureAwait(false);
				Assert.IsNotNull(cached);
				Assert.AreEqual(serviceUri, cached!.ServiceUri);
				Assert.AreEqual(token, cached.AccessToken);

				var cachePath = TokenCache.GetTokenCachePath();
				Assert.IsTrue(Directory.Exists(cachePath));
				Assert.IsFalse(File.Exists(Path.Combine(cachePath, "tokens.json")));
				Assert.AreEqual(0, Directory.GetFiles(cachePath, "*", SearchOption.AllDirectories).Length);

				await TokenCache.ClearAccessTokenAsync(connectionName).ConfigureAwait(false);

				var cleared = await TokenCache.TryGetAccessTokenAsync(connectionName).ConfigureAwait(false);
				Assert.IsNull(cleared);
				Assert.IsFalse(File.Exists(Path.Combine(cachePath, "tokens.json")));
			}
			finally
			{
				Environment.SetEnvironmentVariable("GREG_XRM_COMMAND_TOKEN_CACHE_PATH", previous);
				if (Directory.Exists(cacheRoot))
				{
					Directory.Delete(cacheRoot, recursive: true);
				}
			}
		}
	}
}
