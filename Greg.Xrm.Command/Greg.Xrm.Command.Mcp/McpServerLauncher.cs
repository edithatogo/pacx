using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Mcp;

namespace Greg.Xrm.Command.Mcp;

public sealed class McpServerLauncher : IMcpServerLauncher
{
	public Task RunAsync(IReadOnlyCommandRegistry registry, IStorage storage, CancellationToken cancellationToken = default)
	{
		return McpServerHost.RunAsync(registry, storage, cancellationToken);
	}
}
