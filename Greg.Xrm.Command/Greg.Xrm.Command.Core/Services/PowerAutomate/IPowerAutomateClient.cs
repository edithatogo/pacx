using System.Text.Json;

namespace Greg.Xrm.Command.Services.PowerAutomate
{
	public interface IPowerAutomateClient
	{
		// Flow CRUD
		Task<JsonDocument> GetFlowAsync(string environmentName, string flowName, CancellationToken cancellationToken);

		Task<JsonDocument> ListFlowsAsync(string environmentName, string? sharingStatus, bool withSolutions, bool asAdmin, CancellationToken cancellationToken);

		Task<JsonDocument> EnableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken);

		Task<JsonDocument> DisableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken);

		Task DeleteFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken);

		Task<JsonDocument> ExportFlowAsJsonAsync(string environmentName, string flowName, CancellationToken cancellationToken);

		Task<(byte[] content, string fileName)> ExportFlowAsZipAsync(string environmentName, string flowName, CancellationToken cancellationToken);

		// Flow permissions (owner management)
		Task<JsonDocument> ListFlowPermissionsAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken);

		Task ModifyFlowPermissionsAsync(string environmentName, string flowName, object putPrincipals, object deletePrincipals, bool asAdmin, CancellationToken cancellationToken);

		// Flow environment
		Task<JsonDocument> ListEnvironmentsAsync(CancellationToken cancellationToken);

		Task<JsonDocument> GetEnvironmentAsync(string environmentName, CancellationToken cancellationToken);

		// Flow recycle bin
		Task<JsonDocument> ListRecycleBinFlowsAsync(string environmentName, CancellationToken cancellationToken);

		Task RestoreRecycleBinFlowAsync(string environmentName, string flowName, CancellationToken cancellationToken);
	}
}
