using System.Text.Json;

namespace Greg.Xrm.Command.Services.DesktopFlows
{
	public interface IDesktopFlowClient
	{
		Task<JsonDocument> ListDesktopFlowsAsync(string? environmentId, CancellationToken cancellationToken);

		Task<JsonDocument> GetDesktopFlowAsync(Guid id, CancellationToken cancellationToken);

		Task<JsonDocument> TriggerDesktopFlowAsync(string environmentId, Guid id, string machineGroup, IReadOnlyDictionary<string, string> inputs, CancellationToken cancellationToken);

		Task<JsonDocument> ListRunsAsync(string environmentId, Guid id, CancellationToken cancellationToken);

		Task<JsonDocument> GetRunAsync(string environmentId, string runId, CancellationToken cancellationToken);

		Task<JsonDocument> ListMachinesAsync(string environmentId, CancellationToken cancellationToken);

		Task<JsonDocument> ListMachineGroupsAsync(string environmentId, CancellationToken cancellationToken);

		Task<JsonDocument> AssignMachineToGroupAsync(string environmentId, string machineId, string groupId, CancellationToken cancellationToken);

		Task<JsonDocument> ListApprovalsAsync(string environmentId, CancellationToken cancellationToken);

		Task<JsonDocument> RespondToApprovalAsync(string environmentId, string approvalId, string decision, string? comment, CancellationToken cancellationToken);
	}
}
