using Greg.Xrm.Command.Services.Mcp;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Output;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.Mcp
{
	public class McpStartCommandExecutor(
		IOutput output,
		IReadOnlyCommandRegistry commandRegistry,
		IStorage storage,
		IMcpServerLauncher launcher) : ICommandExecutor<McpStartCommand>
	{
		public async Task<CommandResult> ExecuteAsync(McpStartCommand command, CancellationToken cancellationToken)
		{
			try
			{
				output.WriteLine("Starting MCP Server...", ConsoleColor.Cyan);
				output.WriteLine($"  Transport: {command.Transport}");
				output.WriteLine($"  Port: {command.Port}");
				output.WriteLine($"  Host: {command.Host}");
				output.WriteLine();

				output.WriteLine($"Discovered {commandRegistry.Commands.Count} PACX commands as MCP tools.", ConsoleColor.Green);

				switch (command.Transport.ToLowerInvariant())
				{
					case "http":
						output.WriteLine($"Starting HTTP transport on {command.Host}:{command.Port}...", ConsoleColor.Yellow);
						output.WriteLine("HTTP transport is not enabled in the current MCP SDK package.", ConsoleColor.Yellow);
						return CommandResult.Fail("HTTP transport is not supported by the current MCP host.");

					case "stdio":
					default:
						output.WriteLine("Starting STDIO transport...", ConsoleColor.Green);
						await launcher.RunAsync(commandRegistry, storage, cancellationToken).ConfigureAwait(false);
						break;
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Error starting MCP server: {ex.Message}", ex);
			}
		}
	}
}
