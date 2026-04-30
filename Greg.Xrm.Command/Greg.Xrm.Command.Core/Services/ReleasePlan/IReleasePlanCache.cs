namespace Greg.Xrm.Command.Services.ReleasePlan
{
	/// <summary>
	/// Local cache for release plan data, supporting offline access.
	/// </summary>
	public interface IReleasePlanCache
	{
		/// <summary>
		/// Gets cached snapshot, or null if no cache exists.
		/// </summary>
		Task<ReleasePlanSnapshot?> GetAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Stores a snapshot in the local cache.
		/// </summary>
		Task SetAsync(ReleasePlanSnapshot snapshot, CancellationToken cancellationToken);

		/// <summary>
		/// Returns the age of the cached data, or null if no cache exists.
		/// </summary>
		Task<TimeSpan?> GetAgeAsync(CancellationToken cancellationToken);
	}
}
