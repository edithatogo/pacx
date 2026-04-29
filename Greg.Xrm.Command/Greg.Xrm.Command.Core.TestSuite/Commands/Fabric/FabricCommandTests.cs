using System.Text.Json;
using Greg.Xrm.Command.Commands.Fabric;
using Greg.Xrm.Command.Services.Fabric;

namespace Greg.Xrm.Command.Commands.Fabric
{
	[TestClass]
	public class FabricCommandTests
	{
		[TestMethod]
		public async Task WorkspaceList_ShouldCallFabricWorkspacesEndpoint()
		{
			var client = new RecordingFabricClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new FabricWorkspaceListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FabricWorkspaceListCommand(), CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("GET", client.LastMethod);
			Assert.AreEqual("/workspaces", client.LastPath);
			StringAssert.Contains(output.ToString(), "\"value\"");
		}

		[TestMethod]
		public async Task OneLakeShortcutCreate_ShouldPostShortcutPayload()
		{
			var client = new RecordingFabricClient(JsonDocument.Parse("""{"id":"shortcut-1"}"""));
			var output = new OutputToMemory();
			var executor = new OneLakeShortcutCreateCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new OneLakeShortcutCreateCommand
			{
				WorkspaceId = "workspace-1",
				ItemId = "lakehouse-1",
				SourcePath = "abfss://raw@storage.dfs.core.windows.net/table",
				TargetPath = "Tables/raw",
				SourceType = "adlsGen2"
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("POST", client.LastMethod);
			Assert.AreEqual("/workspaces/workspace-1/items/lakehouse-1/shortcuts", client.LastPath);
			Assert.IsNotNull(client.LastPayload);
		}

		private sealed class RecordingFabricClient(JsonDocument response) : IFabricClient
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

			public Task<JsonDocument> DeleteAsync(string path, CancellationToken cancellationToken)
			{
				this.LastMethod = "DELETE";
				this.LastPath = path;
				return Task.FromResult(Clone(response));
			}

			private static JsonDocument Clone(JsonDocument document)
			{
				return JsonDocument.Parse(document.RootElement.GetRawText());
			}
		}
	}
}
