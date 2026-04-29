using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command;

public sealed partial class Bootstrapper
{
	[LoggerMessage(LogLevel.Trace, "1. StartAsync has been called.")]
	private static partial void LogStartAsync(ILogger logger);
}
