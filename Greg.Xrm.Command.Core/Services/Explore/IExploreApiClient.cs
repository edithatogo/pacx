using Greg.Xrm.Command.Commands.Explore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Services.Explore
{
	public interface IExploreApiClient
	{
		Task<List<BranchInfo>> GetBranchesAsync(string owner, string repo, CancellationToken ct = default);
		Task<ComparisonResult> CompareBranchesAsync(string owner, string repo, string @base, string head, CancellationToken ct = default);
	}
}
