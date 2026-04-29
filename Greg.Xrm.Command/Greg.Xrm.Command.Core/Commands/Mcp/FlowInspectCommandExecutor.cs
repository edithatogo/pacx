using Greg.Xrm.Command.Services.Mcp;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	public sealed class FlowInspectCommandExecutor(IOutput output) : ICommandExecutor<FlowInspectCommand>
	{
		public Task<CommandResult> ExecuteAsync(FlowInspectCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = FlowMcpCatalog.Load(command.CatalogPath);
				var flow = catalog.Flows.FirstOrDefault(entry =>
					string.Equals(entry.Name, command.Name, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(entry.Provider, command.Name, StringComparison.OrdinalIgnoreCase));

				if (flow is null)
				{
					return Task.FromResult(CommandResult.Fail($"Flow entry <{command.Name}> was not found in <{command.CatalogPath}>."));
				}

				output.WriteLine($"{flow.Name} ({flow.Category})", ConsoleColor.Green);
				output.WriteLine(flow.Summary);

				if (!string.IsNullOrWhiteSpace(flow.HomePage))
				{
					output.WriteLine($"Home page: {flow.HomePage}");
				}

				if (flow.Operations.Count > 0)
				{
					output.WriteLine("Operations:");
					foreach (var operation in flow.Operations)
					{
						output.WriteLine($"- {operation}");
					}
				}

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to inspect flow MCP operations from <{command.CatalogPath}>: {ex.Message}", ex));
			}
		}
	}
}
