using System.Text.Json;

namespace Greg.Xrm.Command.Services.PowerBi
{
	public interface IPowerBiClient
	{
		Task<JsonDocument> GetAsync(string path, CancellationToken cancellationToken);

		Task<JsonDocument> PostAsync(string path, object payload, CancellationToken cancellationToken);

		Task<JsonDocument> PostFileAsync(string path, string filePath, CancellationToken cancellationToken);

		Task<JsonDocument> DeleteAsync(string path, CancellationToken cancellationToken);

		// Gateway
		Task<JsonDocument> ListGatewaysAsync(CancellationToken cancellationToken);

		Task<JsonDocument> GetGatewayAsync(string gatewayId, CancellationToken cancellationToken);
	}
}
