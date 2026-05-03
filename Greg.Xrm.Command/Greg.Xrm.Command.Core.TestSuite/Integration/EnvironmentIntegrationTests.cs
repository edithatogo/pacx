using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Integration
{
	[TestClass]
	[TestCategory("Integration")]
	public class EnvironmentIntegrationTests : IntegrationTestBase
	{
		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestConnectToEnvironment()
		{
			if (!IsConnected) return;

			Assert.IsTrue(CrmService!.IsReady, "ServiceClient should be ready after connection");
			Assert.IsNotNull(CrmService.ConnectedOrgFriendlyName, "Should have connected org friendly name");

			TestContext.WriteLine($"Connected to: {CrmService.ConnectedOrgFriendlyName}");
			TestContext.WriteLine($"Environment ID: {CrmService.EnvironmentId}");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestOrganizationDetails()
		{
			if (!IsConnected) return;

			var orgDetail = CrmService!.OrganizationDetail;
			Assert.IsNotNull(orgDetail, "Should retrieve organization details");

			Assert.IsNotNull(orgDetail.OrganizationId, "Organization ID should not be null");
			Assert.AreNotEqual(Guid.Empty, orgDetail.OrganizationId, "Organization ID should not be empty");

			TestContext.WriteLine($"Organization ID: {orgDetail.OrganizationId}");
			TestContext.WriteLine($"Organization Friendly Name: {orgDetail.FriendlyName ?? "(not set)"}");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestEnvironmentId()
		{
			if (!IsConnected) return;

			var envId = CrmService!.EnvironmentId;
			Assert.IsFalse(string.IsNullOrEmpty(envId), "Environment ID should not be null or empty");

			TestContext.WriteLine($"Environment ID: {envId}");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestOrganizationEntity()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("organization");
			query.ColumnSet.AddColumn("organizationid");
			query.ColumnSet.AddColumn("name");
			query.ColumnSet.AddColumn("friendlyname");
			query.ColumnSet.AddColumn("currencyname");
			query.ColumnSet.AddColumn("currencycode");
			query.ColumnSet.AddColumn("organizationversion");
			query.TopCount = 1;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result, "Organization query should not return null");
			Assert.AreEqual(1, result.Entities.Count, "Should retrieve exactly one organization record");

			var org = result.Entities[0];
			var orgId = org.GetAttributeValue<Guid?>("organizationid");
			var orgName = org.GetAttributeValue<string>("name");
			var friendlyName = org.GetAttributeValue<string>("friendlyname");
			var currencyName = org.GetAttributeValue<string>("currencyname");
			var currencyCode = org.GetAttributeValue<string>("currencycode");
			var orgVersion = org.GetAttributeValue<string>("organizationversion");

			Assert.AreNotEqual(Guid.Empty, orgId.GetValueOrDefault(), "Organization ID should not be empty");
			Assert.IsFalse(string.IsNullOrEmpty(orgName), "Organization name should not be empty");

			TestContext.WriteLine($"Organization name: {orgName}");
			TestContext.WriteLine($"Organization friendly name: {friendlyName ?? "(not set)"}");
			TestContext.WriteLine($"Currency: {currencyName ?? "(not set)"} ({currencyCode ?? "N/A"})");
			TestContext.WriteLine($"Version: {orgVersion ?? "(not set)"}");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestInstanceSuspendedState()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("organization");
			query.ColumnSet.AddColumn("organizationid");
			query.ColumnSet.AddColumn("name");
			query.ColumnSet.AddColumn("isinstancesuspended");
			query.ColumnSet.AddColumn("instancesuspendedreason");
			query.TopCount = 1;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result, "Organization query should not return null");
			Assert.IsTrue(result.Entities.Count > 0, "Should retrieve organization record");

			var org = result.Entities[0];
			var isSuspended = org.GetAttributeValue<bool?>("isinstancesuspended");
			var suspendReason = org.GetAttributeValue<string>("instancesuspendedreason");
			var orgName = org.GetAttributeValue<string>("name");

			TestContext.WriteLine($"Organization: {orgName}");
			TestContext.WriteLine($"Instance suspended: {isSuspended}");
			TestContext.WriteLine($"Suspend reason: {suspendReason ?? "(none)"}");

			if (isSuspended.GetValueOrDefault())
			{
				Assert.IsFalse(string.IsNullOrEmpty(suspendReason),
					"If instance is suspended, a reason should be provided");
				TestContext.WriteLine("WARNING: Environment is in suspended/administration mode.");
			}
			else
			{
				TestContext.WriteLine("Environment is active (not suspended).");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestOrganizationVersion()
		{
			if (!IsConnected) return;

			var orgDetail = CrmService!.OrganizationDetail;
			Assert.IsNotNull(orgDetail, "OrganizationDetail should not be null");

			var version = orgDetail.OrganizationVersion;
			Assert.IsFalse(string.IsNullOrEmpty(version), "Organization version should not be empty");

			TestContext.WriteLine($"Organization version: {version}");

			var query = new QueryExpression("organization");
			query.ColumnSet.AddColumn("organizationversion");
			query.TopCount = 1;

			var result = CrmService.RetrieveMultiple(query);
			Assert.IsTrue(result.Entities.Count > 0, "Should retrieve organization version from entity");

			var org = result.Entities[0];
			var entityVersion = org.GetAttributeValue<string>("organizationversion");
			Assert.AreEqual(version, entityVersion,
				"OrganizationDetail version should match organization entity version");
			TestContext.WriteLine($"Verified version consistency: {version}");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestServiceEndpointIsValid()
		{
			if (!IsConnected) return;

			var serviceUri = CrmService!.ConnectedOrgUriActual;
			Assert.IsNotNull(serviceUri, "ConnectedOrgUriActual should not be null");
			Assert.IsTrue(serviceUri.IsAbsoluteUri, "Service URI should be absolute");

			TestContext.WriteLine($"Service URI: {serviceUri}");
			TestContext.WriteLine($"Service URI scheme: {serviceUri.Scheme}");
			TestContext.WriteLine($"Service URI host: {serviceUri.Host}");

			Assert.AreEqual("https", serviceUri.Scheme, "Service URI should use HTTPS");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestWhoAmI()
		{
			if (!IsConnected) return;

			var request = new Microsoft.Crm.Sdk.Messages.WhoAmIRequest();
			var response = (Microsoft.Crm.Sdk.Messages.WhoAmIResponse)CrmService!.Execute(request);

			Assert.IsNotNull(response, "WhoAmI response should not be null");
			Assert.AreNotEqual(Guid.Empty, response.UserId, "UserId should not be empty");
			Assert.AreNotEqual(Guid.Empty, response.BusinessUnitId, "BusinessUnitId should not be empty");
			Assert.AreNotEqual(Guid.Empty, response.OrganizationId, "OrganizationId should not be empty");

			TestContext.WriteLine($"Current user ID: {response.UserId}");
			TestContext.WriteLine($"Business unit ID: {response.BusinessUnitId}");
			TestContext.WriteLine($"Organization ID: {response.OrganizationId}");

			var orgDetail = CrmService.OrganizationDetail;
			Assert.IsNotNull(orgDetail, "OrganizationDetail should be available");
			Assert.AreEqual(orgDetail.OrganizationId, response.OrganizationId,
				"OrganizationDetail ID should match WhoAmI organization ID");
		}
	}
}
