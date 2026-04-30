namespace Greg.Xrm.Command.Services.ReleasePlan
{
	/// <summary>
	/// Orchestrates fetching, caching, and querying release plan data.
	/// </summary>
	public interface IReleasePlanService
	{
		/// <summary>
		/// Gets release plan items, fetching fresh data if the cache is stale or missing.
		/// </summary>
		Task<List<ReleasePlanItem>> GetItemsAsync(ReleasePlanFilter? filter, CancellationToken cancellationToken);

		/// <summary>
		/// Forces a fresh fetch from the Roadmap API.
		/// </summary>
		Task<ReleasePlanSnapshot> RefreshAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Gets the current cache age.
		/// </summary>
		Task<TimeSpan?> GetCacheAgeAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Lists all distinct products found in the cached data.
		/// </summary>
		Task<List<string>> GetProductsAsync(CancellationToken cancellationToken);
	}
}
