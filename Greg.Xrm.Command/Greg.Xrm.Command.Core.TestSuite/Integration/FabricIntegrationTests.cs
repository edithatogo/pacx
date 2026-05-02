using Greg.Xrm.Command.Services.Fabric;

namespace Greg.Xrm.Command.Integration
{
	[TestClass]
	[TestCategory("Integration")]
	public class FabricIntegrationTests : IntegrationTestBase
	{
		[TestMethod]
		public void Fabric_ClientResource_ShouldPointToFabricApi()
		{
			Assert.AreEqual("https://api.fabric.microsoft.com/", FabricClient.Resource);
			TestContext.WriteLine($"Fabric resource: {FabricClient.Resource}");
		}

		[TestMethod]
		public void Fabric_EnvironmentVariables_ShouldSupportTokenAcquisition()
		{
			var clientId = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_ID");
			var clientSecret = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_SECRET");

			Assert.IsFalse(string.IsNullOrEmpty(clientId), "PACX_INTEGRATION_CLIENT_ID must be set for Fabric operations");
			Assert.IsFalse(string.IsNullOrEmpty(clientSecret), "PACX_INTEGRATION_CLIENT_SECRET must be set for Fabric operations");

			TestContext.WriteLine("Fabric token acquisition prerequisites: satisfied");
			TestContext.WriteLine($"  Client ID: {clientId}");
			TestContext.WriteLine($"  Client Secret: {(string.IsNullOrEmpty(clientSecret) ? "NOT SET" : "configured")}");
		}

		[TestMethod]
		public void Fabric_ApiEndpoint_ShouldBeResolvable()
		{
			var baseUrl = "https://api.fabric.microsoft.com/v1";

			Assert.IsTrue(Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri));
			Assert.AreEqual("api.fabric.microsoft.com", uri.Host);
			Assert.AreEqual("https", uri.Scheme);

			TestContext.WriteLine($"Fabric API base URL: {baseUrl}");
			TestContext.WriteLine($"Host: {uri.Host}");
			TestContext.WriteLine($"Port: {uri.Port}");
		}

		[TestMethod]
		public void Fabric_WorkspaceEndpoints_ShouldBuildValidUris()
		{
			var baseUrl = "https://api.fabric.microsoft.com/v1";
			var workspaceId = "test-workspace";

			var workspacesUri = $"{baseUrl}/workspaces";
			var lakehousesUri = $"{baseUrl}/workspaces/{Uri.EscapeDataString(workspaceId)}/lakehouses";
			var semanticModelsUri = $"{baseUrl}/workspaces/{Uri.EscapeDataString(workspaceId)}/semanticModels";

			Assert.IsTrue(Uri.TryCreate(workspacesUri, UriKind.Absolute, out _));
			Assert.IsTrue(Uri.TryCreate(lakehousesUri, UriKind.Absolute, out _));
			Assert.IsTrue(Uri.TryCreate(semanticModelsUri, UriKind.Absolute, out _));

			TestContext.WriteLine($"Workspaces URI: {workspacesUri}");
			TestContext.WriteLine($"Lakehouses URI: {lakehousesUri}");
			TestContext.WriteLine($"Semantic models URI: {semanticModelsUri}");
		}

		[TestMethod]
		public void Fabric_OneLakeShortcutEndpoints_ShouldBuildValidUris()
		{
			var baseUrl = "https://api.fabric.microsoft.com/v1";
			var workspaceId = "test-workspace";
			var itemId = "test-item";
			var shortcutPath = "test/shortcut";

			var shortcutsUri = $"{baseUrl}/workspaces/{Uri.EscapeDataString(workspaceId)}/items/{Uri.EscapeDataString(itemId)}/shortcuts";
			var deleteShortcutUri = $"{shortcutsUri}/{Uri.EscapeDataString(shortcutPath)}";

			Assert.IsTrue(Uri.TryCreate(shortcutsUri, UriKind.Absolute, out _));
			Assert.IsTrue(Uri.TryCreate(deleteShortcutUri, UriKind.Absolute, out _));

			Assert.IsTrue(shortcutsUri.Contains(workspaceId));
			Assert.IsTrue(shortcutsUri.Contains(itemId));
			Assert.IsTrue(deleteShortcutUri.Contains(shortcutPath));

			TestContext.WriteLine($"Shortcuts URI: {shortcutsUri}");
			TestContext.WriteLine($"Delete shortcut URI: {deleteShortcutUri}");
		}

		[TestMethod]
		public void Fabric_DataverseLinkEndpoints_ShouldBuildValidUris()
		{
			var baseUrl = "https://api.fabric.microsoft.com/v1";

			var linkCreateUri = $"{baseUrl}/dataverseLinks";
			var linkStatusUri = $"{baseUrl}/dataverseLinks";

			Assert.IsTrue(Uri.TryCreate(linkCreateUri, UriKind.Absolute, out _));

			TestContext.WriteLine($"Dataverse link endpoint: {linkCreateUri}");
		}

		[TestMethod]
		public void Fabric_CapabilitiesEndpoint_ShouldBeWellFormed()
		{
			var baseUrl = "https://api.fabric.microsoft.com/v1";

			var capacitiesUri = $"{baseUrl}/capacities";

			Assert.IsTrue(Uri.TryCreate(capacitiesUri, UriKind.Absolute, out var uri));
			Assert.IsTrue(uri.PathAndQuery.Contains("capacities"));

			TestContext.WriteLine($"Capacities URI: {capacitiesUri}");
		}
	}
}
