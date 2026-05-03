using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Integration
{
	[TestClass]
	[TestCategory("Integration")]
	public class ConnectorIntegrationTests : IntegrationTestBase
	{
		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestListConnectors()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("connector");
			query.ColumnSet.AddColumn("name");
			query.ColumnSet.AddColumn("connectorid");
			query.ColumnSet.AddColumn("description");
			query.TopCount = 20;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result, "Should retrieve connectors");

			TestContext.WriteLine($"Connectors found: {result.Entities.Count}");

			foreach (var connector in result.Entities)
			{
				var name = connector.GetAttributeValue<string>("name");
				var description = connector.GetAttributeValue<string>("description");
				TestContext.WriteLine($"Connector: {name ?? "(unnamed)"} ({description ?? "no description"})");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestConnectorMetadata()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("connector");
			query.ColumnSet.AddColumn("name");
			query.ColumnSet.AddColumn("connectorid");
			query.ColumnSet.AddColumn("description");
			query.ColumnSet.AddColumn("createdon");
			query.ColumnSet.AddColumn("statecode");
			query.ColumnSet.AddColumn("statuscode");
			query.TopCount = 10;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result, "Should retrieve connector metadata");

			if (result.Entities.Count > 0)
			{
				foreach (var connector in result.Entities)
				{
					var name = connector.GetAttributeValue<string>("name");
					var id = connector.GetAttributeValue<Guid>("connectorid");
					var createdOn = connector.GetAttributeValue<DateTime?>("createdon");
					var state = connector.GetAttributeValue<OptionSetValue>("statecode")?.Value;
					var status = connector.GetAttributeValue<OptionSetValue>("statuscode")?.Value;

					TestContext.WriteLine($"Connector: {name ?? "(unnamed)"}");
					TestContext.WriteLine($"  ID: {id}");
					TestContext.WriteLine($"  Created: {createdOn?.ToString("yyyy-MM-dd") ?? "N/A"}");
					TestContext.WriteLine($"  State: {state}");
					TestContext.WriteLine($"  Status: {status}");
				}
			}
			else
			{
				TestContext.WriteLine("No custom connectors found in this environment.");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestCreateDeleteConnector()
		{
			if (!IsConnected) return;

			var connectorName = CreateTestName("connector").ToLowerInvariant();
			var connectorId = Guid.Empty;

			try
			{
				var openApiDefinition = "{\"openapi\":\"3.0.0\",\"info\":{\"title\":\"Test Connector\",\"version\":\"1.0.0\"},\"paths\":{\"/test\":{\"get\":{\"operationId\":\"testGet\",\"responses\":{\"200\":{\"description\":\"OK\"}}}}}}";

				var connector = new Entity("connector");
				connector["name"] = connectorName;
				connector["openapidefinition"] = openApiDefinition;
				connector["connectortype"] = new OptionSetValue(1);

				connectorId = CrmService!.Create(connector);
				Assert.AreNotEqual(Guid.Empty, connectorId, "Should create custom connector");
				TestContext.WriteLine($"Created connector: {connectorName} (ID: {connectorId})");

				var query = new QueryExpression("connector");
				query.ColumnSet.AddColumn("name");
				query.ColumnSet.AddColumn("openapidefinition");
				query.ColumnSet.AddColumn("connectortype");
				query.Criteria.AddCondition("connectorid", ConditionOperator.Equal, connectorId);

				var result = CrmService.RetrieveMultiple(query);
				Assert.AreEqual(1, result.Entities.Count, "Should find created connector");

				var created = result.Entities[0];
				Assert.AreEqual(connectorName, created.GetAttributeValue<string>("name"));
				Assert.IsFalse(string.IsNullOrEmpty(created.GetAttributeValue<string>("openapidefinition")),
					"Connector should have an OpenAPI definition");
				var connectorType = created.GetAttributeValue<OptionSetValue>("connectortype")?.Value;
				Assert.AreEqual(1, connectorType, "Connector type should be Custom (1)");

				CrmService.Delete("connector", connectorId);
				TestContext.WriteLine($"Deleted connector: {connectorName}");

				result = CrmService.RetrieveMultiple(query);
				Assert.AreEqual(0, result.Entities.Count, "Should not find deleted connector");
			}
			finally
			{
				CleanupConnector(connectorName, connectorId);
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestConnectorQueryByName()
		{
			if (!IsConnected) return;

			var connectorName = CreateTestName("connq").ToLowerInvariant();
			var connectorId = Guid.Empty;

			try
			{
				var openApiDefinition = "{\"openapi\":\"3.0.0\",\"info\":{\"title\":\"Query Test Connector\",\"version\":\"1.0.0\"},\"paths\":{\"/query\":{\"get\":{\"operationId\":\"queryGet\",\"responses\":{\"200\":{\"description\":\"OK\"}}}}}}";

				var connector = new Entity("connector");
				connector["name"] = connectorName;
				connector["openapidefinition"] = openApiDefinition;
				connector["connectortype"] = new OptionSetValue(1);

				connectorId = CrmService!.Create(connector);
				Assert.AreNotEqual(Guid.Empty, connectorId, "Should create connector for query test");
				TestContext.WriteLine($"Created connector: {connectorName} (ID: {connectorId})");

				var query = new QueryExpression("connector");
				query.ColumnSet.AddColumn("name");
				query.ColumnSet.AddColumn("connectorid");
				query.ColumnSet.AddColumn("createdon");
				query.ColumnSet.AddColumn("statecode");
				query.Criteria.AddCondition("name", ConditionOperator.Equal, connectorName);

				var result = CrmService.RetrieveMultiple(query);
				Assert.AreEqual(1, result.Entities.Count, "Should find connector by name");

				var found = result.Entities[0];
				Assert.AreEqual(connectorName, found.GetAttributeValue<string>("name"));
				Assert.AreEqual(connectorId, found.GetAttributeValue<Guid>("connectorid"));

				TestContext.WriteLine($"Retrieved connector by name: {connectorName}");
				TestContext.WriteLine($"  ID: {connectorId}");
				TestContext.WriteLine($"  Created: {found.GetAttributeValue<DateTime?>("createdon")?.ToString("yyyy-MM-dd") ?? "N/A"}");
			}
			finally
			{
				CleanupConnector(connectorName, connectorId);
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestConnectorSchemaValidation()
		{
			if (!IsConnected) return;

			var validator = new Greg.Xrm.Command.Commands.Connector.ConnectorSchemaValidator();

			var validDefinition = "{\"openapi\":\"3.0.0\",\"info\":{\"title\":\"Valid Connector\",\"version\":\"1.0.0\"},\"paths\":{\"/valid\":{\"get\":{\"operationId\":\"validGet\",\"responses\":{\"200\":{\"description\":\"OK\"}}}}}}";
			var validResult = validator.Validate(validDefinition);
			Assert.IsTrue(validResult.IsValid, "A well-formed connector definition should pass validation");
			Assert.AreEqual(0, validResult.Errors.Count, "Should have no errors");
			TestContext.WriteLine("Valid connector definition passed schema validation");

			var missingPaths = "{\"openapi\":\"3.0.0\",\"info\":{\"title\":\"No Paths Connector\",\"version\":\"1.0.0\"}}";
			var missingPathsResult = validator.Validate(missingPaths);
			Assert.IsFalse(missingPathsResult.IsValid, "Definition missing 'paths' should fail validation");
			Assert.IsTrue(missingPathsResult.Errors.Count > 0, "Should report missing paths error");
			TestContext.WriteLine($"Definition missing 'paths': {missingPathsResult.Errors.Count} error(s)");
			foreach (var err in missingPathsResult.Errors)
			{
				TestContext.WriteLine($"  Error: {err}");
			}

			var invalidJson = "this is not json";
			var invalidJsonResult = validator.Validate(invalidJson);
			Assert.IsFalse(invalidJsonResult.IsValid, "Invalid JSON should fail");
			Assert.IsNotNull(invalidJsonResult.Exception, "Should capture parse exception");
			TestContext.WriteLine($"Invalid JSON error: {invalidJsonResult.Exception?.Message}");

			var missingInfo = "{\"openapi\":\"3.0.0\",\"paths\":{\"/x\":{\"get\":{\"operationId\":\"x\",\"responses\":{\"200\":{\"description\":\"OK\"}}}}}}";
			var missingInfoResult = validator.Validate(missingInfo);
			TestContext.WriteLine($"Missing 'info': {missingInfoResult.Warnings.Count} warning(s), {missingInfoResult.Errors.Count} error(s)");
			Assert.IsTrue(missingInfoResult.IsValid, "Missing 'info' should be a warning, not an error");
		}

		private void CleanupConnector(string connectorName, Guid connectorId)
		{
			try
			{
				if (connectorId != Guid.Empty)
				{
					CrmService!.Delete("connector", connectorId);
					TestContext.WriteLine($"Cleanup: Deleted connector {connectorName}");
					return;
				}

				var query = new QueryExpression("connector");
				query.Criteria.AddCondition("name", ConditionOperator.Equal, connectorName);
				var result = CrmService!.RetrieveMultiple(query);
				if (result.Entities.Count > 0)
				{
					CrmService.Delete("connector", result.Entities[0].Id);
					TestContext.WriteLine($"Cleanup: Deleted connector {connectorName}");
				}
			}
			catch { /* Ignore cleanup errors */ }
		}
	}
}
