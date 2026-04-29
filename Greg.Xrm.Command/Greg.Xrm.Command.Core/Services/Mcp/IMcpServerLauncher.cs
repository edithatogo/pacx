using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Services.Mcp;

public interface IMcpServerLauncher
{
	Task RunAsync(IReadOnlyCommandRegistry registry, IStorage storage, CancellationToken cancellationToken = default);
}
