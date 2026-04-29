using System.Text.Json;

namespace Greg.Xrm.Command.Services.CopilotStudio
{
	public interface ICopilotStudioClient
	{
		Task<JsonDocument> GetAsync(string environmentId, string path, CancellationToken cancellationToken);

		Task<JsonDocument> PostAsync(string environmentId, string path, object payload, CancellationToken cancellationToken);
	}
}
