using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Model;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Integration
{
	[TestClass]
	[TestCategory("Integration")]
	public class ConnectionLifecycleIntegrationTests : IntegrationTestBase
	{
		[TestMethod]
		public void ConnectionLifecycle_EnvironmentVariables_ShouldBeConfigured()
		{
			var url = Environment.GetEnvironmentVariable("PACX_INTEGRATION_ENV_URL");
			var clientId = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_ID");
			var clientSecret = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_SECRET");

			Assert.IsFalse(string.IsNullOrWhiteSpace(url), "PACX_INTEGRATION_ENV_URL must be set");
			Assert.IsFalse(string.IsNullOrWhiteSpace(clientId), "PACX_INTEGRATION_CLIENT_ID must be set");
			Assert.IsFalse(string.IsNullOrWhiteSpace(clientSecret), "PACX_INTEGRATION_CLIENT_SECRET must be set (or set to empty if not needed)");

			TestContext.WriteLine($"Environment URL: {url}");
			TestContext.WriteLine($"Client ID: {clientId}");
		}

		[TestMethod]
		public void ConnectionLifecycle_Reconnect_ShouldCreateDisposeAndReconnect()
		{
			if (!IsConnected) return;

			var url = Environment.GetEnvironmentVariable("PACX_INTEGRATION_ENV_URL");
			var clientId = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_ID");
			var clientSecret = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_SECRET");

			Assert.IsNotNull(url);
			Assert.IsNotNull(clientId);
			Assert.IsNotNull(clientSecret);

			var originalOrgName = CrmService!.ConnectedOrgFriendlyName;
			TestContext.WriteLine($"Original connection: {originalOrgName}");

			CrmService.Dispose();
			TestContext.WriteLine("Disposed original ServiceClient");

			var connectionOptions = new ConnectionOptions
			{
				AuthenticationType = AuthenticationType.ClientSecret,
				ServiceUri = new Uri(url),
				ClientId = clientId,
				ClientSecret = clientSecret,
				RequireNewInstance = true,
			};

			using var newService = new ServiceClient(
				connectionOptions,
				true,
				new ConfigurationOptions());

			Assert.IsTrue(newService.IsReady, "New ServiceClient should be ready after reconnect");
			Assert.IsNotNull(newService.ConnectedOrgFriendlyName, "New connection should have a friendly name");
			Assert.AreEqual(originalOrgName, newService.ConnectedOrgFriendlyName, "Should reconnect to the same org");

			TestContext.WriteLine($"Reconnected to: {newService.ConnectedOrgFriendlyName}");
			TestContext.WriteLine($"Environment ID: {newService.EnvironmentId}");
			TestContext.WriteLine($"IsReady: {newService.IsReady}");
		}

		[TestMethod]
		public void ConnectionLifecycle_ConnectionUrl_ShouldBeProperlyFormed()
		{
			if (!IsConnected) return;

			var url = Environment.GetEnvironmentVariable("PACX_INTEGRATION_ENV_URL");
			Assert.IsNotNull(url);

			Assert.IsTrue(url.StartsWith("https://"), "URL should start with https://");
			Assert.IsTrue(Uri.TryCreate(url, UriKind.Absolute, out var uri), "URL should be a valid absolute URI");
			Assert.AreEqual("https", uri.Scheme, "URL scheme should be https");
			Assert.IsTrue(uri.Host.EndsWith(".dynamics.com") || uri.Host.EndsWith(".api.crm.dynamics.com"),
				"URL host should be a valid Dynamics 365 endpoint");

			TestContext.WriteLine($"Configured URL: {url}");
			TestContext.WriteLine($"Parsed host: {uri.Host}");
			TestContext.WriteLine($"Connected org: {CrmService!.ConnectedOrgFriendlyName}");
		}

		[TestMethod]
		public void ConnectionLifecycle_ConnectionReferences_ShouldBeQueryable()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("connectionreference");
			query.ColumnSet.AddColumns("connectionreferencelogicalname", "connectorid", "connectionid", "createdon");
			query.TopCount = 10;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result, "ConnectionReference query result should not be null");

			TestContext.WriteLine($"Found {result.Entities.Count} connection reference(s)");

			foreach (var cr in result.Entities)
			{
				var name = cr.GetAttributeValue<string>("connectionreferencelogicalname");
				var connectorId = cr.GetAttributeValue<string>("connectorid");
				var connectionId = cr.GetAttributeValue<string>("connectionid");
				var createdOn = cr.GetAttributeValue<DateTime?>("createdon");
				TestContext.WriteLine($"  {name ?? "(unnamed)"} | Connector: {connectorId ?? "-"} | Connection: {connectionId ?? "-"} | Created: {createdOn?.ToString("yyyy-MM-dd") ?? "-"}");
			}
		}

		[TestMethod]
		public void ConnectionLifecycle_EnvironmentCapabilities_ShouldBeAccessible()
		{
			if (!IsConnected) return;

			var orgDetails = CrmService!.OrganizationDetail;
			Assert.IsNotNull(orgDetails, "OrganizationDetail should not be null");

			var versionString = orgDetails.OrganizationVersion;
			Assert.IsFalse(string.IsNullOrEmpty(versionString), "OrganizationVersion should not be empty");

			Assert.IsTrue(Version.TryParse(versionString, out var version), "OrganizationVersion should be a valid version string");

			TestContext.WriteLine($"Dataverse version: {versionString}");
			TestContext.WriteLine($"Major: {version.Major}, Minor: {version.Minor}, Build: {version.Build}");
			TestContext.WriteLine($"Org friendly name: {orgDetails.FriendlyName}");
			TestContext.WriteLine($"Org ID: {orgDetails.OrganizationId}");
			TestContext.WriteLine($"Environment ID: {CrmService.EnvironmentId}");
		}

		[TestMethod]
		public void ConnectionLifecycle_ConnectionStatus_ShouldExposeMetadata()
		{
			if (!IsConnected) return;

			Assert.IsTrue(CrmService!.IsReady, "ServiceClient should be ready");
			Assert.IsNull(CrmService.LastError, "LastError should be null when connected successfully");

			TestContext.WriteLine($"IsReady: {CrmService.IsReady}");
			TestContext.WriteLine($"LastError: {CrmService.LastError ?? "(none)"}");
			TestContext.WriteLine($"EnvironmentId: {CrmService.EnvironmentId}");
			TestContext.WriteLine($"ConnectedOrgFriendlyName: {CrmService.ConnectedOrgFriendlyName}");
		}

		[TestMethod]
		public void ConnectionLifecycle_MultipleQueries_ShouldMaintainConnection()
		{
			if (!IsConnected) return;

			for (var i = 0; i < 5; i++)
			{
				var query = new QueryExpression("organization");
				query.ColumnSet.AddColumn("organizationid");
				query.TopCount = 1;

				var result = CrmService!.RetrieveMultiple(query);

				Assert.IsNotNull(result, $"Query attempt {i + 1} should succeed");
				Assert.IsTrue(result.Entities.Count > 0, $"Query attempt {i + 1} should return data");
				Assert.IsTrue(CrmService.IsReady, $"Connection should remain ready after query {i + 1}");
			}

			TestContext.WriteLine("Successfully executed 5 sequential queries on the same connection");
		}
	}
}
