using Greg.Xrm.Command.Services.Mcp;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	public sealed class FlowGovernCommandExecutor(IOutput output) : ICommandExecutor<FlowGovernCommand>
	{
		public Task<CommandResult> ExecuteAsync(FlowGovernCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = FlowMcpCatalog.Load(command.CatalogPath);
				var flows = catalog.Flows
					.Where(flow => IsGovernanceCapable(flow))
					.Where(flow => MatchesQuery(flow, command.Query))
					.OrderBy(flow => flow.Name, StringComparer.OrdinalIgnoreCase)
					.ToList();

				if (flows.Count == 0)
				{
					output.WriteLine("No governance-ready flow entries found.", ConsoleColor.Yellow);
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
				return Task.FromResult(CommandResult.Fail($"Unable to browse flow governance entries from <{command.CatalogPath}>: {ex.Message}", ex));
			}
		}

		private static bool IsGovernanceCapable(FlowMcpCatalogEntry flow)
			=> string.Equals(flow.Category, "Governance", StringComparison.OrdinalIgnoreCase)
			|| flow.Operations.Any(operation =>
				operation.Contains("govern", StringComparison.OrdinalIgnoreCase) ||
				operation.Contains("approve", StringComparison.OrdinalIgnoreCase) ||
				operation.Contains("audit", StringComparison.OrdinalIgnoreCase));

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
