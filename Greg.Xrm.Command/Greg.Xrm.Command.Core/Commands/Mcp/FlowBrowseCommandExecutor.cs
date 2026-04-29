using Greg.Xrm.Command.Services.Mcp;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	public sealed class FlowBrowseCommandExecutor(IOutput output) : ICommandExecutor<FlowBrowseCommand>
	{
		public Task<CommandResult> ExecuteAsync(FlowBrowseCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = FlowMcpCatalog.Load(command.CatalogPath);
				var flows = catalog.Flows
					.Where(flow => MatchesCategory(flow, command.Category))
					.Where(flow => MatchesQuery(flow, command.Query))
					.OrderBy(flow => flow.Category, StringComparer.OrdinalIgnoreCase)
					.ThenBy(flow => flow.Name, StringComparer.OrdinalIgnoreCase)
					.ToList();

				if (flows.Count == 0)
				{
					output.WriteLine("No flow MCP operations found.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				output.WriteTable(
					flows,
					() => ["Name", "Provider", "Category", "Kind", "Summary"],
					flow => [flow.Name, flow.Provider, flow.Category, flow.Kind, flow.Summary]);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to browse flow MCP operations from <{command.CatalogPath}>: {ex.Message}", ex));
			}
		}

		private static bool MatchesCategory(FlowMcpCatalogEntry flow, string? category)
			=> string.IsNullOrWhiteSpace(category) || string.Equals(flow.Category, category, StringComparison.OrdinalIgnoreCase);

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
