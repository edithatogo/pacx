using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Integration
{
	[TestClass]
	[TestCategory("Integration")]
	public class SolutionIntegrationTests : IntegrationTestBase
	{
		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestListSolutions()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("solution");
			query.ColumnSet.AddColumn("uniquename");
			query.ColumnSet.AddColumn("friendlyname");
			query.ColumnSet.AddColumn("version");
			query.ColumnSet.AddColumn("installedon");
			query.TopCount = 10;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Entities.Count > 0, "Should retrieve at least one solution");

			foreach (var solution in result.Entities)
			{
				var name = solution.GetAttributeValue<string>("uniquename");
				var version = solution.GetAttributeValue<string>("version");
				var installedOn = solution.GetAttributeValue<DateTime?>("installedon");
				TestContext.WriteLine($"Solution: {name} v{version} (installed: {installedOn?.ToString("yyyy-MM-dd") ?? "N/A"})");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestCreateDeleteSolution()
		{
			if (!IsConnected) return;

			var solutionName = CreateTestName("solution").ToLowerInvariant();
			var publisherName = CreateTestName("publisher").ToLowerInvariant();
			var publisherId = Guid.Empty;
			var solutionId = Guid.Empty;

			try
			{
				publisherId = CreatePublisher(publisherName);

				var solution = new Entity("solution");
				solution["uniquename"] = solutionName;
				solution["friendlyname"] = $"Integration Test Solution {solutionName}";
				solution["version"] = "1.0.0.0";
				solution["publisherid"] = new EntityReference("publisher", publisherId);

				solutionId = CrmService!.Create(solution);
				Assert.AreNotEqual(Guid.Empty, solutionId, "Should create solution");
				TestContext.WriteLine($"Created solution: {solutionName} (ID: {solutionId})");

				var query = new QueryExpression("solution");
				query.ColumnSet.AddColumn("uniquename");
				query.ColumnSet.AddColumn("friendlyname");
				query.ColumnSet.AddColumn("version");
				query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, solutionId);

				var result = CrmService.RetrieveMultiple(query);
				Assert.AreEqual(1, result.Entities.Count, "Should find created solution");

				var created = result.Entities[0];
				Assert.AreEqual(solutionName, created.GetAttributeValue<string>("uniquename"));
				Assert.AreEqual("1.0.0.0", created.GetAttributeValue<string>("version"));

				CrmService.Delete("solution", solutionId);
				TestContext.WriteLine($"Deleted solution: {solutionName}");

				result = CrmService.RetrieveMultiple(query);
				Assert.AreEqual(0, result.Entities.Count, "Should not find deleted solution");
			}
			finally
			{
				CleanupSolution(solutionName, solutionId);
				CleanupPublisher(publisherName, publisherId);
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestExportSolutionMetadata()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("solution");
			query.ColumnSet.AddColumn("uniquename");
			query.ColumnSet.AddColumn("friendlyname");
			query.ColumnSet.AddColumn("version");
			query.ColumnSet.AddColumn("installedon");
			query.ColumnSet.AddColumn("publisherid");
			query.ColumnSet.AddColumn("description");
			query.ColumnSet.AddColumn("solutionpackageversion");
			query.TopCount = 5;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Entities.Count > 0, "Should retrieve solutions with metadata");

			foreach (var solution in result.Entities)
			{
				var uniqueName = solution.GetAttributeValue<string>("uniquename");
				var friendlyName = solution.GetAttributeValue<string>("friendlyname");
				var version = solution.GetAttributeValue<string>("version");
				var description = solution.GetAttributeValue<string>("description");
				var publisherRef = solution.GetAttributeValue<EntityReference>("publisherid");

				TestContext.WriteLine($"Solution: {uniqueName}");
				TestContext.WriteLine($"  Friendly Name: {friendlyName}");
				TestContext.WriteLine($"  Version: {version}");
				TestContext.WriteLine($"  Publisher: {publisherRef?.Name ?? publisherRef?.Id.ToString() ?? "N/A"}");
				TestContext.WriteLine($"  Description: {description ?? "(none)"}");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestListPublishers()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("publisher");
			query.ColumnSet.AddColumn("uniquename");
			query.ColumnSet.AddColumn("friendlyname");
			query.ColumnSet.AddColumn("customizationprefix");
			query.ColumnSet.AddColumn("customizationoptionvalueprefix");
			query.ColumnSet.AddColumn("createdon");
			query.TopCount = 20;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Entities.Count > 0, "Should retrieve at least one publisher");

			TestContext.WriteLine($"Found {result.Entities.Count} publisher(s):");
			foreach (var publisher in result.Entities)
			{
				var uniqueName = publisher.GetAttributeValue<string>("uniquename");
				var friendlyName = publisher.GetAttributeValue<string>("friendlyname");
				var prefix = publisher.GetAttributeValue<string>("customizationprefix");
				var optionSetPrefix = publisher.GetAttributeValue<int?>("customizationoptionvalueprefix");
				var createdOn = publisher.GetAttributeValue<DateTime?>("createdon");
				TestContext.WriteLine($"  Publisher: {uniqueName} ({friendlyName}) prefix={prefix} optionset={optionSetPrefix} created={createdOn?.ToString("yyyy-MM-dd") ?? "N/A"}");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestListSolutionComponents()
		{
			if (!IsConnected) return;

			var solutionQuery = new QueryExpression("solution");
			solutionQuery.ColumnSet.AddColumn("uniquename");
			solutionQuery.Criteria.AddCondition("uniquename", ConditionOperator.Equal, "Default");
			solutionQuery.TopCount = 1;

			var solutionResult = CrmService!.RetrieveMultiple(solutionQuery);
			if (solutionResult.Entities.Count == 0)
			{
				TestContext.WriteLine("No 'Default' solution found; skipping component listing test.");
				return;
			}

			var defaultSolutionId = solutionResult.Entities[0].Id;
			TestContext.WriteLine($"Found Default solution: {defaultSolutionId}");

			var componentQuery = new QueryExpression("solutioncomponent");
			componentQuery.ColumnSet.AddColumn("solutioncomponentid");
			componentQuery.ColumnSet.AddColumn("componenttype");
			componentQuery.ColumnSet.AddColumn("objectid");
			componentQuery.Criteria.AddCondition("solutionid", ConditionOperator.Equal, defaultSolutionId);
			componentQuery.TopCount = 20;

			var componentResult = CrmService.RetrieveMultiple(componentQuery);

			Assert.IsNotNull(componentResult, "Should retrieve solution components");
			TestContext.WriteLine($"Default solution has {componentResult.Entities.Count} component(s) (showing up to 20):");

			foreach (var component in componentResult.Entities)
			{
				var componentType = component.GetAttributeValue<OptionSetValue>("componenttype")?.Value;
				var objectId = component.GetAttributeValue<Guid?>("objectid");
				TestContext.WriteLine($"  Component Type={componentType} ObjectId={objectId}");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestCreateSolutionWithCustomPrefix()
		{
			if (!IsConnected) return;

			var solutionName = CreateTestName("custsol").ToLowerInvariant();
			var publisherName = CreateTestName("custpub").ToLowerInvariant();
			var publisherId = Guid.Empty;
			var solutionId = Guid.Empty;

			try
			{
				var publisher = new Entity("publisher");
				publisher["uniquename"] = publisherName;
				publisher["friendlyname"] = $"Integration Custom Publisher {publisherName}";
				publisher["customizationprefix"] = "cz";
				publisher["customizationoptionvalueprefix"] = 60000;
				publisher["email"] = $"publisher_{publisherName}@pacx.local";

				publisherId = CrmService!.Create(publisher);
				Assert.AreNotEqual(Guid.Empty, publisherId, "Should create publisher with custom prefix");
				TestContext.WriteLine($"Created publisher: {publisherName} (ID: {publisherId}) prefix=cz");

				var solution = new Entity("solution");
				solution["uniquename"] = solutionName;
				solution["friendlyname"] = $"Integration Custom Solution {solutionName}";
				solution["version"] = "2.0.0.0";
				solution["publisherid"] = new EntityReference("publisher", publisherId);

				solutionId = CrmService.Create(solution);
				Assert.AreNotEqual(Guid.Empty, solutionId, "Should create solution with custom publisher");
				TestContext.WriteLine($"Created solution: {solutionName} (ID: {solutionId})");

				var query = new QueryExpression("solution");
				query.ColumnSet.AddColumn("uniquename");
				query.ColumnSet.AddColumn("version");
				query.ColumnSet.AddColumn("publisherid");
				query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, solutionId);

				var result = CrmService.RetrieveMultiple(query);
				Assert.AreEqual(1, result.Entities.Count, "Should find created solution");

				var created = result.Entities[0];
				Assert.AreEqual(solutionName, created.GetAttributeValue<string>("uniquename"));
				Assert.AreEqual("2.0.0.0", created.GetAttributeValue<string>("version"));
				var retrievedPublisherRef = created.GetAttributeValue<EntityReference>("publisherid");
				Assert.IsNotNull(retrievedPublisherRef);
				Assert.AreEqual(publisherId, retrievedPublisherRef.Id, "Solution should reference the correct publisher");

				CrmService.Delete("solution", solutionId);
				TestContext.WriteLine($"Deleted solution: {solutionName}");

				result = CrmService.RetrieveMultiple(query);
				Assert.AreEqual(0, result.Entities.Count, "Should not find deleted solution");
			}
			finally
			{
				CleanupSolution(solutionName, solutionId);
				CleanupPublisher(publisherName, publisherId);
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void SmokeTestSolutionManagedFlagQuery()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("solution");
			query.ColumnSet.AddColumn("uniquename");
			query.ColumnSet.AddColumn("ismanaged");
			query.ColumnSet.AddColumn("isvisible");
			query.TopCount = 20;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Entities.Count > 0, "Should retrieve solutions for managed flag analysis");

			var managedCount = 0;
			var unmanagedCount = 0;
			var hiddenCount = 0;

			foreach (var solution in result.Entities)
			{
				var name = solution.GetAttributeValue<string>("uniquename");
				var isManaged = solution.GetAttributeValue<bool>("ismanaged");
				var isVisible = solution.GetAttributeValue<bool>("isvisible");

				if (isManaged) managedCount++;
				else unmanagedCount++;

				if (!isVisible) hiddenCount++;

				TestContext.WriteLine($"Solution: {name} managed={isManaged} visible={isVisible}");
			}

			TestContext.WriteLine($"Summary: {managedCount} managed, {unmanagedCount} unmanaged, {hiddenCount} hidden");
			Assert.IsTrue(managedCount > 0, "Should have at least one managed solution");
		}

		private Guid CreatePublisher(string publisherName)
		{
			var publisher = new Entity("publisher");
			publisher["uniquename"] = publisherName;
			publisher["friendlyname"] = $"Integration Test Publisher {publisherName}";
			publisher["customizationprefix"] = "px";
			publisher["email"] = $"publisher_{publisherName}@pacx.local";

			var publisherId = CrmService!.Create(publisher);
			TestContext.WriteLine($"Created publisher: {publisherName} (ID: {publisherId})");
			return publisherId;
		}

		private void CleanupSolution(string solutionName, Guid solutionId)
		{
			try
			{
				if (solutionId != Guid.Empty)
				{
					CrmService!.Delete("solution", solutionId);
					TestContext.WriteLine($"Cleanup: Deleted solution {solutionName}");
					return;
				}

				var query = new QueryExpression("solution");
				query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionName);
				var result = CrmService!.RetrieveMultiple(query);
				if (result.Entities.Count > 0)
				{
					CrmService.Delete("solution", result.Entities[0].Id);
					TestContext.WriteLine($"Cleanup: Deleted solution {solutionName}");
				}
			}
			catch { /* Ignore cleanup errors */ }
		}

		private void CleanupPublisher(string publisherName, Guid publisherId)
		{
			try
			{
				if (publisherId != Guid.Empty)
				{
					CrmService!.Delete("publisher", publisherId);
					TestContext.WriteLine($"Cleanup: Deleted publisher {publisherName}");
					return;
				}

				var query = new QueryExpression("publisher");
				query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, publisherName);
				var result = CrmService!.RetrieveMultiple(query);
				if (result.Entities.Count > 0)
				{
					CrmService.Delete("publisher", result.Entities[0].Id);
					TestContext.WriteLine($"Cleanup: Deleted publisher {publisherName}");
				}
			}
			catch { /* Ignore cleanup errors */ }
		}
	}
}
