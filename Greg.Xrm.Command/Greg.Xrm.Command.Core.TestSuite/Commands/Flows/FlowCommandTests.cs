using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerAutomate;

namespace Greg.Xrm.Command.Commands.Flows
{
	[TestClass]
	public class FlowCommandTests
	{
		private const string TestEnvironment = "test-env";
		private const string TestFlowName = "flow-123";

		[TestMethod]
		public async Task FlowList_ShouldCallListFlowsWithCorrectParameters()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("""{"value":[{"name":"flow-1"}]}"""));
			var output = new OutputToMemory();
			var executor = new FlowListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowListCommand
			{
				EnvironmentName = TestEnvironment,
				SharingStatus = "personal",
				WithSolutions = true,
				AsAdmin = false
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ListFlows", client.LastCall);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.AreEqual("personal", client.LastSharingStatus);
			Assert.IsTrue(client.LastWithSolutions);
			Assert.IsFalse(client.LastAsAdmin);
			StringAssert.Contains(output.ToString(), "\"name\"");
		}

		[TestMethod]
		public async Task FlowList_WithAdminFlag_ShouldPassAdminMode()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new FlowListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowListCommand
			{
				EnvironmentName = TestEnvironment,
				AsAdmin = true
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.IsTrue(client.LastAsAdmin);
		}

		[TestMethod]
		public async Task FlowGet_ShouldCallGetFlowWithCorrectParameters()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("""{"name":"flow-1","properties":{"displayName":"Test Flow"}}"""));
			var output = new OutputToMemory();
			var executor = new FlowGetCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowGetCommand
			{
				EnvironmentName = TestEnvironment,
				FlowName = TestFlowName
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("GetFlow", client.LastCall);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.AreEqual(TestFlowName, client.LastFlowName);
			StringAssert.Contains(output.ToString(), "Test Flow");
		}

		[TestMethod]
		public async Task FlowEnable_ShouldCallEnableFlowWithCorrectParameters()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new FlowEnableCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowEnableCommand
			{
				EnvironmentName = TestEnvironment,
				FlowName = TestFlowName,
				AsAdmin = true
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("EnableFlow", client.LastCall);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.AreEqual(TestFlowName, client.LastFlowName);
			Assert.IsTrue(client.LastAsAdmin);
		}

		[TestMethod]
		public async Task FlowDisable_ShouldCallDisableFlowWithCorrectParameters()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new FlowDisableCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowDisableCommand
			{
				EnvironmentName = TestEnvironment,
				FlowName = TestFlowName,
				AsAdmin = false
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("DisableFlow", client.LastCall);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.AreEqual(TestFlowName, client.LastFlowName);
			Assert.IsFalse(client.LastAsAdmin);
		}

		[TestMethod]
		public async Task FlowRemove_ShouldCallDeleteFlowWithCorrectParameters()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new FlowRemoveCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowRemoveCommand
			{
				EnvironmentName = TestEnvironment,
				FlowName = TestFlowName,
				Confirm = true
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("DeleteFlow", client.LastCall);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.AreEqual(TestFlowName, client.LastFlowName);
			StringAssert.Contains(output.ToString(), "has been deleted");
		}

		[TestMethod]
		public async Task FlowRemove_WhenApiFails_ShouldReturnFailure()
		{
			var client = new ThrowingPowerAutomateClient(new InvalidOperationException("Flow not found"));
			var output = new OutputToMemory();
			var executor = new FlowRemoveCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowRemoveCommand
			{
				EnvironmentName = TestEnvironment,
				FlowName = "non-existent"
			}, CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(result.ErrorMessage, "Failed to delete flow");
		}

		[TestMethod]
		public async Task FlowList_WhenApiFails_ShouldReturnFailure()
		{
			var client = new ThrowingPowerAutomateClient(new InvalidOperationException("API unavailable"));
			var output = new OutputToMemory();
			var executor = new FlowListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowListCommand
			{
				EnvironmentName = TestEnvironment
			}, CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(result.ErrorMessage, "Failed to list flows");
		}

		private sealed class RecordingPowerAutomateClient(JsonDocument response) : IPowerAutomateClient
		{
			public string? LastCall { get; private set; }
			public string? LastEnvironment { get; private set; }
			public string? LastFlowName { get; private set; }
			public string? LastSharingStatus { get; private set; }
			public bool LastWithSolutions { get; private set; }
			public bool LastAsAdmin { get; private set; }

			public Task<JsonDocument> ListFlowsAsync(string environmentName, string? sharingStatus, bool withSolutions, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "ListFlows";
				LastEnvironment = environmentName;
				LastSharingStatus = sharingStatus;
				LastWithSolutions = withSolutions;
				LastAsAdmin = asAdmin;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> GetFlowAsync(string environmentName, string flowName, CancellationToken cancellationToken)
			{
				LastCall = "GetFlow";
				LastEnvironment = environmentName;
				LastFlowName = flowName;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> EnableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "EnableFlow";
				LastEnvironment = environmentName;
				LastFlowName = flowName;
				LastAsAdmin = asAdmin;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> DisableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "DisableFlow";
				LastEnvironment = environmentName;
				LastFlowName = flowName;
				LastAsAdmin = asAdmin;
				return Task.FromResult(Clone(response));
			}

			public Task DeleteFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "DeleteFlow";
				LastEnvironment = environmentName;
				LastFlowName = flowName;
				LastAsAdmin = asAdmin;
				return Task.CompletedTask;
			}

			public Task<JsonDocument> ExportFlowAsJsonAsync(string environmentName, string flowName, CancellationToken cancellationToken)
			{
				LastCall = "ExportFlowAsJson";
				LastEnvironment = environmentName;
				LastFlowName = flowName;
				return Task.FromResult(Clone(response));
			}

			public Task<(byte[] content, string fileName)> ExportFlowAsZipAsync(string environmentName, string flowName, CancellationToken cancellationToken)
			{
				LastCall = "ExportFlowAsZip";
				LastEnvironment = environmentName;
				LastFlowName = flowName;
				return Task.FromResult<(byte[], string)>((Array.Empty<byte>(), $"{flowName}.zip"));
			}

			private static JsonDocument Clone(JsonDocument document)
			{
				return JsonDocument.Parse(document.RootElement.GetRawText());
			}
		}

		private sealed class ThrowingPowerAutomateClient(Exception exception) : IPowerAutomateClient
		{
			public Task<JsonDocument> ListFlowsAsync(string environmentName, string? sharingStatus, bool withSolutions, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task<JsonDocument> GetFlowAsync(string environmentName, string flowName, CancellationToken cancellationToken)
				=> throw exception;

			public Task<JsonDocument> EnableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task<JsonDocument> DisableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task DeleteFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task<JsonDocument> ExportFlowAsJsonAsync(string environmentName, string flowName, CancellationToken cancellationToken)
				=> throw exception;

			public Task<(byte[] content, string fileName)> ExportFlowAsZipAsync(string environmentName, string flowName, CancellationToken cancellationToken)
				=> throw exception;
		}
	}
}
