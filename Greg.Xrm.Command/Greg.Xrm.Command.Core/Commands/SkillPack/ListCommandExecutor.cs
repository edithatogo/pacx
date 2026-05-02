using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Tooling;

namespace Greg.Xrm.Command.Commands.SkillPack
{
	public sealed class ListCommandExecutor(IOutput output) : ICommandExecutor<ListCommand>
	{
		public Task<CommandResult> ExecuteAsync(ListCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = SkillPackCatalog.Load(command.CatalogPath);
				var packs = catalog.Packs
					.Where(p => MatchesQuery(p, command.Query))
					.Where(p => MatchesTag(p, command.Tag))
					.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
					.ToList();

				if (packs.Count == 0)
				{
					output.WriteLine("No skill packs found.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				output.WriteTable(
					packs,
					() => ["Name", "Version", "Author", "Tags", "Description"],
					pack => [pack.Name, pack.Version, pack.Author, string.Join(", ", pack.Tags), pack.Description]);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to load skill pack catalog from <{command.CatalogPath}>: {ex.Message}", ex));
			}
		}

		private static bool MatchesQuery(SkillPackEntry pack, string? query)
		{
			if (string.IsNullOrWhiteSpace(query)) return true;
			return pack.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
				|| pack.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
				|| pack.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase));
		}

		private static bool MatchesTag(SkillPackEntry pack, string? tag)
		{
			if (string.IsNullOrWhiteSpace(tag)) return true;
			return pack.Tags.Any(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase));
		}
	}
}
