using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Explore;
using Greg.Xrm.Command.Commands.Pr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.Explore
{
	public class ExploreBranchesCommandExecutor : ICommandExecutor<ExploreBranchesCommand>
	{
		private readonly IOutput output;
		private readonly IExploreApiClient exploreApiClient;

		public ExploreBranchesCommandExecutor(IOutput output, IExploreApiClient exploreApiClient)
		{
			this.output = output;
			this.exploreApiClient = exploreApiClient;
		}

		public async Task<CommandResult> ExecuteAsync(ExploreBranchesCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var repo = ResolveRepository(command.Owner, command.Repo);
				if (string.IsNullOrEmpty(repo))
				{
					return CommandResult.Fail("Unable to determine repository. Specify --owner and --repo or configure git remote.");
				}

				var parts = repo!.Split('/');
				if (parts.Length != 2)
				{
					return CommandResult.Fail($"Invalid repository '{repo}'. Expected 'owner/repo'.");
				}
				var (owner, name) = (parts[0], parts[1]);

				output.WriteLine($"Fetching branches from {repo}...", ConsoleColor.Cyan);

				var branches = await exploreApiClient.GetBranchesAsync(owner, name, cancellationToken).ConfigureAwait(false);

				if (branches.Count == 0)
				{
					output.WriteLine("No branches found.", ConsoleColor.Yellow);
					return CommandResult.Success();
				}

				output.WriteLine($"Found {branches.Count} branches:", ConsoleColor.Green);

				if (command.Format == "json")
				{
					var json = JsonSerializer.Serialize(branches, new JsonSerializerOptions { WriteIndented = true });
					output.WriteLine(json);
				}
				else
				{
					output.WriteTable(branches,
						() => new[] { "Name", "Protected", "LastCommit" },
						b => new[] {
							b.Name,
							b.Protected ? "Yes" : "",
							b.LastCommit?.ToString("yyyy-MM-dd") ?? "-"
						}
					);
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Error fetching branches: {ex.Message}", ex);
			}
		}

		private static string? ResolveRepository(string? owner, string? repo)
		{
			if (!string.IsNullOrWhiteSpace(owner) && !string.IsNullOrWhiteSpace(repo))
			{
				return $"{owner.Trim()}/{repo.Trim()}";
			}

			if (!string.IsNullOrWhiteSpace(owner) || !string.IsNullOrWhiteSpace(repo))
			{
				return null;
			}

			return GitRepositoryResolver.ResolveFromRemote();
		}

	}

	public class BranchInfo
	{
		public string Name { get; init; } = "";
		public bool Protected { get; init; }
		public DateTime? LastCommit { get; init; }
	}

	public class ExploreCompareCommandExecutor : ICommandExecutor<ExploreCompareCommand>
	{
		private readonly IOutput output;
		private readonly IExploreApiClient exploreApiClient;

		public ExploreCompareCommandExecutor(IOutput output, IExploreApiClient exploreApiClient)
		{
			this.output = output;
			this.exploreApiClient = exploreApiClient;
		}

		public async Task<CommandResult> ExecuteAsync(ExploreCompareCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var repo = ResolveRepository(command.Owner, command.Repo);
				if (string.IsNullOrEmpty(repo))
				{
					return CommandResult.Fail("Unable to determine repository. Specify --owner and --repo or configure git remote.");
				}

				var parts = repo!.Split('/');
				if (parts.Length != 2)
				{
					return CommandResult.Fail($"Invalid repository '{repo}'. Expected 'owner/repo'.");
				}
				var (owner, name) = (parts[0], parts[1]);

				output.WriteLine($"Comparing {command.Base}...{command.Head} in {repo}...", ConsoleColor.Cyan);

				var comparison = await exploreApiClient.CompareBranchesAsync(
					owner, name, command.Base, command.Head, cancellationToken).ConfigureAwait(false);

				output.WriteLine();
				output.WriteLine($"Base: {command.Base}", ConsoleColor.Gray);
				output.WriteLine($"Head: {command.Head}", ConsoleColor.Gray);
				output.WriteLine($"Ahead: {comparison.AheadBy} commits", ConsoleColor.Green);
				output.WriteLine($"Behind: {comparison.BehindBy} commits", ConsoleColor.Yellow);
				output.WriteLine();

				if (comparison.Commits.Count > 0)
				{
					output.WriteLine($"Commits in {command.Head} not in {command.Base} ({comparison.Commits.Count}):", ConsoleColor.Cyan);

					if (command.Format == "json")
					{
						var json = JsonSerializer.Serialize(comparison.Commits, new JsonSerializerOptions { WriteIndented = true });
						output.WriteLine(json);
					}
					else
					{
						foreach (var commit in comparison.Commits.Take(20))
						{
							var shortSha = commit.Sha.Length > 7 ? commit.Sha[..7] : commit.Sha;
							output.WriteLine($"  {shortSha} - {commit.Message} ({commit.Author})");
						}

						if (comparison.Commits.Count > 20)
						{
							output.WriteLine($"  ... and {comparison.Commits.Count - 20} more commits", ConsoleColor.Gray);
						}
					}
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Error comparing branches: {ex.Message}", ex);
			}
		}

		private static string? ResolveRepository(string? owner, string? repo)
		{
			if (!string.IsNullOrWhiteSpace(owner) && !string.IsNullOrWhiteSpace(repo))
			{
				return $"{owner.Trim()}/{repo.Trim()}";
			}

			if (!string.IsNullOrWhiteSpace(owner) || !string.IsNullOrWhiteSpace(repo))
			{
				return null;
			}

			return GitRepositoryResolver.ResolveFromRemote();
		}
	}

	public class ComparisonResult
	{
		public int AheadBy { get; set; }
		public int BehindBy { get; set; }
		public List<CommitInfo> Commits { get; set; } = new();
	}

	public class CommitInfo
	{
		public string Sha { get; set; } = "";
		public string Message { get; set; } = "";
		public string Author { get; set; } = "";
	}
}
