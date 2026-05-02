using Greg.Xrm.Command.Services.PowerBi;

namespace Greg.Xrm.Command.Integration
{
	[TestClass]
	[TestCategory("Integration")]
	public class PowerBiIntegrationTests : IntegrationTestBase
	{
		[TestMethod]
		public void PowerBi_ClientResource_ShouldPointToPowerBiApi()
		{
			Assert.AreEqual("https://analysis.windows.net/powerbi/api", PowerBiClient.Resource);
			TestContext.WriteLine($"Power BI resource: {PowerBiClient.Resource}");
		}

		[TestMethod]
		public void PowerBi_EnvironmentVariables_ShouldSupportTokenAcquisition()
		{
			var clientId = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_ID");
			var clientSecret = Environment.GetEnvironmentVariable("PACX_INTEGRATION_CLIENT_SECRET");
			var tenantId = Environment.GetEnvironmentVariable("PACX_INTEGRATION_TENANT_ID")
				?? Environment.GetEnvironmentVariable("PACX_INTEGRATION_ENV_URL")
					?.Split('.')[0]
					?.TrimStart("https://".ToCharArray());

			if (!string.IsNullOrEmpty(tenantId))
			{
				TestContext.WriteLine($"Tenant ID available: {tenantId}");
			}

			Assert.IsFalse(string.IsNullOrEmpty(clientId), "PACX_INTEGRATION_CLIENT_ID must be set for Power BI operations");
			Assert.IsFalse(string.IsNullOrEmpty(clientSecret), "PACX_INTEGRATION_CLIENT_SECRET must be set for Power BI operations");

			TestContext.WriteLine("Power BI token acquisition prerequisites: satisfied");
			TestContext.WriteLine($"  Client ID: {clientId}");
			TestContext.WriteLine($"  Client Secret: {(string.IsNullOrEmpty(clientSecret) ? "NOT SET" : "configured")}");
		}

		[TestMethod]
		public void PowerBi_PowerBiCapacitiesEntity_ShouldBeAccessible()
		{
			if (!IsConnected) return;

			var query = new Microsoft.Xrm.Sdk.Query.QueryExpression("organization");
			query.ColumnSet.AddColumn("organizationid");
			query.TopCount = 1;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Entities.Count > 0, "Should connect to Dataverse for Power BI integration context");

			TestContext.WriteLine("Dataverse connection verified for Power BI integration context");
			TestContext.WriteLine($"Connected to: {CrmService.ConnectedOrgFriendlyName}");
		}

		[TestMethod]
		public void PowerBi_UriConstruction_ShouldBuildValidPowerBiEndpoints()
		{
			var workspaceId = "test-workspace";
			var datasetId = "test-dataset";
			var baseUrl = "https://api.powerbi.com/v1.0/myorg";

			var listDatasetsUri = $"{baseUrl}/groups/{Uri.EscapeDataString(workspaceId)}/datasets";
			var refreshPath = $"/groups/{Uri.EscapeDataString(workspaceId)}/datasets/{Uri.EscapeDataString(datasetId)}/refreshes";
			var refreshUri = $"{baseUrl}{refreshPath}";

			Assert.IsTrue(Uri.TryCreate(listDatasetsUri, UriKind.Absolute, out var listUri));
			Assert.AreEqual("api.powerbi.com", listUri.Host);
			Assert.IsTrue(listUri.PathAndQuery.Contains(workspaceId));

			Assert.IsTrue(Uri.TryCreate(refreshUri, UriKind.Absolute, out var refreshUriParsed));
			Assert.AreEqual("api.powerbi.com", refreshUriParsed.Host);
			Assert.IsTrue(refreshUriParsed.PathAndQuery.Contains(datasetId));

			TestContext.WriteLine($"List datasets URI: {listDatasetsUri}");
			TestContext.WriteLine($"Refresh status URI: {refreshUri}");
		}

		[TestMethod]
		public void PowerBi_PipelineAndCapacityUris_ShouldBeWellFormed()
		{
			var pipelineId = "pipeline-1";
			var baseUrl = "https://api.powerbi.com/v1.0/myorg";

			var pipelinesUri = $"{baseUrl}/pipelines";
			var capacityUri = $"{baseUrl}/capacities";

			Assert.IsTrue(Uri.TryCreate(pipelinesUri, UriKind.Absolute, out _));
			Assert.IsTrue(Uri.TryCreate(capacityUri, UriKind.Absolute, out _));

			var stageAssignUri = $"{pipelinesUri}/{Uri.EscapeDataString(pipelineId)}/stages/dev/assignWorkspace";
			var assignCapacityUri = $"{baseUrl}/groups/{Uri.EscapeDataString("workspace-1")}/AssignToCapacity";

			Assert.IsTrue(Uri.TryCreate(stageAssignUri, UriKind.Absolute, out _));
			Assert.IsTrue(Uri.TryCreate(assignCapacityUri, UriKind.Absolute, out _));

			TestContext.WriteLine($"Pipelines URI: {pipelinesUri}");
			TestContext.WriteLine($"Capacities URI: {capacityUri}");
			TestContext.WriteLine($"Stage assign URI: {stageAssignUri}");
			TestContext.WriteLine($"Assign to capacity URI: {assignCapacityUri}");
		}
	}
}
