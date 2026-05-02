using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Model;

namespace Greg.Xrm.Command.Integration
{
	public abstract class IntegrationTestBase
	{
		public TestContext TestContext { get; set; } = null!;

		protected static string IntegrationEnvironmentUrl
		{
			get
			{
				var value = Environment.GetEnvironmentVariable("PACX_INTEGRATION_ENV_URL");
				if (string.IsNullOrWhiteSpace(value))
				{
					Assert.Inconclusive("Set PACX_INTEGRATION_ENV_URL to run Dataverse integration tests.");
				}

				return value;
			}
		}

		protected ServiceClient? CrmService { get; private set; }
		protected bool IsConnected { get; private set; }

		[TestInitialize]
		public void TestInitialize()
		{
			var url = Environment.GetEnvironmentVariable("PACX_INTEGRATION_ENV_URL");
			var clientId = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_ID");
			var clientSecret = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_SECRET");
			if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
			{
				Assert.Inconclusive("Integration test skipped: PACX_INTEGRATION_ENV_URL, PACX_INTEGRATION_CLIENT_ID, and PACX_INTEGRATION_CLIENT_SECRET environment variables are required.");
				return;
			}

			try
			{
				var connectionOptions = new ConnectionOptions
				{
					AuthenticationType = AuthenticationType.ClientSecret,
					ServiceUri = new Uri(url),
					ClientId = clientId,
					ClientSecret = clientSecret,
					RequireNewInstance = true,
				};

				CrmService = new ServiceClient(
					connectionOptions,
					true,
					new ConfigurationOptions());

				if (CrmService.IsReady)
				{
					IsConnected = true;
				}
				else
				{
					Assert.Inconclusive($"Integration test skipped: Could not connect to Dataverse. Error: {CrmService?.LastError}");
				}
			}
			catch (Exception ex)
			{
				Assert.Inconclusive($"Integration test skipped: Failed to create ServiceClient. Error: {ex.Message}");
			}
		}

		[TestCleanup]
		public void TestCleanup()
		{
			CrmService?.Dispose();
		}

		protected static string CreateTestName(string testName)
		{
			return $"pacx_test_{testName}_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";
		}
	}
}
