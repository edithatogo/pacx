using System.Text.Json;

namespace Greg.Xrm.Command.Services.Fabric
{
	public interface IFabricClient
	{
		Task<JsonDocument> GetAsync(string path, CancellationToken cancellationToken);

		Task<JsonDocument> PostAsync(string path, object payload, CancellationToken cancellationToken);

		Task<JsonDocument> DeleteAsync(string path, CancellationToken cancellationToken);
	}
}
