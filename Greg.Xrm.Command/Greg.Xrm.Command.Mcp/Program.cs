using Greg.Xrm.Command;
using Greg.Xrm.Command.Commands.Help;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Mcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command.Mcp;

internal static class Program
{
	private static async Task<int> Main(string[] args)
	{
		var services = new ServiceCollection();
		services.AddSingleton<IStorage>(new Storage());
		services.AddSingleton<CommandRegistry>();
		services.AddSingleton<IReadOnlyCommandRegistry>(sp => sp.GetRequiredService<CommandRegistry>());
		services.AddSingleton<ICommandRegistry>(sp => sp.GetRequiredService<CommandRegistry>());
		services.AddSingleton<IOutput, OutputToMemory>();
		services.AddSingleton<IMcpServerLauncher, McpServerLauncher>();
		services.AddLogging(logging =>
		{
			logging.ClearProviders();
			logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);
		});

		using var provider = services.BuildServiceProvider();
		var registry = provider.GetRequiredService<IReadOnlyCommandRegistry>();
		var storage = provider.GetRequiredService<IStorage>();

		if (registry is ICommandRegistry writableRegistry)
		{
			writableRegistry.InitializeFromAssembly(typeof(HelpCommand).Assembly);
			writableRegistry.ScanPluginsFolder(new CommandLineArguments(args));
		}

		var launcher = provider.GetRequiredService<IMcpServerLauncher>();
		await launcher.RunAsync(registry, storage, CancellationToken.None);
		return 0;
	}
}
