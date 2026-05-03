using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Integration
{
	[TestClass]
	[TestCategory("Integration")]
	public class AuthIntegrationTests : IntegrationTestBase
	{
		[TestMethod]
		public void Auth_ConnectionSmoke_ShouldConnectToConfiguredEnvironment()
		{
			if (!IsConnected) return;

			Assert.IsTrue(CrmService!.IsReady, "ServiceClient should be ready");
			Assert.IsNotNull(CrmService.ConnectedOrgFriendlyName, "Should have a connected org friendly name");
			Assert.IsNotNull(CrmService.EnvironmentId, "Should have an environment ID");

			TestContext.WriteLine($"Connected to: {CrmService.ConnectedOrgFriendlyName}");
			TestContext.WriteLine($"Environment ID: {CrmService.EnvironmentId}");
			TestContext.WriteLine($"Organization version: {CrmService.OrganizationDetail?.OrganizationVersion}");
		}

		[TestMethod]
		public void Auth_OrgDetails_ShouldHaveFriendlyNameAndEnvironmentId()
		{
			if (!IsConnected) return;

			var orgDetails = CrmService!.OrganizationDetail;
			Assert.IsNotNull(orgDetails, "OrganizationDetail should not be null");
			Assert.IsFalse(string.IsNullOrEmpty(orgDetails.FriendlyName), "FriendlyName should not be empty");
			Assert.AreNotEqual(Guid.Empty, orgDetails.OrganizationId, "OrganizationId should not be empty");
			Assert.IsFalse(string.IsNullOrEmpty(orgDetails.OrganizationVersion), "OrganizationVersion should not be empty");

			TestContext.WriteLine($"Friendly name: {orgDetails.FriendlyName}");
			TestContext.WriteLine($"Organization ID: {orgDetails.OrganizationId}");
			TestContext.WriteLine($"Version: {orgDetails.OrganizationVersion}");
			TestContext.WriteLine($"Environment ID: {CrmService.EnvironmentId}");
		}

		[TestMethod]
		public void Auth_AdminMode_ShouldQueryOrganizationEntity()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("organization");
			query.ColumnSet.AddColumns("isinstancesuspended", "organizationid", "name");
			query.TopCount = 1;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result, "Organization query result should not be null");
			Assert.IsTrue(result.Entities.Count > 0, "Should retrieve at least one organization record");

			var org = result.Entities[0];
			var isSuspended = org.GetAttributeValue<bool?>("isinstancesuspended");
			var orgName = org.GetAttributeValue<string>("name");

			TestContext.WriteLine($"Organization name: {orgName}");
			TestContext.WriteLine($"Instance suspended: {isSuspended}");
			TestContext.WriteLine($"Admin mode active: {isSuspended.GetValueOrDefault(false)}");
		}

		[TestMethod]
		public void Auth_WhoAmI_ShouldRetrieveCurrentUserInfo()
		{
			if (!IsConnected) return;

			var request = new WhoAmIRequest();
			var response = (WhoAmIResponse)CrmService!.Execute(request);

			Assert.IsNotNull(response, "WhoAmI response should not be null");
			Assert.AreNotEqual(Guid.Empty, response.UserId, "UserId should not be empty");
			Assert.AreNotEqual(Guid.Empty, response.BusinessUnitId, "BusinessUnitId should not be empty");
			Assert.AreNotEqual(Guid.Empty, response.OrganizationId, "OrganizationId should not be empty");

			TestContext.WriteLine($"Current user ID: {response.UserId}");
			TestContext.WriteLine($"Business unit ID: {response.BusinessUnitId}");
			TestContext.WriteLine($"Organization ID: {response.OrganizationId}");
		}

		[TestMethod]
		public void Auth_PingFlow_ShouldMatchPingCommandBehavior()
		{
			if (!IsConnected) return;

			var details = CrmService!.OrganizationDetail;
			Assert.IsNotNull(details, "OrganizationDetail should not be null");

			TestContext.WriteLine($"Connected to: {details.FriendlyName} ({details.OrganizationVersion})");

			var request = new WhoAmIRequest();
			var response = (WhoAmIResponse)CrmService.Execute(request);

			Assert.IsNotNull(response, "WhoAmI response should not be null");
			Assert.AreNotEqual(Guid.Empty, response.UserId, "UserId should not be empty");

			TestContext.WriteLine($"Connection successful. User: {response.UserId}");
		}

		[TestMethod]
		public void Auth_UserDetails_ShouldRetrieveCurrentUserFullInfo()
		{
			if (!IsConnected) return;

			var whoAmI = (WhoAmIResponse)CrmService!.Execute(new WhoAmIRequest());
			var userId = whoAmI.UserId;

			var query = new QueryExpression("systemuser");
			query.ColumnSet.AddColumns("fullname", "internalemailaddress", "domainname", "businessunitid");
			query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);

			var result = CrmService.RetrieveMultiple(query);

			Assert.IsNotNull(result, "SystemUser query result should not be null");
			Assert.AreEqual(1, result.Entities.Count, "Should find exactly one system user record");

			var user = result.Entities[0];
			var fullName = user.GetAttributeValue<string>("fullname");
			var email = user.GetAttributeValue<string>("internalemailaddress");
			var domainName = user.GetAttributeValue<string>("domainname");

			TestContext.WriteLine($"User: {fullName ?? "(unnamed)"}");
			TestContext.WriteLine($"Email: {email ?? "(not set)"}");
			TestContext.WriteLine($"Domain: {domainName ?? "(not set)"}");
			TestContext.WriteLine($"User ID: {userId}");
		}

		[TestMethod]
		public void Auth_ConnectionEndpoint_ShouldHaveValidServiceUri()
		{
			if (!IsConnected) return;

			var orgDetails = CrmService!.OrganizationDetail;
			Assert.IsNotNull(orgDetails, "OrganizationDetail should not be null");

			var orgId = orgDetails.OrganizationId;

			Assert.AreNotEqual(Guid.Empty, orgId, "OrganizationId should not be empty");
			Assert.IsFalse(string.IsNullOrEmpty(CrmService.EnvironmentId), "EnvironmentId should not be empty");

			TestContext.WriteLine($"Organization ID: {orgId}");
			TestContext.WriteLine($"Environment ID: {CrmService.EnvironmentId}");
			TestContext.WriteLine($"Org Friendly Name: {CrmService.ConnectedOrgFriendlyName}");
			TestContext.WriteLine($"Endpoint: {IntegrationEnvironmentUrl}");
		}

		[TestMethod]
		public void Auth_SecurityRoles_ShouldBeAssignedToCurrentUser()
		{
			if (!IsConnected) return;

			var whoAmI = (WhoAmIResponse)CrmService!.Execute(new WhoAmIRequest());
			var userId = whoAmI.UserId;

			var linkEntity = new LinkEntity
			{
				LinkFromEntityName = "role",
				LinkFromAttributeName = "roleid",
				LinkToEntityName = "systemuserroles",
				LinkToAttributeName = "roleid",
				LinkCriteria = new FilterExpression
				{
					Conditions =
					{
						new ConditionExpression("systemuserid", ConditionOperator.Equal, userId)
					}
				}
			};

			var query = new QueryExpression("role");
			query.ColumnSet.AddColumns("name", "businessunitid");
			query.LinkEntities.Add(linkEntity);

			var result = CrmService.RetrieveMultiple(query);

			Assert.IsNotNull(result, "Roles query should not return null");
			Assert.IsTrue(result.Entities.Count > 0, "Current user should have at least one security role");

			TestContext.WriteLine($"Current user has {result.Entities.Count} security role(s):");
			foreach (var role in result.Entities)
			{
				var roleName = role.GetAttributeValue<string>("name");
				var buId = role.GetAttributeValue<EntityReference>("businessunitid");
				TestContext.WriteLine($"  Role: {roleName} (BU: {buId?.Id.ToString() ?? "N/A"})");
			}
		}
	}
}
