using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageSourceBrowseCommandExecutor(IOutput output) : ICommandExecutor<PackageSourceBrowseCommand>
	{
		public Task<CommandResult> ExecuteAsync(PackageSourceBrowseCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = Greg.Xrm.Command.Services.Package.PackageSourceCatalog.Load(command.CatalogPath);
				var sources = catalog.Sources
					.Where(source => MatchesCategory(source, command.Category))
					.Where(source => MatchesQuery(source, command.Query))
					.OrderBy(source => source.Category, StringComparer.OrdinalIgnoreCase)
					.ThenBy(source => source.Name, StringComparer.OrdinalIgnoreCase)
					.ToList();

				if (sources.Count == 0)
				{
					output.WriteLine("No package sources found.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				output.WriteTable(
					sources,
					() => ["Name", "Provider", "Category", "Kind", "Summary"],
					source => [source.Name, source.Provider, source.Category, source.Kind, source.Summary]);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to browse package sources from <{command.CatalogPath}>: {ex.Message}", ex));
			}
		}

		private static bool MatchesCategory(Greg.Xrm.Command.Services.Package.PackageSourceCatalogEntry source, string? category)
			=> string.IsNullOrWhiteSpace(category) || string.Equals(source.Category, category, StringComparison.OrdinalIgnoreCase);

		private static bool MatchesQuery(Greg.Xrm.Command.Services.Package.PackageSourceCatalogEntry source, string? query)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return true;
			}

			return Contains(source.Name, query)
				|| Contains(source.Provider, query)
				|| Contains(source.Category, query)
				|| Contains(source.Kind, query)
				|| Contains(source.Summary, query)
				|| source.Packages.Any(pkg => Contains(pkg, query))
				|| source.Capabilities.Any(cap => Contains(cap, query));
		}

		private static bool Contains(string value, string query)
			=> value.Contains(query, StringComparison.OrdinalIgnoreCase);
	}
}
