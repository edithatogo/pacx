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

		[TestMethod]
		public async Task FlowOwnerList_ShouldCallListPermissions()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("""{"value":[{"properties":{"roleName":"CanEdit"}}]}"""));
			var output = new OutputToMemory();
			var executor = new FlowOwnerListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowOwnerListCommand
			{
				EnvironmentName = TestEnvironment,
				FlowName = TestFlowName
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ListFlowPermissions", client.LastCall);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.AreEqual(TestFlowName, client.LastFlowName);
		}

		[TestMethod]
		public async Task FlowOwnerEnsure_ShouldCallModifyPermissionsWithPut()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new FlowOwnerEnsureCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowOwnerEnsureCommand
			{
				EnvironmentName = TestEnvironment,
				FlowName = TestFlowName,
				PrincipalId = "user-abc-123",
				PrincipalType = "User",
				Role = "CanEdit"
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ModifyFlowPermissions", client.LastCall);
			StringAssert.Contains(output.ToString(), "added/updated");
		}

		[TestMethod]
		public async Task FlowOwnerRemove_ShouldCallModifyPermissionsWithDelete()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new FlowOwnerRemoveCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowOwnerRemoveCommand
			{
				EnvironmentName = TestEnvironment,
				FlowName = TestFlowName,
				PrincipalId = "user-abc-123"
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ModifyFlowPermissions", client.LastCall);
			StringAssert.Contains(output.ToString(), "removed from");
		}

		[TestMethod]
		public async Task FlowEnvironmentList_ShouldCallListEnvironments()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("""{"value":[{"name":"env-1"}]}"""));
			var output = new OutputToMemory();
			var executor = new FlowEnvironmentListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowEnvironmentListCommand(), CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ListEnvironments", client.LastCall);
		}

		[TestMethod]
		public async Task FlowEnvironmentGet_ShouldCallGetEnvironment()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("""{"name":"env-1"}"""));
			var output = new OutputToMemory();
			var executor = new FlowEnvironmentGetCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowEnvironmentGetCommand
			{
				EnvironmentName = TestEnvironment
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("GetEnvironment", client.LastCall);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
		}

		[TestMethod]
		public async Task FlowRecycleBinList_ShouldCallListRecycleBin()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new FlowRecycleBinListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowRecycleBinListCommand
			{
				EnvironmentName = TestEnvironment
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ListRecycleBinFlows", client.LastCall);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
		}

		[TestMethod]
		public async Task FlowRecycleBinRestore_ShouldCallRestore()
		{
			var client = new RecordingPowerAutomateClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new FlowRecycleBinRestoreCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new FlowRecycleBinRestoreCommand
			{
				EnvironmentName = TestEnvironment,
				FlowName = TestFlowName
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("RestoreRecycleBinFlow", client.LastCall);
			StringAssert.Contains(output.ToString(), "restored");
		}

		private sealed class RecordingPowerAutomateClient(JsonDocument response) : IPowerAutomateClient
		{
			public string? LastCall { get; private set; }
			public string? LastEnvironment { get; private set; }
			public string? LastFlowName { get; private set; }
			public string? LastSharingStatus { get; private set; }
			public bool LastWithSolutions { get; private set; }
			public bool LastAsAdmin { get; private set; }
			public string? LastPrincipalId { get; private set; }
			public object? LastPutPrincipals { get; private set; }
			public object? LastDeletePrincipals { get; private set; }

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

			public Task<JsonDocument> ListFlowPermissionsAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "ListFlowPermissions";
				LastEnvironment = environmentName;
				LastFlowName = flowName;
				LastAsAdmin = asAdmin;
				return Task.FromResult(Clone(response));
			}

			public Task ModifyFlowPermissionsAsync(string environmentName, string flowName, object putPrincipals, object deletePrincipals, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "ModifyFlowPermissions";
				LastEnvironment = environmentName;
				LastFlowName = flowName;
				LastAsAdmin = asAdmin;
				LastPutPrincipals = putPrincipals;
				LastDeletePrincipals = deletePrincipals;
				return Task.CompletedTask;
			}

			public Task<JsonDocument> ListEnvironmentsAsync(CancellationToken cancellationToken)
			{
				LastCall = "ListEnvironments";
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> GetEnvironmentAsync(string environmentName, CancellationToken cancellationToken)
			{
				LastCall = "GetEnvironment";
				LastEnvironment = environmentName;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> ListRecycleBinFlowsAsync(string environmentName, CancellationToken cancellationToken)
			{
				LastCall = "ListRecycleBinFlows";
				LastEnvironment = environmentName;
				return Task.FromResult(Clone(response));
			}

			public Task RestoreRecycleBinFlowAsync(string environmentName, string flowName, CancellationToken cancellationToken)
			{
				LastCall = "RestoreRecycleBinFlow";
				LastEnvironment = environmentName;
				LastFlowName = flowName;
				return Task.CompletedTask;
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

			public Task<JsonDocument> ListFlowPermissionsAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task ModifyFlowPermissionsAsync(string environmentName, string flowName, object putPrincipals, object deletePrincipals, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task<JsonDocument> ListEnvironmentsAsync(CancellationToken cancellationToken)
				=> throw exception;

			public Task<JsonDocument> GetEnvironmentAsync(string environmentName, CancellationToken cancellationToken)
				=> throw exception;

			public Task<JsonDocument> ListRecycleBinFlowsAsync(string environmentName, CancellationToken cancellationToken)
				=> throw exception;

			public Task RestoreRecycleBinFlowAsync(string environmentName, string flowName, CancellationToken cancellationToken)
				=> throw exception;
		}
	}
}
