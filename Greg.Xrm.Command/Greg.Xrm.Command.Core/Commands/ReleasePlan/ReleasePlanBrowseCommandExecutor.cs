using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.ReleasePlan;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	public sealed class ReleasePlanBrowseCommandExecutor(IOutput output) : ICommandExecutor<ReleasePlanBrowseCommand>
	{
		public Task<CommandResult> ExecuteAsync(ReleasePlanBrowseCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = ReleasePlanCatalog.Load(command.CatalogPath);
				var families = catalog.Families
					.Where(family => MatchesCategory(family, command.Category))
					.Where(family => MatchesQuery(family, command.Query))
					.OrderBy(family => family.Category, StringComparer.OrdinalIgnoreCase)
					.ThenBy(family => family.Name, StringComparer.OrdinalIgnoreCase)
					.ToList();

				if (families.Count == 0)
				{
					output.WriteLine("No release-plan families found.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				output.WriteTable(
					families,
					() => ["Name", "Category", "Summary"],
					family => [family.Name, family.Category, family.Summary]);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to browse release-plan families from <{command.CatalogPath}>: {ex.Message}", ex));
			}
		}

		private static bool MatchesCategory(ReleasePlanFamily family, string? category)
			=> string.IsNullOrWhiteSpace(category) || string.Equals(family.Category, category, StringComparison.OrdinalIgnoreCase);

		private static bool MatchesQuery(ReleasePlanFamily family, string? query)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return true;
			}

			return Contains(family.Name, query)
				|| Contains(family.Category, query)
				|| Contains(family.Summary, query);
		}

		private static bool Contains(string value, string query)
			=> value.Contains(query, StringComparison.OrdinalIgnoreCase);
	}
}
