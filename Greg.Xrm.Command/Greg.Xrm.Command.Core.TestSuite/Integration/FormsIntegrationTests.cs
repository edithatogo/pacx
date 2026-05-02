using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Integration
{
	[TestClass]
	[TestCategory("Integration")]
	public class FormsIntegrationTests : IntegrationTestBase
	{
		[TestMethod]
		public void Forms_SystemFormList_ShouldRetrieveFormRecords()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("systemform");
			query.ColumnSet.AddColumn("name");
			query.ColumnSet.AddColumn("formid");
			query.ColumnSet.AddColumn("type");
			query.ColumnSet.AddColumn("objecttypecode");
			query.ColumnSet.AddColumn("ismanaged");
			query.TopCount = 20;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result, "SystemForm query result should not be null");
			Assert.IsTrue(result.Entities.Count > 0, "Should retrieve at least one system form");

			foreach (var form in result.Entities)
			{
				var name = form.GetAttributeValue<string>("name");
				var type = form.GetAttributeValue<OptionSetValue>("type")?.Value;
				var table = form.GetAttributeValue<string>("objecttypecode");
				var isManaged = form.GetAttributeValue<bool>("ismanaged");

				TestContext.WriteLine($"Form: {name ?? "(unnamed)"} | Type: {type} | Table: {table} | Managed: {isManaged}");
			}
		}

		[TestMethod]
		public void Forms_MainFormsByTable_ShouldRetrieveForKnownTable()
		{
			if (!IsConnected) return;

			var tableName = "account";

			var query = new QueryExpression("systemform");
			query.ColumnSet.AddColumns("name", "formid", "formxml", "type", "ismanaged");
			query.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, tableName);
			query.Criteria.AddCondition("type", ConditionOperator.Equal, 2);
			query.AddOrder("formid", OrderType.Ascending);
			query.TopCount = 5;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result, "Should retrieve main forms for account table");

			TestContext.WriteLine($"Found {result.Entities.Count} main form(s) for table '{tableName}'");

			foreach (var form in result.Entities)
			{
				var name = form.GetAttributeValue<string>("name");
				var formId = form.GetAttributeValue<Guid>("formid");
				var hasFormXml = !string.IsNullOrEmpty(form.GetAttributeValue<string>("formxml"));

				TestContext.WriteLine($"  Form: {name ?? "(unnamed)"} (ID: {formId}) | Has FormXml: {hasFormXml}");
			}
		}

		[TestMethod]
		public void Forms_FormTypeMetadata_ShouldContainExpectedValues()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("systemform");
			query.ColumnSet.AddColumn("type");
			query.ColumnSet.AddColumn("name");
			query.ColumnSet.AddColumn("objecttypecode");
			query.TopCount = 50;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Entities.Count > 0, "Should retrieve forms for type analysis");

			var formTypes = result.Entities
				.Select(e => e.GetAttributeValue<OptionSetValue>("type")?.Value)
				.Where(v => v.HasValue)
				.Select(v => v!.Value)
				.Distinct()
				.OrderBy(v => v)
				.ToList();

			TestContext.WriteLine($"Distinct form types found in environment: {string.Join(", ", formTypes)}");

			foreach (var form in result.Entities)
			{
				var name = form.GetAttributeValue<string>("name");
				var type = form.GetAttributeValue<OptionSetValue>("type")?.Value;
				var table = form.GetAttributeValue<string>("objecttypecode");
				TestContext.WriteLine($"  [{type}] Table={table}, Name={name ?? "(unnamed)"}");
			}
		}

		[TestMethod]
		public void Forms_FormXmlContent_ShouldBeValidForSampleForms()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("systemform");
			query.ColumnSet.AddColumns("name", "formxml", "objecttypecode", "type");
			query.Criteria.AddCondition("type", ConditionOperator.Equal, 2);
			query.TopCount = 3;

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result, "Should retrieve forms with FormXml");
			Assert.IsTrue(result.Entities.Count > 0, "Should have at least one main form");

			foreach (var form in result.Entities)
			{
				var formXml = form.GetAttributeValue<string>("formxml");
				var name = form.GetAttributeValue<string>("name");
				var table = form.GetAttributeValue<string>("objecttypecode");

				Assert.IsFalse(string.IsNullOrEmpty(formXml), $"Form '{name}' on table '{table}' should have FormXml content");
				Assert.IsTrue(formXml.Contains("<form"), $"FormXml for '{name}' should contain <form> element");
				Assert.IsTrue(formXml.Contains("<tabs>"), $"FormXml for '{name}' should contain <tabs> element");

				TestContext.WriteLine($"Form '{name}' on '{table}': FormXml length = {formXml.Length} chars");
			}
		}

		[TestMethod]
		public void Forms_SystemFormCount_ShouldExceedMinimum()
		{
			if (!IsConnected) return;

			var query = new QueryExpression("systemform");
			query.ColumnSet.AddColumn("formid");

			var result = CrmService!.RetrieveMultiple(query);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Entities.Count >= 5,
				$"Environment should have at least 5 system forms (found {result.Entities.Count})");

			TestContext.WriteLine($"Total system forms in environment: {result.Entities.Count}");
		}
	}
}
