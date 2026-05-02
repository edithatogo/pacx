using System.Text.Json;

namespace Greg.Xrm.Command.Services.Tabular
{
	public interface ITabularClient
	{
		Task<string> DeployBimAsync(string workspaceId, string datasetName, string bimContent, CancellationToken ct);

		Task<string?> GetDeployedBimAsync(string workspaceId, string datasetId, CancellationToken ct);

		Task<string?> GetDatasetIdByNameAsync(string workspaceId, string datasetName, CancellationToken ct);

		Task UpdateDefinitionAsync(string workspaceId, string datasetId, string bimContent, CancellationToken ct);
	}
}
