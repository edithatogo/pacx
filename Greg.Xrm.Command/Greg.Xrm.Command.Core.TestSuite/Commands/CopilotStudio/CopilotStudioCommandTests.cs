using System.Text.Json;
using Greg.Xrm.Command.Services.CopilotStudio;

namespace Greg.Xrm.Command.Commands.CopilotStudio
{
	[TestClass]
	public class CopilotStudioCommandTests
	{
		[TestMethod]
		public async Task AgentList_ShouldCallAgentsEndpoint()
		{
			var client = new RecordingCopilotClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new CopilotAgentListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new CopilotAgentListCommand { EnvironmentId = "env-1" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("GET", client.LastMethod);
			Assert.AreEqual("env-1", client.LastEnvironment);
			Assert.AreEqual("/agents", client.LastPath);
		}

		[TestMethod]
		public async Task KnowledgeAdd_ShouldPostSource()
		{
			var client = new RecordingCopilotClient(JsonDocument.Parse("""{"id":"knowledge-1"}"""));
			var output = new OutputToMemory();
			var executor = new CopilotKnowledgeAddCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new CopilotKnowledgeAddCommand { EnvironmentId = "env-1", AgentId = "agent-1", Source = "https://contoso" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("POST", client.LastMethod);
			Assert.AreEqual("/agents/agent-1/knowledge", client.LastPath);
			Assert.IsNotNull(client.LastPayload);
		}

		private sealed class RecordingCopilotClient(JsonDocument response) : ICopilotStudioClient
		{
			public string? LastMethod { get; private set; }
			public string? LastEnvironment { get; private set; }
			public string? LastPath { get; private set; }
			public object? LastPayload { get; private set; }

			public Task<JsonDocument> GetAsync(string environmentId, string path, CancellationToken cancellationToken)
			{
				this.LastMethod = "GET";
				this.LastEnvironment = environmentId;
				this.LastPath = path;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> PostAsync(string environmentId, string path, object payload, CancellationToken cancellationToken)
			{
				this.LastMethod = "POST";
				this.LastEnvironment = environmentId;
				this.LastPath = path;
				this.LastPayload = payload;
				return Task.FromResult(Clone(response));
			}

			private static JsonDocument Clone(JsonDocument document)
			{
				return JsonDocument.Parse(document.RootElement.GetRawText());
			}
		}
	}
}
