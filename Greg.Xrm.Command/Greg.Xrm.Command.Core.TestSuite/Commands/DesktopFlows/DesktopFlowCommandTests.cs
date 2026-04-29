using System.Text.Json;
using Greg.Xrm.Command.Services.DesktopFlows;

namespace Greg.Xrm.Command.Commands.DesktopFlows
{
	[TestClass]
	public class DesktopFlowCommandTests
	{
		[TestMethod]
		public async Task Trigger_ShouldForwardMachineGroupAndInputs()
		{
			var client = new RecordingDesktopFlowClient(JsonDocument.Parse("""{"runId":"run-1"}"""));
			var output = new OutputToMemory();
			var executor = new DesktopFlowTriggerCommandExecutor(client, output);
			var flowId = Guid.NewGuid();

			var result = await executor.ExecuteAsync(new DesktopFlowTriggerCommand
			{
				EnvironmentId = "env-1",
				Id = flowId,
				MachineGroup = "group-1",
				Input = "name=value" + '\x1F' + "count=2"
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("trigger", client.LastCall);
			Assert.AreEqual("env-1", client.LastEnvironment);
			Assert.AreEqual(flowId, client.LastFlowId);
			Assert.AreEqual("value", client.LastInputs?["name"]);
			Assert.AreEqual("2", client.LastInputs?["count"]);
		}

		[TestMethod]
		public async Task MachineList_ShouldCallMachineEndpoint()
		{
			var client = new RecordingDesktopFlowClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new DesktopFlowMachineListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new DesktopFlowMachineListCommand { EnvironmentId = "env-1" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("machines", client.LastCall);
			Assert.AreEqual("env-1", client.LastEnvironment);
		}

		private sealed class RecordingDesktopFlowClient(JsonDocument response) : IDesktopFlowClient
		{
			public string? LastCall { get; private set; }
			public string? LastEnvironment { get; private set; }
			public Guid LastFlowId { get; private set; }
			public IReadOnlyDictionary<string, string>? LastInputs { get; private set; }

			public Task<JsonDocument> ListDesktopFlowsAsync(string? environmentId, CancellationToken cancellationToken)
			{
				this.LastCall = "list";
				this.LastEnvironment = environmentId;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> GetDesktopFlowAsync(Guid id, CancellationToken cancellationToken)
			{
				this.LastCall = "get";
				this.LastFlowId = id;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> TriggerDesktopFlowAsync(string environmentId, Guid id, string machineGroup, IReadOnlyDictionary<string, string> inputs, CancellationToken cancellationToken)
			{
				this.LastCall = "trigger";
				this.LastEnvironment = environmentId;
				this.LastFlowId = id;
				this.LastInputs = inputs;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> ListRunsAsync(string environmentId, Guid id, CancellationToken cancellationToken)
			{
				this.LastCall = "runs";
				this.LastEnvironment = environmentId;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> GetRunAsync(string environmentId, string runId, CancellationToken cancellationToken)
			{
				this.LastCall = "run";
				this.LastEnvironment = environmentId;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> ListMachinesAsync(string environmentId, CancellationToken cancellationToken)
			{
				this.LastCall = "machines";
				this.LastEnvironment = environmentId;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> ListMachineGroupsAsync(string environmentId, CancellationToken cancellationToken)
			{
				this.LastCall = "machine-groups";
				this.LastEnvironment = environmentId;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> AssignMachineToGroupAsync(string environmentId, string machineId, string groupId, CancellationToken cancellationToken)
			{
				this.LastCall = "assign";
				this.LastEnvironment = environmentId;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> ListApprovalsAsync(string environmentId, CancellationToken cancellationToken)
			{
				this.LastCall = "approvals";
				this.LastEnvironment = environmentId;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> RespondToApprovalAsync(string environmentId, string approvalId, string decision, string? comment, CancellationToken cancellationToken)
			{
				this.LastCall = "approval-response";
				this.LastEnvironment = environmentId;
				return Task.FromResult(Clone(response));
			}

			private static JsonDocument Clone(JsonDocument document)
			{
				return JsonDocument.Parse(document.RootElement.GetRawText());
			}
		}
	}
}
