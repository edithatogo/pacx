using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Tooling;

namespace Greg.Xrm.Command.Commands.Tool
{
	public sealed class BrowseCommandExecutor(IOutput output) : ICommandExecutor<BrowseCommand>
	{
		public Task<CommandResult> ExecuteAsync(BrowseCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = ToolCatalog.Load(command.CatalogPath);
				var tools = catalog.Tools
					.Where(tool => MatchesCategory(tool, command.Category))
					.Where(tool => MatchesQuery(tool, command.Query))
					.OrderBy(tool => tool.Category, StringComparer.OrdinalIgnoreCase)
					.ThenBy(tool => tool.Name, StringComparer.OrdinalIgnoreCase)
					.ToList();

				if (tools.Count == 0)
				{
					output.WriteLine("No tools found.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				output.WriteTable(
					tools,
					() => ["Name", "Provider", "Category", "Kind", "Summary"],
					tool => [tool.Name, tool.Provider, tool.Category, tool.Kind, tool.Summary]);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to browse tools from <{command.CatalogPath}>: {ex.Message}", ex));
			}
		}

		private static bool MatchesCategory(ToolCatalogEntry tool, string? category)
			=> string.IsNullOrWhiteSpace(category) || string.Equals(tool.Category, category, StringComparison.OrdinalIgnoreCase);

		private static bool MatchesQuery(ToolCatalogEntry tool, string? query)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return true;
			}

			return Contains(tool.Name, query)
				|| Contains(tool.Provider, query)
				|| Contains(tool.Category, query)
				|| Contains(tool.Kind, query)
				|| Contains(tool.Summary, query)
				|| tool.Capabilities.Any(cap => Contains(cap, query));
		}

		private static bool Contains(string value, string query)
			=> value.Contains(query, StringComparison.OrdinalIgnoreCase);
	}
}
