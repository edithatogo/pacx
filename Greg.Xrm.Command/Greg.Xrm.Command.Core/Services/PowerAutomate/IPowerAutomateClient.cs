using System.Text.Json;

namespace Greg.Xrm.Command.Services.PowerAutomate
{
	public interface IPowerAutomateClient
	{
		Task<JsonDocument> GetFlowAsync(string environmentName, string flowName, CancellationToken cancellationToken);

		Task<JsonDocument> ListFlowsAsync(string environmentName, string? sharingStatus, bool withSolutions, bool asAdmin, CancellationToken cancellationToken);

		Task<JsonDocument> EnableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken);

		Task<JsonDocument> DisableFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken);

		Task DeleteFlowAsync(string environmentName, string flowName, bool asAdmin, CancellationToken cancellationToken);

		Task<JsonDocument> ExportFlowAsJsonAsync(string environmentName, string flowName, CancellationToken cancellationToken);

		Task<(byte[] content, string fileName)> ExportFlowAsZipAsync(string environmentName, string flowName, CancellationToken cancellationToken);
	}
}
