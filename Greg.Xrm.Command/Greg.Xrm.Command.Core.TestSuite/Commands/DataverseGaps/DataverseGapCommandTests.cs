using System.Text.Json;
using Greg.Xrm.Command.Services.DataverseGaps;

namespace Greg.Xrm.Command.Commands.DataverseGaps
{
	[TestClass]
	public class DataverseGapCommandTests
	{
		[TestMethod]
		public async Task BusinessRuleList_ShouldFilterWorkflowCategoryAndTable()
		{
			var client = new RecordingDataverseGapClient();
			var executor = new BusinessRuleListCommandExecutor(client, new OutputToMemory());

			var result = await executor.ExecuteAsync(new BusinessRuleListCommand { Table = "account" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("workflow", client.LastEntityName);
			Assert.AreEqual(2, client.LastFilters["category"]);
			Assert.AreEqual("account", client.LastFilters["primaryentity"]);
		}

		[TestMethod]
		public async Task EndpointRegister_ShouldCreateServiceEndpoint()
		{
			var client = new RecordingDataverseGapClient();
			var executor = new EndpointRegisterCommandExecutor(client, new OutputToMemory());

			var result = await executor.ExecuteAsync(new EndpointRegisterCommand { Url = "https://example.test/hook", Auth = "WebhookKey", Name = "Contoso Hook" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("serviceendpoint", client.LastEntityName);
			Assert.AreEqual("Contoso Hook", client.LastAttributes["name"]);
			Assert.AreEqual("https://example.test/hook", client.LastAttributes["url"]);
			Assert.AreEqual("WebhookKey", client.LastAttributes["authtype"]);
		}

		[TestMethod]
		public async Task BusinessRuleExport_ShouldWriteFile()
		{
			var client = new RecordingDataverseGapClient();
			var outputFile = Path.Combine(Path.GetTempPath(), "pacx-business-rule-export-test.json");
			if (File.Exists(outputFile))
			{
				File.Delete(outputFile);
			}

			var executor = new BusinessRuleExportCommandExecutor(client, new OutputToMemory());
			var result = await executor.ExecuteAsync(new BusinessRuleExportCommand { Id = Guid.NewGuid(), FilePath = outputFile }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.IsTrue(File.Exists(outputFile));
			StringAssert.Contains(await File.ReadAllTextAsync(outputFile), "workflow");
		}

		private sealed class RecordingDataverseGapClient : IDataverseGapClient
		{
			public string? LastEntityName { get; private set; }
			public IReadOnlyDictionary<string, object?> LastFilters { get; private set; } = new Dictionary<string, object?>();
			public IReadOnlyDictionary<string, object?> LastAttributes { get; private set; } = new Dictionary<string, object?>();

			public Task<JsonDocument> QueryAsync(string entityName, IReadOnlyCollection<string> columns, IReadOnlyDictionary<string, object?> filters, CancellationToken cancellationToken)
			{
				this.LastEntityName = entityName;
				this.LastFilters = filters;
				return Task.FromResult(JsonDocument.Parse("""{"value":[]}"""));
			}

			public Task<JsonDocument> ExportWorkflowAsync(Guid id, CancellationToken cancellationToken)
				=> Task.FromResult(JsonDocument.Parse("""{"logicalName":"workflow"}"""));

			public Task<JsonDocument> ImportWorkflowAsync(string filePath, string? tableName, int category, CancellationToken cancellationToken)
				=> Task.FromResult(JsonDocument.Parse("""{"id":"00000000-0000-0000-0000-000000000001"}"""));

			public Task<JsonDocument> SetStateAsync(string entityName, Guid id, int stateCode, int statusCode, CancellationToken cancellationToken)
				=> Task.FromResult(JsonDocument.Parse("""{"stateCode":1}"""));

			public Task<JsonDocument> CreateAsync(string entityName, IReadOnlyDictionary<string, object?> attributes, CancellationToken cancellationToken)
			{
				this.LastEntityName = entityName;
				this.LastAttributes = attributes;
				return Task.FromResult(JsonDocument.Parse("""{"id":"00000000-0000-0000-0000-000000000001"}"""));
			}

			public Task<JsonDocument> DeleteAsync(string entityName, Guid id, CancellationToken cancellationToken)
				=> Task.FromResult(JsonDocument.Parse("""{"deleted":true}"""));

			public Task<JsonDocument> ExecuteActionAsync(string actionName, IReadOnlyDictionary<string, object?> parameters, CancellationToken cancellationToken)
				=> Task.FromResult(JsonDocument.Parse("""{"action":"ok"}"""));

			public Task<JsonDocument> UploadFileAsync(string tableName, Guid recordId, string columnName, string filePath, CancellationToken cancellationToken)
				=> Task.FromResult(JsonDocument.Parse("""{"uploaded":true}"""));
		}
	}
}
