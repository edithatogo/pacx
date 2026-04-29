using System.Text.Json;
using Greg.Xrm.Command.Services.PowerBi;

namespace Greg.Xrm.Command.Commands.PowerBi
{
	[TestClass]
	public class PowerBiCommandTests
	{
		[TestMethod]
		public async Task DatasetList_ShouldCallWorkspaceDatasetsEndpoint()
		{
			var client = new RecordingPowerBiClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new DatasetListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new DatasetListCommand { WorkspaceId = "workspace-1" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("GET", client.LastMethod);
			Assert.AreEqual("/groups/workspace-1/datasets", client.LastPath);
		}

		[TestMethod]
		public async Task RefreshTrigger_ShouldPostRefreshRequest()
		{
			var client = new RecordingPowerBiClient(JsonDocument.Parse("""{}"""));
			var output = new OutputToMemory();
			var executor = new DatasetRefreshTriggerCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new DatasetRefreshTriggerCommand { WorkspaceId = "workspace-1", DatasetId = "dataset-1" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("POST", client.LastMethod);
			Assert.AreEqual("/groups/workspace-1/datasets/dataset-1/refreshes", client.LastPath);
			Assert.IsNotNull(client.LastPayload);
		}

		private sealed class RecordingPowerBiClient(JsonDocument response) : IPowerBiClient
		{
			public string? LastMethod { get; private set; }
			public string? LastPath { get; private set; }
			public object? LastPayload { get; private set; }

			public Task<JsonDocument> GetAsync(string path, CancellationToken cancellationToken)
			{
				this.LastMethod = "GET";
				this.LastPath = path;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> PostAsync(string path, object payload, CancellationToken cancellationToken)
			{
				this.LastMethod = "POST";
				this.LastPath = path;
				this.LastPayload = payload;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> PostFileAsync(string path, string filePath, CancellationToken cancellationToken)
			{
				this.LastMethod = "POSTFILE";
				this.LastPath = path;
				this.LastPayload = filePath;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> DeleteAsync(string path, CancellationToken cancellationToken)
			{
				this.LastMethod = "DELETE";
				this.LastPath = path;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> ListGatewaysAsync(CancellationToken cancellationToken)
			{
				this.LastMethod = "GET";
				this.LastPath = "gateways";
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> GetGatewayAsync(string gatewayId, CancellationToken cancellationToken)
			{
				this.LastMethod = "GET";
				this.LastPath = $"gateways/{gatewayId}";
				return Task.FromResult(Clone(response));
			}

			private static JsonDocument Clone(JsonDocument document)
			{
				return JsonDocument.Parse(document.RootElement.GetRawText());
			}
		}
	}
}
