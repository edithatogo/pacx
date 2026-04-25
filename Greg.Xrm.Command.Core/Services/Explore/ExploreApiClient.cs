using Greg.Xrm.Command.Commands.Explore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Services.Explore
{
	public class ExploreApiClient : IExploreApiClient
	{
		private const string AcceptHeader = "application/vnd.github.v3+json";

		private readonly IHttpClientFactory httpClientFactory;

		public ExploreApiClient(IHttpClientFactory httpClientFactory)
		{
			this.httpClientFactory = httpClientFactory;
		}

		public async Task<List<BranchInfo>> GetBranchesAsync(string owner, string repo, CancellationToken ct = default)
		{
			var url = $"https://api.github.com/repos/{owner}/{repo}/branches";
			var branches = await GetJsonAsync<List<GitHubBranch>>(url, "pacx-explore-branches", ct).ConfigureAwait(false) ?? new();

			return branches.ConvertAll(b => new BranchInfo
			{
				Name = b.Name,
				Protected = b.Protected,
				LastCommit = b.Commit?.Commit?.Author?.Date?.UtcDateTime
			});
		}

		public async Task<ComparisonResult> CompareBranchesAsync(string owner, string repo, string @base, string head, CancellationToken ct = default)
		{
			var url = $"https://api.github.com/repos/{owner}/{repo}/compare/{@base}...{head}";
			var comparison = await GetJsonAsync<GitHubComparison>(url, "pacx-explore-compare", ct).ConfigureAwait(false) ?? new();

			return new ComparisonResult
			{
				AheadBy = comparison.AheadBy,
				BehindBy = comparison.BehindBy,
				Commits = comparison.Commits?.ConvertAll(c => new CommitInfo
				{
					Sha = c.Sha,
					Message = c.Commit?.Message ?? "",
					Author = c.Commit?.Author?.Name ?? ""
				}) ?? new()
			};
		}

		private async Task<T?> GetJsonAsync<T>(string url, string userAgent, CancellationToken ct)
		{
			var httpClient = httpClientFactory.CreateClient();
			using var request = new HttpRequestMessage(HttpMethod.Get, url);
			request.Headers.UserAgent.ParseAdd(userAgent);
			request.Headers.Accept.ParseAdd(AcceptHeader);

			using var response = await httpClient.SendAsync(request, ct).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct).ConfigureAwait(false);
		}

		private sealed class GitHubBranch
		{
			public string Name { get; init; } = "";
			public bool Protected { get; init; }
			public GitHubCommit? Commit { get; init; }
		}

		private sealed class GitHubComparison
		{
			public int AheadBy { get; set; }
			public int BehindBy { get; set; }
			public List<GitHubCommit>? Commits { get; set; }
		}

		private sealed class GitHubCommit
		{
			public string Sha { get; set; } = "";
			public GitHubCommitDetail? Commit { get; set; }
		}

		private sealed class GitHubCommitDetail
		{
			public string Message { get; set; } = "";
			public GitHubAuthor? Author { get; set; }
		}

		private sealed class GitHubAuthor
		{
			public string Name { get; set; } = "";
			public DateTimeOffset? Date { get; set; }
		}
	}
}
