using Greg.Xrm.Command.Services.Mcp;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	public sealed class FlowMonitorCommandExecutor(IOutput output) : ICommandExecutor<FlowMonitorCommand>
	{
		public Task<CommandResult> ExecuteAsync(FlowMonitorCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = FlowMcpCatalog.Load(command.CatalogPath);
				var flows = catalog.Flows
					.Where(flow => MatchesCategory(flow, command.Category))
					.Where(flow => MatchesMonitoringOperation(flow))
					.Where(flow => MatchesQuery(flow, command.Query))
					.OrderBy(flow => flow.Category, StringComparer.OrdinalIgnoreCase)
					.ThenBy(flow => flow.Name, StringComparer.OrdinalIgnoreCase)
					.ToList();

				if (flows.Count == 0)
				{
					output.WriteLine("No monitor-ready flow MCP operations found.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				output.WriteTable(
					flows,
					() => ["Name", "Provider", "Category", "Operations"],
					flow => [flow.Name, flow.Provider, flow.Category, string.Join(", ", flow.Operations)]);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to monitor flow MCP operations from <{command.CatalogPath}>: {ex.Message}", ex));
			}
		}

		private static bool MatchesCategory(FlowMcpCatalogEntry flow, string? category)
			=> string.IsNullOrWhiteSpace(category) || string.Equals(flow.Category, category, StringComparison.OrdinalIgnoreCase);

		private static bool MatchesMonitoringOperation(FlowMcpCatalogEntry flow)
			=> flow.Operations.Any(operation =>
				operation.Contains("monitor", StringComparison.OrdinalIgnoreCase) ||
				operation.Contains("watch", StringComparison.OrdinalIgnoreCase) ||
				operation.Contains("observe", StringComparison.OrdinalIgnoreCase));

		private static bool MatchesQuery(FlowMcpCatalogEntry flow, string? query)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return true;
			}

			return Contains(flow.Name, query)
				|| Contains(flow.Provider, query)
				|| Contains(flow.Category, query)
				|| Contains(flow.Kind, query)
				|| Contains(flow.Summary, query)
				|| flow.Operations.Any(operation => Contains(operation, query));
		}

		private static bool Contains(string value, string query)
			=> value.Contains(query, StringComparison.OrdinalIgnoreCase);
	}
}
